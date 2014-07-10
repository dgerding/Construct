using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Alvas.Audio;

namespace NetcodeClient
{
    class Client_old
    {
        static void Main(string[] args)
        {
            //if (!File.Exists(args[0]) || !File.Exists(args[1]))
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Inproper argument format: One or both filenames provided was not found");
                //Console.WriteLine("Proper Usage:\"UtteranceValenceNetcodeTester.exe <Utterance file> <Text file> <openSMILE URI>\"");
                Console.WriteLine("Proper Usage:\"UtteranceValenceNetcodeTester.exe <Utterance file>\"");
            }

            const string DEFAULT_TEXT = "Hello World";

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), 13000);
            NetworkStream stream = client.GetStream();

            WaveReader waveReader = new WaveReader(File.OpenRead(args[0]));
            byte[] wavFile = waveReader.ReadData();

            sTcpWaveSourceHeader header = new sTcpWaveSourceHeader();
            header.setHeaderInfo(TCPWAVESOURCE_CHUNKTYPE.TCPWAVESOURCE_CHUNK_DATAANDTEXT, 1, DEFAULT_TEXT.Length, wavFile.Length);

            byte[] outboundWavePacket = new byte[(header.headerSize * 4) + DEFAULT_TEXT.Length + wavFile.Length];
            byte[] outboundTextString = new byte[DEFAULT_TEXT.Length];
            for (int i = 0; i < DEFAULT_TEXT.Length; ++i)
            {
                try
                {
                    outboundTextString[i] = Convert.ToByte(DEFAULT_TEXT[i]);
                }
                catch
                {
                    outboundTextString[i] = Convert.ToByte((Convert.ToInt16(DEFAULT_TEXT[i]) % 256));
                }
            }

            System.Buffer.BlockCopy(GetBytes(header), 0, outboundWavePacket, 0, (header.headerSize * 4));
            System.Buffer.BlockCopy(outboundTextString, 0, outboundWavePacket, (header.headerSize * 4), DEFAULT_TEXT.Length);
            System.Buffer.BlockCopy(wavFile, 0, outboundWavePacket, (header.headerSize * 4) + DEFAULT_TEXT.Length, wavFile.Length);

            stream.Write(outboundWavePacket, 0, outboundWavePacket.Length);
            byte[] response = new byte[60];
            int bytesRead;
            if (stream.CanRead)
            {
                while (true)
                {
                    try
                    {
                        bytesRead = stream.Read(response, 0, response.Length);
                    }
                    catch
                    {
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("The server did not respond");
                        break;
                    }
                    stream.Close();
                    client.Close();
                    Console.WriteLine("Magic header:" + BitConverter.ToUInt32(response, 0));
                    Console.WriteLine("Header size:" + BitConverter.ToInt32(response, 4));
                    Console.WriteLine("Type:" + BitConverter.ToInt32(response, 8));
                    Console.WriteLine("ID:" + BitConverter.ToInt32(response, 12));
                    Console.WriteLine("Text Length:" + BitConverter.ToInt32(response, 16));
                    Console.WriteLine("Data Length:" + BitConverter.ToInt32(response, 20));
                    Console.WriteLine("Checksum:" + BitConverter.ToInt32(response, 24));
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Error: Stream is not readable");
            }
        }

        public static byte[] GetBytes(sTcpWaveSourceHeader header)
        {
            byte[] ret = new byte[60];
            Array.Copy(BitConverter.GetBytes(header.magicHeader), 0, ret, 0, 4);
            Array.Copy(BitConverter.GetBytes(header.headerSize), 0, ret, 4, 4);
            Array.Copy(BitConverter.GetBytes(header.type), 0, ret, 8, 4);
            Array.Copy(BitConverter.GetBytes(header.ID), 0, ret, 12, 4);
            Array.Copy(BitConverter.GetBytes(header.textLength), 0, ret, 16, 4);
            Array.Copy(BitConverter.GetBytes(header.dataLength), 0, ret, 20, 4);
            Array.Copy(BitConverter.GetBytes(header.checksum), 0, ret, 24, 4);
            Array.Copy(GetBytes(header.reservedByteBuffer), 0, ret, 28, 32);

            return ret;
        }

        public static byte[] GetBytes(ReserveredBuffer buffer)
        {
            byte[] ret = new byte[32];
            Array.Copy(BitConverter.GetBytes(buffer.b0), 0, ret, 0, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b1), 0, ret, 4, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b2), 0, ret, 8, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b3), 0, ret, 12, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b4), 0, ret, 16, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b5), 0, ret, 20, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b6), 0, ret, 24, 4);
            Array.Copy(BitConverter.GetBytes(buffer.b7), 0, ret, 28, 4);
            return ret;
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
