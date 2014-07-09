using System;

namespace faceAPISensorCommercialCS
{
	class faceAPISensorCommercialCSDriver
	{
		static void Main(string[] args)
		{
//#if TEST_LOGIC
//			faceAPI4SensorLogic logic = new faceAPI4SensorLogic();
//			logic.OnNewData += logic_OnNewData;
//			logic.Start();
//			while (Console.ReadKey().Key != ConsoleKey.Escape)
//			{

//			}
//			logic.Stop();
//			return;
//#else
			//bool loop = true;
			//while (loop)
			//{
			//	System.Threading.Thread.Sleep(1);
			//}
			faceAPISensorCommercialCSSensor obj = new faceAPISensorCommercialCSSensor(args);
			Console.ReadLine();
//#endif
		}

		static void logic_OnNewData(SMFramework.FaceData faceData)
		{
			Console.WriteLine("Got data");
		}
	}
}
