using System;
using System.Net;

namespace netclient
{
	public class AppConfig
	{
		public void DisplayWarnings()
		{

		}

		public bool ConfigComplete
		{
			get
			{
				return
					TargetHost != null &&
					SourcePort.HasValue &&
					DestinationPort.HasValue;
			}
		}

		public IPAddress TargetHost = null;
		public ushort? SourcePort = null;
		public ushort? DestinationPort = null;
		public bool RunSilent = false;


		public void Parse(String param)
		{
			String[] components = param.Split(':');

			switch (components[0])
			{
				case ("host"):
					TargetHost = IPAddressResolver.Resolve(components[1]);
					break;

				case ("sport"):
					SourcePort = ushort.Parse(components[1]);
					break;

				case ("dport"):
					DestinationPort = ushort.Parse(components[1]);
					break;

				case ("silent"):
					RunSilent = true;
					break;
			}
		}
	}
}
