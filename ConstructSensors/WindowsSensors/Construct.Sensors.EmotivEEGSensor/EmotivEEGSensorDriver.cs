using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Construct.Sensors.EmotivEEGSensor
{
    class EmotivEEGSensorDriver
    {
        /// <summary>
        /// Entry point for sensor executable
        /// </summary>
        static void Main(string[] args)
        {
            // create a non-static instance for consumption as singleton
            EmotivEEGSensor obj = new EmotivEEGSensor(args);

           // EventLog.WriteEntry("RandomNumberSensor", "Loaded Sensor");

            Console.ReadLine();
        }
    }
}
