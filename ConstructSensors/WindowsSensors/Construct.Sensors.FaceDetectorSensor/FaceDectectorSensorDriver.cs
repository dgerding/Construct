using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Sensors.FaceDetectorSensor
{
    public class FaceDetectorSensorDriver
    {
        static void Main(string[] args)
        {
            FaceDetectorSensor obj = new FaceDetectorSensor(args);

            Console.ReadLine();
        }
    }
}
