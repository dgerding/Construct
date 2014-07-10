using System;

namespace FaceLab
{
    class FaceLabDriver
    {
        static void Main(string[] args)
        {
            FaceLabSensor obj = new FaceLabSensor(args);
            Console.ReadLine();
        }
    }
}
