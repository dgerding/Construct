using System;
using Construct.Sensors.DragonTranscription;

namespace DragonTranscription
{
    class DragonTranscriptionDriver
    {
        static void Main(string[] args)
        {
			System.Diagnostics.Debugger.Launch();
			System.Diagnostics.Debugger.Break();

            DragonTranscriptionSensor obj = new DragonTranscriptionSensor(args);
            Console.ReadLine();
        }
    }
}
