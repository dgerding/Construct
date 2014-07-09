using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace netclient
{
	class ForwardingThread
	{
		private static Task m_ForwardTask;
		private static ushort m_ListenPort;

		private static bool m_ContinueThread = false;
		public static bool ContinueThread
		{
			get { return m_ContinueThread; }
		}

		private static List<KeyValuePair<IPAddress, ushort>> m_ForwardTargets = new List<KeyValuePair<IPAddress,ushort>>();

		public delegate void OnErrorHandler(String errorMessage);
		public static event OnErrorHandler OnError;

		public delegate void OnStatisticsUpdateHandler(uint newBytesPerSecond);
		public static event OnStatisticsUpdateHandler OnStatisticsUpdate;

		public static void ThreadFunc()
		{
			Socket listenSocket = null, forwardSocket = null;

			try
			{
				listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				listenSocket.Blocking = true;
				listenSocket.Bind(new IPEndPoint(IPAddress.Any, m_ListenPort));

				forwardSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				forwardSocket.Blocking = true;

				DateTime lastUpdateTime = new DateTime();
				uint currentBytes = 0;

				byte[] buffer = new byte[2048];
				while (m_ContinueThread)
				{
					ArrayList listenSockets = new ArrayList();
					listenSockets.Add(listenSocket);

					Socket.Select(listenSockets, null, null, 1000);

					if ((DateTime.Now - lastUpdateTime).TotalSeconds >= 1.0F)
					{
						if (OnStatisticsUpdate != null)
							OnStatisticsUpdate(currentBytes);

						currentBytes = 0;
						lastUpdateTime = DateTime.Now;
					}

					if (listenSockets.Count == 0)
						continue;

					int receivedBytes = listenSocket.Receive(buffer);
					currentBytes += (uint)receivedBytes;

					foreach (var target in m_ForwardTargets)
					{
						IPAddress targetIP = target.Key;
						ushort targetPort = target.Value;
						forwardSocket.SendTo(buffer, receivedBytes, SocketFlags.None, new IPEndPoint(targetIP, targetPort));
					}
				}
			}
			catch (Exception ex)
			{
				if (OnError != null)
					OnError(ex.Message);

				m_ContinueThread = false;
			}

			if (listenSocket != null)
				listenSocket.Close();

			if (forwardSocket != null)
				forwardSocket.Close();
		}

		public static void ClearForwardTargets()
		{
			m_ForwardTargets.Clear();
		}

		public static void AddForwardTarget(ushort forwardPort, IPAddress forwardTarget)
		{
			if (m_ContinueThread)
				throw new Exception("Attempted to add forwarding target while forwarding-thread was running");

			m_ForwardTargets.Add(new KeyValuePair<IPAddress, ushort>(forwardTarget, forwardPort));
		}

		public static void Start(ushort listenPort)
		{
			if (m_ContinueThread)
				throw new Exception("Attempted to start forwarding when forwarding was already running");

			m_ListenPort = listenPort;

			m_ContinueThread = true;
			m_ForwardTask = Task.Factory.StartNew((Action)ThreadFunc);
		}

		public static void Stop()
		{
			if (!m_ContinueThread)
				return;

			m_ContinueThread = false;
			m_ForwardTask.Wait();
		}
	}
}
