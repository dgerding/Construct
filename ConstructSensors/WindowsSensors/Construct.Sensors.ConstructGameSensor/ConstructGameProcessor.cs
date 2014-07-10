using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using NLog;

namespace ConstructTcpServer
{
    /// <summary>
    /// Processes messages for the ConstructGame
    /// </summary>
    public class ConstructGameProcessor : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        private static readonly string strConnectionString = ConfigurationManager.ConnectionStrings["ConstructGameDB"].ConnectionString;
        int iGameId; // ID of the game session represented by this thread
        DateTime gameStartTime;
        /// <summary>
        /// Represents the boundary of the last second-level interval that was processed. 
        /// The timestamp is just outside the boundary of the interval meaning
        /// that any value less than the timestamp belongs to processed intervals.
        /// An interval is any value such that
        /// start timestamp &lt;= value &lt; end timestamp
        /// </summary>
        DateTime lastSampleIntervalEndTime;
        /// <summary>
        /// Represents the boundary of the last abstraction interval that was processed. 
        /// The timestamp is just outside the boundary of the interval meaning
        /// that any value less than the timestamp belongs to processed intervals.
        /// An interval is any value such that
        /// start timestamp &lt;= value &lt; end timestamp
        /// </summary>
        DateTime lastAbstractionIntervalEndTime;
        int abstractionIntervalWindowInSeconds;
        int secondsToWaitToProcessAbstractionInterval;
        int gameIntervalDefId;
        SqlConnection aSqlConnection = null;
        Thread[] dBLoggerThreads;
        int[] threadsMsDelta;
        string gameDataPath;
        int playerCount;

        public ConstructGameProcessor(ProducerConsumerQueue<string[]> messageQueue)
        {
            //_strConnectionString = ConfigurationManager.ConnectionStrings["ConstructGameDB"].ConnectionString;
            iGameId = 0;
            lastSampleIntervalEndTime = DateTime.MaxValue;
            lastAbstractionIntervalEndTime = DateTime.MaxValue;
            abstractionIntervalWindowInSeconds = 15; // Default. It will be overwritten with the value from DB
            secondsToWaitToProcessAbstractionInterval = 0;
            gameIntervalDefId = 0;
            MessageQueue = messageQueue;

            aSqlConnection = new SqlConnection(strConnectionString);

            playerCount = 0;
        }

        ~ConstructGameProcessor()
        {
            Dispose();
        }

        public ProducerConsumerQueue<string[]> MessageQueue { private get; set; }

