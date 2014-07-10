using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Utilities
{
    public static class LogAPI
    {
        private static string logDirectory;
        private static LogLevel? logLevel;

        public static void InitializeLogging(string path)
        {
            LogDirectory = path;
        }

        private static ILog sensorServiceLog;
        public static ILog SensorServiceLog
        {
            get
            {
                if (sensorServiceLog == null)
                {
                    sensorServiceLog = StaticLogManager.GetLogger("SensorServiceLog");
                }
                return sensorServiceLog;
            }
        }

        private static ILog webUILog;
        public static ILog WebUILog
        {
            get
            {
                if (webUILog == null)
                {
                    webUILog = StaticLogManager.GetLogger("WebUILog");
                }
                return webUILog;
            }
        }

        public static LogLevel LogLevel
        {
            get
            {
                if ( logLevel == null )
                {
                    logLevel = readLogLevelFromConfigFile();
                }
                return logLevel.Value;
            }

            set
            {
                // only make the change if there is a difference because a property change
                // causes a file parse and write
                if ( ( logLevel == null) || ( logLevel.Value != value ))
                {
                    // cache the value to prevent unecessary file reads
                    logLevel = value;

                    writeLogLevelToConfigFile(logLevel.Value);
                }
            }
        }

        public static string GetDateTimeFolderName(DateTime dateTime)
        {
            DateTime time = DateTime.Now;
            return time.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-");
        }

        private static void writeLogLevelToConfigFile( LogLevel level )
        {
            //<root>
            //<level value="DEBUG" />
            //</root>
            forceLogConfigFileToExist();
            XmlDocument doc = new XmlDocument();
            doc.Load(logConfigFile);
            XmlNodeList nodes = doc.SelectNodes("//root/level");
            XmlElement levelElement = nodes[0] as XmlElement;
            levelElement.Attributes["value"].Value = level.ToString();
            doc.Save(logConfigFile);
        }

        private static LogLevel readLogLevelFromConfigFile()
        {
            //<root>
            //<level value="DEBUG" />
            //</root>
            forceLogConfigFileToExist();
            XmlDocument doc = new XmlDocument();
            doc.Load(logConfigFile);
            XmlNodeList nodes = doc.SelectNodes("//root/level");
            XmlElement levelElement = nodes[0] as XmlElement;
            return (LogLevel)Enum.Parse(typeof(LogLevel), levelElement.Attributes["value"].Value);
        }

        private static string logConfigFile
        {
            get
            {
                return Path.Combine(logDirectory, "LogConfig.xml");
            }
        }

        public static string GetProcessInfo()
        {
            Process process = Process.GetCurrentProcess();
            string machineName = process.MachineName;
            if ( machineName == "." )
            {
                machineName = Environment.MachineName;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Process Name: {0}\r\n", process.ProcessName);
            sb.AppendFormat("Process ID: {0}\r\n", process.Id);
            sb.AppendFormat("Machine Name: {0}\r\n", machineName);

            return sb.ToString().TrimEnd();
        }

        private static string LogDirectory
        {
            get
            {
                return logDirectory;
            }
            set
            {
                try
                {
                    if (logDirectory != null)
                    {
                        return;
                    }
                    logDirectory = value;

                    // force the directory to exist
                    if (Directory.Exists(logDirectory) == false)
                    {
                        Directory.CreateDirectory(logDirectory);
                    }

                    forceLogConfigFileToExist();

                    // force all FileAppender instances to use the time-stamped directory as a base path
                    string timeStampedDirectory = Path.Combine(logDirectory, GetDateTimeFolderName(DateTime.Now));
                    if (Directory.Exists(timeStampedDirectory) == false)
                    {
                        Directory.CreateDirectory(timeStampedDirectory);
                    }
                    log4net.Appender.FileAppender.BasePath = timeStampedDirectory;

                    // initialize the repository from the config file
                    if (StaticLogManager.IsInitialized == false)
                    {
                        StaticLogManager.Initialize("web-core-repository");
                        ILoggerRepository loggerRepository = StaticLogManager.GetRepository();
                        if (loggerRepository.Configured == false)
                        {
                            FileInfo fi = new FileInfo(logConfigFile);
                            XmlConfigurator.ConfigureAndWatch(loggerRepository, fi);
                        }
                        loggerRepository.ShutdownEvent += new LoggerRepositoryShutdownEventHandler(loggerRepository_ShutdownEvent);
                    }
                }
                catch
                {
                    logDirectory = null;
                }
            }
        }

        private static void forceLogConfigFileToExist()
        {
            if (File.Exists(logConfigFile) == false)
            {
                // read it from the embedded resource and write it to a file
                string configXml = ResourceAPI.GetStringResource("Utilities.Logging.LogConfig.xml");
                File.WriteAllText(logConfigFile, configXml);
            }
        }

        private static void loggerRepository_ShutdownEvent(object sender, EventArgs e)
        {
            if ( logDirectory != null )
            {
                // no-op
            }
        }

        public static string[] GetLogFolderNames()
        {
            List<string> names = new List<string>();
            foreach (string path in Directory.GetDirectories(LogRootDirectory))
            {
                string normalizedPath = path.Replace("\\", "/");
                string terminalFolderName = normalizedPath.Substring( normalizedPath.LastIndexOf("/") + 1 );
                names.Add(terminalFolderName);
            }
            return names.ToArray();
        }

        public static string GetCurrentLogDirectoryName()
        {
            string[] logFolderNames = GetLogFolderNames();
            string mostRecentFolder = null;
            DateTime mostRecentTime = DateTime.MinValue;
            foreach ( string folder in logFolderNames )
            {
                DateTime creationTime = Directory.GetCreationTime(folder);
                if ( creationTime > mostRecentTime )
                {
                    mostRecentFolder = folder;
                }
            }

            string normalizedPath = mostRecentFolder.Replace("\\", "/");
            string terminalFolderName = normalizedPath.Substring(normalizedPath.LastIndexOf("/") + 1);
            return terminalFolderName;
        }

        public static string[] GetLogFileNames(string logDirectoryName)
        {
            string path = Path.Combine(LogRootDirectory, logDirectoryName);
            if (Directory.Exists(path) == false)
            {
                throw new ArgumentException(string.Format("no log directory '{0}' exists", path));
            }
            return Directory.GetFiles(path);
        }

        public static string LogRootDirectory
        {
            get
            {
                return LogDirectory;
            }
        }

        public static void LogSensorServiceException(Exception ex)
        {
            LogException( LogAPI.SensorServiceLog, ex );
        }

        public static void LogException(ILog logger, Exception ex)
        {
            try
            {
                // wrap in try catch so we don't let logging error bring it all down
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}\r\n\r\n{1}\r\n\r\n", ex.Message, ex.StackTrace);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    sb.AppendFormat("[INNER EXCEPTION] {0}\r\n\r\n{1}\r\n\r\n", innerException.Message, innerException.StackTrace);
                    innerException = innerException.InnerException;
                }
                logger.Error(sb.ToString());
            }
            catch
            {
                logger.Error("Detailed exception logging failed");
            }
        }

    }
}
