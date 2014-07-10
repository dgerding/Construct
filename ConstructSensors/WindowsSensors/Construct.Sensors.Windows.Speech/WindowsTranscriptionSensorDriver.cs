using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Construct.Sensors.WindowsTranscriptionSensor
{
    class WindowsTranscriptionSensorDriver
    {
        static void Main(string[] args)
        {
            WindowsTranscriptionSensor obj = new WindowsTranscriptionSensor(args);

            Console.ReadLine();
        }
    }
}
