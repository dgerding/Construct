using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Sensors.TextDetectorSensor
{
    public class TextDetectorSensorDriver
    {
        static void Main(string[] args)
        {
            TextDetectorSensor obj = new TextDetectorSensor(args);

            Console.ReadLine();
        }
    }
}
