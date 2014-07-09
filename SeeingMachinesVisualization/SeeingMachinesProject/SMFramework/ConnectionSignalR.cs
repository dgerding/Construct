using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;

/*
 * Reference: http://blogs.microsoft.co.il/blogs/ranw/archive/2012/12/29/using-signalr-with-net-client.aspx
 */


/*
 * Note: For strings containing a UTC-formatted date, SignalR will automatically recognize it as a
 *	serialized DateTime object for .NET clients. It will deserialize the string automatically and
 *	pass the DateTime object to the appropriate callback in the .NET client; if the message is read
 *	as a String in a .NET client, then you will end up with a DateTime.ToString() - formatted string that does not
 *	contain all of the data. Receive the data as a DateTime object instead and you'll receive all
 *	of the information.
 *	
 * This behavior doesn't seem to be documented by SignalR itself. This was just noticed after an
 *	hour of debugging. LEARN FROM MY MISTAKES (callback should take DateTime, not String.)
 * 
 */

namespace SMFramework
{
	public class ConnectionSignalR
	{
		public HubConnection Connection
		{
			get;
			private set;
		}

		public IHubProxy Proxy
		{
			get;
			private set;
		}

		DateTime m_CurrentTimeRequest = new DateTime();
		public DateTime CurrentTimeRequest
		{
			get
			{
				return m_CurrentTimeRequest;
			}

			set
			{
				if (Connection.State == ConnectionState.Connected)
				{
					String timestampUTC = value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
					Proxy.Invoke("TimeChange", timestampUTC);
				}
				else
				{
					m_CurrentTimeRequest = value;
				}
			}
		}


		public delegate void TimeChangedEventHandler(DateTime newTime);
		public event TimeChangedEventHandler TimeChanged;


		public ConnectionSignalR(String targetServer, String hubName)
		{
			Connection = new HubConnection(targetServer);
			Proxy = Connection.CreateHubProxy(hubName);

			Proxy.On<DateTime>("TimeChange", (newTime) =>
				{
					try
					{
						m_CurrentTimeRequest = newTime;
						DebugOutputStream.SlowInstance.WriteLine(newTime.ToString());
						if (TimeChanged != null)
							TimeChanged(m_CurrentTimeRequest);
					}
					catch (Exception e)
					{
						DebugOutputStream.SlowInstance.WriteLine("Warning: Failed to process DateTime received from SignalR.\nData: " + newTime + "\nException: " + e.Message);
					}
				});
		}

		public void BeginSync()
		{
			Connection.Start();
		}

		public void StopSync()
		{
			if (Connection.State != ConnectionState.Disconnected)
				Connection.Stop();
		}
	}
}
