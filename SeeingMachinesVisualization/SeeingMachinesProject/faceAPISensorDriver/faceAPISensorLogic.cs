using System;
using System.Threading.Tasks;
using faceAPISensorDriver.SensorWrapper;

namespace faceAPISensorDriver
{
	class faceAPISensorLogic
	{
		public delegate void OnNewHeadPoseDataHandler(HeadPose data);
		public event OnNewHeadPoseDataHandler OnNewHeadPose;

		Task m_FacePollTask;
		bool m_ContinuePoll = false;
		private static void FacePollThread(object data)
		{
			faceAPISensorLogic logicInstance = data as faceAPISensorLogic;

			while (logicInstance.m_ContinuePoll)
			{
				if (SensorWrapper.FaceRecognizer.HeadPoseDataAvailable())
				{
					HeadPose poseData = SensorWrapper.FaceRecognizer.GetCurrentHeadPose();
					if (logicInstance.OnNewHeadPose != null)
						logicInstance.OnNewHeadPose(poseData);

					continue;
				}

				System.Threading.Thread.Sleep(10);
			}
		}

		public void Start()
		{
			SensorWrapper.FaceRecognizer.Start();
			m_ContinuePoll = true;
			m_FacePollTask = Task.Factory.StartNew((Action<object>)FacePollThread, this);
		}

		public void Stop()
		{
			m_ContinuePoll = false;
			m_FacePollTask.Wait();
			SensorWrapper.FaceRecognizer.Stop();
		}

	}
}
