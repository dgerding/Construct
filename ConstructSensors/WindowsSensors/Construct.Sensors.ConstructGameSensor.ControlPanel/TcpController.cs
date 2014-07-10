using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

namespace Columbia.CCF.UI.ControlPanel
{
    public class TcpController : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string _conStr = ConfigurationManager.ConnectionStrings["Columbia.CCF.UI.Properties.Settings.CCFConnectionString"].ConnectionString;

        #region Constructors

        public TcpController()
        {
        }

        ~TcpController()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public Dictionary<string, TcpServerContext> AudioSockets;
        public Dictionary<string, TcpServerContext> VideoSockets;
        public TcpServerContext MainTcpServerSocket;
        public List<def_Object> PlayerNames;
        public int CurrentGameId { get; set; }

        #endregion

        public void Initialize()
        {
            TcpServerContext tcpContext;
            int iGameIdAudioVideo = 0; // to be used for the audio/video services
            List<AutoResetEvent> colDoneEvents = new List<AutoResetEvent>();

            VideoSockets = new Dictionary<string, TcpServerContext>(6);
            AudioSockets = new Dictionary<string, TcpServerContext>(5);

            try
            {
                using (SqlConnection cn = new SqlConnection(_conStr))
                {
                    using (SqlCommand cm = cn.CreateCommand())
                    {
                        cm.CommandType = CommandType.Text;
                        cm.CommandText = "SELECT TcpHost, TcpPort FROM dbo.def_TcpServers WHERE GameDefId = 1 AND Active = 1 AND TcpServerType = 'WebCam'";

                        cn.Open();
                        using (SqlDataReader drTcpServers = cm.ExecuteReader())
                        {
                            while (drTcpServers.Read())
                            {
                                logger.Info("Connecting to TCP Server for Video control: {0}:{1} ...", drTcpServers.GetValue(0), drTcpServers.GetValue(1));
                                AutoResetEvent doneEvent = new AutoResetEvent(false);
                                colDoneEvents.Add(doneEvent);
                                tcpContext = new TcpServerContext(iGameIdAudioVideo, drTcpServers.GetString(0), drTcpServers.GetInt32(1), null, doneEvent);
                                VideoSockets.Add(drTcpServers.GetString(0), tcpContext);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToTcpServer), tcpContext);
                            }
                        }
                    }
                }

