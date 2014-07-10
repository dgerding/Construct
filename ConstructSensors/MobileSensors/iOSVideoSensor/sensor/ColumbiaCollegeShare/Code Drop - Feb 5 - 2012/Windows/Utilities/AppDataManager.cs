using System;
using System.IO;
using Utilities;

namespace Utilities
{
    public static class AppDataManager
    {
        private static string companyName = "Construct Services";
        public static string CompanyName
        {
            get
            {
                return companyName;
            }
        }

        private static string applicationName = "Sensor Service";
        public static string ApplicationName
        {
            get
            {
                return applicationName;
            }
        }

        public static string CreateDirectoryIfNeeded(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == true)
            {
                return directoryPath;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                    if (Directory.Exists(directoryPath) == false)
                    {
                        return null;
                    }
                    return directoryPath;
                }
                catch
                {
                    return null;
                }
            }
        }


        private static string BasePath
        {
            get
            {
                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
                return CreateDirectoryIfNeeded(Path.Combine(path, companyName));
            }
        }

        public static string ApplicationDataPath
        {
            get
            {
                return CreateDirectoryIfNeeded(Path.Combine(BasePath, applicationName));
            }
        }

        public static string LogDirectory
        {
            get
            {
                return CreateDirectoryIfNeeded(Path.Combine(ApplicationDataPath, "Logs"));
            }
        }

        public static string SensorDataDirectory
        {
            get
            {
                return CreateDirectoryIfNeeded(Path.Combine(ApplicationDataPath, "SensorData"));
            }
        }

        public static string ToolDirectory
        {
            get
            {
                // TODO - add a post-build step or just stream this out of a resource. or just add a step
                //        to the deployment guide doc
                if (Directory.Exists(@"D:\ConstructSensorHelpers\"))
                {
                    return @"D:\ConstructSensorHelpers\";
                }
                else
                {
                    return @"E:\ConstructSensorHelpers\";
                }
            }
        }

        public static string GetSensorDirectory(string sensorID)
        {
            string sensorSpecificDirName = "SensorID_" + sensorID.Trim();
            return AppDataManager.CreateDirectoryIfNeeded(Path.Combine(AppDataManager.SensorDataDirectory, sensorSpecificDirName));
        }

        public static string GetStreamPartDirectory( string sensorID, string streamID )
        {
            string directory = AppDataManager.GetSensorDirectory(sensorID);
            directory = Path.Combine(directory, "StreamParts");
            directory = Path.Combine(directory, "StreamID_" + streamID);
            return AppDataManager.CreateDirectoryIfNeeded(directory);
        }

        public static string GetStreamPartDirectory(string sensorID)
        {
            string directory = AppDataManager.GetSensorDirectory(sensorID);
            directory = Path.Combine(directory, "StreamParts");
            return AppDataManager.CreateDirectoryIfNeeded(directory);
        }

        public static string GetStreamRecordingDirectory(string sensorID)
        {
            string directory = AppDataManager.GetSensorDirectory(sensorID);
            directory = Path.Combine(directory, "RecordedStreams");
            return AppDataManager.CreateDirectoryIfNeeded(directory);
        }



    }
}
