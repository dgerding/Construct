using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows.Data;
using System.Xml.Linq;
using Construct.UX.ViewModels.Sources.SourcesServiceReference;
using ICSharpCode.SharpZipLib.Zip;
using Construct.MessageBrokering;
using System.ServiceModel.Description;
using Construct.Utilities.Shared;
using System.Threading;

namespace Construct.UX.ViewModels.Sources
{
    [Export("SourcesViewModel")]
    public class ViewModel : ViewModels.ViewModel
    {
        private ObservableCollection<DataType> observableDataTypes;
        private ObservableCollection<DataType> observableCoreDataTypes;
        private ObservableCollection<SensorHostType> observableSensorHostTypes;
        private ObservableCollection<SensorHost> observableSensorHosts;
        private ObservableCollection<Sensor> observableSensors;
        private ObservableCollection<HumanReadableSensor> observableHumanReadableSensors;
        private ObservableCollection<SensorCommand> observableSensorCommands;
        private ObservableCollection<DataTypeSource> observableRootDataTypeSources;
        private ObservableCollection<DataTypeSource> observableDataTypeSources;
        private ObservableCollection<SensorTypeSource> observableSensorTypeSources;
        private ObservableCollection<SensorRuntime> observableSensorRuntimes;

		private CallbackImplementation callback = new CallbackImplementation();
		private InstanceContext instanceContext = null;
		private ModelClient client = null;

		private ModelClient GetModel()
		{
			if (client == null || client.State == CommunicationState.Closed || client.State == CommunicationState.Closing || client.State == CommunicationState.Faulted)
			{
				bool isDone = false;

				//	http://tech.pro/tutorial/914/wcf-callbacks-hanging-wpf-applications
				ThreadPool.QueueUserWorkItem((data) =>
					{
						instanceContext = new InstanceContext(callback);
						client = new ModelClient(instanceContext, "WsDualHttpBinding", RemoteAddress);
						ModelClientHelper.EnhanceModelClientBandwidth<ModelClient>(client);
						client.Open();

						isDone = true;
					});

				while (!isDone)
					System.Threading.Thread.Sleep(10);
			}
			return client;
		}

        public ObservableCollection<DataType> ObservableDataTypes
        {
            set
            {
                observableDataTypes = value;
                NotifyPropertyChanged("ObservableDataTypes");
            }
            get
            {
                return observableDataTypes;
            }
        }
        public ObservableCollection<DataType> ObservableCoreDataTypes
        {
            set
            {
                observableCoreDataTypes = value;
                NotifyPropertyChanged("ObservableCoreDataTypes");
            }
            get
            {
                return observableCoreDataTypes;
            }
        }
        public ObservableCollection<SensorHostType> ObservableSensorHostTypes
        {
            set
            {
                observableSensorHostTypes = value;
                NotifyPropertyChanged("ObservableSensorHostTypes");
            }
            get
            {
                return observableSensorHostTypes;
            }
        }
        public ObservableCollection<SensorHost> ObservableSensorHosts
        {
            set
            {
                observableSensorHosts = value;
                NotifyPropertyChanged("ObservableSensorHosts");
            }
            get
            {
                return observableSensorHosts;
            }
        }
        public ObservableCollection<Sensor> ObservableSensors
        {
            set
            {
                observableSensors = value;
                NotifyPropertyChanged("ObservableSensors");
            }
            get
            {
                return observableSensors;
            }
        }
        public ObservableCollection<HumanReadableSensor> ObservableHumanReadableSensors
        {
            set
            {
                observableHumanReadableSensors = value;
                NotifyPropertyChanged("ObservableHumanReadableSensors");
            }
            get
            {
                return observableHumanReadableSensors;
            }
        }
        public ObservableCollection<SensorTypeSource> ObservableSensorTypeSources
        {
            set
            {
                observableSensorTypeSources = value;
                NotifyPropertyChanged("ObservableSensorTypeSources");
            }
            get
            {
                return observableSensorTypeSources;
            }
        }
        public ObservableCollection<SensorCommand> ObservableSensorCommands
        {
            set
            {
                observableSensorCommands = value;
                NotifyPropertyChanged("ObservableSensorCommands");
            }
            get
            {
                return observableSensorCommands;
            }
        }
        public ObservableCollection<DataTypeSource> ObservableRootDataTypeSources
        {
            set
            {
                observableRootDataTypeSources = value;
                NotifyPropertyChanged("ObservableRootTypeSources");
            }
            get
            {
                return observableRootDataTypeSources;
            }
        }
        public ObservableCollection<DataTypeSource> ObservableDataTypeSources
        {
            set
            {
                observableDataTypeSources = value;
                NotifyPropertyChanged("ObservableTypeSources");
            }
            get
            {
                return observableDataTypeSources;
            }
        }
        public ObservableCollection<SensorRuntime> ObservableSensorRuntimes
        {
            set
            {
                observableSensorRuntimes = value;
                NotifyPropertyChanged("ObservableSensorRuntimes");
            }
            get
            {
                return observableSensorRuntimes;
            }
        }

