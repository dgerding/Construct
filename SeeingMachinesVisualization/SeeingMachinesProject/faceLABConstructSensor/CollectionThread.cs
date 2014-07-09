using SMFramework;
using System;
using System.Threading.Tasks;

namespace faceLABConstructSensor
{
	static class CollectionThread
	{
		private static Task m_Task = null;

		private static bool m_IsRunning = false;
		public static bool IsRunning
		{
			get { return m_IsRunning; }
			private set { m_IsRunning = value; }
		}

		private static FaceLabSignalConfiguration m_Signal = DefaultSignalConfiguration.Config;
		public static FaceLabSignalConfiguration SourceSignal
		{
			get { return m_Signal; }
			set { if (!IsRunning) m_Signal = value; }
		}

		private static bool m_UseConstantPoll = false;
		public static bool UseConstantPoll
		{
			get { return m_UseConstantPoll; }
			set { if (!IsRunning) m_UseConstantPoll = value; }
		}

		public static void Start()
		{
			if (m_Task != null)
				return;

			IsRunning = true;
			m_Task = Task.Run((Action)ThreadFunc);
		}

		public static void Stop()
		{
			if (m_Task == null)
				return;

			IsRunning = false;
			m_Task.Wait();
			m_Task = null;
		}

		public delegate void OnNewDataHandler(FaceData newData);
		public static OnNewDataHandler OnNewData;

		public delegate void OnErrorHandler(Exception error);
		public static OnErrorHandler OnError;

		private static void ThreadFunc()
		{
			FaceLabConnection connection;

			try
			{
				connection = new FaceLabConnection(SourceSignal);
			}
			catch (Exception ex)
			{
				if (OnError != null)
					OnError(ex);
				return;
			}

			while (IsRunning && connection.IsValid)
			{
				if (connection.RetrieveNewData())
				{
					if (OnNewData != null)
						OnNewData(connection.CurrentData);

#if DEBUG
					//	Limit the send rate in Debug mode; connection.RetrieveNewData() will ALWAYS return
					//	true in Debug, which will cause an infinite flooding of data over the Construct transport.
					System.Threading.Thread.Sleep(10);
#endif
					continue;
				}

				if (m_UseConstantPoll)
					continue;
				else
					//	Sleep for the shortest amount of time
					System.Threading.Thread.Sleep(1);
			}

			connection.Disconnect();
			connection = null;
			GC.Collect();
		}
	}
}