                using (SqlConnection cn = new SqlConnection(_conStr))
                {
                    using (SqlCommand cm = cn.CreateCommand())
                    {
                        cm.CommandType = CommandType.Text;
                        cm.CommandText = "SELECT TcpHost, TcpPort FROM dbo.def_TcpServers WHERE GameDefId = 1 AND Active = 1 AND TcpServerType = 'HeadsetAudio'";

                        cn.Open();
                        using (SqlDataReader drTcpServers = cm.ExecuteReader())
                        {
                            while (drTcpServers.Read())
                            {
                                logger.Info("Connecting to TCP Server for Audio control: {0}:{1} ...", drTcpServers.GetValue(0), drTcpServers.GetValue(1));
                                AutoResetEvent doneEvent = new AutoResetEvent(false);
                                colDoneEvents.Add(doneEvent);
                                tcpContext = new TcpServerContext(iGameIdAudioVideo, drTcpServers.GetString(0), drTcpServers.GetInt32(1), null, doneEvent);
                                AudioSockets.Add(drTcpServers.GetString(0), tcpContext);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToTcpServer), tcpContext);
                            }
                        }
                    }
                }

                if (!WaitAll(colDoneEvents.ToArray()))
                {
                    throw new ApplicationException("Timeout expired before all servers connected successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }

            LoadPlayers();
        }

        public void ConnectToGameServer(string serverName)
        {
            int iGameId = 0;
            using (SqlConnection cn = new SqlConnection(_conStr))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "dbo.usp_GetCurrentGame";

                    cn.Open();
                    iGameId = (int)cm.ExecuteScalar();
                }

                CurrentGameId = iGameId;

                logger.Info("Connecting to Main TCP Server for control: {0}:{1} ...", serverName, 1234);
                //AutoResetEvent doneEvent = new AutoResetEvent(false);
                //colDoneEvents.Add(doneEvent);
                MainTcpServerSocket = new TcpServerContext(iGameId, "construct1", 1234, null, null);
                ConnectToTcpServer(MainTcpServerSocket);
            }
        }

        public void LoadPlayers()
        {
            DataClassesDataContext ctx = new DataClassesDataContext();
            PlayerNames = (from d in ctx.def_Objects
                           where d.ObjectTypeId == 1 && d.Pseudonim != null & d.isAvailable==true
                           select d).ToList();
        }

        private bool WaitAll(WaitHandle[] handles)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                foreach (WaitHandle handle in handles)
                {
                    if (!handle.WaitOne(30000, false))
                        return false;
                }
                return true;
            }
            else
                return WaitHandle.WaitAll(handles, 30000, false);
        }

        private static void ConnectToTcpServer(object oTcpServerContext)
        {
            TcpServerContext tcpContext = oTcpServerContext as TcpServerContext;

            try
            {
                TcpClient clientSocket = new TcpClient(tcpContext.TcpHost, tcpContext.TcpPort);
                tcpContext.ClientSocket = clientSocket;
                tcpContext.ClientStream = clientSocket.GetStream();
                logger.Info("Connected to Tcp Server: {0}:{1}", tcpContext.TcpHost, tcpContext.TcpPort);
                if (tcpContext.AfterConnectAction != null)
                    tcpContext.AfterConnectAction(tcpContext);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error Connecting to Tcp Server: {0}:{1}\n", tcpContext.TcpHost, tcpContext.TcpPort), ex);
            }
            if (tcpContext.ActionDoneEvent != null)
                tcpContext.ActionDoneEvent.Set();
        }

        public void SendAudioMessage(string strServer, string strMessage)
        {
            SendMessage(AudioSockets[strServer], strMessage);
        }

        public void SendAudioMessage(string strServer, IEnumerable<string> colMessages)
        {
            StringBuilder sbMessage = new StringBuilder();
            bool bFirst = true;
            foreach (string strMessage in colMessages)
            {
                if (bFirst)
                    bFirst = false;
                else
                    sbMessage.Append('|');
                sbMessage.Append(strMessage);
            }
            SendAudioMessage(strServer, sbMessage.ToString());
        }

        public void SendVideoMessage(string strServer, string strMessage)
        {
            SendMessage(VideoSockets[strServer], strMessage);
        }

        public void SendVideoMessage(string strServer, IEnumerable<string> colMessages)
        {
            StringBuilder sbMessage = new StringBuilder();
            bool bFirst = true;
            foreach (string strMessage in colMessages)
            {
                if (bFirst)
                    bFirst = false;
                else
                    sbMessage.Append('|');
                sbMessage.Append(strMessage);
            }
            SendVideoMessage(strServer, sbMessage.ToString());
        }

        private static void SendMessage(TcpServerContext tcpServerContext, string strMessage)
        {
            if (string.IsNullOrEmpty(strMessage))
                return;

            if (tcpServerContext.ClientSocket == null)
                return;

            logger.Info("Message sent to server {0}:{1}\n{2}", tcpServerContext.TcpHost, tcpServerContext.TcpPort, strMessage);

            byte[] inputToBeSent = System.Text.Encoding.ASCII.GetBytes(string.Format("{0}\r", strMessage).ToCharArray());
            if (tcpServerContext.ClientStream != null && tcpServerContext.ClientSocket != null && tcpServerContext.ClientSocket.Connected)
            {
                try
                {
                    tcpServerContext.ClientStream.Write(inputToBeSent, 0, inputToBeSent.Length);
                    tcpServerContext.ClientStream.Flush();
                }
                catch (System.IO.IOException ex)
                {
                    logger.Info("Error writing to {0}:{1}. Error Message: {2}", tcpServerContext.TcpHost, tcpServerContext.TcpPort, ex.Message);
                }
            }
        }

        public string SendReceiveAudioMessage(string strServer, string strMessage)
        {
            return SendReceiveMessage(AudioSockets[strServer], strMessage);
        }

        public string SendReceiveVideoMessage(string strServer, string strMessage)
        {
            return SendReceiveMessage(VideoSockets[strServer], strMessage);
        }

        public string SendReceiveMainServerMessage(string strMessage)
        {
            return SendReceiveMessage(MainTcpServerSocket, strMessage);
        }

        private string SendReceiveMessage(TcpServerContext tcpServerContext, string strMessage)
        {
            string strReturnValue = "Status not retrieved";

            if (string.IsNullOrEmpty(strMessage))
                return strReturnValue;

            if (tcpServerContext.ClientSocket == null)
                return strReturnValue;

            byte[] inputToBeSent = System.Text.Encoding.ASCII.GetBytes(string.Format("{0}\r", string.Format(strMessage, tcpServerContext.GameId)).ToCharArray());
            if (tcpServerContext.ClientStream != null && tcpServerContext.ClientSocket != null && tcpServerContext.ClientSocket.Connected)
            {
                try
                {
                    tcpServerContext.ClientStream.Write(inputToBeSent, 0, inputToBeSent.Length);
                    tcpServerContext.ClientStream.Flush();

                    NetworkStream tcpStream = tcpServerContext.ClientStream;

                    bool DONE = false;
                    Byte[] received = new Byte[1024];
                    strReturnValue = string.Empty;
                    while (tcpStream.CanRead && !DONE)
                    {
                        //We need the DONE condition here because there is possibility that
                        //the stream is ready to be read while there is nothing to be read.
                        //if (_tcpStream.DataAvailable)
                        {
                            int nBytesReceived = tcpStream.Read(received, 0, received.Length);

                            if (nBytesReceived > 1 || received[nBytesReceived - 1] != 10)
                            //Console.WriteLine(dataReceived);
                                strReturnValue += Encoding.ASCII.GetString(received, 0, nBytesReceived);

                            if (received[nBytesReceived - 1] == 10 && !tcpStream.DataAvailable)
                                DONE = true;
                        }
                    }
                }
                catch (System.IO.IOException ex)
                {
                    logger.Info("Error writing to {0}:{1}. Error Message: {2}", tcpServerContext.TcpHost, tcpServerContext.TcpPort, ex.Message);
                }
            }
            return strReturnValue;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (AudioSockets != null)
            {
                foreach (TcpServerContext socketContext in AudioSockets.Values)
                {
                    if (socketContext.ClientStream != null)
                    {
                        socketContext.Dispose();
                    }
                }
                AudioSockets.Clear();
                AudioSockets = null;
            }

            if (VideoSockets != null)
            {
                foreach (TcpServerContext socketContext in VideoSockets.Values)
                {
                    if (socketContext.ClientStream != null)
                    {
                        socketContext.Dispose();
                    }
                }
                VideoSockets.Clear();
                VideoSockets = null;
            }
            if (MainTcpServerSocket != null && MainTcpServerSocket.ClientStream != null)
            {
                MainTcpServerSocket.Dispose();
            }
        }
        #endregion
    }

    /// <summary>
    /// Holds all information needed to associate a thread
    /// with a particular TcpServer, such as the Tcp Server used to manage services
    /// </summary>
    public class TcpServerContext : IDisposable
    {
        public delegate void OnAfterConnect(TcpServerContext tcpContext);

        public int GameId;
        public string TcpHost;
        public int TcpPort;
        public TcpClient ClientSocket;
        public NetworkStream ClientStream;
        public OnAfterConnect AfterConnectAction;
        public AutoResetEvent ActionDoneEvent;

        public TcpServerContext(int iGameId, string strTcpHost, int iTcpPort, OnAfterConnect delAfterConnectAction, AutoResetEvent doneEvent)
        {
            GameId = iGameId;
            TcpHost = strTcpHost;
            TcpPort = iTcpPort;
            ClientSocket = null;
            ClientStream = null;
            AfterConnectAction = delAfterConnectAction;
            ActionDoneEvent = doneEvent;
        }

        ~TcpServerContext()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (ClientStream != null)
            {
                ClientStream.Close();
                ClientStream = null;
            }
        }
        #endregion
    }
}