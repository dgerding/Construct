using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Threading;
using Utilities;
using System.IO;
using SensorSharedTypes;
using System.Diagnostics;

namespace MobileVideoSensorService
{
    public static class SensorManager
    {
        // this queue allows sensor uploads to be dropped off quickly so that service calls do not block during processing
        private static ConcurrentQueue<StreamPart> streamPartQueue;

        // this table maps sensor IDS to streams in progress
        private static ConcurrentDictionary<string, List<StreamPart>> sensorStreamPartsMap;

        // map of sensors
        private static ConcurrentDictionary<string, SensorInfo> sensorTable;

        private static Thread thread = null;
        private static ManualResetEvent threadShutDownEvent = new ManualResetEvent(false);
        private static object dataLock = new object();

        // type initializer
        static SensorManager()
        {
            initializeCollections();
            Start();
        }

        private static void initializeCollections()
        {
            streamPartQueue = new ConcurrentQueue<StreamPart>();
            sensorStreamPartsMap = new ConcurrentDictionary<string, List<StreamPart>>();
            sensorTable = new ConcurrentDictionary<string, SensorInfo>();
        }

        // ths is for unnit testing purposes
        public static void DropAllSensors()
        {
            // signal thread to shutdown
            Stop();

            // wait for shutdown to complete
            while (threadShutDownEvent.WaitOne(0) == true)
            {
                Thread.Sleep(10);
            }

            lock (dataLock)
            {
                initializeCollections();
            }

            Start();
        }

        public static bool IsSensorConnected(string sensorID)
        {
            lock (dataLock)
            {
                if (sensorTable.ContainsKey(sensorID) == false)
                {
                    return false;
                }
                return sensorTable[sensorID].IsConnected;
            }
        }

        public static void ConnectSensor(string sensorID, string displayName)
        {
            lock (dataLock)
            {
                SensorInfo sensor = null;
                if (sensorTable.ContainsKey(sensorID) == false)
                {
                    sensor = new SensorInfo();
                    sensor.SensorID = sensorID;
                    sensor.DisplayName = displayName;
                    sensorTable[sensorID] = sensor;
                }
                else
                {
                    sensor = sensorTable[sensorID];
                }

                sensor.IsConnected = true;
                sensor.LastCommandCheckAt = DateTime.Now;
                LogAPI.SensorServiceLog.InfoFormat("sensor {0} connected", sensorID);
            }
        }

        public static void DisconnectSensor(string sensorID)
        {
            lock (dataLock)
            {
                if (sensorTable.ContainsKey(sensorID) == true)
                {
                    sensorTable[sensorID].IsConnected = false;
                    LogAPI.SensorServiceLog.InfoFormat("sensor {0} disconnected", sensorID);
                }
                else
                {
                    LogAPI.SensorServiceLog.InfoFormat("sensor {0} disconnect command ignored because it is an unknown sensor", sensorID);
                }
            }
        }

        public static List<SensorInfo> GetSensors()
        {
            lock (dataLock)
            {
                List<SensorInfo> sensors = new List<SensorInfo>(sensorTable.Values);
                sensors.Sort(SensorInfo.GetSorter());
                return sensors;
            }
        }

        public static SensorCommand GetSensorCommand(string sensorID)
        {
            lock (dataLock)
            {
                if (sensorTable.ContainsKey(sensorID) == true)
                {
                    SensorInfo sensor = sensorTable[sensorID];
                    sensor.LastCommandCheckAt = DateTime.Now;
                    SensorCommand command = sensor.GetPendingCommand();
                    if (command == SensorCommand.None)
                    {
                        return SensorCommand.None;
                    }
                    else
                    {
                        // clear the command because commands are 'one shots' and the act of fetching them is supposed to clear them
                        sensor.PendingCommandAsInt = (int)SensorCommand.None;
                        return command;
                    }
                }
                else
                {
                    LogAPI.SensorServiceLog.InfoFormat("sensor {0} get command ignored because it is an unknown sensor", sensorID);
                    return SensorCommand.None;
                }
            }
        }

        public static void SetSensorCommand(string sensorID, SensorCommand command)
        {
            lock (dataLock)
            {
                if (sensorTable.ContainsKey(sensorID) == true)
                {
                    SensorInfo sensor = sensorTable[sensorID];
                    sensor.PendingCommandAsInt = (int)command;
                    switch (command)
                    {
                        case SensorCommand.StartUpload_LowRes:
                            sensor.ResolutionAsInt = (int)SensorInfo.StreamingResolution.Low;
                            break;
                        case SensorCommand.StartUpload_MediumRes:
                            sensor.ResolutionAsInt = (int)SensorInfo.StreamingResolution.Medium;
                            break;
                        case SensorCommand.StopUpload:
                        case SensorCommand.None:
                        default:
                            sensor.ResolutionAsInt = (int)SensorInfo.StreamingResolution.None;
                            break;
                    }
                }
                else
                {
                    LogAPI.SensorServiceLog.InfoFormat("sensor {0} set command ignored because it is an unknown sensor", sensorID);
                }
            }
        }