        [ImportingConstructor]
        public ViewModel() : base("Sources")
        {

        }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo) 
                : base(applicationSessionInfo, "Sources")
        {
            callback.OnAvailableSensorCommandsCallbackReceived += HandleAvailableSensorCommands;
        }

        public event Action<Guid> SensorLoadedCallbackReceived
        {
            add
            {
                callback.OnSensorLoadedCallbackReceived += value;
            }
            remove
            {
                callback.OnSensorLoadedCallbackReceived -= value;
            }
        }
        public event Action<Guid> SensorInstalledCallbackReceived
        {
            add
            {
                callback.OnSensorInstalledCallbackReceived += value;
            }
            remove
            {
                callback.OnSensorInstalledCallbackReceived -= value;
            }
        }

        public override void Load()
        {
            LoadSensors();
            LoadHumanReadableSensors();
            LoadSensorCommands();
            LoadDataTypes();
            LoadSensorHosts();
            LoadSensorHostTypes();
            LoadDataTypeSources();
            LoadSensorTypeSources();
            LoadRootTypeSources();
            LoadSensorRuntimes();
        }

        public string AddSensorDefinition(object parameter)
        {
            // this is the same starting point
            dynamic typed = parameter as ExpandoObject;
            string lastInstallSensorResults = "";

            if (typed.InstallationPath != "Click here to add sensor app...")
            {
                System.Uri theUri = new System.Uri(typed.InstallationPath);
                string currentDownloadFilename = Path.GetFileName(theUri.AbsolutePath);

                lastInstallSensorResults += "Started Install at " + DateTime.UtcNow.ToString() + "." + System.Environment.NewLine;

                string[] splitDownloadFilename = currentDownloadFilename.Split('.');
                string sensorName = splitDownloadFilename[0];
                string sensorTypeSourceID = splitDownloadFilename[1];
                string sensorVersion = splitDownloadFilename[2];

                string unzipFolderPath = Path.Combine(global::Construct.UX.ViewModels.Properties.Settings.Default.SensorInstallerFilesDirectoryPath, sensorName + "." + sensorTypeSourceID, sensorVersion);
                Unzip(theUri.AbsolutePath, unzipFolderPath);

                string xmlText = new StreamReader(File.OpenRead(Path.Combine(unzipFolderPath, "construct.xml"))).ReadToEnd();

                SensorRuntime runtime = new SensorRuntime();

                runtime.ID = Guid.NewGuid();
                runtime.SensorTypeSourceID= Guid.Parse(sensorTypeSourceID);
                runtime.InstallerUri = theUri.AbsolutePath;
                runtime.CacheUri = "Not Implemented";
                runtime.InstallerXml = xmlText;
                runtime.InstallerZip = new byte[1] { 1 };
                runtime.RecCreationDate = DateTime.Now;

                client = GetModel();

                if (client.AddSensorDefinition(runtime))
                {
                    lastInstallSensorResults += "Installed sensor to cache in " + Path.Combine(unzipFolderPath, currentDownloadFilename) + ".\n";

                    lock (observableSensorRuntimes)
                    {
                        observableSensorRuntimes.Add(runtime);
                    }
                    //TODO: This reallocation may be very slow? Instead of complete reallocation, we can use a callback 
                    //to add SPECIFICALLY which entities we just created with this call
                    ObservableDataTypeSources = new ObservableCollection<DataTypeSource>(client.GetDataTypeSources());
                    ObservableSensorTypeSources = new ObservableCollection<SensorTypeSource>(client.GetSensorTypeSources());
                    ObservableDataTypes = new ObservableCollection<DataType>(client.GetDataTypes());
                }
                else
                {
                    lastInstallSensorResults += "Sensor install failed. A sensor definition matching this one's Name or ID was already installed, " +
                                                    "the installation process failed from a malformed xml file, or the attempt Timed Out.\n";
                }

            }
            return lastInstallSensorResults;
        }

