using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Alvas.Audio;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

namespace UtteranceValenceServer
{
    public class UtteranceValenceServer
    {
        static void Main(string[] args)
        {
            //begin server hosting
            TcpListener server = null;
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);

            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                byte[] inboundWavePacketHeader = new byte[60];
                int textLength, waveDataLength;
                byte[] textBytes, wavDataBytes;
                while (true)
                {
                    int bytesRead = 0;
                    int wavBytesRead = 0;

                    try
                    {
                        bytesRead = stream.Read(inboundWavePacketHeader, 0, inboundWavePacketHeader.Length);

                        if (bytesRead == inboundWavePacketHeader.Length)
                        {
                            textLength = BitConverter.ToInt32(inboundWavePacketHeader, 16);
                            waveDataLength = BitConverter.ToInt32(inboundWavePacketHeader, 20);
                            textBytes = new byte[textLength];
                            wavDataBytes = new byte[waveDataLength];

                            bytesRead += stream.Read(textBytes, 0, textLength);
                            //bytesRead += stream.Read(wavDataBytes, 0, waveDataLength);

                            while (wavBytesRead != waveDataLength)
                            {
                                wavBytesRead += stream.Read(wavDataBytes, wavBytesRead, (waveDataLength - wavBytesRead));
                            }
                            bytesRead += wavBytesRead;
                        }

                    }
                    catch
                    {
                        //Console.WriteLine("A socket error has occured");
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("The client has disconnected from the server");
                        break;
                    }
                    stream.Write(inboundWavePacketHeader, 0, inboundWavePacketHeader.Length);
                }
                stream.Close();
                client.Close();
            }
        }
    }

    [Serializable]
    public struct sTcpWaveSourceHeader
    {
        public uint magicHeader;
        public int headerSize; // size of header in uint32_t  (= size in bytes / 4)
        public int type;
        public int ID;
        public int textLength;
        public int dataLength; // length in bytes of the following data 
        public int checksum;  // sum of all header fields
        public ReserveredBuffer reservedByteBuffer;

        public void setHeaderInfo(TCPWAVESOURCE_CHUNKTYPE chunkType, int theID, int textLen, int dataLen)
        {
            magicHeader = uint.MaxValue;
            headerSize = 15;
            type = (int)chunkType;
            ID = theID;
            textLength = textLen;
            dataLength = dataLen;
            checksum = headerSize + type + ID + textLength + dataLength;
        }
    }

    [Serializable]
    public struct ReserveredBuffer
    {
        public float b0;
        public int b1, b2, b3, b4, b5, b6, b7;
    }

    public enum TCPWAVESOURCE_CHUNKTYPE { TCPWAVESOURCE_CHUNK_DATA = 1, TCPWAVESOURCE_CHUNK_DATAANDTEXT = 3, TCPWAVESOURCE_CHUNK_CONTROL = 11, TCPWAVESOURCE_CHUNK_RESULT = 21 }
}
