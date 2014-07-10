using System;
using System.ComponentModel;

namespace RemoteSensor
{
	public class ConnectionManager
	{
		public static bool IsNetworkAvailable
		{
			get
			{
				return Reachability.IsHostReachable("www.google.com");	
			}
		}
		
		
	}
}

