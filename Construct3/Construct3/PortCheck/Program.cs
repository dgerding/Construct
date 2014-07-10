using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

/* Port check is both a library and an application that can be ran on its own. The application
 *	is to allow a general firewall rule to be applied to a single port checking program that
 *	would be called externally. The class library for the project also provides a quick API for
 *	managing a PortCheck process instance (for convenience.)
 *	
 * The TcpScanner is the direct API, PortCheckProgram.RunForList is a simple API for running
 *	the EXE and getting a result easily.
 */

namespace Construct.Utilities.PortCheck
{
	public static class PortCheckProgram
	{
		//	Note: Blocking function

		//	Instantiates the given PortCheck program and has it check the
		//		given port list. Returns true if all of the ports in the
		//		list are open.
		public static bool RunForList(IPAddress target, List<ushort> portList, String portCheckExe)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = portCheckExe;

			List<String> paramsList = new List<String>();
			paramsList.Add(target.ToString());
			for (int i = 0; i < portList.Count; i++)
				paramsList.Add(portList[i].ToString());

			psi.Arguments = String.Join(" ", paramsList);

			Process checkerProcess = Process.Start(psi);
			checkerProcess.WaitForExit();

			return checkerProcess.ExitCode == 1;
		}

		static int Main(string[] args)
		{
			bool isOpen = TcpScanner.PortIsOpen(IPAddress.Parse("172.25.110.12"), 1025);

			//	Param layout is:
			// 0: Target IP
			// 1: Port 1
			// 2: Port 2
			// x: Port x

			IPAddress targetAddress;
			if (!IPAddress.TryParse(args[0], out targetAddress))
				return -1;

			List<ushort> portList = new List<ushort>();

			for (int i = 1; i < args.Length; i++)
			{
				ushort currentPort;
				if (!ushort.TryParse(args[i], out currentPort))
					return -1;
				portList.Add(currentPort);
			}

			bool portsAreOpen = false;
			try
			{
				portsAreOpen = TcpScanner.PortsAreOpen(targetAddress, portList);
			}
			catch
			{
				return -1;
			}

			return portsAreOpen ? 1 : 0;
		}
	}
}
