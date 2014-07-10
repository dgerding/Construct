using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;
using System.Configuration;

namespace ConstructGameListener
{
    /// <summary>
    /// TCP/IP Listening server for logging game events and user speech 
    /// </summary>
    public class ListenerSocket
    {

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private bool isStarted = false;

        #region Port Related Members
        private readonly int TcpPort = 1234;
        private readonly TcpListener tcpListener;
        private readonly SortedList<string, GameContext> socketContext;
        

        #endregion

        #region Threading Related Memebers
        private readonly Thread ListenerThread;
        private readonly object syncRoot;
        private readonly List<SocketConnectionProcessor> _socketProcessorThreads;
        

        #endregion

        public ListenerSocket()
        {
            ReadySocketConnection();

            ReadyMessageHandlingThread();
        }

        private void ReadyMessageHandlingThread()
        {
            /*
            this._socketProcessorThreads = new List<SocketConnectionProcessor>();
            this.syncRoot = new object();
            this.tcpListener = new TcpListener(IPAddress.Any, this.TcpPort);
            this.ListenerThread = new Thread(new ThreadStart(StartListen));
            this.ListenerThread.Name = "TcpListenerThread"; // listens for connections on the port
             */
        }

        private void ReadySocketConnection()
        {
            int.TryParse(ConfigurationManager.AppSettings["TcpPort"], out this.TcpPort);

            /*
            this.socketContext = new SortedList<string, GameContext>();
             */
        }

