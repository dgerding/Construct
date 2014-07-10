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
using System.Threading;

/*
 * Sensor package requirements:
 * 
 * 1) All sensor files be placed in a ZIP
 * 2) The name of the sensor ZIP be SensorName.SensorGuid.SensorVersion
 * 3) The name of the executable within the package must match the name
 *			of the package itself.
 * 
 */


namespace Pretend
{
	public class PretendSensor : Sensor
	{
		private readonly Dictionary<string, string>
		availableCommands;
		Timer timer, itemsPerSecondTimer;
		int totalItems = 0;

		public PretendSensor(string[] args)
			: base(
				Protocol.HTTP,
				args, // args sent by Construct to initialize sensor
				Guid.Parse("E94765C3-F31A-4158-86AE-FAEBDD2FF478"), // ID of this sensor type
				new Dictionary<string, Guid>() {
					//	Mapping of payload types and their IDs (matches the sensor XML)
					{ "PretendPayload", Guid.Parse("11878896-8D84-4904-B6C6-1BF66E53F957") }
				})
		{
			availableCommands = new Dictionary<string, string>();

			//Gathers and sends telemetry for available Sensor Commands
			GatherAvailableCommands();
			SendAvailableCommandsTelemetry();

			//Allows Sensor broker to handle incoming Sensor Commands
			broker.OnCommandReceived += broker_OnCommandReceived;

			itemsPerSecondTimer = new Timer(delegate (object o) {
				Debug.WriteLine(totalItems + " items per second");
				totalItems = 0;
			}, null, 0, 1000);
		}

		void broker_OnCommandReceived(object sender, string commandString)
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
			broker.Publish(new Telemetry(message, new Dictionary<string, string>()));
		}

		//Additional Sensor Command functions go below here.
		#endregion

		#region Send Item
		//Place function here to call SendItem with your sensor payload
		//ex. SendItem(payload object to match serializer selected in your ItemOutbox, SourceTypeID);
		private void SupposedToSendItem(object state)
		{
			//	Name of item sent must match the mapping done in the constructor (and, by proxy,
			//		must match the XML definition.)
			Benchmark.Begin("SendItem");
			SendItem(new PretendPayload(20), DateTime.Now, "PretendPayload");
			Benchmark.End();
			++totalItems;
		}
		#endregion

		#region Start / Stop Sensor Functions
		protected override string Start()
		{
			timer = new Timer(SupposedToSendItem, null, 0, 1);

			//Add any calls you need to make to start your sensors functionality
			return base.Start();
		}

		protected override string Stop()
		{
			timer.Change(0, System.Threading.Timeout.Infinite);

			//Add any calls you need to make to start your sensors functionality
			return base.Stop();
		}
		#endregion
	}
}