        public Guid AddSensorHost(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            string protocol = typed.Protocol;
            string hostName = typed.HostName;
            string hostID = typed.HostID;

            SensorHost newSensorHost = new SensorHost();

            newSensorHost.ID = Guid.Parse(hostID);
            newSensorHost.HostName = hostName;
            newSensorHost.HostUri = String.Format("{0}://{1}/{2}/{3}", protocol, hostName, Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"), Guid.Parse(hostID));
            newSensorHost.SensorHostTypeID = typed.SensorHostTypeID;

            client = GetModel();
            if (client.AddSensorHost(newSensorHost))
            {
                lock (observableSensorHosts)
                {
                    observableSensorHosts.Add(newSensorHost);
                }
                return newSensorHost.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid AddSensorToSensorHost(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid hostID = typed.HostID;
            SensorHost sensorHost = observableSensorHosts.Where(sh => sh.ID == hostID).Single();

            string zippedFileName = typed.ZippedFileName;
            string[] parsedSensorFileName = zippedFileName.Split('.');

            string humanReadableName = parsedSensorFileName[0];
            Guid sensorTypeSourceID = Guid.Parse(parsedSensorFileName[1]);
            string version = parsedSensorFileName[2];

            string overwrite = typed.Overwrite.ToString();
            string downloadUri = typed.UriToDownloadFrom.ToString();

            Guid sensorID = Guid.NewGuid();
            Sensor newSensor = new Sensor()
            {
                ID = sensorID,
                SensorHostID = hostID,
				SensorTypeSourceID = sensorTypeSourceID,
                DataTypeSourceID = new Guid("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8"),
                IsHealthy = true,
                InstalledFromServerDate = DateTime.Now,
                CurrentRendezvous = new Rendezvous<Command>(Protocol.HTTP, sensorHost.HostName, Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), sensorID).Uri.ToString()
            };

            AddSensorArgs args = new AddSensorArgs();
            args.DownloadUri = downloadUri;
            args.ZippedFileName = zippedFileName;
            args.HumanName = humanReadableName;
            args.Version = version;
            args.Overwrite = overwrite;

            client = GetModel();
            if (client.AddSensor(newSensor, args))
            {
                lock (observableSensors)
                {
                    observableSensors.Add(newSensor);
                }
                HumanReadableSensor tempHumanSensor = new HumanReadableSensor();
                tempHumanSensor.ID = newSensor.ID;
                tempHumanSensor.CurrentRendezvous = newSensor.CurrentRendezvous;
                tempHumanSensor.DataTypeSourceID = newSensor.DataTypeSourceID;
                tempHumanSensor.InstalledFromServerDate = newSensor.InstalledFromServerDate;
                tempHumanSensor.IsHealthy = newSensor.IsHealthy;
				tempHumanSensor.Name = ObservableDataTypeSources.Single(dts => dts.ID == sensorTypeSourceID).Name; ;
                tempHumanSensor.SensorHostID = newSensor.SensorHostID;
                tempHumanSensor.SensorTypeSourceID = newSensor.SensorTypeSourceID;

                ObservableHumanReadableSensors.Add(tempHumanSensor);
                return newSensor.ID;
                
            }
            else
            {
                return Guid.Empty;
            }

        }

        public void LoadSensor(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid sensorTypeSourceID = typed.SensorTypeSourceID;
            string version = observableSensorTypeSources.Where(s => s.ID == sensorTypeSourceID).SingleOrDefault().Version;

            Guid sourceID = typed.SourceID;
            Guid hostID = typed.HostID;
            string startupArgs = typed.CommandLineArgs;

            LoadSensorArgs loadArgs = new LoadSensorArgs();
            loadArgs.SensorTypeSourceID = sensorTypeSourceID.ToString();
            loadArgs.Version = version;
            loadArgs.SourceID = sourceID.ToString();
            loadArgs.HostID = hostID.ToString();
            loadArgs.StartupArgs = startupArgs;

            client = GetModel();
            client.LoadSensor(loadArgs);
        }

        public void UnloadSensor(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid sensorTypeSourceID = typed.SensorTypeSourceID;
            string version = observableSensorTypeSources.Where(s => s.ID == sensorTypeSourceID).SingleOrDefault().Version;

            Guid sourceID = typed.SourceID;
            Guid hostID = typed.HostID;

            UnloadSensorArgs unloadArgs = new UnloadSensorArgs();
            unloadArgs.SensorTypeSourceID = sensorTypeSourceID.ToString();
            unloadArgs.Version = version;
            unloadArgs.SourceID = sourceID.ToString();
            unloadArgs.HostID = hostID.ToString();

            client = GetModel();
            client.UnloadSensor(unloadArgs);
        }

        public void StartSensor(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid sensorTypeSourceID = typed.SensorTypeSourceID;
            string version = observableSensorTypeSources.Where(s => s.ID == sensorTypeSourceID).SingleOrDefault().Version;

            Guid sourceID = typed.SourceID;
            Guid hostID = typed.HostID;

            StartSensorArgs startArgs = new StartSensorArgs();
            startArgs.SensorTypeSourceID= sensorTypeSourceID.ToString();
            startArgs.Version = version;
            startArgs.SourceID = sourceID.ToString();
            startArgs.HostID = hostID.ToString();

            client = GetModel();
            client.StartSensor(startArgs);
        }

        public void StopSensor(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid sensorTypeSourceID = typed.SensorTypeSourceID;
            string version = observableSensorTypeSources.Where(s => s.ID == sensorTypeSourceID).SingleOrDefault().Version;

            Guid sourceID = typed.SourceID;
            Guid hostID = typed.HostID;

            StopSensorArgs stopArgs = new StopSensorArgs();
            stopArgs.SensorTypeSourceID = sensorTypeSourceID.ToString();
            stopArgs.Version = version;
            stopArgs.SourceID = sourceID.ToString();
            stopArgs.HostID = hostID.ToString();

            client = GetModel();
            client.StopSensor(stopArgs);
        }

        public void GenericSensorCommand()
        {
            Dictionary<string, string> sensorCommandParametersDictionary = new Dictionary<string, string>();
            foreach (SensorCommandParameter sensorCommandParameter in CurrentSensorCommand.SensorCommandParameters)
            {
                sensorCommandParametersDictionary.Add(sensorCommandParameter.Key, sensorCommandParameter.Value);
            }

            GenericSensorCommandArgs commandArgs = new GenericSensorCommandArgs();

            commandArgs.CommandName = CurrentSensorCommand.CommandName;
            commandArgs.ArgsList = sensorCommandParametersDictionary;
            commandArgs.SensorRendezvous = CurrentHumanReadableSensor.CurrentRendezvous;

            client = GetModel();
            client.InvokeGenericCommand(commandArgs);
        }

        public void HandleAvailableSensorCommands(List<SensorCommand> commands)
        {
            IEnumerable<SensorCommand> commandsOfSensorTypeSource = null;

            foreach (SensorCommand command in commands)
            {
                if (commandsOfSensorTypeSource == null)
                {
                    commandsOfSensorTypeSource = observableSensorCommands.Where(sc => sc.SensorTypeSourceID == command.SensorTypeSourceID);
                }
                if (commandsOfSensorTypeSource.Where(sc => sc.CommandName == command.CommandName).Count() == 0)
                {
                    lock (observableSensorCommands)
                    {
                        observableSensorCommands.Add(command);
                    }
                }
            }
            if (CurrentSensorCommandList.Count == 0)
            {
                foreach (SensorCommand command in commands)
                {
                    CurrentSensorCommandList.Add(command);
                }
            }
        }

        public void Unzip(string SrcFile, string DstDir)
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
        }

        public void SetDefaultSensorCommand()
        {
            sensorCommandView = CollectionViewSource.GetDefaultView(this.CurrentSensorCommandList);
            NotifyPropertyChanged("CurrentSensorCommand");
        }

        #region View-related Members
        private ICollectionView rootTypeSourceView;
        public DataTypeSource CurrentRootTypeSource
        {
            get
            {
                if (this.rootTypeSourceView != null)
                {
                    return rootTypeSourceView.CurrentItem as DataTypeSource;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    rootTypeSourceView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentRootTypeSource");
                }
            }
        }

        private ICollectionView sensorView;
        public Sensor CurrentSensor
        {
            get
            {
                if (this.sensorView != null)
                {
                    return sensorView.CurrentItem as Sensor;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    sensorView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentSensor");
                }
            }
        }

        private ICollectionView humanReadableSensorView;
        public HumanReadableSensor CurrentHumanReadableSensor
        {
            get
            {
                if (this.humanReadableSensorView != null)
                {
                    return humanReadableSensorView.CurrentItem as HumanReadableSensor;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    humanReadableSensorView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentHumanReadableSensor");
                }
            }
        }

        private ICollectionView dataTypeView;
        public DataType CurrentDataType
        {
            get
            {
                if (this.dataTypeView != null)
                {
                    return dataTypeView.CurrentItem as DataType;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    dataTypeView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentDataType");
                }
            }
        }

        private ICollectionView dataTypeSourceView;
        public DataTypeSource CurrentDataTypeSource
        {
            get
            {
                if (this.dataTypeSourceView != null)
                {
                    return dataTypeSourceView.CurrentItem as DataTypeSource;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    dataTypeSourceView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentDataTypeSource");
                }
            }
        }

        private ICollectionView sensorTypeSourceView;
        public SensorTypeSource CurrentSensorTypeSource
        {
            get
            {
                if (this.sensorTypeSourceView != null)
                {
                    return sensorTypeSourceView.CurrentItem as SensorTypeSource;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    sensorTypeSourceView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentSensorTypeSource");
                }
            }
        }

        private ICollectionView sensorHostTypeView;
        public SensorHostType CurrentSensorHostType
        {
            get
            {
                if (this.sensorHostTypeView != null)
                {
                    return sensorHostTypeView.CurrentItem as SensorHostType;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    sensorHostTypeView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentSensorHostType");
                }
            }
        }

        private ICollectionView sensorHostView;
        public SensorHost CurrentSensorHost
        {
            get
            {
                if (this.sensorHostView != null)
                {
                    return sensorHostView.CurrentItem as SensorHost;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    sensorHostView.MoveCurrentTo(value);

                    NotifyPropertyChanged("CurrentSensorHost");
                }
            }
        }

        private ICollectionView sensorCommandView;
        public SensorCommand CurrentSensorCommand
        {
            get
            {
                if (this.sensorCommandView != null)
                {
                    return sensorCommandView.CurrentItem as SensorCommand;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    sensorCommandView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentSensorCommand");
                }
            }
        }

        private ICollectionView sensorRuntimeView;
        public DataTypeSource CurrentSensorRuntime
        {
            get
            {
                if (this.sensorRuntimeView != null)
                {
                    return sensorRuntimeView.CurrentItem as DataTypeSource;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    sensorRuntimeView.MoveCurrentTo(value);
                    NotifyPropertyChanged("CurrentSensorRuntime");
                }
            }
        }

        private ObservableCollection<SensorCommand> currentSensorCommandList = new ObservableCollection<SensorCommand>();
        public ObservableCollection<SensorCommand> CurrentSensorCommandList
        {
            set
            {
                currentSensorCommandList = value;
                NotifyPropertyChanged("CurrentSensorCommandList");
            }
            get
            {
                return currentSensorCommandList;
            }
        }
        #endregion

        #region Load Entities
        private void LoadSensors()
        {
            client = GetModel();
            observableSensors = new ObservableCollection<Sensor>(client.GetSensors());

            sensorView = CollectionViewSource.GetDefaultView(this.observableSensors);
            NotifyPropertyChanged("CurrentSensor");
        }

        private void LoadHumanReadableSensors()
        {
            client = GetModel();
            observableHumanReadableSensors= new ObservableCollection<HumanReadableSensor>(client.GetHumanReadableSensors());

            humanReadableSensorView = CollectionViewSource.GetDefaultView(this.observableHumanReadableSensors);
            NotifyPropertyChanged("CurrentHumanReadableSensor");
        }

        private void LoadSensorCommands()
        {
            client = GetModel();
            observableSensorCommands = new ObservableCollection<SensorCommand>(client.GetSensorCommands());
            sensorCommandView = CollectionViewSource.GetDefaultView(this.observableSensorCommands);
            NotifyPropertyChanged("CurrentSensorCommand");
        }

        private void LoadDataTypes()
        {
            client = GetModel();
            observableDataTypes = new ObservableCollection<DataType>(client.GetDataTypes());
            dataTypeView = CollectionViewSource.GetDefaultView(this.observableDataTypes);
            NotifyPropertyChanged("CurrentDataType");
        }

        //NB THIS ONE
        private void LoadSensorHosts()
        {
            Guid hostTypeID = Guid.Parse("EDA0FF3E-108B-45D5-BF58-F362FABF2EFE");
            client = GetModel();
            observableSensorHosts = new ObservableCollection<SensorHost>(client.GetSensorHosts());

            if (observableSensorHosts.Where(sh => sh.HostName == System.Net.Dns.GetHostName()).Count() == 0)
            {
                string localHostName = System.Net.Dns.GetHostName();
                SensorHost localSensorHost = new SensorHost();
                localSensorHost.HostName = localHostName;
                localSensorHost.SensorHostTypeID = hostTypeID;

                Guid TELEMETRY_ID = Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3");

                switch (localHostName.ToLower())
                {
                    case "daisy":
                        localSensorHost.ID = Guid.Parse("EA191801-9A6D-4F0D-B3C4-9CD4E18857BB");
                        localSensorHost.HostUri = String.Format("net.pipe://{0}/{1}/{2}", "localhost", TELEMETRY_ID, localSensorHost.ID);
                        break;
                    case "skynet":
                        localSensorHost.ID = Guid.Parse("1F0AB154-5E32-410A-9305-6A03FFB6996C");
                        localSensorHost.HostUri = String.Format("net.pipe://{0}/{1}/{2}", "localhost", TELEMETRY_ID, localSensorHost.ID);
                        break;
                    case "palpatine":
                        localSensorHost.ID = Guid.Parse("FDED09D0-1F38-4A5E-8534-6ED8D7D5894F");
                        localSensorHost.HostUri = String.Format("net.pipe://{0}/{1}/{2}", "localhost", TELEMETRY_ID, localSensorHost.ID);
                        break;
                    case "davros":
                        localSensorHost.ID = Guid.Parse("CEDEBCE8-F3BC-4FD5-859A-FDF9380D2C5F");
                        localSensorHost.HostUri = String.Format("net.pipe://{0}/{1}/{2}", "localhost", TELEMETRY_ID, localSensorHost.ID);
                        break;
                    case "johnny5":
                        localSensorHost.ID = Guid.Parse("741009C1-B8C0-433E-95F7-C75F8995828D");
                        localSensorHost.HostUri = String.Format("net.pipe://{0}/{1}/{2}", "localhost", TELEMETRY_ID, localSensorHost.ID);
                        break;
                }
                if (localSensorHost.ID != Guid.Empty)
                {
                    client = GetModel();
                    client.AddSensorHost(localSensorHost);
                    
                    observableSensorHosts.Add(localSensorHost);
                }
            }

            sensorHostView = CollectionViewSource.GetDefaultView(this.observableSensorHosts);
            NotifyPropertyChanged("CurrentSensorHost");
        }

        private void LoadSensorHostTypes()
        {
            client = GetModel();
            observableSensorHostTypes = new ObservableCollection<SensorHostType>(client.GetSensorHostTypes());
            sensorHostTypeView = CollectionViewSource.GetDefaultView(this.observableSensorHostTypes);
            NotifyPropertyChanged("CurrentSensorHostType");
        }

        private void LoadDataTypeSources()
        {
            client = GetModel();
            observableDataTypeSources = new ObservableCollection<DataTypeSource>(client.GetDataTypeSources());

            dataTypeSourceView = CollectionViewSource.GetDefaultView(this.observableDataTypeSources);
            NotifyPropertyChanged("CurrentDataTypeSource");
        }

        private void LoadSensorTypeSources()
        {
            client = GetModel();
            observableSensorTypeSources = new ObservableCollection<SensorTypeSource>(client.GetSensorTypeSources());

            sensorTypeSourceView = CollectionViewSource.GetDefaultView(this.observableSensorTypeSources);
            NotifyPropertyChanged("CurrentSensorTypeSource");
        }

        private void LoadRootTypeSources()
        {
            client = GetModel();
            observableRootDataTypeSources = new ObservableCollection<DataTypeSource>(client.GetDataTypeSources().Where(data => data.IsCategory == true));

            rootTypeSourceView = CollectionViewSource.GetDefaultView(this.observableRootDataTypeSources);
            NotifyPropertyChanged("CurrentRootTypeSource");
        }

        private void LoadSensorRuntimes()
        {
            client = GetModel();
            observableSensorRuntimes = new ObservableCollection<SensorRuntime>(client.GetSensorRuntimes());

            sensorRuntimeView = CollectionViewSource.GetDefaultView(this.observableSensorRuntimes);
            NotifyPropertyChanged("CurrentSensorRuntime");
        }
        #endregion
    }
}