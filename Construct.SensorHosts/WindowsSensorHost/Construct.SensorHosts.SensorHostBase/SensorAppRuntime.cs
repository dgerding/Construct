using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Net;
using System.ComponentModel;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Construct.SensorManagers.SensorManagerBase
{
    public class SensorAppRuntime
    { 
        #region fields
        public Dictionary<Guid, SensorAppInstance> SensorInstances = new Dictionary<Guid, SensorAppInstance>();
        private string currentDownloadUri;
        private string currentDownloadFolderPath;
        private string currentDownloadName;
        private string processPath;
        private string sensorName;
        private string version;
        private const int DEFAULTPORT = 8086;
        #endregion fields

        #region properties
        public Guid TypeSourceID { get; set; }
        public string CurrentDownloadUri
        {
            get { return currentDownloadUri; }
            set { currentDownloadUri = value; }
        }
        public string CurrentDownloadFolderPath
        {
            get { return currentDownloadFolderPath; }
            set { currentDownloadFolderPath = value; }
        }
        public string CurrentDownloadFileName
        {
            get { return currentDownloadName; }
            set { currentDownloadName = value; }
        }
        public Guid LatestSensorID { get; set; }
        #endregion properties

        public SensorAppRuntime(string path, string name, Guid aTypeSourceID, Guid latestSensorID, string sensorVersion)
        {
            processPath = path;
            sensorName = name;
            TypeSourceID = aTypeSourceID;
            LatestSensorID = latestSensorID;
            version = sensorVersion;
        }
        public event Action<Guid> DownloadCompletedEvent;

        public string LoadInstance(Guid sourceID, string constructServerUri, string sensorHostUri, string startUpArguments)
        {
            string processArgs = sourceID.ToString() + " " + constructServerUri + " " + sensorHostUri + " " + startUpArguments;
            try
            {
                SensorInstances[sourceID].ProcessRef = new Process();
                SensorInstances[sourceID].ProcessRef.StartInfo.FileName = processPath;
                SensorInstances[sourceID].ProcessRef.StartInfo.Arguments = processArgs;
                SensorInstances[sourceID].IsSensorProcessLoaded = true;
                DirectoryInfo workingDir = new FileInfo(processPath).Directory;
                SensorInstances[sourceID].ProcessRef.StartInfo.WorkingDirectory = workingDir.FullName;
                SensorInstances[sourceID].ProcessRef.Start();
            }
            catch (Exception e) { }
            return "Sensor Loaded";
        }

        private void StartDownload()
        {
            WebClient InstallClient = new WebClient();
            InstallClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
            InstallClient.DownloadFileAsync(new Uri(CurrentDownloadUri + "/" + CurrentDownloadFileName), CurrentDownloadFolderPath + "\\" + CurrentDownloadFileName);
        }
        
        public void PrepareRuntimeDownload(string installerFileUri, string installerFile, string absoluteInstallPath)
        {
            CurrentDownloadUri = installerFileUri;
            CurrentDownloadFileName = installerFile;
            currentDownloadFolderPath = absoluteInstallPath;
            Directory.CreateDirectory(absoluteInstallPath);
            StartDownload();
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            FileInfo FileInfo = new FileInfo(CurrentDownloadFolderPath + "\\" + CurrentDownloadFileName);
            Decompress(FileInfo);
            if (DownloadCompletedEvent != null)
            {
                DownloadCompletedEvent(LatestSensorID);
            }
        }

        private void Decompress(FileInfo fi)
        {
            UnZip(fi.FullName, CurrentDownloadFolderPath);
        }

        private void UnZip(string SrcFile, string DstDir)
        {
            FileStream fileStreamIn = new FileStream(SrcFile, FileMode.Open, FileAccess.Read);
            ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);

            ZipEntry entry;
            int size;
            byte[] buffer = new byte[4096];
            while ((entry = zipInStream.GetNextEntry()) != null)
            {
                if (entry.IsFile)
                {
                    string entryPath = Path.Combine(DstDir, entry.Name.Replace('/', '\\'));
                    string entryFolderPath = entryPath.Remove(entryPath.LastIndexOf('\\'));
                    if (Directory.Exists(entryFolderPath) == false)
                    {
                        Directory.CreateDirectory(entryFolderPath);
                    }
                    FileStream fileStreamOut = new FileStream(entryPath, FileMode.Create, FileAccess.Write);
                    while ((size = zipInStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStreamOut.Write(buffer, 0, size);
                    }
                    fileStreamOut.Close();

                }
            }
            zipInStream.Close();
            fileStreamIn.Close();
            File.Delete(SrcFile);
        }

    }
}
