using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

namespace ConstructGameListener
{
    /// <summary>
    /// Reads messaged sent via a TCP/IP connection for a game session
    /// and sends back responses if any
    /// </summary>
    public class SocketConnectionProcessor
    {
        //private static Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly Logger Log = LogManager.GetLogger("Connection");
        private static readonly Logger LogMsg = LogManager.GetLogger("Connection.Msg");
        private static readonly Logger LogMsgVi = LogManager.GetLogger("Connection.Msg.VI");

        Socket _clientSocket;
        byte[] _readBuffer;
        Encoding _textEncoding = Encoding.ASCII;
        ProducerConsumerQueue<string[]> _messageQueue;
        ConstructGameProcessor _gameProcessor;
        SortedList<string, GameContext> _socketContext;
        GameContext _context;
        bool _bExitGame = false;
        int _iReceiveTimeout;
        Action _abortTcpServer;
        Action<SocketConnectionProcessor> _finishAction;

        //void InstantiateCounter(int iTcpPort)
        //{
        //    // setup the counter to write to
        //    string strCounterInstance = string.Format("Socket_{0}", iTcpPort);
        //    _ctrTotalSocketsCurrentlyOpen = new PerformanceCounter("ConstructTcpServer", "Total Sockets Currently Open", strCounterInstance, false);
        //    _ctrMessagesReceived = new PerformanceCounter("ConstructTcpServer", "Msg Received", strCounterInstance, false);
        //}
        public SocketConnectionProcessor(Socket clientSocket, SortedList<string, GameContext> socketContext, Action abortTcpServer, Action<SocketConnectionProcessor> finishAction)
        {
            //InstantiateCounter(1234);
            _clientSocket = clientSocket;
            _readBuffer = new Byte[4096];
            if (!int.TryParse(ConfigurationManager.AppSettings["BeforeStartPlayTimeout"], out _iReceiveTimeout))
                _iReceiveTimeout = 900000; // Wait at most 15 minutes
            _clientSocket.ReceiveTimeout = _iReceiveTimeout;
            _socketContext = socketContext;
            _abortTcpServer = abortTcpServer;
            _finishAction = finishAction;
        }

        public Thread ProcessorThread { get; set; }
        //PerformanceCounter _ctrTotalSocketsCurrentlyOpen;
        //PerformanceCounter _ctrMessagesReceived;