        public static void Stop()
        {
            threadShutDownEvent.Set();
        }

        public static void Start()
        {
            thread = new Thread(new ThreadStart(doWork));
            thread.IsBackground = true;
            thread.Name = "Sensor Data Worker";

            // reset the signal
            threadShutDownEvent.Reset();

            // start it up
            thread.Start();
        }

        public static void UploadStreamPart(StreamPart info)
        {
            enqueueSensorData(info);
        }

        private static void enqueueSensorData(StreamPart streamPartInfo)
        {
            streamPartQueue.Enqueue(streamPartInfo);
        }

        private static void doWork()
        {
            DateTime lastCleanTime = DateTime.Now;
            while (true)
            {
                try
                {
                    if (threadShutDownEvent.WaitOne(0) == true)
                    {
                        break;
                    }

                    StreamPart queueItem;
                    if (streamPartQueue.TryDequeue(out queueItem) == true)
                    {
                        processStreamPart(queueItem);
                    }

                    // clean out any stale data from sensors that never completed their uploads
                    // and/or sensors that stopped polling for commands
                    if ( DateTime.Now.Subtract( lastCleanTime ) > TimeSpan.FromSeconds(10) )
                    {
                        lock (dataLock)
                        {
                            cleanSensorData();
                        }
                        lastCleanTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    LogAPI.LogSensorServiceException(ex);
                }

                Thread.Sleep(10);
            }
            threadShutDownEvent.Reset();
            LogAPI.SensorServiceLog.Info("Sensor Manager worker thread ended");
        }

        private static void processStreamPart(StreamPart part)
        {
            // change the data so that bytes loaded from a remote location are now stored locally and referenced locally
            part = transformStreamPart(part);

            // get the part list (or create a new one) for this sensor
            List<StreamPart> parts = sensorStreamPartsMap.GetOrAdd(part.SensorID, new List<StreamPart>());
            parts.Add(part);

            // time stamp the 'last receive time' for this sensor
            sensorTable[part.SensorID].LastStreamPartReceivedAt = DateTime.Now;

            // process the part
            if (part.IsLastPart == true)
            {
                // process the parts as a whole collection
                processStreamParts(part.SensorID, parts);

                // clear the entry for this sensor
                lock( dataLock )
                {
                    removeSensorStreamData(part.SensorID);
                }
            }
        }

        private static StreamPart transformStreamPart(StreamPart part)
        {
            // get the local file name
            string localFileName = string.Format("{0}_{1}", part.SequenceNumber, Path.GetFileName(part.FileName));
            string localDirectory = AppDataManager.GetStreamPartDirectory( part.SensorID, part.StreamID );
            string localFileMovPath = Path.Combine(localDirectory, localFileName);

            // handle file name collision
            if ( File.Exists( localFileMovPath ) == true )
            {
                File.Delete( localFileMovPath );
            }

            File.WriteAllBytes(localFileMovPath, Convert.FromBase64String(part.Base64Bytes));

            part.FileName = localFileMovPath;
            part.Base64Bytes = null;
            return part;
        }

        private static void processStreamParts(string sensorID, List<StreamPart> parts)
        {
            if ( parts.Count == 0 )
            {
                return;
            }

            // sort by sequence number
            parts.Sort(new StreamPartInfoSorter());

            // aggregate the parts into a single MPG file.  We convert because the mov format (h.264) does not work when 
            // simply concatenated but MPG does.
            ByteBuilder bb = new ByteBuilder();
            DateTime streamStartTime = parts[0].StartTime;
            DateTime streamEndTime = parts[ parts.Count - 1 ].StartTime;
            TimeSpan streamDuration = streamEndTime.Subtract(streamStartTime) + TimeSpan.FromMilliseconds(parts[parts.Count - 1].DurationMilliSeconds);

            foreach ( StreamPart part in parts )
            {
                // use FFMPEG to turn the mov file into an mpg file because those can be stacked up back to back during 
                // stream recombination
                string localFileMovPath = part.FileName;
                string localFileMpegPath = part.FileName.Replace(".mov", "") + ".mpg";
                string commandLineArgs = string.Format("-i \"{0}\" -same_quant \"{1}\"", localFileMovPath, localFileMpegPath);
                invokeFFMPEG(commandLineArgs, 500);

                try
                {
                    // if the conversion process is not complete we get an exception when trying to read the converetd file.
                    // just ignore it - hopefully all the parts wont be dropped.
                    byte[] bytes = File.ReadAllBytes(localFileMpegPath);
                    bb.Append( bytes );
                }
                catch
                {
                }
            }
            byte[] streamBytes = bb.ToBytes();

            // copy the final file to the target directory
            string finalDirectory = AppDataManager.GetStreamRecordingDirectory(parts[0].SensorID);
            string finalFile = fileNameFromDateTime(parts[0].StartTime) + ".mpg.tmp";
            string writePath = Path.Combine(finalDirectory, finalFile);

            File.WriteAllBytes(writePath, streamBytes);

            // change the time scale on the final MPG file
            double numSeconds = streamDuration.TotalSeconds;
            string rescaledFilePath = writePath.Replace(".tmp", "");
            string commandLineArgs2 = string.Format("-i \"{0}\" -t {1} \"{2}\"", writePath, numSeconds, rescaledFilePath);
            invokeFFMPEG(commandLineArgs2, 10000);

            LogAPI.SensorServiceLog.InfoFormat("Final MPG file timespan: {0} seconds", numSeconds);

            try
            {
                File.Delete( writePath );
            }
            catch
            {
            }

            // delete the parts
            string partDirectory = AppDataManager.GetStreamPartDirectory(parts[0].SensorID, parts[0].StreamID);
            deleteDirectory(partDirectory);
        }

        private static void invokeFFMPEG( string commandLineArgs, int maxTimeToBlockInMilliSeconds )
        {
            string exePath = string.Format(@"{0}\ffmpeg.exe", AppDataManager.ToolDirectory);
            ProcessStartInfo ProcessInfo = new ProcessStartInfo(exePath, commandLineArgs);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;
            Process Process = Process.Start(ProcessInfo);
            Process.WaitForExit(maxTimeToBlockInMilliSeconds);
            Process.Close();
        }

        private static void deleteDirectory(string dir)
        {
            // best effort delete
            if (Directory.Exists(dir) == false)
            {
                return;
            }

            // delete any files
            foreach ( string file in Directory.GetFiles(dir) )
            {
                // best effort file delete
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    LogAPI.SensorServiceLog.InfoFormat("Failed to delete file {0}", file);
                    LogAPI.LogSensorServiceException(ex);
                }
            }

            // delete any child dirs recursively
            foreach ( string childDir in Directory.GetDirectories( dir ) )
            {
                deleteDirectory( childDir );
            }

            // delete self
            try
            {
                Directory.Delete( dir );
            }
            catch (Exception ex)
            {
                LogAPI.SensorServiceLog.InfoFormat("Failed to delete directory {0}", dir);
                LogAPI.LogSensorServiceException(ex);
            }
        }

