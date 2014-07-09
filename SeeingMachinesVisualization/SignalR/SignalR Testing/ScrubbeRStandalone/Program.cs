using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;

/*
 * 
 * Required packages:
 * 
 * (Import Owin first)
 * Microsoft.AspNet.SignalR.Client
 * Microsoft.AspNet.SignalR
 * Microsoft.Owin.Hosting -pre
 * Microsoft.Owin.Host.HttpListener -pre
 * 
 */

namespace ScrubbeRStandalone
{
	class Program
	{
		static void Main(string[] args)
		{
			string url = "http://daisy.colum.edu/ScrubbeR";
			//string url = "http://localhost/Scrubber";

			using (WebApplication.Start<Startup>(url))
			{
				Console.WriteLine("Server running on {0}", url);
				Console.ReadLine();
			}
		}
	}

	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// Turn cross domain on 
			app.MapHubs(new HubConfiguration() { EnableCrossDomain = true });
		}
	}

	public class TestHub : Hub
	{
		public void TimeChange(String newTime)
		{
			Clients.All.TimeChange(newTime);
			Console.WriteLine("newTime: " + newTime);
		}

		public override Task OnConnected()
		{
			Console.WriteLine("Received new client");
			return base.OnConnected();
		}

		public override Task OnDisconnected()
		{
			Console.WriteLine("Client disconnected");
			return base.OnDisconnected();
		}

		public override Task OnReconnected()
		{
			Console.WriteLine("Client reconnected");
			return base.OnReconnected();
		}
	}
}
