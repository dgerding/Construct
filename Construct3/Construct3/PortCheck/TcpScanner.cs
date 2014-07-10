using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Construct.Utilities.PortCheck
{
	class ScanItem
	{
		public ushort TargetPort;
		public IPAddress TargetAddress;

		public bool PortIsOpen;
	}

	public static class TcpScanner
	{
		private static void QueryPort(object scanItemObject)
		{
			ScanItem scanItem = scanItemObject as ScanItem;

			// http://stackoverflow.com/questions/8016326/timeout-on-tcp-connection-attempt
			TcpClient client = new TcpClient();
			IAsyncResult result = client.BeginConnect(scanItem.TargetAddress, scanItem.TargetPort, null, null);
			result.AsyncWaitHandle.WaitOne(1000);
			scanItem.PortIsOpen = client.Connected;

			if (scanItem.PortIsOpen)
				client.Close();
		}

		public static List<ushort> GetActivePorts(IPAddress target, ushort minPort, ushort maxPort)
		{
			return GetActivePorts(target, minPort, maxPort, 1);
		}

		public static List<ushort> GetActivePorts(IPAddress target, ushort minPort, ushort maxPort, ushort interval)
		{
			if (minPort > maxPort)
			{
				ushort temp = minPort;
				minPort = maxPort;
				maxPort = temp;
			}

			List<KeyValuePair<Task, ScanItem>> queryList = new List<KeyValuePair<Task, ScanItem>>();
			for (ushort i = minPort; i <= maxPort; i += interval)
			{
				ScanItem currentItem = new ScanItem();
				currentItem.TargetAddress = target;
				currentItem.TargetPort = i;

				Task queryTask = Task.Factory.StartNew((Action<object>)QueryPort, currentItem);

				queryList.Add(new KeyValuePair<Task, ScanItem>(queryTask, currentItem));
			}

			for (int i = 0; i < queryList.Count; i++)
			{
				queryList[i].Key.Wait();
			}

			List<ushort> activePorts = new List<ushort>();
			foreach (var query in queryList)
			{
				ScanItem scanItem = query.Value;
				if (scanItem.PortIsOpen)
					activePorts.Add(scanItem.TargetPort);
			}

			return activePorts;
		}

		public static bool PortIsOpen(IPAddress targetAddress, ushort port)
		{
			ScanItem scanItem = new ScanItem();
			scanItem.TargetAddress = targetAddress;
			scanItem.TargetPort = port;

			QueryPort(scanItem);
			return scanItem.PortIsOpen;
		}

		public static bool PortsAreOpen(IPAddress targetAddress, List<ushort> portList)
		{
			foreach (ushort port in portList)
			{
				if (!PortIsOpen(targetAddress, port))
					return false;
			}

			return true;
		}
	}
}