        public int StartGame(string strGameName)
        {
            using (SqlConnection cn = new SqlConnection(strConnectionString))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "usp_StartGame";
                    cm.Parameters.AddWithValue("@strGameName", strGameName);

                    cn.Open();
                    object oGameId = cm.ExecuteScalar();

                    // Timers used to keep track of when to generate missing log data and abstractions
                    //_dtGameStart = DateTime.Now;
                    //_dtLastSampleIntervalEndTime = _dtGameStart.AddSeconds(1);

                    cm.CommandText = "usp_GetDefaultAbstractionInterval";
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@iGameId", oGameId);

                    using (SqlDataReader drInterval = cm.ExecuteReader())
                    {
                        if (drInterval.Read())
                        {
                            gameIntervalDefId = drInterval.GetInt32(drInterval.GetOrdinal("GameIntervalDefId"));
                            abstractionIntervalWindowInSeconds = drInterval.GetInt32(drInterval.GetOrdinal("IntervalWindowInSeconds"));
                            secondsToWaitToProcessAbstractionInterval = drInterval.GetInt32(drInterval.GetOrdinal("SecondsToWaitToProcessInterval"));
                            //_dtLastAbstractionIntervalEndTime = _dtGameStart.AddSeconds(_iAbstractionIntervalWindowInSeconds + _iSecondsToWaitToProcessAbstractionInterval);
                        }
                    }

                    if (oGameId != null)
                    {
                        iGameId = (int)oGameId;
                    }
                }
            }

            // Create DB Logger Threads
            int iDBLoggerThreadCount;
            if (!int.TryParse(ConfigurationManager.AppSettings["DBLoggerThreadCount"], out iDBLoggerThreadCount))
            {
                iDBLoggerThreadCount = 1;
            }
            else if (iDBLoggerThreadCount < 1)
            {
                iDBLoggerThreadCount = 1;
            }
            threadsMsDelta = new int[iDBLoggerThreadCount];
            dBLoggerThreads = new Thread[iDBLoggerThreadCount];
            for (int iIndex = 0; iIndex < iDBLoggerThreadCount; iIndex++)
            {
                GameMessageLogger logger = new GameMessageLogger(iGameId, iIndex, MessageQueue, UpdateThreadMsDelta);
                dBLoggerThreads[iIndex] = new Thread(new ParameterizedThreadStart(logger.QueueProcessor));
                dBLoggerThreads[iIndex].Name = string.Format("GameDBLogger{0}", iIndex);
            }

            // Create the folder to hold all the data
            try
            {
                gameDataPath = Path.GetFullPath(Path.Combine(ConfigurationManager.AppSettings["GameDataPath"], iGameId.ToString()));
                Directory.CreateDirectory(gameDataPath);
                Log.Info("Game data folder created: {0}", gameDataPath);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to create Game data folder: {0}", gameDataPath);
                Log.Error(ex);
            }

            return iGameId;
        }

        public void StartPlay(string[] colMessageParts)
        {
            // Timers used to keep track of when to generate missing log data and abstractions
            gameStartTime = DateTime.Parse(colMessageParts[2]);
            //using (SqlConnection cn = new SqlConnection(_strConnectionString))
            {
                using (SqlCommand cm = aSqlConnection.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "usp_StartPlay";
                    cm.Parameters.AddWithValue("@iGameId", iGameId);
                    cm.Parameters.AddWithValue("@dtGameStart", gameStartTime);

                    //cn.Open();
                    object oGameStart = cm.ExecuteScalar();
                    if (oGameStart != null)
                    {
                        gameStartTime = (DateTime)oGameStart;
                    }
                }
            }

            lastSampleIntervalEndTime = gameStartTime.AddSeconds(1);
            if (abstractionIntervalWindowInSeconds > 0)
            {
                lastAbstractionIntervalEndTime = gameStartTime.AddSeconds(abstractionIntervalWindowInSeconds + secondsToWaitToProcessAbstractionInterval);
            }

            // Start the threads to log data from the message queue to the DB
            foreach (Thread loggerThread in dBLoggerThreads)
            {
                loggerThread.Start();
            }
        }

        /// <summary>
        /// Closes the game in the DB.
        /// </summary>
        /// <param name="bEndGameReceived">Indicates a normal exit if a EndGame message was received from the game. 
        /// If false, assume the game died or crashed or otherwise exited without sending the EndGame message.</param>
        public void EndGame(int iMsDelta, bool bEndGameReceived)
        {
            using (SqlConnection cn = new SqlConnection(strConnectionString))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "usp_EndGame";
                    cm.Parameters.AddWithValue("@iGameId", iGameId);
                    cm.Parameters.AddWithValue("@iMsDelta", iMsDelta);
                    //cm.Parameters.AddWithValue("@bNormalExit", bEndGameReceived);

                    cn.Open();
                    cm.ExecuteNonQuery();
                }
            }
        }

        public int AddObject(int iGameId, string strObjectType, string strObjectName)
        {
            bool bAddFolder = false;
            using (SqlConnection cn = new SqlConnection(strConnectionString))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    if (string.Compare(strObjectType, "Player") == 0)
                    {
                        bAddFolder = true;
                        cm.CommandText = "usp_AddPlayer";
                        cm.Parameters.AddWithValue("@strPlayerName", strObjectName);
                    }
                    else
                    {
                        cm.CommandText = "usp_AddObject";
                        cm.Parameters.AddWithValue("@strObjectName", strObjectName);
                    }
                    cm.Parameters.AddWithValue("@iGameId", iGameId);

                    cn.Open();
                    object oObjectId = cm.ExecuteScalar();

                    if (oObjectId != null)
                    {
                        if (bAddFolder)
                        {
                            string strPlayerDataPath = Path.Combine(gameDataPath, string.Format("{0}-{1}-{2}", ++playerCount, strObjectName, oObjectId));
                            try
                            {
                                // Create the folder to hold all the data
                                Directory.CreateDirectory(strPlayerDataPath);
                                Log.Info("Game data folder created for player: {0}", strPlayerDataPath);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Unable to create Game data folder for player: {0}", strPlayerDataPath);
                                Log.Error(ex);
                            }
                        }
                        return (int)oObjectId;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public delegate void ThreadMsDeltaUpdater(int iThreadIndex, int iMsDelta);
        public void UpdateThreadMsDelta(int iThreadIndex, int iMsDelta)
        {
            threadsMsDelta[iThreadIndex] = iMsDelta;
        }

        public void QueueProcessor()
        {
            string[] colMessageParts;
            bool bExitThread = false;
            int iWaitHandle;
            DateTime dtCurrentLogTimestamp;
            ManualResetEvent currentResetEvent = null, lastResetEvent = null;
            GameAbstractionContext abstractionContext;
            bool bEndGameReceived = false;
            int iMainThreadTimeout = 1000; // Milliseconds to sleep before doing another loop to check on activity and data to be written
            int iAfterStartPlayTimeout; // Milliseconds to wait for activity after which the game would be considered abandoned.
            int iBeforeStartPlayTimeout; // Milliseconds to wait for the first activity on the queue.

            if (!int.TryParse(ConfigurationManager.AppSettings["BeforeStartPlayTimeout"], out iBeforeStartPlayTimeout))
            {
                iBeforeStartPlayTimeout = 900000; // Wait at most 15 minutes
            }// Wait at most 15 minutes

            if (!int.TryParse(ConfigurationManager.AppSettings["AfterStartPlayTimeout"], out iAfterStartPlayTimeout))
            {
                iAfterStartPlayTimeout = 60000; // Wait at most 60 seconds
            }// Wait at most 60 seconds

            aSqlConnection.Open();

            bool bStartPlayNotReceived = true;
            do
            {
                // Wait for the StartPlay event.
                iWaitHandle = WaitHandle.WaitAny(MessageQueue.EventArray, iBeforeStartPlayTimeout, false); // Wait the specified timeout to receive the StartPlay event
                if (iWaitHandle == 0 || iWaitHandle == WaitHandle.WaitTimeout)
                {
                    bExitThread = true;
                    // EndGame event was received
                    if (iWaitHandle == 0)
                    {
                        bEndGameReceived = true;
                    }
                }
                else
                {
                    while (bStartPlayNotReceived && ((colMessageParts = MessageQueue.SyncDequeue()) != null))
                        switch (colMessageParts[0])
                        {
                            case "StartPlay":
                                Log.Info("Play Started at {0}", colMessageParts[2]);
                                StartPlay(colMessageParts);
                                bStartPlayNotReceived = false;
                                break;
                            default:
                                Log.Info("Message Received before StartPlay, discarded: {0}", colMessageParts[0]);
                                //Console.WriteLine("  Message Received before StartPlay, discarded: - " + colMessageParts[0]);
                                break;
                        }
                }
            }
            while (bStartPlayNotReceived && !bExitThread);

            int iMinMsDelta, iSumMsDelta, iPrevSumMsDelta = 0, iTimePassedSinceActivity = 0;
            while (!bExitThread)
            {
                // Every second check if aggregations or log data fill need to run
                if (MessageQueue.EventArray[0].WaitOne(iMainThreadTimeout, false))
                {
                    // EndGame event was received
                    bEndGameReceived = true;
                    bExitThread = true;
                }
                else
                {
                    // Check for activity. If no activity for given period, exit this thread
                    // Activity is determined by checking the sum of all MsDelta values. If it changes
                    // then reset the no activity counter. If it doesn't change, add iMainThreadTimeout
                    // to the inactivity counter. When that value surpasses the iAfterStartPlayTimeout
                    // then exit the thread.
                    iMinMsDelta = int.MaxValue;
                    iSumMsDelta = 0;
                    // Determine minimum MsDelta to use in data fills and aggregations
                    // as well as the sum of deltas to check for activity
                    for (int iIndex = threadsMsDelta.Length - 1; iIndex >= 0; iIndex--)
                    {
                        iMinMsDelta = Math.Min(iMinMsDelta, threadsMsDelta[iIndex]);
                        iSumMsDelta += threadsMsDelta[iIndex];
                    }
  		
                    // If there's a change in sum of deltas (for all threads), assume there's activity
                    // and reset the inactivity counter
                    if (iSumMsDelta != iPrevSumMsDelta)
                    {
                        iTimePassedSinceActivity = 0;
                    }
                    else
                    {
                        iTimePassedSinceActivity += iMainThreadTimeout;
                    }
                    iPrevSumMsDelta = iSumMsDelta;

                    // Check if inactivity length is passed the threshold.
                    if (iTimePassedSinceActivity > iAfterStartPlayTimeout)
                    {
                        bExitThread = true;
                        foreach (Thread loggerThread in dBLoggerThreads)
                        {
                            loggerThread.Abort();
                        }
                    }
                    else
                    {
                        // Do any data fills necessary
                        // The assumption here is that data fills are very fast, and therefore abstractions
                        // are not negatively affected by data fills calls.
                        // If that turns out not to be the case, then data fills can go on their own thread
                        // Is the current time stamp passed the boundary of the last processed interval?
                        dtCurrentLogTimestamp = gameStartTime.AddMilliseconds(iMinMsDelta);
                        if (dtCurrentLogTimestamp >= lastSampleIntervalEndTime)
                        {
                            while (lastSampleIntervalEndTime <= dtCurrentLogTimestamp)
                            {
                                lastSampleIntervalEndTime = lastSampleIntervalEndTime.AddSeconds(1);
                            }
                            // Process the interval before the interval in which the current date/time stamp falls into
                            InferMissingLogData(iMinMsDelta / 1000 - 1);
                        }

                        // Do any abstractions necessary
                        if (dtCurrentLogTimestamp >= lastAbstractionIntervalEndTime)
                        {
                            while (lastAbstractionIntervalEndTime <= dtCurrentLogTimestamp)
                            {
                                lastAbstractionIntervalEndTime = lastAbstractionIntervalEndTime.AddSeconds(abstractionIntervalWindowInSeconds);
                            }
                            // Process the interval before the interval in which the current date/time stamp - _iSecondsToWaitToProcessAbstractionInterval falls into
                            currentResetEvent = new ManualResetEvent(false);
                            abstractionContext = new GameAbstractionContext(iGameId, gameIntervalDefId, (iMinMsDelta / 1000 - secondsToWaitToProcessAbstractionInterval) / abstractionIntervalWindowInSeconds - 1, currentResetEvent, lastResetEvent);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(RunAbstractions), abstractionContext);
                            lastResetEvent = currentResetEvent;
                        }
                    }
                }
            }

            // Wait for all logger threads to finish logging
            if (!bStartPlayNotReceived)
            {
                foreach (Thread loggerThread in dBLoggerThreads)
                {
                    loggerThread.Join();
                }
            }

            // Do any data fills necessary
            // First, determined the max value of MsDelta. That's the last value
            // for which we saw activity
            int iMaxMsDelta = 0;
            for (int iIndex = threadsMsDelta.Length - 1; iIndex >= 0; iIndex--)
            {
                iMaxMsDelta = Math.Max(iMaxMsDelta, threadsMsDelta[iIndex]);
            }
            InferMissingLogData(iMaxMsDelta / 1000);

            // Do any abstractions necessary
            abstractionContext = new GameAbstractionContext(iGameId, gameIntervalDefId, (iMaxMsDelta / 1000) / abstractionIntervalWindowInSeconds, null, lastResetEvent);
            RunAbstractions(abstractionContext);

            Log.Info("Exiting Queue Processor Thread. EndGame Received = {0}", bEndGameReceived);
            //Console.WriteLine("Exiting Queue Processor Thread. EndGame Received = " + bEndGameReceived.ToString());
            EndGame(iMaxMsDelta, bEndGameReceived);
            Dispose();
        }

        private void InferMissingLogData(int iRootIntervalIndex)
        {
            Log.Debug("InferMissingLogData({0})", iRootIntervalIndex);
            // Infer log data rows that are missing
            //using (SqlConnection cn = new SqlConnection(_strConnectionString))
            {
                using (SqlCommand cm = aSqlConnection.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "usp_CreateVISampleData";
                    cm.Parameters.AddWithValue("@iGameId", iGameId);
                    cm.Parameters.AddWithValue("@iEndRootIntervalIndex", iRootIntervalIndex);

                    //cn.Open();
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        //Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private static void RunAbstractions(object oGameAbstractionContext)
        {
            GameAbstractionContext context = oGameAbstractionContext as GameAbstractionContext;

            // If the previous abstraction has not yet finished running, wait for it to finish first
            if (context.LastResetEvent != null)
            {
                context.LastResetEvent.WaitOne();
            }

            Log.Debug("RunAbstractions(GameIntervalDefId={0}, AbstractionIntervalIndex={1})", context.GameIntervalDefId, context.AbstractionIntervalIndex);
            // Create data abstractions for given intervals
            using (SqlConnection cn = new SqlConnection(strConnectionString))
            {
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "usp_AbstractVisualInventory";
                    cm.Parameters.AddWithValue("@iGameId", context.GameId);
                    cm.Parameters.AddWithValue("@iGameIntervalDefId", context.GameIntervalDefId);
                    cm.Parameters.AddWithValue("@iEndAbstractionIntervalIndex", context.AbstractionIntervalIndex);

                    cn.Open();
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        //Console.WriteLine(ex.Message);
                    }
                }
            }
            if (context.CurrentResetEvent != null)
            {
                context.CurrentResetEvent.Set();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (aSqlConnection != null)
            {
                aSqlConnection.Dispose();
            }
            aSqlConnection = null;

            dBLoggerThreads = null;
        }
        #endregion
    }
}