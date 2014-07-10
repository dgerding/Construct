using System;
using System.Net;
using Construct.Utilities.PortCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace PortCheck.Tests
{
	class PortOccupant : IDisposable
	{
		List<TcpListener> m_Listeners = new List<TcpListener>();

		//	The IAsyncResult for each listener is kept in sync to be the current async request
		List<IAsyncResult> m_AsyncResults = new List<IAsyncResult>();

		void OnNewConnection(IAsyncResult result)
		{
			int listenerIndex = m_Listeners.IndexOf(result.AsyncState as TcpListener);

			TcpListener listener = m_Listeners[listenerIndex];
			listener.EndAcceptSocket(result);

			m_AsyncResults[listenerIndex] = listener.BeginAcceptSocket(OnNewConnection, listener);
		}

		void GenerateNewListener(ushort port)
		{
			TcpListener newListener = new TcpListener(IPAddress.Any, port);
			newListener.Start();

			m_Listeners.Add(newListener);
			m_AsyncResults.Add(newListener.BeginAcceptSocket(OnNewConnection, newListener));
		}

		public void Dispose()
		{
			for (int i = 0; i < m_Listeners.Count; i++)
			{
				//m_Listeners[i].EndAcceptSocket(m_AsyncResults[i]);
				m_Listeners[i].Stop();
			}
		}

		public PortOccupant(ushort port)
		{
			GenerateNewListener(port);
		}

		public PortOccupant(ushort minPort, ushort maxPort)
		{
			for (ushort i = minPort; i <= maxPort; i++)
				GenerateNewListener(i);
		}

		public PortOccupant(ushort minPort, ushort maxPort, ushort interval)
		{
			for (ushort i = minPort; i <= maxPort; i += interval)
				GenerateNewListener(i);
		}
	}

	[TestClass]
	public class PortCheckTests
	{
		[TestMethod]
		public void TcpScanner_GetActivePorts()
		{
			PortOccupant occupant;
			List<ushort> ports;

			occupant = new PortOccupant(38880, 38890, 2);
			ports = TcpScanner.GetActivePorts(IPAddress.Loopback, 38880, 38890);
			Assert.IsTrue(new List<ushort>()
			{
				38880,
				38882,
				38884,
				38886,
				38888,
				38890
			}.Except(ports).Count() == 0);




			occupant = new PortOccupant(28880, 28890, 2);
			ports = TcpScanner.GetActivePorts(IPAddress.Loopback, 28880, 28890, 3);
			Assert.IsTrue(new List<ushort>()
			{
				28880,
				28886
			}.Except(ports).Count() == 0);



			ports = TcpScanner.GetActivePorts(IPAddress.Loopback, 45660, 45670);
			Assert.IsTrue(ports.Count == 0);
		}

		[TestMethod]
		public void TcpScanner_PortIsOpen()
		{
			PortOccupant occupant = new PortOccupant(13870);
			Assert.IsTrue(TcpScanner.PortIsOpen(IPAddress.Loopback, 13870));
			
			//	Pick some random value (shouldn't be open)
			Assert.IsFalse(TcpScanner.PortIsOpen(IPAddress.Loopback, 2252));
		}

		[TestMethod]
		public void TcpScanner_PortsAreOpen()
		{
			PortOccupant occupant = new PortOccupant(4330, 4350);
			List<ushort> portCheckList = new List<ushort>()
			{
				4332,
				4345,
				4339
			};

			Assert.IsTrue(TcpScanner.PortsAreOpen(IPAddress.Loopback, portCheckList));

			portCheckList.Add(29292); // Assuming this is free (if this fails, mess with this port)
			Assert.IsFalse(TcpScanner.PortsAreOpen(IPAddress.Loopback, portCheckList));
		}

		[TestMethod]
		public void Program_RunForList()
		{
			PortOccupant occupant = new PortOccupant(20200, 20210);

			String portCheckExecutable = "..\\..\\..\\PortCheck\\bin\\Debug\\PortCheck.exe";
			portCheckExecutable = Path.Combine(Directory.GetCurrentDirectory(), portCheckExecutable);
			bool exists = File.Exists(portCheckExecutable);

			if (!exists)
				Assert.Fail("PortCheck.exe could not be found. Make sure PortCheck is not a Class Library during testing.");

			List<ushort> portList = new List<ushort>()
			{
				20200,
				20201,
				20202,
				20203,
				20204,
				20205
			};

			Assert.IsTrue(PortCheckProgram.RunForList(IPAddress.Loopback, portList, portCheckExecutable));

			portList.Add(50000);
			Assert.IsFalse(PortCheckProgram.RunForList(IPAddress.Loopback, portList, portCheckExecutable));
		}
	}
}
