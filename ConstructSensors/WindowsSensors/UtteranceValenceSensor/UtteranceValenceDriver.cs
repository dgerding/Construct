using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Construct.Sensors.UtteranceValenceSensor
{
    class UtteranceValenceDriver
    {
        static void Main(string[] args)
        {

            UtteranceValenceSensor sensor = new UtteranceValenceSensor(args);
            Console.ReadLine();
            //byte[] audioBytes, textBytes, response;

            //WaveReader waveReader = new WaveReader(File.OpenRead(@"C:\C2CC\Construct2\ConstructSensors\WindowsSensors\UtteranceValenceSensor\bin\Debug\happy.wav"));
            //audioBytes = waveReader.ReadData();

            //ASCIIEncoding encoding = new ASCIIEncoding();
            //textBytes = encoding.GetBytes("This is happy valence");

            //OpenSmileClient client = new OpenSmileClient();
            //response = client.Transmit(audioBytes, textBytes);

            //sTcpWaveSourceHeader packedResponse = new sTcpWaveSourceHeader();
            //packedResponse.packResponse(response);
        }
    }
}
