using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Construct.Sensors;
using Construct.MessageBrokering;
using Newtonsoft.Json;

namespace Construct.Sensors.UtteranceValenceSensor
{
    public class UtteranceValenceSensor : Sensor
    {
        private Process smileServer;
        private OpenSmileClient client;
        private List<Data> waveBytesList, textList;
        private readonly Dictionary<string, string> availableCommands;
        private Rendezvous<Data> utteranceDataRendezvous, dragonTranscriptionDataRendezvous;
        public Broker Broker { get { return broker; } }

        public UtteranceValenceSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("CB9CD3E2-5EA9-485A-ABCF-9ADDB4018A47"), new Dictionary<string, Guid>{ {"UtteranceValence", Guid.Parse("2E3B1152-F926-4EC7-B060-1437F1684B27")} })
        {
            availableCommands = new Dictionary<string, string>();
            SendAvailableCommandsTelemetry();

            #region Pre-create utterance/Dragon transcription Rendezvous
            utteranceDataRendezvous = new Rendezvous<Data>(Protocol.HTTP, Rendezvous<Data>.GetHostName(), Guid.Parse("3FE979D6-6D8C-4954-97CD-D49B43323AE6"), SourceID);
            dragonTranscriptionDataRendezvous = new Rendezvous<Data>(Protocol.HTTP, Rendezvous<Data>.GetHostName(), Guid.Parse("1027AB45-7059-4BD0-84F0-ED2AE472B70A"), SourceID);
            #endregion
            broker.AddPeer(new Inbox<Data>(utteranceDataRendezvous));
            broker.AddPeer(new Inbox<Data>(dragonTranscriptionDataRendezvous));

            broker.OnCommandReceived += broker_OnCommandReceived;
            broker.OnItemReceived += broker_OnItemReceived;

            TurnOnLocalSMILExtractServer();
            client = new OpenSmileClient();
            waveBytesList = new List<Data>();
            textList = new List<Data>();
        }

        ~UtteranceValenceSensor()
        {
            smileServer.Kill();
        }
        private void TurnOnLocalSMILExtractServer()
        {
            // if process is not in memory
            smileServer = new Process();
            string args = @"-C .\emo_valence_net.conf";

            smileServer.StartInfo.Arguments = args;
            smileServer.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            smileServer.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "SMILExtract.exe");
            //smileServer.StartInfo.UseShellExecute = false;
            //smileServer.StartInfo.CreateNoWindow = true;
            //smileServer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //smileServer.StartInfo.RedirectStandardError = true;
            //smileServer.StartInfo.RedirectStandardInput = true;
            //smileServer.StartInfo.RedirectStandardOutput = true;
            //smileServer.StartInfo.ErrorDialog = false;

