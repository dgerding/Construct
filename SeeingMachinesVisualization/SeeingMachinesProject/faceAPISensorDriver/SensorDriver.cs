using System;
using System.Diagnostics;

namespace faceAPISensorDriver
{
	class faceAPISensorDriverDriver
	{
		static void Main(string[] args)
		{
#if !TEST_LOGIC
			faceAPISensorDriverSensor obj = new faceAPISensorDriverSensor(args);
			Console.ReadLine();
#else
			faceAPISensorLogic sensorLogic = new faceAPISensorLogic();
			sensorLogic.OnNewHeadPose += sensorLogic_OnNewHeadPose;
			sensorLogic.Start();

			while (Console.ReadKey(true).Key != ConsoleKey.Q)
				System.Threading.Thread.Sleep(20);
			//SensorWrapper.FaceRecognizer.FaceDataAvailable();

			sensorLogic.Stop();
#endif
		}

		static void sensorLogic_OnNewHeadPose(SensorWrapper.HeadPose data)
		{
			Console.WriteLine(data);
		}
	}
}
