using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Construct.Sensors;
using Construct.MessageBrokering;
using Newtonsoft.Json;


namespace $safeprojectname$
{
    public class $safeprojectname$Sensor : Sensor
    {
        private readonly Dictionary<string, string>
        availableCommands;

        public $safeprojectname$Sensor(string[] args)
            : base(
				Protocol.HTTP,
				args, // args sent by Construct Studio to initialize sensor
				Guid.Parse("YourSensorTypeGUID"), // ID of this sensor type
				new Dictionary<string, Guid>() {
					//	Mapping of payload types and their IDs (matches the sensor XML)
					{ "YourDatatypeName", Guid.Parse("DatatypeGUID") }
				})
        {
			//Gathers and sends telemetry for available Sensor Commands
			GatherAvailableCommands();
			SendAvailableCommandsTelemetry();

			//Allows Sensor broker to handle incoming Sensor Commands
			broker.OnCommandReceived += broker_OnCommandReceived;
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
			Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
            //To use a custom serializer, use the overloaded constructor for Outbox<Item>
            //default serializer handles strings.
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
            //Add additional SensorCommands here
                default:
                    break;
            }
        }
    #region Sensor Commands
        private void SendAvailableCommandsTelemetry()
        {
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }
    
        private void GatherAvailableCommands()
        {
            //Add command names (MUST MATCH IN SWITCH STATEMENT) here, with comma separated parameters
            availableCommands.Add("SendTelemetryMessage", "Message");
        }

        private void SendTelemetryMessage(string message)
        {
            broker.Publish(new Telemetry(message, new Dictionary<string,string>()));
        }

        //Additional Sensor Command functions go below here.
    #endregion

    #region Send Item
    //Place function here to call SendItem with your sensor payload
    //ex. SendItem(payload object to match serializer selected in your ItemOutbox, SourceTypeID);
    #endregion

    #region Start / Stop Sensor Functions
        protected override string Start()
        {
            //Add any calls you need to make to start your sensors functionality
            return base.Start();
        }

        protected override string Stop()
        {
            //Add any calls you need to make to start your sensors functionality
            return base.Stop();
        }
    #endregion
    }
}
