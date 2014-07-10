using System;
using System.IO;
using Alvas.Audio;

namespace SapiSensor
{
    class SapiSensorDriver
    {
        static void Main(string[] args)
        {
#if TEST_LOGIC
            SapiBase sapi = new SapiBase();
            sapi.OnTranscribed += sapi_OnTranscribed;
            sapi.Start();

			String filename = "utterance1.wav";

			byte[] fileData = new byte[new FileInfo(filename).Length];
			File.OpenRead(filename).Read(fileData, 0, fileData.Length);
			sapi.ProcessUtterance(fileData);

            Console.ReadLine();
            
            sapi.Stop();
#else
            SapiSensorSensor obj = new SapiSensorSensor(args);
            Console.ReadLine();
#endif
        }

        static void sapi_OnTranscribed(Transcription args)
        {
            Console.WriteLine(args.TranscribedText);
        }
    }
}
