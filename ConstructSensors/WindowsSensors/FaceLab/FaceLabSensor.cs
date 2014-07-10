using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;
using Construct.Sensors;
using Construct.MessageBrokering;

namespace FaceLab
{
    public class FaceLabSensor : Sensor
    {
        private readonly Dictionary<string, string>
        availableCommands;

        private FaceLabDataCollector faceLabDataCollector;

        public FaceLabSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Parse("ab85294f-c288-45a5-935e-5d376ce07102"), new Dictionary<string, Guid> { { "FaceLab", Guid.Empty } })
        {
            Debugger.Launch();
            Debugger.Break();

            faceLabDataCollector = new FaceLabDataCollector();
            
            availableCommands = new Dictionary<string, string>();

            //Gathers and sends telemetry for available Sensor Commands
            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

            //Allows Sensor broker to handle incoming Sensor Commands
            broker.OnCommandReceived += broker_OnCommandReceived;

            faceLabDataCollector.OnDataReceived += new FaceLabDataCollector.FaceLabDataReceivedEventHandler(faceLabDataCollector_OnDataReceived);
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
            //To use a custom serializer, use the overloaded constructor for Outbox<Item>
            //default serializer handles strings.
                case "SetTargetUDPSocket":
                    SetTargetUDPSocket(command.Args["TargetSocket"]);
                    break;
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                case "ResetSensorCommands":
                    ResetSensorCommands();
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
            if (availableCommands.Keys.Contains("SetTargetUDPSocket") == false)
            {
                availableCommands.Add("SetTargetUDPSocket", "TargetSocket");
            }
            if (availableCommands.Keys.Contains("ResetSensorCommands") == false)
            {
                availableCommands.Add("ResetSensorCommands", "");
            }
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

        private void SetTargetUDPSocket(string targetSocket)
        {
            faceLabDataCollector.SetTargetSocket(int.Parse(targetSocket));
        }

        //Additional Sensor Command functions go below here.
        #endregion

        #region Send Item
        //Place function here to call SendItem with your sensor payload
        private void faceLabDataCollector_OnDataReceived(HeadInfo data)
        {
            SendItem(data, "FaceLab");
        }
        #endregion

        #region Start / Stop Sensor Functions
        protected override string Start()
        {
            //Add any calls you need to make to start your sensors functionality
            faceLabDataCollector.Run();
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
