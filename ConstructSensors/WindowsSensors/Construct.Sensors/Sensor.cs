using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Construct.MessageBrokering;

namespace Construct.Sensors
{
    /// <summary>Sensor is the abstract base class from which all sensors inherit.</summary>
    public abstract class Sensor
    {
        private readonly string
            startMessage = "started",
            stopMessage = "stopped",
            port = "8086";
        private readonly string name;

        private bool isStarted = false;
        protected bool IsStarted
        {
            get
            {
                return isStarted;
            }
            private set
            {
                isStarted = value;
            }
        }

        public Guid DataTypeSourceID { get; private set; }
        public Guid SourceID { get; protected set; }
        public Dictionary<string, Guid> DataTypes { get; protected set; }

        protected Broker broker;
        protected SensorSerializationAssistant assistant = new SensorSerializationAssistant();
        
        public enum BindingTypes
        {
            HTTP,
            NetNamedPipe,
            TCP
        }

        //
        //TODO: Change sensors to take name value pairs instead of indexes with encoded/infered values
        //
        /// <summary>Creates a sensor using sensor initialization data contained in "args" and a binding type which specifies the protocol the sensor will communicate over. This
        /// constructor is used by child constructor</summary>
         
        protected Sensor(Protocol protocol, string[] args, Guid DataTypeSourceID, Dictionary<string, Guid> dataTypesDictionary)
        {
            this.DataTypeSourceID = DataTypeSourceID;
            this.SourceID = Guid.Parse(args[0]);
            string constructServerUri = args[1];
            string sensorHostUri = args[2];

            this.name = this.GetType().Name;
            this.DataTypes = dataTypesDictionary;

            #region Telemetry Peers
            Rendezvous<Telemetry> constructRendezvous = new Rendezvous<Telemetry>(constructServerUri);
            Rendezvous<Telemetry> hostRendezvous = new Rendezvous<Telemetry>(sensorHostUri);
            Outbox<Telemetry> telemetryOutbox = new Outbox<Telemetry>(new Rendezvous<Telemetry>[2] { constructRendezvous, hostRendezvous });

            Rendezvous<Telemetry> telemetryRendezvous = new Rendezvous<Telemetry>(protocol, Dns.GetHostName(), GlobalRuntimeSettings.TELEMETRY_GUID, SourceID);
            Inbox<Telemetry> telemetryInbox = new Inbox<Telemetry>(telemetryRendezvous);
            #endregion

            #region Command Peers
            Rendezvous<Command> commandRendezvous = new Rendezvous<Command>(protocol, Dns.GetHostName(), GlobalRuntimeSettings.COMMAND_GUID, SourceID);
            Inbox<Command> commandInbox = new Inbox<Command>(commandRendezvous);
            #endregion

            IPeer[] peers = new IPeer[] { commandInbox, telemetryInbox, telemetryOutbox };
            broker = new Broker(peers, SourceID);
            broker.OnCommandReceived += new Action<object, string>(broker_OnCommandReceived);
            broker.OnTelemetryReceived += new Action<object, string>(broker_OnTelemetryReceived);

            Telemetry inboxReady;
            Dictionary<string, string> telemetryArgs = new Dictionary<string, string>();
            telemetryArgs.Add("Uri", commandInbox.CurrentRendezvous.Uri.ToString());
            telemetryArgs.Add("DataTypeSourceID", DataTypeSourceID.ToString());
            inboxReady = new Telemetry("AnnounceCommandInbox", telemetryArgs);
            
            broker.Publish(inboxReady);
        }

        public void SendItem(object dataPayload, string dataName, DateTime dataTimestamp)
        {
            Guid dataTypeID = DataTypes[dataName];
	        if (dataTimestamp.Kind == DateTimeKind.Local)
		        dataTimestamp = dataTimestamp.ToUniversalTime();
	        else if (dataTimestamp.Kind == DateTimeKind.Unspecified)
		        dataTimestamp = DateTime.SpecifyKind(dataTimestamp, DateTimeKind.Utc);

            Data data = new Data(dataPayload, this.SourceID, this.DataTypeSourceID, dataName, dataTypeID, dataTimestamp);
            if (assistant.GetJsonHeader(dataName) == String.Empty)
            {
                assistant.SetJsonHeader(data);
            }
            broker.Publish(data, assistant.GetJsonHeader(dataName));
        }

