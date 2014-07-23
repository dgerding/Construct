using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Construct.MessageBrokering.Serialization;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using NLog;



/*
 * TECHNICAL DEBT NOTICE:
 * 
 *	ItemPersistor sits at the top level of the item persistence process, which has direct conceptual bindings to SQL
 *		and disk persistence. If you look at the call hierarchy of ConstructSerializationAssistant.Persist() you'll find
 *		that there is only references to SQL at the opposite end of the process. ItemPersistor was developed *after* the
 *		majority of the other Construct Server functionality was written, and was a response to the issue of throughput
 *		under load and prevention of client-side (sensor) data buffering. The existing infrastructure for item persistence
 *		is more generic, and this level of coding would be more appropriate as sub-level of that generic code. However,
 *		for the sake of performance this code was required to be higher-level despite this being less appropriate.
 *	
 *		*Persistor classes have their PersistItem methods optimized to be near-instantaneous to minimize latency of
 *			sensor calls via WCF RPC. Because of this, Items are queued as strings rather than deserialized objects
 *			due to the overhead of JSON parsing for deserialization.
 *		
 * SUGGESTION:
 *	The logic handled in *Persistor classes should be invoked somewhere in the Model.Add*PropertyValue methods for
 *		flexibility. Note, however, that those are per-property persistence methods, whereas this logic is per-item.
 *		Also note that, in order to persist a property, the item has to have its JSON fully parsed and the properties
 *		extracted. This would heavily impact the latency that this mechanism was intended to mitigate.
 */



namespace Construct.Server.Models.Data
{
	//	SqlItemPersistor simply uses the ThreadPool, but the DiskItemPersistor maintains at least one DiskBatchWriter. If batches
	//		are being persisted to disk, we need to be able to always write to disk. If the ThreadPool is saturated, disk-write
	//		work items will be queued, preventing disk caching under load. Each DiskBatchWriter maintains a dedicated thread instead.
	class ParallelDiskBatchWriter : IDisposable
	{
		Thread m_WriterThread;
		bool m_Continue = false;
		String m_TargetBatchDirectory;

		ConcurrentQueue<ConcurrentQueue<String>> m_RequestedBatchesForCaching = new ConcurrentQueue<ConcurrentQueue<String>>();

		public ParallelDiskBatchWriter(String batchDirectory)
		{
			m_TargetBatchDirectory = batchDirectory;

			m_WriterThread = new Thread(WriterThread);
			m_WriterThread.Name = "DiskBatchWriter";
			m_WriterThread.Start();

			m_Continue = true;
		}

		public void EnqueueBatch(ConcurrentQueue<String> batch)
		{
			m_RequestedBatchesForCaching.Enqueue(batch);
		}

		public void Dispose()
		{
			m_Continue = false;
			m_WriterThread.Join();
		}

		public delegate void BatchFinishedWritingHandler(String batchFileName);
		public event BatchFinishedWritingHandler OnBatchFinishedWriting;

		private void WriterThread()
		{
			while (m_Continue)
			{
				System.Threading.Thread.Sleep(1);

				ConcurrentQueue<String> currentBatch = null;
				if (!m_RequestedBatchesForCaching.TryDequeue(out currentBatch))
					continue;


				Debugger.Log(0, "", "ParallelDiskBatchWriter writing batch\n");
				String newPersistBatchName = Guid.NewGuid().ToString();
				using (FileStream stream = File.Create(Path.Combine(m_TargetBatchDirectory, newPersistBatchName)))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, currentBatch);
				}

