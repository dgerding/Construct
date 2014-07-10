using System;

namespace Pretend
{
	class PretendDriver
	{
		static void Main(string[] args)
		{
			//	For debugging (invokes debugger at launch for immediate debugging)
			//System.Diagnostics.Debugger.Launch();
			//System.Diagnostics.Debugger.Break();

			PretendSensor obj = new PretendSensor(args);
			Console.ReadLine();
		}
	}
}
