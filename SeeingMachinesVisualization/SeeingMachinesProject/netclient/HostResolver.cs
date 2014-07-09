using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace netclient
{
	static class IPAddressResolver
	{
		public static IPAddress Resolve(String hostname)
		{
			IPAddress target;
			if (!IPAddress.TryParse(hostname, out target))
			{

				IPAddress[] dnsAddresses;
				try
				{
					dnsAddresses = Dns.GetHostAddresses(hostname);
				}
				catch (Exception e)
				{
					return null;
				}

				if (dnsAddresses.Length == 0)
					return null;

				foreach (IPAddress address in dnsAddresses)
				{
					//	Use the first IPv4 address that we come across
					if (address.AddressFamily == AddressFamily.InterNetwork)
					{
						target = address;
						break;
					}
				}
			}

			return target;
		}
	}
}