        public void ProcessClientConnection()
        {
            try
            {
                Log.Debug("in process client connection");
                ProcessorThread = Thread.CurrentThread;
                string strMessage = string.Empty;
                string strNewMessage;
                bool bContinue = true;
                string[] colMessages = null;
                int iIndex = 0;
                byte[] returningBytes;
                // SRZ Change -- use return + newline instead of just return for Torque's processing
                byte[] lineTerminator = new byte[] { 13, 10 };

                try
                {
                    while (bContinue && _clientSocket.Connected)
                    {
                        int iBytesReceived = ReadMessage(out strNewMessage);

                        if (iBytesReceived == 0)
                            bContinue = false;
                        else
                        {
                            //Console.WriteLine(strNewMessage.Replace("\r", "\n"));
                            colMessages = (strMessage + strNewMessage).Split('\r');
                            for (iIndex = 0; iIndex < colMessages.Length - 1; iIndex++)
                            {
                                returningBytes = ProcessMessage(colMessages[iIndex]);
                                if (returningBytes != null)
                                {
                                    _clientSocket.Send(returningBytes, returningBytes.Length, SocketFlags.None);
                                    _clientSocket.Send(lineTerminator);
                                }
                            }

                            strMessage = colMessages[colMessages.Length - 1];
                            if (_bExitGame)
                            {
                                // We've reached the end of the game
                                bContinue = false;
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Log.Warn("Socket connection timed out after {0} seconds.", _iReceiveTimeout / 1000);
                        //Console.WriteLine("Socket connection timed out.");
                    }
                    else if (ex.SocketErrorCode == SocketError.ConnectionAborted)
                    {
                        // the connection was aborted most likely because the server was shut down
                        Log.Error(ex);
                    }
                    else
                    {
                        Log.Error(ex);
                        //Console.WriteLine(ex.ToString());
                        if (colMessages != null && colMessages.Length > iIndex)
                            Log.Error("Last message processed: {0}\n", colMessages[iIndex]);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    //Console.WriteLine(ex.ToString());
                    if (colMessages != null && colMessages.Length > iIndex)
                        Log.Error("Last message processed: {0}\n", colMessages[iIndex]);
                }


                if (_clientSocket != null)
                {
                    Log.Info("Socket {0} - {1} - Client Disconnected", _clientSocket.GetHashCode(), _clientSocket.LocalEndPoint);
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                }
                //_ctrTotalSocketsCurrentlyOpen.RawValue--;
            }
            finally
            {
                _finishAction(this);
            }
        }

        public void Close()
        {
            Log.Info("Socket {0} - {1} - Connection Aborted due to server shutdown", _clientSocket.GetHashCode(), _clientSocket.LocalEndPoint);
            Socket clientSocket = _clientSocket;
            _clientSocket = null;
            clientSocket.Close();
        }

        protected void InitContext(string strKey)
        {
            InitContext(ref strKey);
        }

        protected void InitContext(ref string strKey)
        {
            if (_context == null && _socketContext != null && _socketContext.ContainsKey(strKey))
            {
                _context = _socketContext[strKey];
                if (_context == null)
                    throw new ApplicationException(string.Format("Invalid Game ID: {0}. Could not determine game context.", strKey));
                _messageQueue = _context.MessageQueue;
                _gameProcessor = _context.GameProcessor;
            }
        }

        private static void ConnectToTcpServer(object oTcpServerContext)
        {
            TcpServerContext tcpContext = oTcpServerContext as TcpServerContext;

            try
            {
                TcpClient clientSocket = new TcpClient(tcpContext.TcpHost, tcpContext.TcpPort);
                tcpContext.ClientSocket = clientSocket;
                tcpContext.ClientStream = clientSocket.GetStream();
                Log.Info("Connected to Tcp Server: {0}:{1}", tcpContext.TcpHost, tcpContext.TcpPort);
                //Console.WriteLine(string.Format("Connected to Tcp Server: {0}:{1}", tcpContext.TcpHost, tcpContext.TcpPort));
                if (tcpContext.AfterConnectAction != null)
                    tcpContext.AfterConnectAction(tcpContext);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error Connecting to Tcp Server: {0}:{1}\n", tcpContext.TcpHost, tcpContext.TcpPort), ex);
                //Console.WriteLine(string.Format("Error Connecting to Tcp Server: {0}:{1}", tcpContext.TcpHost, tcpContext.TcpPort));
                //Console.WriteLine(ex.ToString());
            }
        }

        
        // Read a string from a socket.
        int ReadMessage(out string strMessage)
        {
            int iBytes = 0;
            iBytes = _clientSocket.Receive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None);
            if (iBytes > 0)
                strMessage = _textEncoding.GetString(_readBuffer, 0, iBytes);
            else
                strMessage = string.Empty;
            return iBytes;
        }

        byte[] ProcessMessage(string strMessage)
        {
            TcpServerContext tcpContext;
            byte[] inputToBeSent;
            int iGameId;
            string strReturnValue = null;
            string[] colMessageParts = strMessage.Split('|');
            string strAppName;
            string strAppFileAndPath;

            if (colMessageParts.Length > 0)
            {
                switch (colMessageParts[0])
                {
                    case "StartGame":
                        //* StartGame|<Game Name>\r
                        //* GameID|<New ID from DB>\r

                        LogMsg.Debug(strMessage);
                        //Console.WriteLine(strMessage);
                        _messageQueue = new ProducerConsumerQueue<string[]>(100);
                        _gameProcessor = new ConstructGameProcessor(_messageQueue);
                        iGameId = _gameProcessor.StartGame(colMessageParts[1]);
                        strReturnValue = string.Format("GameID|{0}", iGameId);

                        LogMsg.Info("{0} game started. GameId = {1}", colMessageParts[1], iGameId);

                        Thread queueThread = new Thread(new ThreadStart(_gameProcessor.QueueProcessor));
                        queueThread.Name = "GameProcessor";
                        queueThread.Start();

                        _context = new GameContext(iGameId, _messageQueue, _gameProcessor, queueThread);
                        _socketContext.Add(iGameId.ToString(), _context);

                        _colAudioSockets = new List<TcpServerContext>(5);
                        try
                        {
                            // Create data abstractions for given intervals
                            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConstructGameDB"].ConnectionString))
                            {
                                using (SqlCommand cm = cn.CreateCommand())
                                {
                                    cm.CommandType = CommandType.Text;
                                    cm.CommandText = "SELECT TcpHost, TcpPort FROM dbo.def_TcpServers WHERE GameDefId = 1 AND Active = 1 AND TcpServerType = 'Audio'";

                                    cn.Open();
                                    using (SqlDataReader drTcpServers = cm.ExecuteReader())
                                    {
                                        while (drTcpServers.Read())
                                        {
                                            Log.Info("Connecting to TCP Server for Audio control: {0}:{1} ...", drTcpServers.GetValue(0), drTcpServers.GetValue(1));
                                            //tcpContext = new TcpServerContext(iGameId, drTcpServers.GetString(0), drTcpServers.GetInt32(1), StartAudioApp);
                                            tcpContext = new TcpServerContext(iGameId, drTcpServers.GetString(0), drTcpServers.GetInt32(1), null);
                                            _colAudioSockets.Add(tcpContext);
                                            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToTcpServer), tcpContext);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warn(ex);
                            //Console.WriteLine(ex.ToString());
                        }

                        _colVideoSockets = new List<TcpServerContext>(5);
                        try
                        {
                            // Create data abstractions for given intervals
                            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConstructGameDB"].ConnectionString))
                            {
                                using (SqlCommand cm = cn.CreateCommand())
                                {
                                    cm.CommandType = CommandType.Text;
                                    cm.CommandText = "SELECT TcpHost, TcpPort FROM dbo.def_TcpServers WHERE GameDefId = 1 AND Active = 1 AND TcpServerType = 'Video'";

                                    cn.Open();
                                    using (SqlDataReader drTcpServers = cm.ExecuteReader())
                                    {
                                        while (drTcpServers.Read())
                                        {
                                            Log.Info("Connecting to TCP Server for Video control: {0}:{1} ...", drTcpServers.GetValue(0), drTcpServers.GetValue(1));
                                            //tcpContext = new TcpServerContext(iGameId, drTcpServers.GetString(0), drTcpServers.GetInt32(1), StartVideoApp);
                                            tcpContext = new TcpServerContext(iGameId, drTcpServers.GetString(0), drTcpServers.GetInt32(1), null);
                                            _colVideoSockets.Add(tcpContext);
                                            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToTcpServer), tcpContext);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warn(ex);
                            //Console.WriteLine(ex.ToString());
                        }
                        if (_context != null)
                            _context.CurrentStatus = "Game Started";

                        break;
                    case "CountdownStarted":
                        if (_colVideoSockets != null)
                        {
                            inputToBeSent = System.Text.Encoding.ASCII.GetBytes(string.Format("StartRecordVideo|{0}\r", _context.GameId).ToCharArray());
                            foreach (TcpServerContext videoSocket in _colVideoSockets)
                            {
                                if (videoSocket.ClientStream != null && videoSocket.ClientSocket != null && videoSocket.ClientSocket.Connected)
                                {
                                    try
                                    {
                                        StartVideoApp(videoSocket);
                                    }
                                    catch (System.IO.IOException ex)
                                    {
                                        Log.Error("Unable to start video recording on {0}:{1}. Error: {2}", videoSocket.TcpHost, videoSocket.TcpPort, ex);
                                    }
                                }
                            }
                        }

                        if (_colAudioSockets != null)
                        {
                            foreach (TcpServerContext audioSocket in _colAudioSockets)
                            {
                                if (audioSocket.ClientStream != null && audioSocket.ClientSocket != null && audioSocket.ClientSocket.Connected)
                                {
                                    try
                                    {
                                        StartAudioApp(audioSocket);
                                    }
                                    catch (System.IO.IOException ex)
                                    {
                                        Log.Error("Unable to start audio recording on {0}:{1}. Error: {2}", audioSocket.TcpHost, audioSocket.TcpPort, ex);
                                    }
                                }
                            }
                        }
                        if (_context != null)
                            _context.CurrentStatus = "Countdown Started";
                        break;
                    case "StartPlay":
                        LogMsg.Debug(strMessage);
                        //Console.WriteLine(strMessage);
                        if (_context == null)
                            InitContext(ref colMessageParts[1]);
                        _messageQueue.SyncEnqueue(colMessageParts);

                        if (_colAudioSockets != null)
                        {
                            inputToBeSent = System.Text.Encoding.ASCII.GetBytes(string.Format("StartRecordAudio|{0}\r", _context.GameId).ToCharArray());
                            foreach (TcpServerContext audioSocket in _colAudioSockets)
                            {
                                if (audioSocket.ClientStream != null && audioSocket.ClientSocket != null && audioSocket.ClientSocket.Connected)
                                {
                                    try
                                    {
                                        audioSocket.ClientStream.Write(inputToBeSent, 0, inputToBeSent.Length);
                                        audioSocket.ClientStream.Flush();
                                    }
                                    catch (System.IO.IOException)
                                    {
                                    }
                                }
                            }
                        }

                        if (_colVideoSockets != null)
                        {
                            inputToBeSent = System.Text.Encoding.ASCII.GetBytes(string.Format("StartRecordVideo|{0}\r", _context.GameId).ToCharArray());
                            foreach (TcpServerContext videoSocket in _colVideoSockets)
                            {
                                if (videoSocket.ClientStream != null && videoSocket.ClientSocket != null && videoSocket.ClientSocket.Connected)
                                {
                                    try
                                    {
                                        videoSocket.ClientStream.Write(inputToBeSent, 0, inputToBeSent.Length);
                                        videoSocket.ClientStream.Flush();
                                    }
                                    catch (System.IO.IOException)
                                    {
                                    }
                                }
                            }
                        }

                        int iReceiveTimeout;
                        if (int.TryParse(ConfigurationManager.AppSettings["AfterStartPlayTimeout"], out iReceiveTimeout))
                            _clientSocket.ReceiveTimeout = iReceiveTimeout;
                        else
                            _clientSocket.ReceiveTimeout = 60000; // Wait at most 60 seconds

                        if (_context != null)
                            _context.CurrentStatus = "Play Started";

                        break;
                    case "EndGame":
                        LogMsg.Debug(strMessage);
                        //Console.WriteLine(strMessage);
                        if (_context == null)
                            InitContext(ref colMessageParts[1]);
                        _socketContext.Remove(colMessageParts[1]); // second element is the Game ID
                        _messageQueue.SyncEnqueue(colMessageParts);
                        _messageQueue.ExitThreadEvent.Set();
                        _context = null;
                        _messageQueue = null;
                        _gameProcessor = null;
                        _bExitGame = true;
                        break;
                    case "AddObject":
                        LogMsg.Debug(strMessage);
                        //Console.WriteLine(strMessage);
                        // AddObject|<GameId>|{Player or Block}|<Temp ID from Game>|<Player or Object name>\r
                        // MapID|<Temp ID from Game>|<New ID from DB>\r
                        if (_context == null)
                            InitContext(ref colMessageParts[1]);
                        strReturnValue = string.Format("MapID|{0}|{1}", colMessageParts[3], _gameProcessor.AddObject(_context.GameId, colMessageParts[2], colMessageParts[4]));
                        break;
                    case "LogVisualInventory":
                        LogMsgVi.Debug(strMessage);
                        if (_context == null)
                            InitContext(ref colMessageParts[1]);
                        _messageQueue.SyncEnqueue(colMessageParts);
                        break;
                    case "AbortTcpServer":
                        LogMsg.Debug(strMessage);
                        LogMsg.Info("Server is shutting down");
                        _bExitGame = true;
                        _abortTcpServer();
                        break;
                    case "StartApp":
                        // syntax: StartApp|App Name|Player Alias|Command Line Params

                        // StartApp is called from the control panel. It only handles one game at a time, therefore initialize
                        // only one context and add it to the list of contexts as GameId 0.
                        // Basically, if a new game starts without clearing the current context, simply overwrite the context.
                        // This context is useful for holding the Alias of the player. So, if the control panel wants to
                        // simply set the alias of a player, it can call "StartApp||Alias|", effectively passing no application 
                        // to start nor any command-line parameters

                        strAppName = colMessageParts[1];
                        Log.Info("Starting Application {0}", strAppName);

                        // set the context of this service by using the player alias.
                        if (colMessageParts.Length > 2)
                        {
                            // we received a player alias, so set it
                            _context = new GameContext(colMessageParts[2]);
                        }
                        else
                        {
                            _context = new GameContext(string.Empty);
                        }
                        _socketContext["0"] = _context;

                        // start the app
                        strAppFileAndPath = ConfigurationManager.AppSettings[strAppName];
                        if (!string.IsNullOrEmpty(strAppFileAndPath))
                        {
                            //// First, determine if that process is already running. If so, shut it down
                            //string strWindowTitle = Path.GetFileNameWithoutExtension(strAppFileAndPath);
                            //Window.KillProcess(strWindowTitle);
                            ProcessStartInfo processInfo = new ProcessStartInfo(strAppFileAndPath);
                            processInfo.WorkingDirectory = Path.GetDirectoryName(strAppFileAndPath);
                            for (int iIndex = 3; iIndex < colMessageParts.Length; iIndex++)
                                processInfo.Arguments += colMessageParts[iIndex] + " ";
                            System.Diagnostics.Process.Start(processInfo);
                            Log.Info("Application Started");
                        }
                        if (_context != null)
                            _context.CurrentStatus = string.Format("Application Started: {0}", strAppName);

                        break;
                    case "StopApp":
                        strAppName = colMessageParts[1];
                        Log.Info("Stopping Application {0}", strAppName);
                        strAppFileAndPath = ConfigurationManager.AppSettings[strAppName];
                        if (!string.IsNullOrEmpty(strAppFileAndPath))
                        {
                            // First, determine if that process is already running. If so, shut it down
                            string strWindowTitle = Path.GetFileNameWithoutExtension(strAppFileAndPath);
                            Window.KillProcess(strWindowTitle);
                            Log.Info("Application Stopped");
                        }
                        if (_context != null)
                            _context.CurrentStatus = string.Format("Application Stopped: {0}", strAppName);
                        break;
                    case "GetStatus":
                        //LogMsg.Debug(strMessage);
                        if (colMessageParts.Length < 2)
                            strReturnValue = string.Empty;
                        else
                        {
                            if (_context == null)
                                InitContext(ref colMessageParts[1]);
                            strReturnValue = string.Format("Status|{0}", (_context != null) ? _context.CurrentStatus : string.Empty);
                        }
                        break;
                    default:
                        LogMsg.Debug(strMessage);
                        //Console.WriteLine(strMessage);
                        if (_context == null)
                            InitContext(ref colMessageParts[1]);
                        _messageQueue.SyncEnqueue(colMessageParts);
                        break;
                }
            }
            if (string.IsNullOrEmpty(strReturnValue))
                return null;
            else
                return _textEncoding.GetBytes(strReturnValue.ToCharArray());
        }
    }
}