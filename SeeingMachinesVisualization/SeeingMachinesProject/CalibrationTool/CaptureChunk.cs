using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMFramework;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace CalibrationTool
{
	public class CaptureChunk
	{
		public SMFramework.PairedDatabase RecordingDatabase
		{
			get;
			private set;
		}

		FaceLabSignalConfiguration m_Signal;
		SMFramework.Testing.DataCollectionSource m_CaptureSource;

		public Vector3 TargetPosition;

		public int ValidSnapshots
		{
			get
			{
				int count = 0;
				foreach (DataSnapshot snapshot in RecordingDatabase.PairingSnapshots)
				{
					if (snapshot.ComposeVector3(m_Signal.Label + " Vergence") != Vector3.Zero)
					{
						++count;
					}
				}

				return count;
			}
		}

		public enum Mode
		{
			FileSource,
			StreamSource
		}

		public CaptureChunk(FaceLabSignalConfiguration signal)
		{
			m_Signal = signal;
			RecordingDatabase = new SMFramework.PairedDatabase();
		}

		public void StartCollection()
		{
			SensorClusterConfiguration tempConfig = new SensorClusterConfiguration();
			tempConfig.SensorConfigurations.Add(m_Signal);
			m_CaptureSource = new SMFramework.Testing.FileStreamCollectionSource("D:/Data/Repos/ConstructSeeingMachinesVisualization/Data Samples/Per-Signal Data/Monostation Mixed/Sensor 1-headstraightlookright.csv", FaceData.CoordinateSystemType.Global);
			m_CaptureSource.BeginCollection(tempConfig);
		}

		public void StopCollection()
		{
			m_CaptureSource.StopCollection();
			RecordingDatabase = m_CaptureSource.CollectedData;
			m_CaptureSource = null;
		}
	}
}
