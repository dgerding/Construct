using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SignalR.Hubs;

namespace Construct.Server.Models.Data
{
	[HubName("ItemStreamHub")]
	public class ItemRealtimeStreamer : IDisposable
	{
		private Thread dataStreamThread;
		private SignalR.Hosting.Self.Server signalrServer;
		private bool isRunning = false;

		public ItemRealtimeStreamer(String streamUri)
		{
			signalrServer = new SignalR.Hosting.Self.Server(streamUri);
			signalrServer.MapHubs();
			signalrServer.Start();

			dataStreamThread = new Thread(() => DataStreamThread(this));
			dataStreamThread.Start();
		}

		public void Dispose()
		{


			try
			{
				signalrServer.Stop();
			}
			catch (Exception e) { }

			signalrServer = null;
		}

		public void ProcessItemPayload(String jsonPayload)
		{
			
		}

		public void AddDataSubscription(String client, Guid sensorId, Guid propertyId)
		{
			
		}

		private static void DataStreamThread(ItemRealtimeStreamer streamer)
		{
			while (streamer.isRunning)
			{
				Thread.Sleep(1);
			}
		}
	}
}