        public void SendItem(object dataPayload, DateTime manualTimeStamp, string dataName)
        {
            Guid dataTypeID = DataTypes[dataName];
			if (manualTimeStamp.Kind == DateTimeKind.Local)
				manualTimeStamp = manualTimeStamp.ToUniversalTime();
			else if (manualTimeStamp.Kind == DateTimeKind.Unspecified)
				manualTimeStamp = DateTime.SpecifyKind(manualTimeStamp, DateTimeKind.Utc);

            Data data = new Data(dataPayload, this.SourceID, this.DataTypeSourceID, dataName, dataTypeID);
            data.TimeStamp = manualTimeStamp;
            if (assistant.GetJsonHeader(dataName) == String.Empty)
            {
                assistant.SetJsonHeader(data);
            }
            broker.Publish(data, assistant.GetJsonHeader(dataName));
        }

        protected virtual string Start()
        {
            isStarted = true;
            return this.name + ": " + this.startMessage;
        }

        protected virtual string Stop()
        {
            isStarted = false;
            return this.name + ": " + this.stopMessage;
        }

        protected void ConnectSensorToMyInput(string targetMachine, string targetSensorID, Rendezvous<Data> myDataInboxRendezvous)
        {
            Rendezvous<Command> utteranceCommandRendezvous = new Rendezvous<Command>(Protocol.HTTP, targetMachine, GlobalRuntimeSettings.COMMAND_GUID, Guid.Parse(targetSensorID));
            Outbox<Command> potentialOutbox = new Outbox<Command>(utteranceCommandRendezvous);
            if (broker.Peers.Contains(potentialOutbox) == false)
            {
                broker.AddPeer(potentialOutbox);
            }
            Dictionary<string, string> utteranceArgs = new Dictionary<string, string>();
            utteranceArgs.Add("Uri", myDataInboxRendezvous.Uri.AbsoluteUri);
            Command AddItemOutboxCommand = new Command("AddItemOutbox", utteranceArgs);

            broker.PublishToInbox(utteranceCommandRendezvous, AddItemOutboxCommand);
        }

        protected void ConnectSensorToSensorCommands(string targetMachine, string targetSensorID, Rendezvous<Data> myDataInboxRendezvous)
        {
            Rendezvous<Command> sensorCommandRendezvous = new Rendezvous<Command>(Protocol.HTTP, targetMachine, GlobalRuntimeSettings.COMMAND_GUID, Guid.Parse(targetSensorID));
            Outbox<Command> potentialOutbox = new Outbox<Command>(sensorCommandRendezvous);
            if (broker.Peers.Contains(potentialOutbox) == false)
            {
                broker.AddPeer(potentialOutbox);
            }
            Dictionary<string, string> sensorArgs = new Dictionary<string, string>();
            sensorArgs.Add("Uri", myDataInboxRendezvous.Uri.AbsoluteUri);
            Command AddCommandOutboxCommand = new Command("AddCommandOutbox", sensorArgs);

            broker.PublishToInbox(sensorCommandRendezvous, AddCommandOutboxCommand);
        }


        private void CheckHealth(string callbackUri)
        {
            Dictionary<string, string> healthArgs = new Dictionary<string, string>();
            healthArgs.Add("ID", SourceID.ToString());

            Telemetry healthReport = new Telemetry("HealthReport", healthArgs);
            broker.PublishToInbox(new Rendezvous<Telemetry>(callbackUri), healthReport);
        }
        private void AddTelemetryOutbox(string callbackUri)
        {
            Rendezvous<Telemetry> telemetryRendezvous = new Rendezvous<Telemetry>(callbackUri);
            Outbox<Telemetry> outbox = new Outbox<Telemetry>(telemetryRendezvous);
            broker.AddPeer(outbox);

            Dictionary<string, string> addTelemetryOutboxArgs = new Dictionary<string, string>();
            addTelemetryOutboxArgs.Add("Uri", outbox.AllRendezvous.First().ToString());

            Telemetry response = new Telemetry("AnnounceTelemetryOutbox", addTelemetryOutboxArgs);
            broker.PublishToInbox(telemetryRendezvous, response);
        }
        protected virtual void Exit()
        {
            Environment.Exit(0);
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);
            string sensorCommand = command.Name;

            switch (sensorCommand)
            {
                case "Start":
                    Start();
                    break;
                case "Stop":
                    Stop();
                    break;
                case "CheckHealth":
                    CheckHealth(command.Args["Uri"]);
                    break;
                case "AddTelemetryOutbox":
                    AddTelemetryOutbox(command.Args["Uri"]);
                    break;
                case "Exit":
                    Exit();
                    break;
                default:
                    break;
            }
        }

        private void broker_OnTelemetryReceived(object sender, string telemetryString)
        {
            Telemetry telemetry = JsonConvert.DeserializeObject<Telemetry>(telemetryString);
            string sensorTelemetry = telemetry.Name;

            switch (sensorTelemetry)
            {
                case "AnnounceCommandInbox":
                    break;
                case "AnnounceItemInbox":
                    break;
                case "AnnounceTelemetryInbox":
                    break;
                default:
                    break;
            }
        }
    }
}