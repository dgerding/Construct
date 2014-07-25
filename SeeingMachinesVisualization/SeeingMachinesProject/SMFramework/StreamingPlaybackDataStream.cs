using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMFramework
{
	public class StreamingPlaybackDataStream : FaceLabDataStream
	{
		StreamReader m_SourceFileStream;
		String[] m_DataHeaders;

		public int NumberOfSnapshots
		{
			get;
			private set;
		}

		public DataSnapshot LastSnapshot
		{
			get;
			private set;
		}

		public DataSnapshot FirstSnapshot
		{
			get;
			private set;
		}

		public bool PlaybackIsComplete
		{
			get;
			private set;
		}

		private DateTime m_TargetRecordingTime = new DateTime();

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
				if (m_SourceFileStream.EndOfStream && value > m_TargetRecordingTime)
				{
					PlaybackIsComplete = true;
					return;
				}
				else
				{
					PlaybackIsComplete = false;
				}

				m_TargetRecordingTime = value;

				if (m_TargetRecordingTime <= FirstSnapshot.TimeStamp)
				{
					m_TargetRecordingTime = FirstSnapshot.TimeStamp;
					CurrentSnapshot = FirstSnapshot;
				}
				else if (m_TargetRecordingTime >= LastSnapshot.TimeStamp)
				{
					m_TargetRecordingTime = LastSnapshot.TimeStamp;
					CurrentSnapshot = LastSnapshot;
				}
				else
				{
					var referenceString = m_SourceFileStream.ReadLine();
					DataSnapshot referenceSnapshot = null;
					if (referenceString == null)
						referenceSnapshot = LastSnapshot;
					else
						referenceSnapshot = ParseSnapshotFromString(m_DataHeaders, referenceString);

					var baseFileStream = m_SourceFileStream.BaseStream;

					//	Algorithm to "ballpark" the location of the requested entry (move the cursor closer)
					{
						const int assumedLineSize = 5000;
						const int assumedFps = 30;
						int skipSize = (int)((m_TargetRecordingTime - referenceSnapshot.TimeStamp).TotalSeconds * assumedFps * assumedLineSize);

						//	5 picked since it's just a "good" number
						while (Math.Abs((m_TargetRecordingTime - referenceSnapshot.TimeStamp).TotalSeconds) > 2)
						{
							baseFileStream.Seek(skipSize, SeekOrigin.Current);
							if (baseFileStream.Position > baseFileStream.Length)
								break;

							//	Advance to the next detectable line
							m_SourceFileStream.ReadLine();
							referenceSnapshot = ParseSnapshotFromString(m_DataHeaders, m_SourceFileStream.ReadLine());

							skipSize = (int)((m_TargetRecordingTime - referenceSnapshot.TimeStamp).TotalSeconds * assumedFps * assumedLineSize);
						}
					}


					//	Attempt to move the cursor to the records before the appropriate data entries
					int smallSkipSize = -50000;
					while (m_TargetRecordingTime < referenceSnapshot.TimeStamp)
					{
						if (baseFileStream.Position + smallSkipSize < 0)
							baseFileStream.Seek(0, SeekOrigin.Begin);
						else
							baseFileStream.Seek(smallSkipSize, SeekOrigin.Current);

						//	Advance to the detectable next line
						m_SourceFileStream.ReadLine();

						referenceSnapshot = ParseSnapshotFromString(m_DataHeaders, m_SourceFileStream.ReadLine());
					}

					//	Move forward until we find the two snapshots that encompass the requested time
					DataSnapshot previousSnapshot = null;
					while (referenceSnapshot.TimeStamp < m_TargetRecordingTime)
					{
						previousSnapshot = referenceSnapshot;
						referenceSnapshot = ParseSnapshotFromString(m_DataHeaders, m_SourceFileStream.ReadLine());
					}

					if (previousSnapshot != null)
					{
						//	Referenced time is between previousSnapshot and referenceSnapshot, interpolate between the two to generate our CurrentSnapshot
						float interpolationFactor = (float)(m_TargetRecordingTime - previousSnapshot.TimeStamp).Ticks / (float)(referenceSnapshot.TimeStamp - previousSnapshot.TimeStamp).Ticks;

						CurrentSnapshot = new DataSnapshot();
						CurrentSnapshot.TimeStamp = m_TargetRecordingTime;

						List<String> keys = referenceSnapshot.Data.Keys.ToList();
						foreach (String key in keys)
							CurrentSnapshot.Data.Add(key, previousSnapshot[key] * (1 - interpolationFactor) + referenceSnapshot[key] * interpolationFactor);
					}
					else
					{
						//	Only time previousSnapshot would be null is if referenceSnapshot.TimeStamp == m_TargetRecordingTime; aka, if referenceSnapshot
						//		is the exact time-point we're looking for.

						CurrentSnapshot = referenceSnapshot;
					}
				}

				if (FrameChanged != null)
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

		public StreamingPlaybackDataStream(String sourceFile, SensorClusterConfiguration sensorConfigurations, FaceData.CoordinateSystemType sourceCoordinateSystem)
		{
			CoordinateSystem = sourceCoordinateSystem;
			Sensors = sensorConfigurations;

			IEnumerable<String> linesEnumerable = File.ReadLines(sourceFile);

			NumberOfSnapshots = -1;

			String headerString;
			String firstLineData;
			String lastLineData;

			headerString = linesEnumerable.First();
			firstLineData = linesEnumerable.ElementAt(1);

			m_DataHeaders = headerString.Split(',');
			for (int i = 0; i < m_DataHeaders.Length; i++)
				m_DataHeaders[i] = m_DataHeaders[i].Trim();

			//m_SourceFileStream = new StreamReader(sourceFile, Encoding.UTF8, false, 100000);
			m_SourceFileStream = File.OpenText(sourceFile);

			//	Do some stream manipulation to cheaply get the last data line
			m_SourceFileStream.BaseStream.Seek(-30000, SeekOrigin.End);
			List<String> remainingLines = new List<string>();
			while (!m_SourceFileStream.EndOfStream)
				remainingLines.Add(m_SourceFileStream.ReadLine());

			lastLineData = remainingLines.Last();

			m_SourceFileStream.BaseStream.Seek(0, SeekOrigin.Begin);
			//	Move past the header line
			m_SourceFileStream.ReadLine();
			//	Technically since CurrentData is the first line, SourceFileStream should be set to read the second line next
			m_SourceFileStream.ReadLine();

			FirstSnapshot = ParseSnapshotFromString(m_DataHeaders, firstLineData);
			LastSnapshot = ParseSnapshotFromString(m_DataHeaders, lastLineData);
			CurrentSnapshot = ParseSnapshotFromString(m_DataHeaders, firstLineData);

			m_TargetRecordingTime = FirstSnapshot.TimeStamp;
		}

		public delegate void FrameChangedEventHandler(StreamingPlaybackDataStream instance, DataSnapshot newFrame);
		public event FrameChangedEventHandler FrameChanged;

		public DataSnapshot CurrentSnapshot { get; private set; }

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
			if (PlaybackIsComplete)
				return;

			try
			{
				var nextLine = m_SourceFileStream.ReadLine();
				CurrentSnapshot = ParseSnapshotFromString(m_DataHeaders, nextLine);

				PlaybackIsComplete = m_SourceFileStream.EndOfStream;
			}
			catch (Exception e)
			{
				PlaybackIsComplete = true;
			}

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

		private DataSnapshot ParseSnapshotFromString(String[] headers, String sourceData)
		{
			String[] dataValues = sourceData.Split(',');
			DataSnapshot newSnapshot = new DataSnapshot();
			for (int i = 0; i < dataValues.Length - 1; i++)
			{
				if (dataValues[i].Trim().Length == 0) continue;

				switch (headers[i])
				{
					case ("Time (UTC)"):
						{
							DateTime usableTime = DateTime.Parse(dataValues[i]).ToUniversalTime();
							newSnapshot.TimeStamp = usableTime;
							break;
						}

					default:
						{
							newSnapshot[headers[i]] = double.Parse(dataValues[i]);
							break;
						}
				}
			}

			return newSnapshot;
		}
	}
}
