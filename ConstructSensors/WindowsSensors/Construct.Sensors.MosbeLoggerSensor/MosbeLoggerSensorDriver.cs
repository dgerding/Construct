using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace Construct.Sensors.MosbeLoggerSensor
{
    public class MosbeLoggerSensorDriver
    {
        static void Main(string[] args)
        {
            MosbeLoggerSensor obj = new MosbeLoggerSensor(args);
            Console.ReadLine();
        }
    }
}
