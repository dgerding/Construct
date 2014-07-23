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


namespace SapiSensor
{
    public class SapiSensorSensor : Sensor
    {
        private Dictionary<string, string> availableCommands = new Dictionary<string,string>();

        private SapiBase m_Recognition = new SapiBase();
		private Rendezvous<Data> m_UtteranceItemInboxRendezvous;

        public SapiSensorSensor(string[] args)
            : base(
                Protocol.HTTP,
                args, // args sent by Construct Studio to initialize sensor
                Guid.Parse("07FA86D6-9181-4EBD-84C2-BB3CDF021BBB"), // ID of this sensor type
                new Dictionary<string, Guid>() {
					//	Mapping of payload types and their IDs (matches the sensor XML)
					{ "Transcription", Guid.Parse("26265F71-DB2A-4194-ADE3-DE983600D28D") }
				})
        {
            //Gathers and sends telemetry for available Sensor Commands
            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

			m_UtteranceItemInboxRendezvous = new Rendezvous<Data>(Protocol.HTTP, Rendezvous<Data>.GetHostName(), Guid.Parse("3FE979D6-6D8C-4954-97CD-D49B43323AE6"), SourceID);
			broker.AddPeer(new Inbox<Data>(m_UtteranceItemInboxRendezvous));

            //Allows Sensor broker to handle incoming Sensor Commands
            broker.OnCommandReceived += broker_OnCommandReceived;
			broker.OnItemReceived += broker_OnItemReceived;

            m_Recognition.OnTranscribed += m_Recognition_OnTranscribed;
        }

		void broker_OnItemReceived(object sender, string dataString)
		{
			Data data = assistant.GetItem(dataString);
			switch (data.DataTypeSourceID.ToString().ToUpperInvariant())
			{
				//	Utterance sensor
				case ("3FE979D6-6D8C-4954-97CD-D49B43323AE6"):
					{
						if (base.IsStarted)
						{
							Debug.WriteLine("Utterance received");
							m_Recognition.ProcessUtterance(data.Payload as byte[], data.TimeStamp);
						}
						break;
					}
				default:
					{
						throw new InvalidDataException();
					}
			}
		}

        void m_Recognition_OnTranscribed(Transcription details, DateTime transcriptionStartTime)
        {
			details.TranscriptionEndTime = details.TranscriptionEndTime.ToUniversalTime();
            SendItem(details, transcriptionStartTime.ToUniversalTime(), "Transcription");
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

				case "AddUtteranceSensorSource":
					String targetMachine = command.Args["TargetMachine"];
					String targetSensor = command.Args["TargetSensorInstanceID"];

					ConnectSensorToMyInput(targetMachine, targetSensor, m_UtteranceItemInboxRendezvous);
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
            availableCommands.Add("AddUtteranceSensorSource", "TargetMachine,TargetSensorInstanceID");
        }

        private void SendTelemetryMessage(string message)
        {
            broker.Publish(new Telemetry(message, new Dictionary<string, string>()));
        }

        //Additional Sensor Command functions go below here.
        #endregion

        #region Start / Stop Sensor Functions
        protected override string Start()
        {
            //Add any calls you need to make to start your sensors functionality
            m_Recognition.Start();
            return base.Start();
        }

        protected override string Stop()
        {
            //Add any calls you need to make to start your sensors functionality
            m_Recognition.Stop();
            return base.Stop();
        }
        #endregion
    }
}
