using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Construct.Sensors;
using Construct.Sensors.TestSensorPayload;
using Newtonsoft.Json;
using Construct.MessageBrokering;
using System.IO;
using System.Threading;

namespace Construct.Sensors.TestSensor
{
    public class TestSensor : Sensor
    {
        private readonly Dictionary<string, string> availableCommands = new Dictionary<string, string>();

        private System.Timers.Timer timerToGenerateNumbers;
        private List<TestSensorPayload.TestSensorPayload> payloadList = new List<TestSensorPayload.TestSensorPayload>(100000);
        private StreamReader reader;
        private Thread firehoseThread;
        private bool firehoseMode = false;
        private DateTime lastDataTimeStamp;
        public DateTime LastDataTimeStamp
        {
            get
            {
                if (lastDataTimeStamp == null || lastDataTimeStamp == DateTime.MinValue)
                {
                    lastDataTimeStamp = DateTime.Now;
                }
                return lastDataTimeStamp;
            }
            set
            {
                lastDataTimeStamp = value;
            }
        }

        private int nextPayloadToSendCounter = 0;
        

        public TestSensor(string[] args, int intervalLength = 1000)
            : base(Protocol.HTTP, args, Guid.Parse("38196e9e-a581-4326-b6f2-c4120f89d4cc"), new Dictionary<string, Guid>() { {"Test", Guid.Parse("0c89f6c2-c749-4085-b0ea-163c419f5ac3") } })
        {
            timerToGenerateNumbers = new System.Timers.Timer(intervalLength);
            timerToGenerateNumbers.AutoReset = true;
            timerToGenerateNumbers.Enabled = false;
            timerToGenerateNumbers.Elapsed += new ElapsedEventHandler(OnTimeElapsedEvent);

            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

            broker.OnCommandReceived += broker_OnCommandReceived;

            reader = new StreamReader("testData.csv");
            ReadEntriesFromCSV();
        }

        private void ReadEntriesFromCSV()
        {
            string temp = String.Empty;
            string[] tempSplit;
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                temp = reader.ReadLine();
                tempSplit = temp.Split(',');
                
                payloadList.Add(new TestSensorPayload.TestSensorPayload(Int32.Parse(tempSplit[12]), tempSplit[13], (tempSplit[9] == "0") ? false : true, HexToBytes(tempSplit[10]), Guid.Parse(tempSplit[11]), 
                                                                        new TestSensorPayload.TestSensorPayload.SubClass(Int32.Parse(tempSplit[7]), Double.Parse(tempSplit[6]), tempSplit[8]))); 
            }
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);
            bool tempStartFlag;
            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "SetFirehoseMode":
                    tempStartFlag = IsStarted;

                    Stop();
                    firehoseMode = true;
                 
                    if (tempStartFlag)
                    {
                        Start();
                    }
                    break;
                case "SetTickMode":
                    tempStartFlag = IsStarted;
                    
                    Stop();
                    firehoseMode = false;

                    if (tempStartFlag)
                    {
                        Start();
                    }
                    break;
            }
        }

        private void OnTimeElapsedEvent(object source, EventArgs e)
        {
            SendTestItem();
        }

        private void StreamData()
        {
            while(nextPayloadToSendCounter < payloadList.Count)
            {
                SendTestItem();
            }
        }

        private void SendTestItem()
        {
            SendItem(payloadList[nextPayloadToSendCounter], "Test", LastDataTimeStamp);
            LastDataTimeStamp = LastDataTimeStamp.AddSeconds(1);

            nextPayloadToSendCounter++;
            if (nextPayloadToSendCounter % 5 == 0)
            {
                Telemetry fiveItemsSent = new Telemetry("sentFiveItems", new Dictionary<string, string>());
                broker.Publish(fiveItemsSent);
            }
        }
        protected override string Start()
        {
            if (firehoseMode)
            {
                firehoseThread = new Thread(StreamData);
                firehoseThread.IsBackground = true;
                firehoseThread.Start();

                return base.Start();
            }
            else
            {
                timerToGenerateNumbers.AutoReset = true;
                timerToGenerateNumbers.Start();
                return base.Start();
            }
        }

        protected override string Stop()
        {
            if (firehoseMode)
            {
                if (firehoseThread != null)
                {
                    firehoseThread.Abort();
                    return base.Stop();
                }
                return base.Stop();
            }
            else
            {
                timerToGenerateNumbers.AutoReset = false;
                timerToGenerateNumbers.Stop();
                return base.Stop();
            }
        }

        private void SendAvailableCommandsTelemetry()
        {
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void GatherAvailableCommands()
        {
            if (availableCommands.Keys.Contains("SetFirehoseMode") == false)
            {
                availableCommands.Add("SetFirehoseMode", "");
            }
            if (availableCommands.Keys.Contains("SetTickMode") == false)
            {
                availableCommands.Add("SetTickMode", "");
            }
        }

        public static byte[] HexToBytes(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Substring(2);
            }
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }
    }
}