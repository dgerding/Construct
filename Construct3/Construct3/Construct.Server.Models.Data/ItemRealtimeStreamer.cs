using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Owin;

namespace Construct.Server.Models.Data
{
	[HubName("ItemStreamHub")]
	public class ItemRealtimeStreamer : Hub, IDisposable
	{
		private Thread dataStreamThread;
		private bool isRunning = false;

		public ItemRealtimeStreamer(String streamUri)
		{
			WebApp.Start(streamUri);

			isRunning = true;
			dataStreamThread = new Thread(() => DataStreamThread(this));
			dataStreamThread.Start();
		}

		public override Task OnConnected()
		{
			return base.OnConnected();
		}

		public void Dispose()
		{

		}

		public void ProcessItemPayload(String jsonPayload)
		{
			
		}

		public void AddDataSubscription(String client, Guid sensorId, Guid propertyId)
		{
			
		}

		private static void DataStreamThread(ItemRealtimeStreamer streamer)
		{
			Random random = new Random();
			var hubContext = GlobalHost.ConnectionManager.GetHubContext<ItemRealtimeStreamer>();
			while (streamer.isRunning)
			{
				hubContext.Clients.All.newData("Property Type", "Source ID", random.NextDouble());
				Thread.Sleep(200);
			}
		}
	}
}
