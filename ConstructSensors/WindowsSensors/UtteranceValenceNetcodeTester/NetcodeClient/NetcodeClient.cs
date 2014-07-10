using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alvas.Audio;
using System.IO;
using Construct.Sensors.UtteranceValenceSensor;

namespace NetcodeClient
{
    class NetcodeClient
    {
        static void Main(string[] args)
        {
            const string fileName = "_shakespear.wav";
            const string text = "But that the fear of something after death, the undiscovered country from whose borne no traveler returns";

            string[] sensorArgs = new string[3] { "19cc6eb1-466e-4901-8877-0e28e3ea0dee", "http://skynet/f721f879-9f84-412f-ae00-632cfea5a1f3/00000000-0000-0000-0000-000000000000", "http://skynet/f721f879-9f84-412f-ae00-632cfea5a1f3/1f0ab154-5e32-410a-9305-6a03ffb6996c" };
            UtteranceValenceSensor sensor = new UtteranceValenceSensor(sensorArgs);
            WaveReader waveReader = new WaveReader(File.OpenRead(fileName));
            byte[] wavFile = waveReader.ReadData();
            byte[] returned = sensor.client.Transmit(wavFile, text);
            byte[] returned2 = sensor.client.Transmit(wavFile, text);
            byte[] returned3 = sensor.client.Transmit(wavFile, text);
        }
    }
}
