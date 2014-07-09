/*
    Copyright (C) 2008 Seeing Machines Ltd. All rights reserved.

    This file is part of the CoreData API.

    This file may be distributed under the terms of the SM Non-Commercial License
    Agreement appearing in the file LICENSE.TXT included in the packaging
    of this file.

    This file is provided AS IS with NO WARRANTY OF ANY KIND, INCLUDING THE
    WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

    Further information about the CoreData API is available at:
    http://www.seeingmachines.com/

*/
using System;
using System.Collections.Generic;
using System.Text;
using sm.eod;
using sm.eod.io;

namespace netclient
{
    class Program
    {
        static void handleEngineOutputData(EngineOutputData outputData, InetAddress from)
        {
            // Replace this code with your own
            System.Console.WriteLine(outputData.HeaderText());
            System.Console.WriteLine(outputData.LineText());
        }

        static void handleEngineOutputData(Serializable outputData, InetAddress from)
        {
            // Replace this code with your own
            System.Console.WriteLine(outputData.HeaderText());
            System.Console.WriteLine(outputData.LineText());
        }

        static void Main(string[] args)
        {
            const int EXIT_SUCCESS = 0;
            const int EXIT_FAILURE = 1;

            try
            {
                VectorUInt8 buffer = new VectorUInt8();
                InetAddress from = new InetAddress();
                DatagramSocket input_socket = new DatagramSocket(13001);

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
                            //EngineOutputData eod = serializable as EngineOutputData;

                            if (serializable != null)
                            {
                                handleEngineOutputData(serializable, from);
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
                System.Environment.Exit(EXIT_FAILURE);
            }
            System.Environment.Exit(EXIT_SUCCESS);
        }
    }
}

