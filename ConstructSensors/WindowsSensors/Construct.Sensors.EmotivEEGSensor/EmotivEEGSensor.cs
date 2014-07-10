using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Timers;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Construct.Dataflow.Brokering;
using Construct.Sensors;
using System.Collections.Generic;
using Newtonsoft.Json;
using Construct.Base.Wcf;
using Construct.Dataflow.Brokering.Messaging;


namespace Construct.Sensors.EmotivEEGSensor
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class EmotivEEGSensor : Sensor
    {
        private DataCollector
            data;
        private byte[]
            AffectivData,
            CognitivData,
            ExpressivData,
            HardwareData,
            RawEEGData,
            EmotivItemData,
            TelemetryData;
        private Timer
            timer;

        public EmotivEEGSensor(string[] args, int intervalLength = 50)
            : base(Protocol.HTTP, args)
        {
            broker.OnCommandReceived += broker_OnCommandReceived;

            timer = new Timer(intervalLength);
            timer.AutoReset = true;
            timer.Enabled = false;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            SourceTypeID = Guid.Parse("631B67DC-B41B-44C8-A7A3-761BCB842332");

            data = new DataCollector();
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            Dictionary<string, string> args = command.Args;
            string emotivSensorCommand = command.Name;

            switch (emotivSensorCommand)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                default:
                    break;
            }
        }

        private void OnTimedEvent(object source, EventArgs e)
        {
            //if(data.isAUserConnected())
            //{
                //data.ProcessEvents();
                data.GatherRawData();
                data.PopulateEmotivItemData();
                data.PopulateEmotivTelemetryData();
                SerializeDataGroup();
                SendItem(EmotivItemData, SourceTypeID, "EmotivEEGItem");
  //              }
        }

        private void SerializeDataGroup()
        {
            //EmotivItemData = SerializeData(data.emotivItemData);
            //TelemetryData = SerializeData(data.emotivTelemetryData);
            //AffectivData = SerializeData(data.Affectiv);
            //CognitivData = SerializeData(data.Cognitiv);
            //ExpressivData = SerializeData(data.Expressiv);
            //HardwareData = SerializeData(data.Hardware);
            //RawEEGData = SerializeData(data.RawData);
        }

        private byte[] SerializeData(Object dataStruct)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                binaryFormatter.Serialize(memoryStream, dataStruct);

                byte[] buffer = memoryStream.GetBuffer();

                return buffer;

            }
            catch (Exception exception)
            {
                return null;
            }
        }

        protected override string Start()
        {
            timer.AutoReset = true;
            timer.Start();
            return base.Start();
        }

        protected override string Stop()
        {
            timer.AutoReset = false;
            timer.Stop();
            return base.Stop();
        }
    }
}
