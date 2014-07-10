using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading;
using NLog;

namespace ConstructGameListener
{
    /// <summary>
    /// Processes messages for the ConstructGame
    /// </summary>
    public class ConstructGameDBLogger : IDisposable
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private static string _strConnectionString = ConfigurationManager.ConnectionStrings["ConstructGameDB"].ConnectionString;
        int _iGameId; // ID of the game session represented by this thread
        int _iThreadIndex;
        ProducerConsumerQueue<string[]> _messageQueue;
        ConstructGameListener.ConstructGameProcessor.ThreadMsDeltaUpdater _updateMsDelta;
        SqlConnection _cn = null;

        public ConstructGameDBLogger(int iGameId, int iThreadIndex, ProducerConsumerQueue<string[]> messageQueue, ConstructGameListener.ConstructGameProcessor.ThreadMsDeltaUpdater updateMsDelta)
        {
            _iGameId = iGameId;
            _iThreadIndex = iThreadIndex;
            _messageQueue = messageQueue;
            _updateMsDelta = updateMsDelta;
        }

        ~ConstructGameDBLogger()
        {
            Dispose();
        }

        public ProducerConsumerQueue<string[]> MessageQueue
        {
            set
            {
                _messageQueue = value;
            }
        }

        public void LogVisualInventory(string[] colMessageParts)
        {
            using (SqlCommand cm = _cn.CreateCommand())
            {
                try
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "usp_LogVisualInventory";
                    cm.Parameters.AddWithValue("@iGameId", int.Parse(colMessageParts[1]));
                    cm.Parameters.AddWithValue("@iMsDelta", int.Parse(colMessageParts[2]));
                    cm.Parameters.AddWithValue("@iPlayerID", int.Parse(colMessageParts[3]));
                    cm.Parameters.AddWithValue("@iObjectId", int.Parse(colMessageParts[4]));
                    cm.Parameters.AddWithValue("@bIsVisible", colMessageParts[5] == "1");
                    cm.Parameters.AddWithValue("@decDistance", decimal.Parse(colMessageParts[6], CultureInfo.InvariantCulture));
                    cm.Parameters.AddWithValue("@decAngle", decimal.Parse(colMessageParts[7], CultureInfo.InvariantCulture));
                    cm.Parameters.AddWithValue("@iX", int.Parse(colMessageParts[8]));
                    cm.Parameters.AddWithValue("@iY", int.Parse(colMessageParts[9]));

                    //cn.Open();
                    cm.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Added error handling so Game will not be interrupted. Errors are written to the console.
                    Log.Error(string.Format("SQL Exception in LogVisualInventory: {0}\n", cm.CommandText), ex);
                    //Console.WriteLine("Thread {0}: Exception executing: \n{1}", _iThreadIndex, cm.CommandText);
                    //Console.WriteLine(ex.ToString());
                }
                catch (Exception ex)
                {
                    // Added error handling so Game will not be interrupted. Errors are written to the console.
                    Log.Error("Exception in LogVisualInventory: ", colMessageParts, ex);
                    //Console.WriteLine("Thread {0}: Exception executing: \n{1}", _iThreadIndex, StringArrayToString(colMessageParts));
                    //Console.WriteLine(ex.ToString());
                }
            }
        }

        public void LogData(string[] colMessageParts)
        {
            // Any data logging call must have a command name in the first parameter
            // and the game id in the second parameter
            if (colMessageParts.Length < 2)
                return;

            StringBuilder sbSql = new StringBuilder(80);
            using (SqlCommand cm = _cn.CreateCommand())
            {
                try
                {
                    cm.CommandType = CommandType.Text;
                    sbSql.AppendFormat("usp_Log{0} {1}", colMessageParts[0], colMessageParts[1]);
                    for (int iIndex = 2; iIndex < colMessageParts.Length; iIndex++)
                        sbSql.AppendFormat(",{0}", colMessageParts[iIndex]);
                    cm.CommandText = sbSql.ToString();
                    Log.Info("Thread {0}: {1}", _iThreadIndex, sbSql);
                    //Console.WriteLine("Thread {0}: {1}", _iThreadIndex, sbSql);

                    //cn.Open();
                    cm.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Added error handling so Game will not be interrupted. Errors are written to the console.
                    Log.Error(string.Format("SQL Exception in LogData: {0}\n", cm.CommandText), ex);
                    //Console.WriteLine("Thread {0}: Exception executing: \n{1}", _iThreadIndex, cm.CommandText);
                    //Console.WriteLine(ex.ToString());
                }
                catch (Exception ex)
                {
                    // Added error handling so Game will not be interrupted. Errors are written to the console.
                    Log.Error("Exception in LogData: ", colMessageParts, ex);
                    //Console.WriteLine("Thread {0}: Exception executing: \n{1}", _iThreadIndex, StringArrayToString(colMessageParts));
                    //Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Closes the game in the DB.
        /// </summary>
        /// <param name="bEndGameReceived">Indicates a normal exit if a EndGame message was received from the game. 
        /// If false, assume the game died or crashed or otherwise exited without sending the EndGame message.</param>
        public void EndGame(string[] colMessageParts)
        {
            using (SqlCommand cm = _cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "usp_EndGame";
                cm.Parameters.AddWithValue("@iGameId", _iGameId);
                if (colMessageParts.Length > 2)
                    cm.Parameters.AddWithValue("@iMsDelta", int.Parse(colMessageParts[2]));
                //cm.Parameters.AddWithValue("@bNormalExit", true);

                cm.ExecuteNonQuery();
            }
        }

        public void QueueProcessor(object value)
        {
            string[] colMessageParts;
            bool bExitThread = false;
            int iWaitHandle;
            int iMsDelta = 0; // milliseconds since play began

            try
            {
                Log.Info("Game DB Logger Thread {0} started.", _iThreadIndex);

                _cn = new SqlConnection(_strConnectionString);
                _cn.Open();

                try
                {
                    do
                    {
                        iWaitHandle = WaitHandle.WaitAny(_messageQueue.EventArray); // do not use a timeout as these threads will be aborted by the main thread in case a game was abandoned
                        if (iWaitHandle == 0 || iWaitHandle == WaitHandle.WaitTimeout)
                            bExitThread = true;
                        while ((colMessageParts = _messageQueue.SyncDequeue()) != null)
                        {
                            switch (colMessageParts[0])
                            {
                                case "EndGame":
                                    if (colMessageParts.Length > 2)
                                        if (int.TryParse(colMessageParts[2], out iMsDelta))
                                            _updateMsDelta(_iThreadIndex, iMsDelta);
                                    EndGame(colMessageParts);
                                    break;
                                case "Spoke":
                                    break;
                                case "LogVisualInventory":
                                    if (int.TryParse(colMessageParts[2], out iMsDelta))
                                        _updateMsDelta(_iThreadIndex, iMsDelta);
                                    LogVisualInventory(colMessageParts);
                                    break;
                                default:
                                    if (colMessageParts.Length > 2)
                                    {
                                        if (int.TryParse(colMessageParts[2], out iMsDelta))
                                            _updateMsDelta(_iThreadIndex, iMsDelta);
                                        LogData(colMessageParts);
                                    }
                                    else
                                    {
                                        Log.Warn("No Handler - {0}", colMessageParts[0]);
                                        //Console.WriteLine("  Thread {0}: No Handler - {1}", _iThreadIndex, colMessageParts[0]);
                                    }
                                    break;
                            }
                            // Do we really need to do this?
                            Thread.Sleep(0);
                        }
                    }
                    while (!bExitThread || (colMessageParts != null));
                }
                catch (ThreadAbortException)
                {
                    // The game was most likely abandoned as the main thread requested an abort
                    Log.Info("Thread {0}: Abort Requested.", _iThreadIndex);
                    //Console.WriteLine("Thread {0}: Abort Requested.", _iThreadIndex);
                }
            }
            finally
            {
                Log.Info("Thread {0}: Exiting Queue Logger Thread.", _iThreadIndex);
                //Console.WriteLine("Thread {0}: Exiting Queue Logger Thread.", _iThreadIndex);
                Dispose();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_cn != null)
                _cn.Dispose();
            _cn = null;
        }
        #endregion
    }
}