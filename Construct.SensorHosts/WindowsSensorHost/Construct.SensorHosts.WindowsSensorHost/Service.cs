using System;
using System.Timers;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Linq;
using System.IO;
using System.ServiceModel;
using Microsoft.Win32;
using Construct.MessageBrokering;
using Construct.SensorManagers.SensorManagerBase;
using System.Threading;

using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Construct.SensorManagers.WindowsSensorManager
{   
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    partial class SensorManagerWindowsService : ServiceBase
    {
        public Guid ID { get; private set; }
        private Broker broker;
        private const int DEFAULT_PORT = 8086;

        #region SensorRuntime (available sensor) Members
        private Dictionary<string, string> startupParameters = new Dictionary<string, string>();
        private Dictionary<KeyValuePair<Guid, string>, SensorAppRuntime> sensors = new Dictionary<KeyValuePair<Guid, string>, SensorAppRuntime>();
        #endregion

        #region ConstructServer Members
        private string cachedConstructServerUri;
        private string transportType;
        private string lastGoodTransportType;
        #endregion

        public SensorManagerWindowsService()
        {
            InitializeComponent();
        }

        void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            Dictionary<string, string> args = command.Args;
            switch (command.Name)
            {
                case "InstallSensor":
                    InstallSensor
                    (
                        args["InstallerFileUri"], 
                        args["InstallerFile"], 
                        args["Name"], 
                        Guid.Parse(args["TypeSourceID"]),
                        Guid.Parse(args["SensorID"]),
                        args["Version"], 
                        bool.Parse(args["Overwrite"])
                    );
                    break;
                case "LoadSensor":
                    LoadSensor
                    (
                        Guid.Parse(args["TypeSourceID"]),
                        Guid.Parse(args["SourceID"]),
                        args["Version"],
                        args["ConstructUri"],
                        args["StartUpArguments"]
                    );
                    break;
                case "UnloadSensor":
                    UnloadSensor
                    (
                        Guid.Parse(args["TypeSourceID"]),
                        Guid.Parse(args["SourceID"]),
                        args["Version"]
                    );
                    break;
                case "StartSensor":
                    StartSensor
                    (
                        Guid.Parse(args["TypeSourceID"]),
                        Guid.Parse(args["SourceID"]),
                        args["Version"]
                    );
                    break;
                case "StopSensor":
                    StopSensor
                    (
                        Guid.Parse(args["TypeSourceID"]),
                        Guid.Parse(args["SourceID"]),
                        args["Version"]
                    );
                    break;
                case "CheckHealth":
                    CheckHealth(args["Uri"]);
                    break;
                case "AddTelemetryOutbox":
                    AddTelemetryOutbox(args["Uri"]);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        void broker_OnTelemetryReceived(object sender, string telemetryString)
        {
            Telemetry telemetry = JsonConvert.DeserializeObject<Telemetry>(telemetryString);

            Dictionary<string, string> args = telemetry.Args;
            switch (telemetry.Name)
            {
                case "AnnounceCommandInbox":
                    Outbox<Command> outbox = new Outbox<Command>(new Rendezvous<Command>(args["Uri"]));
                    broker.AddPeer(outbox);

                    string[] potentials = args["Uri"].Split('/');
                    Guid uriSourceId = new Guid();
                    Guid temp = new Guid();
                    foreach (string potential in potentials)
                    {
                        Guid.TryParse(potential, out temp);
                        if (temp != Guid.Empty)
                        {
                            uriSourceId = temp;
                        }
                    }

                    foreach (KeyValuePair<KeyValuePair<Guid, string>, SensorAppRuntime> runtime in sensors)
                    {
                        if (runtime.Value.SensorInstances.Keys.Contains(uriSourceId))
                        {
                            runtime.Value.SensorInstances[uriSourceId].IsWcfServiceRunning = true;
                            runtime.Value.SensorInstances[uriSourceId].SensorUri = args["Uri"];
                        }
                    }
                    break;
                case "AnnounceItemInbox":
                    //TODO: Handle response logic for item inbox notified
                    break;
                case "AnnounceTelemetryInbox":
                    //TODO: Handle response logic for telemetry inbox notified
                    break;
                case "CommandResults":
                    //TODO: Handle response logic for command results notified
                    break;
            }
        }
        protected override void OnStart(string[] args)
        {
            RegistryKey constructKey = null;
            try
            {
                //TODO: get transport type from registry
                transportType = "net.tcp";
                BuildSensorList();
                constructKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Columbia College Chicago");
                cachedConstructServerUri = (string)constructKey.GetValue("ConstructServerURI");
                ID = Guid.Parse((string)constructKey.GetValue("SensorHostID"));
            }
            catch (Exception e) { EventLog.WriteEntry("Could not find cached Construct server Uri."); }

            #region command and telemetry inboxes
            Rendezvous<Command> hostCommandRendezvous = new Rendezvous<Command>(Protocol.NetNamedPipes, System.Net.Dns.GetHostName(), Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), ID);
            Rendezvous<Telemetry> hostTelemetryRendezvous = new Rendezvous<Telemetry>(Protocol.NetNamedPipes, System.Net.Dns.GetHostName(), Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"), ID);

            Inbox<Command> hostCommandInbox = new Inbox<Command>(hostCommandRendezvous);
            Inbox<Telemetry> hostTelemetryInbox = new Inbox<Telemetry>(hostTelemetryRendezvous);
            #endregion

            #region telemetry outbox
            string constructTelemetryUri = cachedConstructServerUri;
            Rendezvous<Telemetry> constructRendezvous = new Rendezvous<Telemetry>(constructTelemetryUri);
            Outbox<Telemetry> ConstructTelemetryOutbox = new Outbox<Telemetry>(constructRendezvous);
            #endregion

            broker = new Broker(new IPeer[3] { hostCommandInbox, hostTelemetryInbox, ConstructTelemetryOutbox });

            broker.OnCommandReceived += new Action<object, string>(broker_OnCommandReceived);
            broker.OnTelemetryReceived += new Action<object, string>(broker_OnTelemetryReceived);
        }

        private void CheckHealth(string callbackUri)
        {
            Rendezvous<Telemetry> telemetryRend = new Rendezvous<Telemetry>(callbackUri);
            if (broker.Peers.OfType<Outbox<Telemetry>>().Count() == 0)
            {
                AddTelemetryOutbox(callbackUri);
            }
            else
            {
                if (broker.Peers.OfType<Outbox<Telemetry>>().First().Contains(telemetryRend) == false)
                {
                    AddTelemetryOutbox(callbackUri);
                }
            }
            Dictionary<string, string> healthArgs = new Dictionary<string, string>();
            healthArgs.Add("ID", ID.ToString());

            Telemetry healthReport = new Telemetry("HealthReport", healthArgs);
            broker.PublishToInbox(telemetryRend, healthReport);
        }

        private void AddTelemetryOutbox(string callbackUri)
        {
            Rendezvous<Telemetry> telemetryRendezvous = new Rendezvous<Telemetry>(callbackUri);
            Outbox<Telemetry> outbox = new Outbox<Telemetry>(telemetryRendezvous);
            broker.AddPeer(outbox);

            Dictionary<string, string> addTelemetryOutboxArgs = new Dictionary<string, string>();
            addTelemetryOutboxArgs.Add("Uri", outbox.AllRendezvous.First().ToString());

            Telemetry response = new Telemetry("AnnounceTelemetryOutbox", addTelemetryOutboxArgs);
            broker.PublishToInbox(telemetryRendezvous, response);
        }

        protected override void OnStop()
        {
            if (broker != null)
            {
				broker = null;
				GC.Collect();
            }
        }

        private bool InstallSensor(string installerFileUri, string installerFile, string name, 
                                  Guid typeSourceID, Guid requestingSensorID, string version, bool overwrite)
        {
            try
            {
                string absoluteInstallPath = @"C:\Program Files (x86)\Construct\SensorHost\Sensors" + "\\" + name + "." + typeSourceID + "\\" + version;
                string absoluteExecuteablePath = absoluteInstallPath + "\\" + name + "." + typeSourceID + "." + version + ".exe";

                SensorAppRuntime newSensorRuntime = new SensorAppRuntime(absoluteExecuteablePath, name, typeSourceID, requestingSensorID, version);
                newSensorRuntime.DownloadCompletedEvent +=new Action<Guid>(sensorRuntime_DownloadCompletedEvent);

                KeyValuePair<Guid, string> sensorVersionIdentifier = new KeyValuePair<Guid, string>(typeSourceID, version);
                if (!sensors.ContainsKey(sensorVersionIdentifier))
                {
                    newSensorRuntime.PrepareRuntimeDownload(installerFileUri, installerFile, absoluteInstallPath);
                    sensors.Add(sensorVersionIdentifier, newSensorRuntime);
                    return true;
                }
                else
                {
                    if (overwrite)
                    {
                        OverwriteRuntime(installerFileUri, installerFile, absoluteInstallPath, sensorVersionIdentifier, requestingSensorID);
                        return true;
                    }
                    else
                    {
                        sensorRuntime_DownloadCompletedEvent(requestingSensorID);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
				String message = "Unable to install sensor '" + name + "':\n" + ex + "\nInstaller URI: " + installerFileUri;
				EventLog.WriteEntry(message, EventLogEntryType.Error);
                return false;
            }
        }
        private void OverwriteRuntime(string installerFileUri, string installerFile, 
                                      string absoluteInstallPath, KeyValuePair<Guid, string> sensorVersionIdentifier, Guid requestingSensorID)
        {
            if (Directory.Exists(absoluteInstallPath))
            {
                Directory.Delete(absoluteInstallPath, true);
            }

            sensors[sensorVersionIdentifier].LatestSensorID = requestingSensorID;
            sensors[sensorVersionIdentifier].PrepareRuntimeDownload(installerFileUri, installerFile, absoluteInstallPath);
        }
        private void sensorRuntime_DownloadCompletedEvent(Guid sensorID)
        {
            Dictionary<string, string> args = new Dictionary<string,string>();
            args.Add("SensorID", sensorID.ToString());
            Telemetry announceInstallComplete = new Telemetry("AnnounceInstallComplete", args);
            broker.Publish(announceInstallComplete);
        }

        private void BuildSensorList()
        {
            EventLog.WriteEntry("Building sensor list");
            Directory.SetCurrentDirectory(@"C:\Program Files (x86)\Construct\SensorHost\Sensors");

            string[] sensorDirs = Directory.GetDirectories(Directory.GetCurrentDirectory());

			String sensorNameMatcher = @"^\w*\.\w{8}-\w{4}-\w{4}-\w{4}-\w{12}\.\d*.exe$";
            
            foreach (string sensorDir in sensorDirs)
            {
                string[] versionDirs = Directory.GetDirectories(sensorDir);
                foreach (string versionDir in versionDirs)
                {
                    //TODO: Currently assuming no folder structure past version level!!
                    string[] filePaths = Directory.GetFileSystemEntries(versionDir);
                    foreach (string currentFilePath in filePaths)
                    {
						string[] splitFilePath = currentFilePath.Split('\\');
						String fileName = splitFilePath.Last();

						if (!Regex.IsMatch(fileName, sensorNameMatcher, RegexOptions.IgnoreCase))
							continue;

						EventLog.WriteEntry("Successfully detected sensor " + fileName);

                        string[] splitExeFile = splitFilePath.Last().Split('.');

						try
						{
							Guid typeSourceID = Guid.Parse(splitExeFile[1]);
							string version = splitExeFile[2];

							SensorAppRuntime runtime = new SensorAppRuntime(currentFilePath, splitExeFile[0], typeSourceID, Guid.Empty, version);
							runtime.DownloadCompletedEvent += new Action<Guid>(sensorRuntime_DownloadCompletedEvent);
							KeyValuePair<Guid, string> sensorVersionIdentifier = new KeyValuePair<Guid, string>(typeSourceID, version);

							sensors.Add(sensorVersionIdentifier, runtime);
						}
						catch (Exception ex)
						{
							EventLog.WriteEntry("While enumerating sensor listing, unable to process sensor " + currentFilePath + "\n" + ex);
						}
                    }
                }
            }
        }

        private bool LoadSensor(Guid typeSourceID, Guid sourceID, string version, string constructServerUri, string startUpArguments)
        {
            Directory.SetCurrentDirectory(@"C:\Program Files (x86)\Construct\SensorHost");
            KeyValuePair<Guid, string> sensorVersionIdentifier = new KeyValuePair<Guid, string>(typeSourceID, version);

            try
            {
                if (!sensors.ContainsKey(sensorVersionIdentifier))
                    return false;

                else
                {
                    if (!sensors[sensorVersionIdentifier].SensorInstances.ContainsKey(sourceID))
                    {
                        sensors[sensorVersionIdentifier].SensorInstances.Add(sourceID, new SensorAppInstance(typeSourceID, sourceID));
                        sensors[sensorVersionIdentifier].LoadInstance(sourceID, constructServerUri, broker.Peers.OfType<Inbox<Telemetry>>().Single().CurrentRendezvous.Uri.ToString(), startUpArguments);
                        return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
				String message = "Unable to load sensor {0} of type {1}.\nConstruct URI: {2}\nLaunch Parameters: {3}\n{4}";
				message = String.Format(message, sourceID, typeSourceID, constructServerUri, startUpArguments, ex);
                EventLog.WriteEntry(ex.ToString());
            }

            if (sensors == null) { }
            return false;
        }

        private bool UnloadSensor(Guid typeSourceID, Guid sourceID, string version)
        {
            KeyValuePair<Guid, string> sensorVersionIdentifier = new KeyValuePair<Guid, string>(typeSourceID, version);

            try
            {
                if (sensors.ContainsKey(sensorVersionIdentifier) && sensors[sensorVersionIdentifier].SensorInstances.ContainsKey(sourceID)
                    && sensors[sensorVersionIdentifier].SensorInstances[sourceID].IsWcfServiceRunning)
                {
                    // TODO: 'IsWcfServiceRunning' may not be the corret bool to check. SensorAppInstance object may have different bool
                    //  structure, create Rendezvous object from SensorAppInstance object.
                    Command stopCommand = new Command("Stop", new Dictionary<string, string>());
                    Command exitCommand = new Command("Exit", new Dictionary<string, string>());
                    Rendezvous<Command> sensorRendezvous = new Rendezvous<Command>(sensors[sensorVersionIdentifier].SensorInstances[sourceID].SensorUri);
                    broker.PublishToInbox(sensorRendezvous, stopCommand);
                    broker.PublishToInbox(sensorRendezvous, exitCommand);
                    sensors[sensorVersionIdentifier].SensorInstances.Remove(sourceID);
                }
            }
            catch (CommunicationException ex)
            {
                EventLog.WriteEntry(ex.ToString());
                sensors[sensorVersionIdentifier].SensorInstances.Remove(sourceID);
                return true;
            }
            catch (Exception ex)
            {
				String message = String.Format("Unable to unload sensor {0} of type {1}\n{2}", sourceID, typeSourceID, ex.ToString());
                EventLog.WriteEntry(message, EventLogEntryType.Error);
            }
            return false;
        }

        private bool StartSensor(Guid typeSourceID, Guid sourceID, string version)
        {
            KeyValuePair<Guid, string> sensorVersionIdentifier = new KeyValuePair<Guid, string>(typeSourceID, version);

            try
            {
                if (sensors.ContainsKey(sensorVersionIdentifier) && sensors[sensorVersionIdentifier].SensorInstances.ContainsKey(sourceID)
                    && sensors[sensorVersionIdentifier].SensorInstances[sourceID].IsWcfServiceRunning)
                {
                    Command startCommand = new Command("Start", new Dictionary<string, string>());
                    Rendezvous<Command> sensorRendezvous = new Rendezvous<Command>(sensors[sensorVersionIdentifier].SensorInstances[sourceID].SensorUri);
                    broker.PublishToInbox(sensorRendezvous, startCommand);
                    return true;
                }
            }
            catch (Exception ex)
            {
				String message = String.Format("Unable to start sensor {0} of type {1}\n{2}", sourceID, typeSourceID, ex.ToString());
                EventLog.WriteEntry(message, EventLogEntryType.Error);
            }
            return false;
        }

        private bool StopSensor(Guid typeSourceID, Guid sourceID, string version)
        {
            KeyValuePair<Guid, string> sensorVersionIdentifier = new KeyValuePair<Guid, string>(typeSourceID, version);
            try
            {
                if (sensors.ContainsKey(sensorVersionIdentifier) && sensors[sensorVersionIdentifier].SensorInstances.ContainsKey(sourceID)
                    && sensors[sensorVersionIdentifier].SensorInstances[sourceID].IsWcfServiceRunning)
                {
                    Command stopCommand = new Command("Stop", new Dictionary<string, string>());
                    Rendezvous<Command> sensorRendezvous = new Rendezvous<Command>(sensors[sensorVersionIdentifier].SensorInstances[sourceID].SensorUri);
                    broker.PublishToInbox(sensorRendezvous, stopCommand);
                    return true;
                }
            }
            catch (Exception ex)
			{
				String message = String.Format("Unable to stop sensor {0} of type {1}\n{2}", sourceID, typeSourceID, ex.ToString());
				EventLog.WriteEntry(message, EventLogEntryType.Error);
			}
            return false;
        }

        public string DefaultTransportType
        {
            get
            {
                return "test";
            }
            set
            {
                PushTransportTypeToRegistry(value);
            }
        }

        private void PushTransportTypeToRegistry(string defaultTransport)
        {
            RegistryKey transportTypeKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Columbia College Chicago");
            transportTypeKey.CreateSubKey("PH").SetValue("Default Transport Type", defaultTransport);
        }

        public string TransportType
        {
            get
            {
                return transportType;
            }
            set
            {
                string transportToTry = value.ToLower();
                if (VerifyLegitimateTransport(transportToTry))
                {
                    lastGoodTransportType = transportType;
                    transportType = transportToTry;
                }
            }
        }

        public string LastGoodTransportType
        {
            get
            {
                return lastGoodTransportType;
            }
            set
            {
                lastGoodTransportType = value;
            }
        }

        private bool VerifyLegitimateTransport(string attemptedTransport)
        {
            return (attemptedTransport == "tcp" || attemptedTransport == "http" ||
                    attemptedTransport == "https" || attemptedTransport == "namedpipes");
        }

        private void SetStartupParameters(Dictionary<string, string> theStartupParameters)
        {
            startupParameters = theStartupParameters;
        }

        private void SetStartupParameters(string theStartupParameters)
        {
            string[] pairs = theStartupParameters.Split(';');
            for (int i = 0; i < pairs.Length; i++)
            {
                string[] items = pairs[i].Split('=');
                startupParameters.Add(items[0], items[1]);
            }
        }

        private Dictionary<string, string> GenerateDictionaryFromString(char pairDelimiter, char valueDelimiter, string args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] pairs = args.Split(pairDelimiter);
            for (int i = 0; i < pairs.Length; i++)
            {
                string[] items = pairs[i].Split(valueDelimiter);
                result.Add(items[0], items[1]);
            }
            return result;
        }
    }
}