        private static string fileNameFromDateTime(DateTime dateTime)
        {
            string text = dateTime.ToString();
            return text.Replace("\\", "_").Replace("/", "_").Replace("-", "_").Replace(":", "_").Replace(" ", "_");
        }

        private static void cleanSensorData()
        {
            List<string> sensorIDs = new List<string>( sensorTable.Keys );

            foreach ( string sensorID in sensorIDs )
            {
                try
                {
                    if ( sensorTable.ContainsKey( sensorID ) == false )
                    {
                        continue;
                    }

                    SensorInfo sensor = sensorTable[sensorID];

                    // Get rid of unprocessed stream parts after a certain amount of time (assume dropped connection will never return)
                    DateTime lastPartReceivedTime = sensor.LastStreamPartReceivedAt;
                    if ( lastPartReceivedTime != DateTime.MinValue )
                    {
                        // see if a certain amount of time has passed since the last part upload
                        if (DateTime.Now.Subtract(lastPartReceivedTime) > TimeSpan.FromSeconds(10))
                        {
                            // assume a dropped upload connection and process the existing parts (if any)
                            List<StreamPart> parts;
                            if (sensorStreamPartsMap.TryGetValue(sensorID, out parts) == true)
                            {
                                processStreamParts(sensorID, parts);
                                removeSensorStreamData(sensorID);
                            }
                        }
                    }

                    // set the connection state to disconnected if a sensor has not polled for commands in a while
                    DateTime lastCommandCheckAt = sensor.LastCommandCheckAt;
                    if ( ( lastCommandCheckAt != DateTime.MinValue) && 
                         (DateTime.Now.Subtract(lastCommandCheckAt) > TimeSpan.FromSeconds(120)) && 
                         (sensor.IsConnected == true) )
                    {
                        LogAPI.SensorServiceLog.InfoFormat("sensor {0} with ID {1} was disconnected because it stopped polling for commands", sensor.DisplayName, sensor.SensorID);
                        sensor.IsConnected = false;
                    }
                }
                catch (Exception ex)
                {
                    LogAPI.LogSensorServiceException(ex);
                }
            }
        }

        private static void removeSensorStreamData( string sensorID )        
        {
            List<StreamPart> dontNeedParts;
            sensorStreamPartsMap.TryRemove(sensorID, out dontNeedParts);
            if (sensorTable.ContainsKey(sensorID) == true)
            {
                sensorTable[sensorID].LastStreamPartReceivedAt = DateTime.MinValue;
            }
        }

    }
}