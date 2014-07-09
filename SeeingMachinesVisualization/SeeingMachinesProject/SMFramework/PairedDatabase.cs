using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * TODO:
 * 
 *	- Separate camera labeling from the actual name of each PairedDatabase entry (i.e. stopping the
 *		CameraLabel + " Vergence" that's going on right now.)
 * 
 * 
 */

/* Problem: Compiling a header from data will have to be done at finalization-time, may need to be prefixed to the file */

namespace SMFramework
{
	/* NOTE: Headers are not currently implemented. Right now we've just got the Database version
	 *	header data.
	 */
	public class PairedDatabaseHeader
	{
		public Dictionary<String, String> Data = new Dictionary<String, String>();

		public String this[String key]
		{
			get
			{
				return Data[key];
			}

			set
			{
				Data[key] = value;
			}
		}
	}

	public class PairedDatabase : FaceLabDataStream
	{
		public LinkedList<DataSnapshot> PairingSnapshots
		{
			get;
			private set;
		}

		public String PersistenceFileName
		{
			get;
			private set;
		}

		PairedDatabaseHeader m_Headers = new PairedDatabaseHeader();

		public void SetHeaderField(String fieldName, String value)
		{
			m_Headers[fieldName] = value;
		}

		public PairedDatabase()
		{
			PairingSnapshots = new LinkedList<DataSnapshot>();
			PersistenceFileName = null;
		}

		//	TODO: This SHOULD end up being thrown in
		//public PairedDatabase(SensorClusterConfiguration clusterConfiguration)
		//{
		//	PairingSnapshots = new LinkedList<DataSnapshot>();
		//	m_Headers["ClusterConfigurationIdentifier"] = clusterConfiguration.GUID.ToString();
		//}

		public PairedDatabase(List<DataSnapshot> previousData)
		{
			PairingSnapshots = new LinkedList<DataSnapshot>();

			PersistenceFileName = null;

			foreach (DataSnapshot data in previousData)
			{
				PairingSnapshots.AddLast(data);
			}
		}

		public PairedDatabase(String sourceFile)
		{
			PairingSnapshots = new LinkedList<DataSnapshot>();

			PersistenceFileName = sourceFile;

			using (StreamReader sw = new StreamReader(sourceFile))
			{
				//	Used to have a versioning string, not supported anymore (made the file not actual CSV)
				String versionString = sw.ReadLine();
				String[] headers;
				if (versionString.IndexOf("SeeingMachines PDB V:") < 0)
					headers = versionString.Split(',');
				else
					headers = sw.ReadLine().Split(',');

				for (int i = 0; i < headers.Length; i++)
				{
					headers[i] = headers[i].Trim();
				}

				while (!sw.EndOfStream)
				{
					String[] currentValues = sw.ReadLine().Split(',');
					DataSnapshot currentSnapshot = new DataSnapshot();
					for (int i = 0; i < currentValues.Length - 1; i++)
					{
						if (currentValues[i].Trim().Length == 0) continue;

						switch (headers[i])
						{
							case ("Time (UTC)"):
								{
									DateTime usableTime = DateTime.Parse(currentValues[i]).ToUniversalTime();
									currentSnapshot.TimeStamp = usableTime;
									break;
								}

							default:
								{
									currentSnapshot[headers[i]] = double.Parse(currentValues[i]);
									break;
								}
						}
					}
					PairingSnapshots.AddLast(currentSnapshot);
				}
			}
		}

		public int NumberOfSnapshots
		{
			get
			{
				return PairingSnapshots.Count();
			}
		}

		public void BeginNextSnapshot()
		{
			PairingSnapshots.AddLast(new DataSnapshot());
		}

		public void BeginNextSnapshot(DateTime snapshotTime)
		{
			PairingSnapshots.AddLast(new DataSnapshot(snapshotTime));
		}

		private bool CheckSnapshotValidity()
		{
			if (PairingSnapshots.Count == 0)
			{
				DebugOutputStream.SlowInstance.WriteLine("PairedDatabase.AddToCurrentSnapshot - Unable to add data to a snapshot when no snapshots exist.");
				return false;
			}

			return true;
		}

		public void AddToCurrentSnapshot(string key, double value)
		{
			if (!CheckSnapshotValidity())
				return;

			DataSnapshot snapshot = PairingSnapshots.Last.Value;
			snapshot.Write(key, value);
		}

		public void AddToCurrentSnapshot(string key, Vector2 value)
		{
			if (!CheckSnapshotValidity())
				return;

			DataSnapshot snapshot = PairingSnapshots.Last.Value;
			snapshot.Write(key, value);
		}

		public void AddToCurrentSnapshot(string key, Vector3 value)
		{
			if (!CheckSnapshotValidity())
				return;

			DataSnapshot snapshot = PairingSnapshots.Last.Value;
			snapshot.Write(key, value);
		}

		public DataSnapshot CurrentSnapshot
		{
			get
			{
				if (PairingSnapshots.Count == 0)
					return null;

				return PairingSnapshots.Last.Value;
			}
		}

		public void ClearDatabase()
		{
			PairingSnapshots.Clear();
		}

		public void WriteToDisk()
		{
			if (PersistenceFileName != null)
				WriteToDisk(PersistenceFileName);
			else
				WriteToDisk(DatabaseFormatMapping.GenerateRecordingFilename());
		}

		String BuildHeaderString()
		{
			/* TODO: Actually put the header to use. Right now there's no header information since
			 *	I'm not sure how the header should be formatted, but the goal is to stop the current
			 *	trend of "SignalName SomeData" for column names, where "SignalName" is always prefixed 
			 */
			String result = "";
			result += "HEADER" + "\n";

			foreach (KeyValuePair<String, String> pair in m_Headers.Data)
			{
				result += pair.Key + ": " + pair.Value + "\n";
			}

			result += "ENDHEADER";

			return result;
		}

		public void WriteToDisk(String outputFileName)
		{
			DatabaseWriter databaseWriter = new DatabaseWriter();
			databaseWriter.Open(outputFileName);

			foreach (DataSnapshot dict in PairingSnapshots)
				databaseWriter.WriteSnapshot(dict);

			databaseWriter.Close();

			PairingSnapshots.Clear();

			PersistenceFileName = outputFileName;
		}
	}
}
