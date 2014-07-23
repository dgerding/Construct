using Construct.MessageBrokering;
using Construct.Sensors;
using SMFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace faceLABConstructSensor
{
	class faceLABSensorWrapper : Sensor
	{
		public faceLABSensorWrapper(string[] args)
			: base(
				Construct.MessageBrokering.Protocol.HTTP,
				args,
				Guid.Parse("e683e0c5-85fb-4788-81a1-f39cb2054687"),
				new Dictionary<String, Guid> { { "FaceData", Guid.Parse("f0070c64-41de-47bb-80b1-e4d0a365c864") } } // Type ID
			)
		{
			CollectionThread.OnNewData += SendFaceData;
			CollectionThread.OnError += ErrorHandler;

			DebugOutputStream.SlowInstance = new DebuggerOutputStream();

			/* Publish commands as telemetry */
			Dictionary<String, String> commands = new Dictionary<string, string>();
			commands.Add("ChangeSensorLabel", "New Sensor Label");
			commands.Add("ChangeListenPort", "New Listen Port");

			broker.Publish(new Telemetry("AvailableSensorCommands", commands));
			broker.OnCommandReceived += broker_OnCommandReceived;
		}

		void ErrorHandler(Exception error)
		{
			Console.WriteLine("CollectionThread error: {0}", error.Message);
		}

		void broker_OnCommandReceived(object sender, string commandString)
		{
			Command command = JsonConvert.DeserializeObject<Command>(commandString);
			switch (command.Name)
			{
				case ("AddItemOutbox"):
					{
						Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
						var newOutbox = new Outbox<Data>(itemRendezvous);
						newOutbox.MinBufferDataSendWaitTimeMS = 2000;
						broker.AddPeer(newOutbox);
						break;
					}

				case ("ChangeSensorLabel"):
					{
						Stop();
						CollectionThread.SourceSignal.Label = command.Args["New Sensor Label"];
						Start();
						break;
					}

				case ("ChangeListenPort"):
					{
						int newPort;
						if (!int.TryParse(command.Args["New Listen Port"], out newPort))
							break;

						CollectionThread.Stop();
						CollectionThread.SourceSignal.Port = newPort;
						CollectionThread.Start();
						break;
					}
			}
		}

		void SendFaceData(FaceData faceData)
		{
			SendItem(new FaceDataConstructAdapter(faceData), DateTime.UtcNow, "FaceData");
		}

		protected override string Start()
		{
			CollectionThread.Start();

			return base.Start();
		}

		protected override string Stop()
		{
			try
			{
				CollectionThread.Stop();
			}
			catch (Exception e)
			{
				
			}

			Debugger.Log(0, "", "Stopping...\n");

			return base.Stop();
		}
	}
}
