using System.Net;
using System.Net.Sockets;

namespace randomudp
{
	class Program
	{
		static void Main(string[] args)
		{
			/* Test program to send data for the netclient to forward */
			Socket udpSender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			while (true)
			{
				udpSender.SendTo(new byte[] { 25, 25, 25 }, new IPEndPoint(IPAddress.Loopback, 500));
				System.Threading.Thread.Sleep(100);
			}
		}
	}
}
