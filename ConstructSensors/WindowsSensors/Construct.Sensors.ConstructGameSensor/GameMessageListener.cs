using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;

namespace ConstructTcpServer
{
    /// <summary>
    /// TCP/IP Listening server for logging game events and user speech 
    /// </summary>
    public class GameMessageListener
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly int TcpPort = 1234;

        private readonly Thread ListenerThread;
        private readonly TcpListener tcpListener;
        private readonly object _syncRoot;
        private readonly SortedList<string, GameContext> _socketContext;
        private readonly List<SocketConnectionProcessor> _socketProcessorThreads;
        private bool isStarted = false;

        //PerformanceCounter _ctrTotalSocketsOpened;
        //PerformanceCounter _ctrTotalSocketsCurrentlyOpen;

        public GameMessageListener()
        {
            int.TryParse(ConfigurationManager.AppSettings["TcpPort"], out TcpPort);
            _socketContext = new SortedList<string, GameContext>();
            _socketProcessorThreads = new List<SocketConnectionProcessor>();
            _syncRoot = new object();
            tcpListener = new TcpListener(IPAddress.Any, TcpPort);
            ListenerThread = new Thread(new ThreadStart(StartListen));
            ListenerThread.Name = "TcpListenerThread"; // listens for connections on the port
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
                //InstantiateCounter();
                // Show the configuration settings
                Log.Info(string.Empty);
                Log.Info("Application Settings:");
                Log.Info(string.Empty);
                foreach (string settingKey in ConfigurationManager.AppSettings.Keys)
                {
                    Log.Info("{0}: {1}", settingKey, ConfigurationManager.AppSettings[settingKey]);
                }
                Log.Info(string.Empty);

                //Starting the TCP Listener thread.
                lock (_syncRoot)
                {
                    if (!isStarted)
                    {
                        isStarted = true;
                        ListenerThread.Start();
                        Log.Info("Started ConstructTcpServer's Listener Thread!");
                    }
                }

                // now wait here and occasionally check the list of contexts to see if any games were abandoned
                GameContext context;
                while (true)
                {
                    Thread.Sleep(20000); // check every few seconds to see if the game queue processor thread is still active
                    for (int iIndex = _socketContext.Count - 1; iIndex >= 0; iIndex--)
                    {
                        context = _socketContext.Values[iIndex];
                        if ((context.QueueThread != null) && (!context.QueueThread.IsAlive))
                        {
                            _socketContext.RemoveAt(iIndex);
                            Log.Info("Game with ID = {0} removed from game context list.", context.GameId);
                            //Console.WriteLine(string.Format("Game with ID = {0} removed from game context list.", context.GameId));
                        }
                    }
                    if (!ListenerThread.IsAlive)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("A TCP Exception has occurred!", ex);
                if (ListenerThread != null)
                {
                    ListenerThread.Abort();
                }
            }
        }

        /// <summary>
        /// Stops the TCP/IP Server
        /// </summary>
        public void StopServer()
        {
            lock (_syncRoot)
            {
                if (!isStarted)
                {
                    return;
                }
                isStarted = false;
                tcpListener.Stop();
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
                    tcpListener.Start();

                    while (IsStarted)
                    {
                        // Program blocks on Accept() until a client connects.
                        Socket soTcp = tcpListener.AcceptSocket();
                        if (IsStopped)
                        {
                            break;
                        }

                        Log.Info("Client is connected to the TCP port.");

                        //_ctrTotalSocketsOpened.RawValue++;
                        //_ctrTotalSocketsCurrentlyOpen.RawValue++;

                        // Start a separate thread to process this connection and then go back
                        // to listening on the port
                        SocketConnectionProcessor processor = new SocketConnectionProcessor(soTcp, _socketContext, StopServer, RemoveSocketProcessorThread);
                        Thread socketThread = new Thread(new ThreadStart(processor.ProcessClientConnection));
                        socketThread.Name = "ConnectionThread";
                        socketThread.Start();
                        // Add this thread to a list for use during shut down
                        AddSocketProcessorThread(processor);
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
                StopServer();
                StopAllCurrentThreads();
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
            lock (_syncRoot)
            {
                // Nicely tell any game processors to shut down by sending an EndGame event
                foreach (GameContext context in _socketContext.Values)
                {
                    // tell the Game Processor to shut down gracefully
                    if (context.MessageQueue != null)
                    {
                        context.MessageQueue.Enqueue(new string[] { "EndGame", context.GameId.ToString() });
                    }
                }
                _socketContext.Clear();
            }

            lock (_socketProcessorThreads)
            {
                foreach (SocketConnectionProcessor processor in _socketProcessorThreads)
                {
                    processor.Close();
                }
            }
            // Wait 3 second, give a chance to all these threads to gracefully shut down
            Thread.Sleep(3000);
            // now forcefully kill any remaining threads
            lock (_socketProcessorThreads)
            {
                foreach (SocketConnectionProcessor processor in _socketProcessorThreads)
                {
                    processor.ProcessorThread.Abort();
                }
            }
        }

        private void AddSocketProcessorThread(SocketConnectionProcessor socketProcessor)
        {
            lock (_socketProcessorThreads)
            {
                _socketProcessorThreads.Add(socketProcessor);
            }
        }

        private void RemoveSocketProcessorThread(SocketConnectionProcessor socketProcessor)
        {
            lock (_socketProcessorThreads)
            {
                _socketProcessorThreads.Remove(socketProcessor);
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
                lock (_syncRoot)
                {
                    return !isStarted;
                }
            }
        }

        public bool IsStarted
        {
            get
            {
                lock (_syncRoot)
                {
                    return isStarted;
                }
            }
        }
        #endregion
    }
}