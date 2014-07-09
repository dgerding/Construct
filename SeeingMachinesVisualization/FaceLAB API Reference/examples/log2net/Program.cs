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
using System.IO;
using sm.eod;
using sm.eod.io;

namespace logtonet
{
    class Program
    {
        static void sendEngineOutputDataFile(string dirpath, DatagramSocket socket)
        {
            VectorInt8 objectsToRead = new VectorInt8();
            objectsToRead.Add((sbyte)TimingOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)HeadOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)EyeOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)WorldOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)FeatureSetsByCamera.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)CustomEventOutputData.THIS_OBJECT_ID);
            objectsToRead.Add((sbyte)GPSOutputDataV2.THIS_OBJECT_ID);

            using (EngineOutputDataFileReader reader = new EngineOutputDataFileReader())
            {
                reader.OpenFiles(objectsToRead, dirpath);
                VectorUInt8 buffer = new VectorUInt8();
                while (!reader.EndOfData())
                {
                    EngineOutputData data = reader.ReadData();
                    buffer.Clear();
                    data.Serialize(buffer);
                    socket.SendDatagram(buffer);
                    Console.Write('.');
                    Console.Out.Flush();
                    System.Threading.Thread.Sleep(15);
                }

            }
        } // end of sendEngineOutputDataFile

        static void sendData(string filepath, string host, int udpPort)
        {
            InetAddress address = new InetAddress(udpPort, host);
            using (DatagramSocket socket = new DatagramSocket())
            {
                socket.ConnectSocket(address);
                Console.WriteLine("Connected to {0}, port {1}...", address.GetHostIP(), address.GetPort());
                Console.WriteLine(@"Sending {0}\*.fll", filepath);
                sendEngineOutputDataFile(filepath, socket);
            }
        } // end of sendData


        static void Main(string[] args)
        {
            const int EXIT_SUCCESS = 0;
            const int EXIT_FAILURE = 1;
            try
            {

                if (args.Length < 1 || args.Length > 3)
                {
                    Console.Error.WriteLine("Bad number of arguments\n"
                        + "Usage: log2net [file_dir] [ hostname | IP address ] [udp port number]\n"
                        + "Reads faceLAB binary logfiles, and sends them to the host address.\n"
                        + "By default, hostname = localhost and port number = 2001.\n"
                        + "For the folder name provided log2net assumes it contains multiple logfiles.\n"
                        + "Data from these multiple files are merged into a single network packet.\n"
                        + "Logfiles must have file extension .fll\n"
                        + "Host must exist -> try ping first!\n");

                    System.Environment.Exit(EXIT_FAILURE);
                }
                int udpPort = eodio.DEFAULT_PORT_NUMBER;
                if (args.Length == 3)
                {
                    try
                    {
                        udpPort = int.Parse(args[2]);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("Failed to parse UDP port\n{0}", e.Message);
                        System.Environment.Exit(EXIT_FAILURE);
                    }
                    if (udpPort < eodio.MINIMUM_PORT_NUMBER || udpPort > eodio.MAXIMUM_PORT_NUMBER)
                    {
                        Console.Error.WriteLine("UDP port numbers must be between {0} and {1}.",
                            eodio.MINIMUM_PORT_NUMBER, eodio.MAXIMUM_PORT_NUMBER);
                        System.Environment.Exit(EXIT_FAILURE);
                    }
                }
                string host = args.Length >= 2 ? args[1] : "localhost";
                string filepath = args[0];
                if (!Directory.Exists(filepath))
                {
                    Console.Error.WriteLine("Path '{0}' is not a directory", filepath);
                    System.Environment.Exit(EXIT_FAILURE);
                }
                try
                {
                    sendData(filepath, host, udpPort);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed to send data\n{0}", e.Message);
                    System.Environment.Exit(EXIT_FAILURE);
                }

                System.Environment.Exit(EXIT_SUCCESS);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}", e.Message);
                System.Environment.Exit(EXIT_FAILURE);
            }
        }
    }
}

