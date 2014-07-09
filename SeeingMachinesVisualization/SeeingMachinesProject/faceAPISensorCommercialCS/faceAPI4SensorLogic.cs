using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Net;
using SMFramework;
using System.Threading.Tasks;

namespace faceAPISensorCommercialCS
{
	class CoreProcess
	{
		Process m_ProcessObject;
		UdpClient m_UDPBridge;

		short m_CommsPort, m_DataPort;

		private static int msgClose = 1000;
		private static int msgChangeDataPort = 1001;

		public CoreProcess(short commsPort, short dataPort)
		{
			ProcessStartInfo psi = new ProcessStartInfo();

//#if TEST_LOGIC
			//psi.FileName = Path.GetFullPath("../../../faceAPISensorCommercial/bin/x64/Debug/faceAPISensorCommercial.exe");
//#else
			psi.FileName = "faceAPISensorCommercialCore.exe";
//#endif

			psi.Arguments = commsPort + " " + dataPort;
			
			m_ProcessObject = Process.Start(psi);

			m_UDPBridge = new UdpClient(AddressFamily.InterNetwork);

			m_CommsPort = commsPort;
			m_DataPort = dataPort;
		}

		public void ChangeDataPort(short newPort)
		{
			SendMessage(msgChangeDataPort);
			SendMessage(newPort);
		}

		public void Close()
		{
			SendMessage(msgClose);
			m_ProcessObject.WaitForExit(3000);
			if (!m_ProcessObject.HasExited)
				m_ProcessObject.Kill();
		}

		private void SendMessage(int id)
		{
			byte[] data = BitConverter.GetBytes(id);
			m_UDPBridge.Send(data, data.Length, new IPEndPoint(IPAddress.Loopback, m_CommsPort));
		}
	}

	class faceAPI4SensorLogic
	{
		CoreProcess m_CoreProcess;

		//  Class says FaceLab, it just uses CoreData
		//  TODO: Rename FaceLabConnection
		SMFramework.FaceLabConnection m_DataConnection;

		public void Start()
		{
			short dataPort = 4005, commsPort = 654;
			m_CoreProcess = new CoreProcess(dataPort, commsPort);

			FaceLabSignalConfiguration config = new FaceLabSignalConfiguration();
			config.Port = dataPort;
			m_DataConnection = new FaceLabConnection(config);

			m_ContinueQuery = true;
			m_QueryTask = Task.Factory.StartNew((Action<object>)QueryTask, this);
		}

		public void ChangeDataPort(short newPort)
		{
			m_CoreProcess.ChangeDataPort(newPort);
		}

		public void Stop()
		{
			m_ContinueQuery = false;
			m_QueryTask.Wait();

			m_CoreProcess.Close();
			m_CoreProcess = null;

			m_DataConnection.Disconnect();
			m_DataConnection = null;
		}

		public delegate void OnNewDataHandler(FaceData faceData);
		public event OnNewDataHandler OnNewData;

		Task m_QueryTask;
		bool m_ContinueQuery = false;
		private static void QueryTask(object data)
		{
			faceAPI4SensorLogic logicObject = data as faceAPI4SensorLogic;
			FaceLabConnection connection = logicObject.m_DataConnection;

			while (logicObject.m_ContinueQuery)
			{
				if (connection.RetrieveNewData())
				{
					FaceData newData = connection.CurrentData;
					if (newData.SnapshotTimestamp.Kind != DateTimeKind.Utc)
						newData.SnapshotTimestamp = newData.SnapshotTimestamp.ToUniversalTime();
					if (logicObject.OnNewData != null)
						logicObject.OnNewData(newData);

					continue;
				}

				System.Threading.Thread.Sleep(1);
			}
		}
	}
}
