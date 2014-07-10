using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    class DragonTranscriptionSensorDriver
    {
        static void Main(string[] args)
        {
            DragonTranscriptionSensor obj = new DragonTranscriptionSensor(args);

            Console.ReadLine();
        }
    }
}
