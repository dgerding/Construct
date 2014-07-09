using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/*
 * TODO: ParallelDatabaseWriter and PairedDatabase have duplicate code for persistence to disk
 * 
 */

namespace SMFramework
{
	public class AsyncPairedDatabase : FaceLabDataStream
	{
		public ConcurrentQueue<DataSnapshot> WriteQueue
		{
			get;
			private set;
		}

		bool m_ContinueThread = false;
		bool m_IsPaused = false;
		Task m_WriteTask = null;
		DataSnapshot m_CurrentSnapshot;
		String m_CurrentTargetFile;
		int m_SnapshotCount = 0;

		public String CurrentDestinationFile
		{
			get { return m_CurrentTargetFile; }
		}

		void WriteThread(String _targetFile)
		{
			/* Ensuring that storing X as the 3rd column in one snapshot will cause X to be in the 3rd
			 * column for all other snapshots (maintain a record of "key"->"column index")
			 */
			List<String> headerMap = new List<String>();
			String targetFile = _targetFile as String;
			DatabaseWriter writer;

			try
			{
				writer = new DatabaseWriter();
				writer.Open(targetFile);
			}
			catch (System.Exception ex)
			{
				DebugOutputStream.SlowInstance.WriteLine(
					"ParallelDatabaseWriter could not access file for writing:\n" + targetFile +
					"\nError: " + ex.Message
					);

				return;
			}

			/* !WriteQueue.IsEmpty to allow for completion of write operations if m_ContinueThread is set to false */
			while (m_ContinueThread || !WriteQueue.IsEmpty)
			{
				if (WriteQueue.IsEmpty || m_IsPaused)
				{
					Thread.Sleep(1); // Sleep the smallest amount of time allowed by OS
				}
				else
				{
					DataSnapshot currentSnapshot;
					if (!WriteQueue.TryDequeue(out currentSnapshot))
					{
						DebugOutputStream.SlowInstance.WriteLine("ParallelDatabaseWriter call to ConcurrentQueue.TryDequeue failed even though WriteQueue.IsEmpty is false.");
						continue;
					}

					writer.WriteSnapshot(currentSnapshot);
				}
			}

			writer.Close();
			writer = null;
			GC.Collect();
		}

		void WriteSnapshot(DataSnapshot snapshot, List<String> headerMap, StreamWriter writer)
		{
			/* Manually write out the timestamp */
			writer.Write(snapshot.TimeStamp.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") +",");

			foreach (String column in headerMap)
			{
				writer.Write(snapshot.Data[column] + ",");
			}

			writer.WriteLine();
		}

		public AsyncPairedDatabase()
		{
			WriteQueue = new ConcurrentQueue<DataSnapshot>();
			m_CurrentSnapshot = new DataSnapshot();
		}

		public AsyncPairedDatabase(String targetFile)
		{
			WriteQueue = new ConcurrentQueue<DataSnapshot>();
			m_CurrentSnapshot = new DataSnapshot();
			Start(targetFile);
		}

		public void Start(String targetFile)
		{
			m_ContinueThread = true;
			m_CurrentTargetFile = targetFile;

			m_IsPaused = false;
			m_WriteTask = Task.Run(() => WriteThread(targetFile));
		}

		public void Pause()
		{
			m_IsPaused = true;
		}

		public void Resume()
		{
			m_IsPaused = false;
		}

		public String Stop(bool keepResultData = true)
		{
			if (!m_ContinueThread)
			{
				DebugOutputStream.SlowInstance.WriteLine("AsyncPairedDatabase attempted to stop write thread when no call to Start() was made.");
				return null;
			}

			if (m_CurrentSnapshot.Data.Count != 0)
				WriteQueue.Enqueue(m_CurrentSnapshot);

			m_ContinueThread = false;
			try
			{
				m_WriteTask.Wait();
			}
			catch (Exception e)
			{
				if (Debugger.IsAttached)
					throw e;
			}


			if (!keepResultData)
			{
				if (!File.Exists(m_CurrentTargetFile))
					throw new FileNotFoundException(m_CurrentTargetFile);
				else
					File.Delete(m_CurrentTargetFile);
			}

			String newFile = m_CurrentTargetFile;
			m_CurrentTargetFile = null;
			m_SnapshotCount = 0;

			return newFile;
		}

		public void AddToCurrentSnapshot(string key, double value)
		{
			m_CurrentSnapshot.Write(key, value);
		}

		public void AddToCurrentSnapshot(string key, Vector2 value)
		{
			m_CurrentSnapshot.Write(key, value);
		}

		public void AddToCurrentSnapshot(string key, Vector3 value)
		{
			m_CurrentSnapshot.Write(key, value);
		}

		public void BeginNextSnapshot()
		{
			WriteQueue.Enqueue(m_CurrentSnapshot);
			m_CurrentSnapshot = new DataSnapshot();

			++m_SnapshotCount;
		}

		public void BeginNextSnapshot(DateTime snapshotTimestamp)
		{
			WriteQueue.Enqueue(m_CurrentSnapshot);
			m_CurrentSnapshot = new DataSnapshot(snapshotTimestamp);

			++m_SnapshotCount;
		}

		public int NumberOfSnapshots
		{
			get { return m_SnapshotCount; }
		}

		public DataSnapshot CurrentSnapshot
		{
			get { return m_CurrentSnapshot; }
		}
	}
}
