using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Construct.Base.Constants;
using Construct.Base.Wcf;
using Construct.Dataflow.Brokering;
using Construct.Sensors.CustomDeserializers;
using Construct.Sources.Sensors;


namespace Construct.Sensors.FaceLabMultiSubjectTabletop
{
    public class FaceLabMultiSubjectTabletopSensor : Sensor
    {
        private readonly Dictionary<string, string>
        availableCommands;

        public Construct.Sensors.FaceLabMultiSubjectTabletopSensor(string[] args)
            : base(Protocol.HTTP, args)
        {
        //Set the unique GUID for your Sensor
        SourceTypeID = Guid.Parse("Your guid here");

        //Gathers and sends telemetry for available Sensor Commands
        GatherAvailableCommands();
        SendAvailableCommandsTelemetry();

        //Allows Sensor broker to handle incoming Sensor Commands
        broker.OnCommandReceived += broker_OnCommandReceived;
        }

        private void broker_OnCommandReceived(object sender, Command command)
        {
            string sensorCommand = command.Name;

            switch (sensorCommand)
            {
            //To use a custom serializer, use the overloaded constructor for Outbox<Item>
            //default serializer handles strings.
                case "AddItemOutbox":
                    Rendezvous<Item> itemRendezvous = new Rendezvous<Item>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Item>(itemRendezvous);
                    break;
                case "ResetSensorCommands":
                    ResetSensorCommands();
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
            availableCommands.Add("ResetSensorCommands", "");
        }

        private void ResetSensorCommands()
        {
            List<string> oldCommandKeys = new List<string>(availableCommands.Keys);

            foreach (string key in oldCommandKeys)
            {
                availableCommands.Remove(key);
            }
            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();
        }

        //Additional Sensor Command functions go below here.
    #endregion

    #region Send Item
    //Place function here to call SendItem with your sensor payload

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
