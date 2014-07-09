using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//	Define TEST_LOGIC as a macro to do the testing
#if TEST_LOGIC
namespace faceLABConstructSensor
{
	class Program
	{
		static void Main(string[] args)
		{
			CollectionThread.Start();

			Console.ReadLine();
			CollectionThread.Stop();
		}
	}
}
#endif