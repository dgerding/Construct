using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMFramework
{
	public class IsolatedDataCaptureThread
	{
		Task m_CaptureTask;
		bool m_ShouldCapture;
		PairedDatabase m_Database;
		bool m_AutoTransform;

		public int DataPollsPerSecond = 100;

		public bool WasSuccessful
		{
			get;
			private set;
		}

		public static IsolatedDataCaptureThread StartNew(SensorClusterConfiguration sourceSensors, PairedDatabase targetDatabase, bool autoTransformData)
		{
			return new IsolatedDataCaptureThread(sourceSensors, targetDatabase, autoTransformData);
		}

		private void CaptureData()
		{

			FaceLabConnection[] connections;

			lock (Sensors)
			{
				connections = new FaceLabConnection[Sensors.SensorConfigurations.Count];

				for (int i = 0; i < connections.Length; i++)
				{
#if DEBUG
					connections[i] = new FaceLabConnection(Sensors.SensorConfigurations[i].Label);
#else
					connections[i] = new FaceLabConnection(Sensors.SensorConfigurations[i].Port, Sensors.SensorConfigurations[i].Label);
#endif
				}
			}

			bool exitedDueToFailure = false;
			DateTime previousPollTime = DateTime.Now;
			while (m_ShouldCapture && !exitedDueToFailure)
			{
				double pollTimeDifference = (DateTime.Now - previousPollTime).TotalMilliseconds;
				
				if (pollTimeDifference < 1000.0 / DataPollsPerSecond)
					System.Threading.Thread.Sleep((int)(1000.0 / DataPollsPerSecond - pollTimeDifference));

				previousPollTime = DateTime.Now;

				lock (Sensors) lock (m_Database)
				{
					m_Database.BeginNextSnapshot();

					for (int i = 0; i < Sensors.SensorConfigurations.Count; i++)
					{
						if (!connections[i].IsValid)
						{
							exitedDueToFailure = true;
							break;
						}

						connections[i].RetrieveNewData();
						if (m_AutoTransform)
						{
							FaceData transformedData =
											SeeingModule.EvaluateCameraData(
																Sensors.SensorConfigurations[i],
																connections[i].CurrentData,
																m_AutoTransform
																);

							FaceDataSerialization.WriteFaceDataToDatabase(transformedData, m_Database);
						}
						else
						{
							FaceDataSerialization.WriteFaceDataToDatabase(connections[i].CurrentData, m_Database);
						}
					}
				}
			}

			if (exitedDueToFailure)
			{
				WasSuccessful = false;
				m_ShouldCapture = false;
			}
			else
			{
				WasSuccessful = true;
			}

			foreach (FaceLabConnection connection in connections)
			{
				connection.Disconnect();
			}

			connections = null;
			GC.Collect();
		}

		internal IsolatedDataCaptureThread(SensorClusterConfiguration sensors, PairedDatabase targetDatabase, bool autoTransformData)
		{
			m_ShouldCapture = true;
			m_AutoTransform = autoTransformData;

			m_Database = targetDatabase;
			Sensors = sensors;

			m_CaptureTask = Task.Factory.StartNew((Action)CaptureData);

			WasSuccessful = true;
		}

		public void StopCapture(bool wait)
		{
			m_ShouldCapture = false;
			if (wait)
				Wait();
		}

		public void Wait()
		{
			m_CaptureTask.Wait();
		}

		public SensorClusterConfiguration Sensors
		{
			get;
			private set;
		}
	}
}
