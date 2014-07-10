using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Construct.Sensors.AVStreamSensor
{
    public class AVStreamSensorDriver
    {
        static void Main(string[] args)
        {
            AVStreamSensor obj = new AVStreamSensor(args);
            
            Console.ReadLine();
        }
    }
}
