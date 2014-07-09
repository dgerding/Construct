using System;
using System.Diagnostics;

#if !TEST_LOGIC
namespace faceLABConstructSensor
{
	class Program
	{
		static void Main(string[] args)
		{
			//Debugger.Launch();

			faceLABSensorWrapper sensor = new faceLABSensorWrapper(args);
			Console.ReadLine();
		}
	}
}
#endif