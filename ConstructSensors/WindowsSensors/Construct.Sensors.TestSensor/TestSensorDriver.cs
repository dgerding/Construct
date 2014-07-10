using System;

namespace Construct.Sensors.TestSensor
{
    class TestSensorDriver
    {
        static void Main(string[] args)
        {
            TestSensor obj = new TestSensor(args);
            Console.ReadLine();
        }
    }
}