				if (OnBatchFinishedWriting != null)
					OnBatchFinishedWriting(newPersistBatchName);
			}
		}
	}



	class SqlItemPersistor
	{
		uint m_BatchSize;
		int m_CurrentTraffic = 0;
		String m_ConnectionString;
		ConcurrentQueue<String> m_CurrentBuffer;
		ConstructSerializationAssistant m_Assistant;

		DateTime m_LastFlushTime;
		Timer m_PushQueueTimer;

		public delegate void SqlTrafficChangeHandler(int itemsInQueue);
		public SqlTrafficChangeHandler OnSqlTrafficChange;

		public event Action<dynamic> OnPersist;

		public SqlItemPersistor(ConstructSerializationAssistant assistant, String connectionString, uint batchSize)
		{
			m_ConnectionString = connectionString;
			m_BatchSize = batchSize;
			m_CurrentBuffer = new ConcurrentQueue<string>();
			m_Assistant = assistant;

			m_LastFlushTime = DateTime.Now;

			//	Tries persist-to-SQL every 2 seconds (hack)
			m_PushQueueTimer = new Timer(delegate (object unused) {

				if (m_CurrentBuffer.IsEmpty)
					return;

				ConcurrentQueue<String> jobBuffer = null;
				lock (m_CurrentBuffer)
				{
					var now = DateTime.Now;
					var timeSinceLastPush = now - m_LastFlushTime;
					if (timeSinceLastPush.TotalMilliseconds >= 2000)
					{
						jobBuffer = m_CurrentBuffer;
						m_CurrentBuffer = new ConcurrentQueue<string>();
						m_LastFlushTime = now;
					}
				}

				if (jobBuffer != null)
				{
					m_CurrentTraffic += jobBuffer.Count;
					NotifyTrafficChange(m_CurrentTraffic);
					ThreadPool.QueueUserWorkItem(RunPersistBatchToSqlJob, jobBuffer);
				}
			}, null, 0, 2000);
		}

		public void PersistItem(String item)
		{
			ConcurrentQueue<String> jobBuffer = null;

			m_CurrentBuffer.Enqueue(item);



			if (m_CurrentBuffer.Count >= m_BatchSize)
			{
				var currentBuffer = m_CurrentBuffer;
				lock (currentBuffer)
				{
					if (currentBuffer.Count >= m_BatchSize)
					{
						jobBuffer = m_CurrentBuffer;
						m_CurrentBuffer = new ConcurrentQueue<string>();
						m_LastFlushTime = DateTime.Now;
					}
				}
			}

			if (jobBuffer != null)
			{
				m_CurrentTraffic += jobBuffer.Count;
				NotifyTrafficChange(m_CurrentTraffic);
				ThreadPool.QueueUserWorkItem(RunPersistBatchToSqlJob, jobBuffer);
			}
		}

		public void PersistBatch(ConcurrentQueue<String> batch)
		{
			m_CurrentTraffic += batch.Count;
			OnSqlTrafficChange(m_CurrentTraffic);
			ThreadPool.QueueUserWorkItem(RunPersistBatchToSqlJob, batch);
		}

		private void NotifyTrafficChange(int traffic)
		{
			if (OnSqlTrafficChange != null)
				OnSqlTrafficChange(traffic);
		}

		private void RunPersistBatchToSqlJob(object itemBatchData)
		{
			IEnumerable<String> itemBatch = itemBatchData as IEnumerable<String>;
			int batchCount = 0;

			using (var model = new Entities.EntitiesModel(m_ConnectionString))
			using (var connection = new SqlConnection())
			{
				if (!m_ConnectionString.ToLower().Contains("Max Pool Size"))
					connection.ConnectionString = m_ConnectionString + "; Max Pool Size=500";
				else
					connection.ConnectionString = m_ConnectionString;

				while (connection.State != System.Data.ConnectionState.Open)
				{
					try
					{
						connection.Open();
					}
					catch (Exception e)
					{
						NLog.Logger logger = LogManager.GetCurrentClassLogger();
						logger.Warn("Unable to establish SQL database connection while persisting item batch. Possibly too many batches running simultaneously. Will retry connection immediately.", e);
						Thread.Sleep(500);
					}
				}

				foreach (String data in itemBatch)
				{
					ProcessIndividualItem(data, model);
					batchCount++;
				}

				model.SaveChanges();
			}

			m_CurrentTraffic -= batchCount;
			NotifyTrafficChange(m_CurrentTraffic);
		}

		private void ProcessIndividualItem(String itemJson, Entities.EntitiesModel model)
		{
			Guid itemID = Guid.NewGuid();

			Entities.Item header = m_Assistant.GetItemHeader(itemJson, itemID);
			dynamic item = m_Assistant.GetItem(itemJson, itemID);

			try
			{
				model.Add(header);

				m_Assistant.Persist(itemJson, itemID);
				if (OnPersist != null)
				{
					OnPersist(item);
				}
			}
			catch (Exception ex)
			{
				//throw ex;
			}
		}
	}

	class DiskItemPersistor
	{
		private String m_DiskBuffersLocation;
		private uint m_BatchSize;

		ConcurrentQueue<String> m_AvailableBatchFiles = new ConcurrentQueue<string>();

		ConcurrentQueue<String> m_CurrentBuffer;

		//	Only use one writer for now, could add a mechanism for adding more as demand increases
		ParallelDiskBatchWriter m_BatchWriter;

		public DiskItemPersistor(String diskBuffersFolder, uint batchSize)
		{
			m_DiskBuffersLocation = diskBuffersFolder;
			if (!Directory.Exists(m_DiskBuffersLocation))
				Directory.CreateDirectory(m_DiskBuffersLocation);


			m_BatchWriter = new ParallelDiskBatchWriter(diskBuffersFolder);
			m_BatchWriter.OnBatchFinishedWriting += OnNewBatchWritten;

			m_BatchSize = batchSize;
			m_CurrentBuffer = new ConcurrentQueue<string>();

			LoadExistingBuffers(m_DiskBuffersLocation);
		}

		private void LoadExistingBuffers(String location)
		{
			String[] buffers = Directory.GetFiles(location);
			foreach (String bufferFile in buffers)
				m_AvailableBatchFiles.Enqueue(bufferFile.Split('\\').Last());
		}

		public void PersistItem(String item)
		{
			m_CurrentBuffer.Enqueue(item);

			ConcurrentQueue<String> jobBuffer = null;

			if (m_CurrentBuffer.Count >= m_BatchSize)
			{
				lock (m_CurrentBuffer)
				{
					//	Second check is intentional
					if (m_CurrentBuffer.Count >= m_BatchSize)
					{
						jobBuffer = m_CurrentBuffer;
						m_CurrentBuffer = new ConcurrentQueue<string>();
					}
				}
			}
			

			if (jobBuffer != null)
				m_BatchWriter.EnqueueBatch(jobBuffer);
		}

		/// <summary>
		/// Reads in a batch of items from disk and deletes the batch from disk.
		/// </summary>
		public ConcurrentQueue<String> PullBatch()
		{
			ConcurrentQueue<String> result;
			String sourceFile = GetAvailableDiskBatch();
			if (sourceFile == null)
				return null;

			try
			{
				using (FileStream stream = File.OpenRead(sourceFile))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					result = formatter.Deserialize(stream) as ConcurrentQueue<String>;
				}

				File.Delete(sourceFile);
			}
			catch (Exception e)
			{
				result = null;
			}

			return result;
		}

		private void OnNewBatchWritten(String batchName)
		{
			lock (m_AvailableBatchFiles)
			{
				m_AvailableBatchFiles.Enqueue(batchName);
			}
		}

		private String GetAvailableDiskBatch()
		{
			String result = null;

			while (!m_AvailableBatchFiles.IsEmpty && (result == null || !File.Exists(result)))
			{
				String entry;
				if (m_AvailableBatchFiles.TryDequeue(out entry))
					result = Path.Combine(m_DiskBuffersLocation, entry);
			}

			return result;
		}
	}



	public class SwitchingItemPersistor
	{
		//	Values determined via trial-and-error
		public uint ItemBatchSize = 400;
		public uint MaxTrafficThreshold = 15000;
		public uint SafeTrafficThreshold = 3000;
		public uint LowTrafficThreshold = 1500;

		private SqlItemPersistor m_SqlItemPersistor;
		private DiskItemPersistor m_DiskItemPersistor;

		bool m_IsWaitingForTrafficDrop = false;
		object m_StateMutex = new Object();

		public Action<dynamic> OnSqlPersist;

		//	Note: The hooking between SwitchingItemPersistor and Construct.Server.Models.Data.Model is hacky.
		public SwitchingItemPersistor(ConstructSerializationAssistant assistant, String sqlConnectionString, String diskBufferFolder)
		{
			m_SqlItemPersistor = new SqlItemPersistor(assistant, sqlConnectionString, ItemBatchSize);
			m_SqlItemPersistor.OnSqlTrafficChange += OnTrafficVolumeChange;
			m_SqlItemPersistor.OnPersist += delegate(dynamic data) {
				if (this.OnSqlPersist != null)
					this.OnSqlPersist(data);
			};

			m_DiskItemPersistor = new DiskItemPersistor(diskBufferFolder, ItemBatchSize);

			//	Allows loading of disk-cached items to begin
			this.OnTrafficVolumeChange(0);
		}

		public void HandleItem(String data)
		{
			if (m_IsWaitingForTrafficDrop)
				m_DiskItemPersistor.PersistItem(data);
			else
				m_SqlItemPersistor.PersistItem(data);
		}

		private void OnTrafficVolumeChange(int newTraffic)
		{
			//Debugger.Log(0, "", "Traffic volume change: " + newTraffic + " items in queue");
			lock (m_StateMutex)
			{
				if (newTraffic >= MaxTrafficThreshold)
				{
					m_IsWaitingForTrafficDrop = true;
					Debugger.Log(0, "", "Surpassed MaxTrafficThreshold, dropping to disk persistence\n");
				}

				if (m_IsWaitingForTrafficDrop && newTraffic < SafeTrafficThreshold)
				{
					m_IsWaitingForTrafficDrop = false;
					Debugger.Log(0, "", "Now within SafeTrafficThreshold, permitting SQL persistence\n");
				}

				if (newTraffic < LowTrafficThreshold)
				{
					QueueBatchFromDisk();
					Debugger.Log(0, "", "Within LowTrafficThreshold, queueing disk -> SQL persistence\n");
				}
			}
		}

		private void QueueBatchFromDisk()
		{
			ConcurrentQueue<String> diskBatch = m_DiskItemPersistor.PullBatch();
			if (diskBatch != null && diskBatch.Count > 0)
				m_SqlItemPersistor.PersistBatch(diskBatch);
		}
	}
}
