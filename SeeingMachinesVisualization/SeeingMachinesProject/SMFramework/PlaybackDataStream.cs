using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMFramework
{
	public class PlaybackDataStream : FaceLabDataStream
	{
		List<DataSnapshot> m_SnapshotHistory;

		public int NumberOfSnapshots
		{
			get
			{
				return m_SnapshotHistory.Count;
			}
		}

		int m_CurrentFrame = 0;
		public int CurrentFrame
		{
			get
			{
				return m_CurrentFrame;
			}

			private set
			{
				if (value < 0)
					m_CurrentFrame = 0;

				else if (value >= m_SnapshotHistory.Count)
					m_CurrentFrame = m_SnapshotHistory.Count - 1;

				else
					m_CurrentFrame = value;
			}
		}

		public List<DataSnapshot> FullRecording
		{
			get
			{
				return new List<DataSnapshot>(m_SnapshotHistory);
			}
		}

		public DataSnapshot LastSnapshot
		{
			get
			{
				if (m_SnapshotHistory.Count > 0)
					return m_SnapshotHistory.Last();
				else
					return null;
			}
		}

		public DataSnapshot FirstSnapshot
		{
			get
			{
				if (m_SnapshotHistory.Count > 0)
					return m_SnapshotHistory.First();
				else
					return null;
			}
		}

		public bool PlaybackIsComplete
		{
			get;
			private set;
		}

		private DateTime m_TargetRecordingTime = new DateTime ();

		/// <summary>
		/// The time in the recording that is attempting to be reached. A time 0.25 seconds into a
		/// recording may be specified, but only frames for 0.2 seconds and 0.3 seconds may be available.
		/// The min/max of the two is used (depending on the direction of seeking.)
		/// </summary>
		public DateTime TargetRecordingTime
		{
			get
			{
				return m_TargetRecordingTime;
			}

			set
			{
				if (CurrentFrame == m_SnapshotHistory.Count - 1)
					PlaybackIsComplete = true;
				else
					PlaybackIsComplete = false;

				m_TargetRecordingTime = value;

				if (m_TargetRecordingTime < FirstSnapshot.TimeStamp)
					m_TargetRecordingTime = FirstSnapshot.TimeStamp;
				if (m_TargetRecordingTime > LastSnapshot.TimeStamp)
					m_TargetRecordingTime = LastSnapshot.TimeStamp;

				if (m_SnapshotHistory.Count == 0) return;

				if (m_TargetRecordingTime > LastSnapshot.TimeStamp)
				{
					CurrentFrame = m_SnapshotHistory.Count - 1;
					return;
				}

				if (m_TargetRecordingTime < FirstSnapshot.TimeStamp)
				{
					CurrentFrame = 0;
					return;
				}

				TimeSpan timeDifference = m_TargetRecordingTime - m_SnapshotHistory[CurrentFrame].TimeStamp;
				if (timeDifference.TotalMilliseconds > 0)
				{
					while (
						(m_TargetRecordingTime - m_SnapshotHistory[CurrentFrame].TimeStamp).TotalMilliseconds > 0 &&
						CurrentFrame < m_SnapshotHistory.Count - 1
						)
						CurrentFrame++;
				}
				else if (timeDifference.TotalMilliseconds < 0)
				{
					while (
						(m_SnapshotHistory[CurrentFrame].TimeStamp - m_TargetRecordingTime).TotalMilliseconds > 0 &&
						CurrentFrame > 0
						)
						CurrentFrame--;
				}

				if (CurrentFrame >= m_SnapshotHistory.Count)
					CurrentFrame = m_SnapshotHistory.Count - 1;

				if (CurrentFrame < 0)
					CurrentFrame = 0;

				if (FrameChanged != null && m_SnapshotHistory.Count > 0)
					FrameChanged(this, CurrentSnapshot);
			}
		}

		public FaceData.CoordinateSystemType CoordinateSystem
		{
			get;
			private set;
		}

		public SensorClusterConfiguration Sensors
		{
			get;
			private set;
		}

		public PlaybackDataStream(String sourceFile, SensorClusterConfiguration sensorConfigurations, FaceData.CoordinateSystemType sourceCoordinateSystem)
		{
			CoordinateSystem = sourceCoordinateSystem;
			Sensors = sensorConfigurations;

			PairedDatabase sourceDatabase = new PairedDatabase(sourceFile);
			m_SnapshotHistory = new List<DataSnapshot>(sourceDatabase.PairingSnapshots);

			m_TargetRecordingTime = FirstSnapshot.TimeStamp;
		}

		public delegate void FrameChangedEventHandler(PlaybackDataStream instance, DataSnapshot newFrame);
		public event FrameChangedEventHandler FrameChanged;

		public DataSnapshot CurrentSnapshot
		{
			get
			{
				return m_SnapshotHistory.ElementAt(CurrentFrame);
			}
		}

		/* Whether or not to create an empty FaceData object for a snapshot when the FaceData
		 * for a sensor is not available within that snapshot.
		 */
		public bool AutoGenerateMissingData = true;

		public FaceData[] CurrentDataInterpretation
		{
			get
			{
				FaceData[] result = new FaceData[Sensors.SensorConfigurations.Count];
				for (int i = 0; i < Sensors.SensorConfigurations.Count; i++)
				{
					String currentLabel = Sensors.SensorConfigurations[i].Label;

					if (!AutoGenerateMissingData && !CurrentSnapshot.ContainsKeyContaining(currentLabel))
						continue;

					result[i] = FaceDataSerialization.ReadFaceDataFromSnapshot(CurrentSnapshot, currentLabel);
				}

				return result;
			}
		}

		public DateTime RecordingStartTime
		{
			get
			{
				return FirstSnapshot.TimeStamp;
			}
		}

		public TimeSpan RecordingLength
		{
			get
			{
				return LastSnapshot.TimeStamp - FirstSnapshot.TimeStamp;
			}
		}

		public void BeginNextSnapshot()
		{
			if (CurrentFrame == m_SnapshotHistory.Count - 1)
				PlaybackIsComplete = true;
			else
				PlaybackIsComplete = false;

			CurrentFrame++;
			m_TargetRecordingTime = CurrentSnapshot.TimeStamp;
		}

		public void BeginNextSnapshot(DateTime snapshotTimestamp)
		{
			/* Don't do anything extra; trying to specify the timestamp of a snapshot doesn't make sense
			 * for a recording. Seeking should be done via the TargetRecordingTime property.
			 */
			BeginNextSnapshot();
		}

		public void AddToCurrentSnapshot(string Key, double Value)
		{
		}

		public void AddToCurrentSnapshot(string key, Vector2 value)
		{
		}

		public void AddToCurrentSnapshot(string Key, Vector3 Value)
		{
		}
	}
}
