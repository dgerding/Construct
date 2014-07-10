using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Net.Sockets;
using System.Net;

namespace Construct.Sensors._TEMP_UDPWebcamStreamer
{
    public class UDPCamStream
    {
        static void Main(string[] args)
        {
            UdpClient UDPClient = new UdpClient("127.0.0.1", 11000);
            Capture capture = new Capture(0);
            byte[] frameMarkerBytes = new byte[10] { 0, 255, 85, 85, 222, 173, 221, 238, 170, 221 };
            byte[] outboundBytes = new byte[1500];
            while (true)
            {
                using (Image<Bgr, byte> nextFrame = capture.QueryFrame())
                {
                    UDPClient.Send(frameMarkerBytes, frameMarkerBytes.Length);
                    int frameLength = nextFrame.Bytes.Length;
                    int beginBytes = 0;
                    int endBytes = 0;
                    do
                    {
                        if (beginBytes + 1500 < frameLength)
                        {
                            endBytes = beginBytes + 1500;
                        }
                        else
                        {
                            endBytes = frameLength;
                        }

                        Buffer.BlockCopy(nextFrame.Bytes, beginBytes, outboundBytes, 0, endBytes - beginBytes);

                        UDPClient.Send(outboundBytes, outboundBytes.Length);
                        beginBytes = endBytes;
                    }
                    while (beginBytes < nextFrame.Bytes.Length);
                    Console.WriteLine("Number bytes sent in image " + beginBytes);
                }
            }
        }
    }
}
