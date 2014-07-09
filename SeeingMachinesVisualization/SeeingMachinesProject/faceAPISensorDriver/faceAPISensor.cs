using System;
using System.Collections.Generic;
using Construct.MessageBrokering;
using Construct.Sensors;
using Newtonsoft.Json;


namespace faceAPISensorDriver
{
	public class faceAPISensorDriverSensor : Sensor
	{
		private readonly Dictionary<string, string>
		availableCommands = new Dictionary<string,string>();

		faceAPISensorLogic m_SensorLogic;

		public faceAPISensorDriverSensor(string[] args)
			: base(
				Protocol.HTTP,
				args, // args sent by Construct Studio to initialize sensor
				Guid.Parse("AA0F1B51-7C50-44D2-8F8C-E1A72DA367CF"), // ID of this sensor type
				new Dictionary<string, Guid>() {
					//	Mapping of payload types and their IDs (matches the sensor XML)
					{ "HeadPose", Guid.Parse("A841FD8F-56C6-4B9B-99EF-BF0249037FA3") }
				})
		{
			//Gathers and sends telemetry for available Sensor Commands
			GatherAvailableCommands();
			SendAvailableCommandsTelemetry();

			//Allows Sensor broker to handle incoming Sensor Commands
			broker.OnCommandReceived += broker_OnCommandReceived;

			m_SensorLogic = new faceAPISensorLogic();
			m_SensorLogic.OnNewHeadPose += m_SensorLogic_OnNewHeadPose;
		}

		void m_SensorLogic_OnNewHeadPose(SensorWrapper.HeadPose data)
		{
			SendItem(data, DateTime.Now, "HeadPose");
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
			//availableCommands.Add("SendTelemetryMessage", "Message");
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
			m_SensorLogic.Start();
			return base.Start();
		}

		protected override string Stop()
		{
			//Add any calls you need to make to start your sensors functionality
			m_SensorLogic.Stop();
			return base.Stop();
		}
		#endregion
	}
}
