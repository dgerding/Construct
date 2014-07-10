using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Construct.Sensors;
using Email;
using Construct.MessageBrokering;
using Newtonsoft.Json;


namespace EmailReader
{
	public class EmailReaderSensor : Sensor
	{
		private readonly Dictionary<string, string>
		availableCommands;
		private List<IMAPListener>
		accounts;

		public EmailReaderSensor(string[] args)
			: base(
				Protocol.HTTP,
				args, // args sent by Construct Studio to initialize sensor
				Guid.Parse("405D4464-2BA8-49A9-AD98-99174B2EBCA3"), // ID of this sensor type
				new Dictionary<string, Guid>() {
					//	Mapping of payload types and their IDs (matches the sensor XML)
					{ "Email", Guid.Parse("C50C1A0C-5251-4FEF-A60B-9224BB3671A5") }
				})
		{
			accounts = new List<IMAPListener>();
			availableCommands = new Dictionary<string, string>();

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
				case "SubscribeToEmailAccount":
					SubscribeToEmailAccount(command.Args["ImapServerHostName"],
						int.Parse(command.Args["ImapServerPort"]),
						command.Args["UserName"], command.Args["Password"],
						bool.Parse(command.Args["IsSSL"]), command.Args["TargetMailboxName"]);
					break;
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
			availableCommands.Add("SubscribeToEmailAccount", "ImapServerHostName,ImapServerPort,UserName,Password,IsSSL,TargetMailboxName");
		}

		private void SendTelemetryMessage(string message)
		{
			broker.Publish(new Telemetry(message, new Dictionary<string, string>()));
		}

		//Additional Sensor Command functions go below here.

		private void SubscribeToEmailAccount(string ImapServerHostName, int ImapServerPort, string UserName, string Password, bool IsSSL, string TargetMailboxName)
		{
			IMAPListener imapListener = new IMAPListener(ImapServerHostName, ImapServerPort, UserName, Password, IsSSL, TargetMailboxName);
			imapListener.imapReader.NewMessageReady += new EventHandler(imapReader_NewMessageReady);
			accounts.Add(imapListener);

		}
		#endregion

		#region Send Item
		//Place function here to call SendItem with your sensor payload
		//ex. SendItem(payload object to match serializer selected in your ItemOutbox, SourceTypeID);

		private void imapReader_NewMessageReady(object sender, EventArgs e)
		{
			IMAPReader castsender = (IMAPReader)sender;

			SendItem(castsender.currentMessage, "EmailItem", DateTime.Now);
		}
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