            smileServer.Start();
        }

        private void SendAvailableCommandsTelemetry()
        {
            GatherAvailableCommands();
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void GatherAvailableCommands()
        {
            if (availableCommands.Keys.Contains("AddTranscriptionSensor") == false)
            {
                availableCommands.Add("AddTranscriptionSensor", "TargetMachine,TargetSensorID,UtteranceSensorID");
            }
            if (availableCommands.Keys.Contains("AddUtteranceSensor") == false)
            {
                availableCommands.Add("AddUtteranceSensor", "TargetMachine,TargetSensorID");
            }
        }

        private void AddUtteranceSensorConnection(string targetMachine, string targetSensorID)
        {
            ConnectSensorToMyInput(targetMachine, targetSensorID, utteranceDataRendezvous);
        }

        private void AddTranscriptionSensorConnection(string targetMachine, string targetSensorID)
        {
            ConnectSensorToMyInput(targetMachine, targetSensorID, dragonTranscriptionDataRendezvous);

            //Dictionary<string, string> speechToTextForwardItemArgs = new Dictionary<string, string>();
            //speechToTextForwardItemArgs.Add("TargetMachine", Rendezvous<Data>.GetHostName());
            //speechToTextForwardItemArgs.Add("TargetSensorID", SourceID.ToString());
            
            //broker.PublishToInbox(new Rendezvous<Command>(Protocol.HTTP, targetMachine, GlobalRuntimeSettings.COMMAND_GUID, Guid.Parse(targetSensorID)),
            //                      new Command("AddForwardingRendezvous", speechToTextForwardItemArgs));
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "AddUtteranceSensor":
                    AddUtteranceSensorConnection(command.Args["TargetMachine"], command.Args["TargetSensorID"]);
                    break;
                case "AddTranscriptionSensor":
                    AddTranscriptionSensorConnection(command.Args["TargetMachine"], command.Args["TargetSensorID"]);
                    break;
                default:
                    break;
            }
        }

        private void broker_OnItemReceived(object sender, string dataString)
        {
            Data data = assistant.GetItem(dataString);

            try
            {
                lock (this)
                {
                    /*data is utterance */
                    if (data.DataTypeSourceID == Guid.Parse("3fe979d6-6d8c-4954-97cd-d49b43323ae6"))
                    {
                        waveBytesList.Add(data);
                    }
                    /*data is transcription */
                    else
                    {
                        textList.Add(data);
                    }

                    if (waveBytesList.Count > 0 && textList.Count > 0)
                    {
                        byte[] response = client.Transmit(waveBytesList.First().Payload as byte[], ((Dictionary<string, object>)textList.First().Payload)["TxtFile"] as string);
                        waveBytesList.RemoveAt(0);
                        textList.RemoveAt(0);
                        double value = BitConverter.ToSingle(response, 28);
                        SendItem(value, "UtteranceValence", DateTime.Now);
                    }
                }
            }
            catch(Exception e)
            {
                EventLog.WriteEntry("Utterance Valence Sensor","Try catch on sensor lock failed");
            }
        }

        protected override string Start()
        {
            return base.Start();
        }

        protected override string Stop()
        {
            return base.Stop();
        }

        protected override void Exit()
        {
            Environment.Exit(0);
        }
    }

    public class OpenSmileClient
    {
        int serverPort = 13001;

        public byte[] Transmit(byte[] waveBytes, string text)
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), serverPort);
                using (NetworkStream stream = client.GetStream())
                {
                    sTcpWaveSourceHeader header = new sTcpWaveSourceHeader();

                    byte[] textBytes = new byte[text.Length * sizeof(char)];
                    System.Buffer.BlockCopy(text.ToCharArray(), 0, textBytes, 0, textBytes.Length);

                    header.setHeaderInfo(TCPWAVESOURCE_CHUNKTYPE.TCPWAVESOURCE_CHUNK_DATAANDTEXT, 1, textBytes.Length, waveBytes.Length);

                    byte[] outboundWavePacket = new byte[(header.headerSize * 4) + textBytes.Length + waveBytes.Length];

                    System.Buffer.BlockCopy(GetBytes(header), 0, outboundWavePacket, 0, (header.headerSize * 4));
                    System.Buffer.BlockCopy(textBytes, 0, outboundWavePacket, (header.headerSize * 4), textBytes.Length);
                    System.Buffer.BlockCopy(waveBytes, 0, outboundWavePacket, (header.headerSize * 4) + textBytes.Length, waveBytes.Length);

                    try
                    {
                        stream.Write(outboundWavePacket, 0, outboundWavePacket.Length);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Could not write to server, stream is not connected.");
                    }
                    byte[] response = new byte[60];
                    int totalBytesRead = 0;
                    int lastBytesRead = 0;
                    if (stream.CanRead)
                    {
                        while (true)
                        {
                            try
                            {
                                lastBytesRead = stream.Read(response, totalBytesRead, response.Length);
                                totalBytesRead += lastBytesRead;
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("Could not read from server.");
                                break;
                            }
                            if (totalBytesRead >= 60)
                            {
                                Console.WriteLine("Finished reading from server.");
                                break;
                            }
                        }
                    }
                    return response;
                }
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
            Array.Copy(header.GetBytes(header.reservedByteBuffer), 0, ret, 28, 32);

            return ret;
        }
    }

    [Serializable]
    public struct sTcpWaveSourceHeader
    {
        public uint magicHeader; // uint_MAX, this is 8 bytes of all 1's
        public int headerSize; // size of header in uint32_t  (this is the size in bytes divided by 4)
        public int type; // 1 or 3 or 11 or 21
        public int ID;
        public int textLength;
        public int dataLength; // length in bytes of the following data 
        public int checksum;  // sum of all header fields
        internal ReserveredBuffer reservedByteBuffer;

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

        internal byte[] GetBytes(ReserveredBuffer buffer)
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

        public void packResponse(byte[] response)
        {
            if (response.Length == 60)
            {
                magicHeader = BitConverter.ToUInt32(response, 0);
                headerSize = BitConverter.ToInt32(response, 4);
                type = BitConverter.ToInt32(response, 8);
                ID = BitConverter.ToInt32(response, 12);
                textLength = BitConverter.ToInt32(response, 16);
                dataLength = BitConverter.ToInt32(response, 20);
                checksum = BitConverter.ToInt32(response, 24);
                reservedByteBuffer.setBuffer(response);
            }
        }

        [Serializable]
        internal struct ReserveredBuffer
        {
            public float b0;
            public int b1, b2, b3, b4, b5, b6, b7;

            public void setBuffer(byte[] response)
            {
                b0 = BitConverter.ToSingle(response, 28);
                b1 = BitConverter.ToInt32(response, 32);
                b2 = BitConverter.ToInt32(response, 36);
                b3 = BitConverter.ToInt32(response, 40);
                b4 = BitConverter.ToInt32(response, 44);
                b5 = BitConverter.ToInt32(response, 48);
                b6 = BitConverter.ToInt32(response, 52);
                b7 = BitConverter.ToInt32(response, 56);
            }
        }
    }

    public enum TCPWAVESOURCE_CHUNKTYPE { TCPWAVESOURCE_CHUNK_DATA = 1, TCPWAVESOURCE_CHUNK_DATAANDTEXT = 3, TCPWAVESOURCE_CHUNK_CONTROL = 11, TCPWAVESOURCE_CHUNK_RESULT = 21 }
}