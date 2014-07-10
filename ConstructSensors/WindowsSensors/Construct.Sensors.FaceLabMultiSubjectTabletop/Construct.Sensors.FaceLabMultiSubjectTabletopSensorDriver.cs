using System;
using Construct.Dataflow.Brokering;

namespace Construct.Sensors.FaceLabMultiSubjectTabletop
{
    class Construct.Sensors.FaceLabMultiSubjectTabletopDriver
    {
        static void Main(string[] args)
        {
            Construct.Sensors.FaceLabMultiSubjectTabletopSensor obj = new Construct.Sensors.FaceLabMultiSubjectTabletopSensor(args);
            Console.ReadLine();
        }
    }
}