        /// <summary>
        /// Starts the TCP/IP Server
        /// </summary>
        /// <remarks>This method runs on the main thread</remarks>
        public void StartServer()
        {
            Log.Debug("in StartServer");
            try
            {
                LogApplicationConfigurationSettings();

                StartTcpListenerThread();

                // now wait here and occasionally check the list of contexts to see if any games were abandoned
                GameContext context;
                while (true)
                {
                    Thread.Sleep(5000); // check every few seconds to see if the game queue processor thread is still active
                    for (int iIndex = this.socketContext.Count - 1; iIndex >= 0; iIndex--)
                    {
                        context = this.socketContext.Values[iIndex];
                        if ((context.QueueThread != null) && (!context.QueueThread.IsAlive))
                        {
                            this.socketContext.RemoveAt(iIndex);
                            Log.Info("Game with ID = {0} removed from game context list.", context.GameId);
                            //Console.WriteLine(string.Format("Game with ID = {0} removed from game context list.", context.GameId));
                        }
                    }
                    if (!this.ListenerThread.IsAlive)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("A TCP Exception has occurred!", ex);
                if (this.ListenerThread != null)
                {
                    this.ListenerThread.Abort();
                }
            }
        }

        private void StartTcpListenerThread()
        {
            //Starting the TCP Listener thread.
            lock (this.syncRoot)
            {
                if (!this.isStarted)
                {
                    this.isStarted = true;
                    this.ListenerThread.Start();
                    Log.Info("Started ConstructTcpServer's Listener Thread!");
                }
            }
        }
  
        private void LogApplicationConfigurationSettings()
        {
            Log.Info(string.Empty);
            Log.Info("Application Settings:");
            Log.Info(string.Empty);
            foreach (string settingKey in ConfigurationManager.AppSettings.Keys)
            {
                Log.Info("{0}: {1}", settingKey, ConfigurationManager.AppSettings[settingKey]);
            }
            Log.Info(string.Empty);
        }

        /// <summary>
        /// Stops the TCP/IP Server
        /// </summary>
        public void StopServer()
        {
            lock (this.syncRoot)
            {
                if (!this.isStarted)
                {
                    return;
                }
                this.isStarted = false;
                this.tcpListener.Stop();
            }
        }

        /// <summary>
        /// This function listens on the TCP port. It is the entry point 
        /// for all connections on this TCP port.
        /// </summary>
        /// <remarks>This method runs on the listener thread started by the main thread</remarks>
        public void StartListen()
        {
            Log.Debug("in StartListen");

            //Create an instance of TcpListener to listen for TCP connection.
            try
            {
                try
                {
                    //_ctrTotalSocketsOpened.RawValue = 0;
                    //_ctrTotalSocketsCurrentlyOpen.RawValue = 0;
                    this.tcpListener.Start();

                    while (this.IsStarted)
                    {
                        // Program blocks on Accept() until a client connects.
                        Socket soTcp = this.tcpListener.AcceptSocket();
                        if (this.IsStopped)
                        {
                            break;
                        }

                        Log.Info("Client is connected to the TCP port.");

                        //_ctrTotalSocketsOpened.RawValue++;
                        //_ctrTotalSocketsCurrentlyOpen.RawValue++;

                        // Start a separate thread to process this connection and then go back
                        // to listening on the port
                        SocketConnectionProcessor processor = new SocketConnectionProcessor(soTcp, socketContext, StopServer, RemoveSocketProcessorThread);
                        Thread socketThread = new Thread(new ThreadStart(processor.ProcessClientConnection));
                        socketThread.Name = "ConnectionThread";
                        socketThread.Start();
                        // Add this thread to a list for use during shut down
                        this.AddSocketProcessorThread(processor);
                    }
                    //tcpListener.Stop();
                }
                catch (SocketException se)
                {
                    // Calling listen.Stop() while Listening will throw
                    // "A blocking operation was interrupted by a call to WSACancelBlockingCall".
                    Log.Fatal("A Socket Exception has occurred!", se);
                }
                catch (ThreadAbortException tae)
                {
                    Log.Fatal("Socket Listener Thread was aborted!", tae);
                }
                catch (Exception ex)
                {
                    Log.Fatal("Bad stuf happened :(", ex);
                }
            }
            finally
            {
                this.StopServer();
                this.StopAllCurrentThreads();
                Log.Info("TCP Server Stopped listening.");
                //_ctrTotalSocketsOpened.RawValue = 0;
                //_ctrTotalSocketsCurrentlyOpen.RawValue = 0;
            }
        }

        /// <summary>
        /// During the shutdown event, signal all current connections to terminate
        /// </summary>
        private void StopAllCurrentThreads()
        {
            lock (this.syncRoot)
            {
                // Nicely tell any game processors to shut down by sending an EndGame event
                foreach (GameContext context in this.socketContext.Values)
                {
                    // tell the Game Processor to shut down gracefully
                    if (context.MessageQueue != null)
                    {
                        context.MessageQueue.Enqueue(new string[] { "EndGame", context.GameId.ToString() });
                    }
                }
                this.socketContext.Clear();
            }

            lock (this._socketProcessorThreads)
            {
                foreach (SocketConnectionProcessor processor in this._socketProcessorThreads)
                {
                    processor.Close();
                }
            }
            // Wait 3 second, give a chance to all these threads to gracefully shut down
            Thread.Sleep(3000);
            // now forcefully kill any remaining threads
            lock (this._socketProcessorThreads)
            {
                foreach (SocketConnectionProcessor processor in this._socketProcessorThreads)
                {
                    processor.ProcessorThread.Abort();
                }
            }
        }

        private void AddSocketProcessorThread(SocketConnectionProcessor socketProcessor)
        {
            lock (this._socketProcessorThreads)
            {
                this._socketProcessorThreads.Add(socketProcessor);
            }
        }

        //void CreatePerfCounters()
        //{
        //    // Create the collection that will hold the data for the counters we are creating
        //    CounterCreationDataCollection colPerfCounters = new CounterCreationDataCollection();

        //    // Create the CreationData
        //    CounterCreationData ctrCounter = new CounterCreationData("Total Sockets Opened", "Number of sockets opened during the lifetime of this server", PerformanceCounterType.NumberOfItems32);
        //    colPerfCounters.Add(ctrCounter);

        //    // Create the CreationData
        //    ctrCounter = new CounterCreationData("Total Sockets Currently Open", "Number of sockets currently open", PerformanceCounterType.NumberOfItems32);
        //    colPerfCounters.Add(ctrCounter);

        //    // Create the CreationData
        //    ctrCounter = new CounterCreationData("Msg Received", "Total Messages Received", PerformanceCounterType.NumberOfItems32);
        //    colPerfCounters.Add(ctrCounter);

        //    // Add the counter to the collection
        //    colPerfCounters.Add(ctrTotalSocketsCurrentlyOpen);

        //    // Create the counter in the system using the collection
        //    PerformanceCounterCategory.Create("ConstructTcpServer", "Construct TCP Server Counters", PerformanceCounterCategoryType.MultiInstance, colPerfCounters);
        //}

        //void InstantiateCounter()
        //{
        //    if (PerformanceCounterCategory.Exists("ConstructTcpServer"))
        //        PerformanceCounterCategory.Delete("ConstructTcpServer");

        //        // if the category doesn't exist, create it
        //    if (!PerformanceCounterCategory.Exists("ConstructTcpServer"))
        //        CreatePerfCounters();
        //    // setup the counter to write to
        //    string strCounterInstance = string.Format("Socket_{0}", TcpPort);
        //    _ctrTotalSocketsOpened = new PerformanceCounter("ConstructTcpServer", "Total Sockets Opened", strCounterInstance, false);
        //    _ctrTotalSocketsCurrentlyOpen = new PerformanceCounter("ConstructTcpServer", "Total Sockets Currently Open", strCounterInstance, false);
        //}

        #region Properties

        public bool IsStopped
        {
            get
            {
                lock (this.syncRoot)
                {
                    return !this.isStarted;
                }
            }
        }

        public bool IsStarted
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.isStarted;
                }
            }
        }
        #endregion
    }
}