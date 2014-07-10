using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using sm.eod;
using sm.eod.io;

namespace FaceLab
{
    class FaceLabDataCollector
    {
        const int 
            EXIT_SUCCESS = 0;
        const int 
            EXIT_FAILURE = 1;


        public delegate void FaceLabDataReceivedEventHandler(HeadInfo dataReceived);
        public event FaceLabDataReceivedEventHandler 
            OnDataReceived;

        public int
            targetSocket;

        public FaceLabDataCollector()
        {
            targetSocket = 2001;
        }

        public void SetTargetSocket(int socket)
        {
            targetSocket = socket;
        }

        public void Run()
        {
            try
            {
                VectorUInt8 buffer = new VectorUInt8();
                InetAddress from = new InetAddress();
                DatagramSocket input_socket = new DatagramSocket(targetSocket);

                while (true)
                {
                    buffer.Clear();

                    input_socket.ReceiveDatagram(buffer, from);
                    int pos = 0;

                    // Extract all the objects from the buffer
                    while (pos < buffer.Count)
                    {
                        // Check for an EngineOutputData object in the buffer
                        using (Serializable serializable = SerializableFactory.NewObject(buffer, ref pos))
                        {
                            EngineOutputData eod = EngineOutputData.Downcast(serializable);

                            if (eod != null)
                            {
                                EngineOutputData outputData = eod;

                                VectorDouble positionOne = outputData.HeadOutputData().HeadPosition();
                                VectorDouble positionTwo = outputData.HeadOutputData().HeadEyeBallPos(EyeId.LEFT_EYE);
                                VectorDouble positionThree = outputData.HeadOutputData().HeadEyeBallPos(EyeId.RIGHT_EYE);

                                HeadInfo visualizationStruct = new HeadInfo((float)positionOne[0], (float)positionOne[1], (float)positionOne[2],
                                    (float)positionTwo[0], (float)positionTwo[1], (float)positionTwo[2], 
                                    (float)positionThree[0], (float)positionThree[1], (float)positionThree[2], 0, 0, 0); 

                                OnDataReceived(visualizationStruct);

                            }
                            else
                            {
                                System.Console.WriteLine("\nUnrecognised packet received, header id: {0}\n", (int)buffer[0]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine("{0}", e.Message);
            }
        }
    }
}
