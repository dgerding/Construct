using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Sensors.MSSqlSensor
{
    class Program
    {
        static void Main(string[] args)
        {
            MSSqlSensor sensor = new MSSqlSensor(args);
            Console.ReadLine();
        }
    }
}
