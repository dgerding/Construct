using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using NLog;
using Construct.MessageBrokering;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Entities;
using Construct.Utilities.Shared;


namespace Construct.Server.Models.Sources
{
    public class Model : Models.Model, IModel
    {
        private readonly Guid serverProcessID;
        private readonly Broker broker;
        private readonly ConstructSerializationAssistant assistant;
        private Data.Model dataModel;

        public readonly Uri serverServiceUri;
        private string entitiesConnectionString;
        private ICallback _callback = null;

        public Model(Uri serverServiceUri, string theEntitiesConnectionString, Models.IServer server, Data.Model dataModel) 
            : base()
        {
            logger.Trace("instantiating Sources model");

            Name = "Sources";
            this.serverServiceUri = serverServiceUri;
            entitiesConnectionString = theEntitiesConnectionString;
            broker = server.Broker;
            this.dataModel = dataModel;

            serverProcessID = UriUtility.GetServerProcessIdFromServiceUri(serverServiceUri);
            Rendezvous<Telemetry> constructTelemetryRendezvous = new Rendezvous<Telemetry>(Protocol.HTTP,
                                                                                            Rendezvous<Telemetry>.GetHostName(),
                                                                                            Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"),
                                                                                            serverProcessID);
            broker.AddPeer(new Inbox<Telemetry>(constructTelemetryRendezvous));
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (SensorHost host in model.SensorHosts)
                {
                    Entities.SensorHost adapter = host;
                    broker.AddPeer(new Outbox<Telemetry>(new Rendezvous<Telemetry>(host.HostUri)));
                    broker.AddPeer(new Outbox<Command>(adapter.CreateCommandRendezvous()));
                }
            }

            broker.OnTelemetryReceived += HandleTelemetry;

            assistant = new ConstructSerializationAssistant();
        }

        private ICallback Callback 
        { 
            get 
            {
                if (OperationContext.Current != null)
                {
                    _callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                }
                return _callback; 
            }
        }

        public bool Add(Entities.Adapters.Sensor sensorAdapter, AddSensorArgs addArgs)
        {
            ICallback serviceOperationContext = Callback;
            SensorHost sensorHost = null;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                Entities.Sensor sensor = sensorAdapter;
                sensorHost = model.SensorHosts.Where(sh => sh.ID == sensor.SensorHostID).Single();

                Dictionary<string, string> commandArgs = new Dictionary<string, string>();
                commandArgs.Add("InstallerFileUri", string.Format("{0}{1}", addArgs.DownloadUri, @"/Sensors/"));
                commandArgs.Add("InstallerFile", addArgs.ZippedFileName);
                commandArgs.Add("Name", addArgs.HumanName);
                commandArgs.Add("TypeSourceID", sensor.SensorTypeSourceID.ToString());
                commandArgs.Add("Version", addArgs.Version);
                commandArgs.Add("Overwrite", addArgs.Overwrite);
                commandArgs.Add("SensorID", sensor.ID.ToString());

                Command installCommand = new Command("InstallSensor", commandArgs);

                Rendezvous<Command> targetRendezvous = new Rendezvous<Command>(sensorHost.GetProtocol(), 
                                                                                sensorHost.HostName,
                                                                                Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"),
                                                                                sensor.SensorHostID);

                try
                {
                    model.Add(sensor);
                    model.SaveChanges();
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    return false;
                }

                broker.PublishToInbox(targetRendezvous, installCommand);
                return true;
            }
        }

        public bool Add(Entities.Adapters.Question question, AddQuestionArgs addArgs)
        {
            ICallback serviceOperationContext = Callback;
            return true;
        }

        public bool Add(Entities.Adapters.SensorHost sensorHostAdapter)
        {
            ICallback serviceOperationContext = Callback;

            Entities.SensorHost sensorHost = sensorHostAdapter;
            Rendezvous<Command> resolvedComRendz = new Rendezvous<Command>(sensorHostAdapter.GetProtocol(),
                sensorHost.HostName,
                Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"),
                sensorHost.ID);
            Rendezvous<Telemetry> resolvedTeleRendz = new Rendezvous<Telemetry>(sensorHostAdapter.GetProtocol(),
                sensorHost.HostName,
                Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"),
                sensorHost.ID);
            try
            {
                broker.AddPeer(new Outbox<Command>(resolvedComRendz));
                broker.AddPeer(new Outbox<Telemetry>(resolvedTeleRendz));

                Dictionary<string, string> commandArgs = new Dictionary<string, string>();
                commandArgs.Add("Uri", broker.Peers.OfType<Inbox<Telemetry>>().Single().CurrentRendezvous.Uri.ToString());

                Command addTelemetryOutboxCommand = new Command("AddTelemetryOutbox", commandArgs);
                broker.PublishToInbox(resolvedComRendz, addTelemetryOutboxCommand);
            }
            catch (EndpointNotFoundException)
            {
                sensorHost.IsHealthy = false;
            }
            catch (Exception e)
            {
                logger.Trace(e.Message);
                throw;
            }

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                try
                {
                    model.Add(sensorHost);
                    model.SaveChanges();
                }
                catch(Exception e)
                {
                    logger.Trace(e.Message);
                    return false;
                }
            }
            return true;
        }

        public bool Add(Entities.Adapters.SensorRuntime sensorRuntimeAdapter)
        {
            ICallback serviceOperationContext = Callback;

            try
            {
                SensorInstaller installer = new SensorInstaller(entitiesConnectionString, dataModel);
                Entities.SensorRuntime sensorRuntime = sensorRuntimeAdapter;
                if (installer.AddSensorEntities(sensorRuntime))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                logger.Trace(e.Message);
                throw;
            }
        }

        public void LoadSensor(LoadSensorArgs loadArgs)
        {
            ICallback serviceOperationContext = Callback;

            Dictionary<string, string> commandArgs = new Dictionary<string, string>();
            commandArgs.Add("TypeSourceID", loadArgs.SensorTypeSourceID);
            commandArgs.Add("SourceID", loadArgs.SourceID);
            commandArgs.Add("Version", loadArgs.Version);
            commandArgs.Add("ConstructUri", broker.Peers.OfType<Inbox<Telemetry>>().First().CurrentRendezvous.Uri.ToString());
            commandArgs.Add("StartUpArguments", loadArgs.StartupArgs);

            Command loadCommand = new Command("LoadSensor", commandArgs);

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                SensorHost sensorHost;

                try
                {
                    sensorHost = model.SensorHosts.Single(sh => sh.ID.ToString() == loadArgs.HostID);
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                try
                {
                    broker.PublishToInbox(new Rendezvous<Command>(sensorHost.GetProtocol(), sensorHost.HostName, Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), Guid.Parse(loadArgs.HostID)), loadCommand);
                }
                catch (EndpointNotFoundException e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
            }
        }

        public void UnloadSensor(UnloadSensorArgs unloadArgs)
        {
            ICallback serviceOperationContext = Callback;

            Dictionary<string, string> commandArgs = new Dictionary<string, string>();
            commandArgs.Add("TypeSourceID", unloadArgs.SensorTypeSourceID);
            commandArgs.Add("SourceID", unloadArgs.SourceID);
            commandArgs.Add("Version", unloadArgs.Version);

            Command unloadCommand = new Command("UnloadSensor", commandArgs);
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                SensorHost sensorHost;
                try
                {
                    sensorHost = model.SensorHosts.Single(sh => sh.ID.ToString() == unloadArgs.HostID);
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                try
                {
                    broker.PublishToInbox(new Rendezvous<Command>(sensorHost.GetProtocol(), sensorHost.HostName, Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), Guid.Parse(unloadArgs.HostID)), unloadCommand);
                }
                catch (EndpointNotFoundException e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
            }
        }

        public void StartSensor(StartSensorArgs startArgs)
        {
            ICallback serviceOperationContext = Callback;

            Dictionary<string, string> AddItemOutboxArgs = new Dictionary<string, string>();
            AddItemOutboxArgs.Add("Uri", broker.Peers
                                               .OfType<Inbox<MessageBrokering.Data>>()
                                               .Where(s => s.GetTypeSourceIDFromRendezvous().ToString() == startArgs.SensorTypeSourceID)
                                               .Single()
                                               .CurrentRendezvous
                                               .Uri
                                               .ToString());

            Command AddItemOutboxCommand = new Command("AddItemOutbox", AddItemOutboxArgs);
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                SensorHost sensorHost;
                try
                {
                    sensorHost = model.SensorHosts.Single(sh => sh.ID.ToString() == startArgs.HostID);
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                try
                {
                    broker.PublishToInbox(new Rendezvous<Command>(Protocol.HTTP, sensorHost.HostName, Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), Guid.Parse(startArgs.SourceID)), AddItemOutboxCommand);
                }
                catch (EndpointNotFoundException e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                Dictionary<string, string> commandArgs = new Dictionary<string, string>();
                commandArgs.Add("TypeSourceID", startArgs.SensorTypeSourceID);
                commandArgs.Add("SourceID", startArgs.SourceID);
                commandArgs.Add("Version", startArgs.Version);

                Command startCommand = new Command("StartSensor", commandArgs);
                try
                {
                    broker.PublishToInbox(new Rendezvous<Command>(sensorHost.GetProtocol(), sensorHost.HostName, Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), Guid.Parse(startArgs.HostID)), startCommand);
                }
                catch (EndpointNotFoundException e)
                {
                    logger.Trace(e.Message);
                    throw e;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
            }
        }

        public void StopSensor(StopSensorArgs stopArgs)
        {
            ICallback serviceOperationContext = Callback;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("TypeSourceID", stopArgs.SensorTypeSourceID);
            args.Add("SourceID", stopArgs.SourceID);
            args.Add("Version", stopArgs.Version);

            Command stopCommand = new Command("StopSensor", args);
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                SensorHost sensorHost;
                try
                {
                    sensorHost = model.SensorHosts.Single(sh => sh.ID.ToString() == stopArgs.HostID);
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                try
                {
                    broker.PublishToInbox(new Rendezvous<Command>(sensorHost.GetProtocol(), sensorHost.HostName, Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), Guid.Parse(stopArgs.HostID)), stopCommand);
                }
                catch (EndpointNotFoundException e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    throw;
                }
            }
        }

        public void InvokeGenericCommand(GenericSensorCommandArgs commandArgs)
        {
            ICallback serviceOperationContext = Callback;

            Command command = new Command(commandArgs.CommandName, commandArgs.ArgsList);
            try
            {
                broker.PublishToInbox(new Rendezvous<Command>(commandArgs.SensorRendezvous), command);
            }
            catch (EndpointNotFoundException e)
            {
                logger.Trace(e.Message);
                throw;
            }
        }

        public IEnumerable<Entities.Adapters.Sensor> GetSensors()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.Sensor> sensors = new List<Entities.Adapters.Sensor>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Sensors)
                {
                    sensors.Add(obj);
                }
            }
            return sensors;
        }

        public IEnumerable<Entities.Adapters.HumanReadableSensor> GetHumanReadableSensors()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.HumanReadableSensor> sensors = new List<Entities.Adapters.HumanReadableSensor>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.HumanReadableSensors)
                {
                    sensors.Add(obj);
                }
            }
            return sensors;
        }

        public IEnumerable<Entities.Adapters.Source> GetSources()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.Source> sources = new List<Entities.Adapters.Source>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Sources)
                {
                    sources.Add(obj);
                }
            }
            return sources;
        }

        public IEnumerable<Entities.Adapters.SensorCommand> GetSensorCommands()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.SensorCommand> sensorCommands = new List<Entities.Adapters.SensorCommand>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.SensorCommands)
                {
                    sensorCommands.Add(obj);
                }
            }
            return sensorCommands;
        }

        public IEnumerable<Entities.Adapters.SensorTypeSource> GetSensorTypeSources()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.SensorTypeSource> sensorTypeSources = new List<Entities.Adapters.SensorTypeSource>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.SensorTypeSources)
                {
                    sensorTypeSources.Add(obj);
                }
            }
            return sensorTypeSources;
        }

        public IEnumerable<Entities.Adapters.DataType> GetDataTypes()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.DataType> dataTypes = new List<Entities.Adapters.DataType>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.DataTypes)
                {
                    dataTypes.Add(obj);
                }
            }
            return dataTypes;
        }

        public IEnumerable<Entities.Adapters.SensorHost> GetSensorHosts()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.SensorHost> sensorHosts = new List<Entities.Adapters.SensorHost>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.SensorHosts)
                {
                    sensorHosts.Add(obj);
                }
            }
            return sensorHosts;
        }

        public IEnumerable<Entities.Adapters.SensorHostType> GetSensorHostTypes()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.SensorHostType> sensorHostTypes = new List<Entities.Adapters.SensorHostType>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.SensorHostTypes)
                {
                    sensorHostTypes.Add(obj);
                }
            }
            return sensorHostTypes;
        }

        public IEnumerable<Entities.Adapters.DataTypeSource> GetDataTypeSources()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.DataTypeSource> dataTypeSources = new List<Entities.Adapters.DataTypeSource>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.DataTypeSources)
                {
                    dataTypeSources.Add(obj);
                }
            }
            return dataTypeSources;
        }

        public IEnumerable<Entities.Adapters.SensorRuntime> GetSensorRuntimes()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.SensorRuntime> sensorRuntimes = new List<Entities.Adapters.SensorRuntime>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.SensorRuntimes)
                {
                    sensorRuntimes.Add(obj);
                }
            }
            return sensorRuntimes;
        }

		public IEnumerable<Entities.Adapters.Sensor> GetSensorsEmittingType(Entities.Adapters.DataType dataType)
		{
			List<Entities.Adapters.Sensor> result = new List<Entities.Adapters.Sensor>();

			using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
			{
				foreach (var sensor in model.Sensors)
				{
					if (sensor.SensorTypeSource.DataTypes.Any((sensorDataType) => sensorDataType.ID == dataType.ID))
						result.Add(sensor);
				}
			}

			return result;
		}

        /*  TODO: Think this is legacy
        public void SetDataContext(string connectionString)
        {
            this.dataServiceUri = new Uri(connectionString);
        }
         */

        public void SetEntitiesContext(string connectionString)
        {
            this.entitiesConnectionString = connectionString;
        }

        private void HandleTelemetry(object sender, string telemetryString)
         {
            Telemetry telemetry = assistant.GetTelemetry(telemetryString);
            logger.Trace(telemetryString);
            switch (telemetry.Name)
            {
                case "AnnounceCommandInbox":
                    AnnounceCommandInbox(telemetry);
                    break;
                case "AnnounceInstallComplete":
                    AnnounceInstallComplete(telemetry);
                    break;
                case "AnnounceTelemetryInbox":
                    break;
                case "AnnounceItemInbox":
                    break;
                case "AnnounceTelemetryOutbox":
                    break;
                case "AvailableSensorCommands":
                    ParseAvailableSensorCommands(telemetry);
                    break;
                case "AnnounceStandAloneSensor":
                    AnnounceStandAloneSensor(telemetry);
                    break;
                case "HealthReport":
                    break;
                default:
                    break;
            }
        }

        private void AnnounceCommandInbox(Telemetry telemetry)
        {
            Rendezvous<Command> commandRend = new Rendezvous<Command>(telemetry.Args["Uri"]);
            broker.AddPeer(new Outbox<Command>(commandRend));

            Rendezvous<MessageBrokering.Data> newDataRend = new Rendezvous<MessageBrokering.Data>(Protocol.NetNamedPipes,
                                                                                                  "localhost",
                                                                                                  Guid.Parse(telemetry.Args["DataTypeSourceID"]),
                                                                                                  serverProcessID);
            Inbox<MessageBrokering.Data> newItemInbox = new Inbox<MessageBrokering.Data>(newDataRend);
            broker.AddPeer(newItemInbox);
            try
            {
                _callback.SensorLoadedCallbackReceived(commandRend.SensorID);
            }
            catch (Exception e)
            {
                logger.Trace(e.Message);
                throw;
            }
        }

        private void AnnounceStandAloneSensor(Telemetry telemetry)
        {
            string dataTypeSourceID = telemetry.Args["DataTypeSourceID"];
            string sourceID = telemetry.Args["SourceID"];
            Rendezvous<MessageBrokering.Data> newDataRend = new Rendezvous<MessageBrokering.Data>(Protocol.NetNamedPipes,
                                                                                      "localhost",
                                                                                      Guid.Parse(dataTypeSourceID),
                                                                                      serverProcessID);
            Inbox<MessageBrokering.Data> newItemInbox = new Inbox<MessageBrokering.Data>(newDataRend);
            broker.AddPeer(newItemInbox);

            // add source entity to DB here
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                Source existingSource = model.Sources.SingleOrDefault(s => s.ID == Guid.Parse(sourceID));
                if (existingSource == null)
                {
                    Source standAloneSource = new Source();
                    standAloneSource.ID = Guid.Parse(sourceID);
                    standAloneSource.DataTypeSourceID = Guid.Parse(dataTypeSourceID);

                    try
                    {
                        model.Add(standAloneSource);
                        model.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        logger.Trace(e.Message);
                        throw;
                    }
                }
            }
        }

        private void AnnounceInstallComplete(Telemetry telemetry)
        {
            Guid sensorID = Guid.Parse(telemetry.Args["SensorID"]);
            try
            {
                _callback.SensorInstalledCallbackReceived(sensorID);
            }
            catch (Exception e)
            {
                logger.Trace(e.Message);
                throw;
            }
        }

        private void ParseAvailableSensorCommands(Telemetry telemetry)
        {
            List<Entities.SensorCommand> commands = new List<Entities.SensorCommand>();
            List<Entities.Adapters.SensorCommand> adapterCommands = new List<Entities.Adapters.SensorCommand>();
            Guid sensorTypeSourceID;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                sensorTypeSourceID = model.Sensors.Single(s => s.ID == telemetry.BrokerID).SensorTypeSourceID;
            }

            foreach (KeyValuePair<string, string> sensorCommand in telemetry.Args)
            {
                Guid IDGuid = Guid.NewGuid();
                
                Entities.SensorCommand newSensorCommand = new Entities.SensorCommand()
                {
                    ID = IDGuid,
                    CommandName = sensorCommand.Key,
                    SensorTypeSourceID = sensorTypeSourceID
                };

                var parameters = getSensorCommandParameters(sensorCommand.Value, IDGuid);
                foreach (var command in parameters)
                {
                    newSensorCommand.SensorCommandParameters.Add(command);
                }
                commands.Add(newSensorCommand);
            }

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {   
                IEnumerable<SensorCommand> commandsOfSensorTypeSource = null;
                foreach (SensorCommand command in commands)
                {
                    if (commandsOfSensorTypeSource == null)
                    {
                        commandsOfSensorTypeSource = model.SensorCommands.Where(sc => sc.SensorTypeSourceID == command.SensorTypeSourceID);
                    }
                    if (commandsOfSensorTypeSource.Where(sc => sc.CommandName == command.CommandName).Count() == 0)
                    {
                        try
                        {
                            model.Add(commands);
                            model.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            logger.Trace(e.Message);
                            throw;
                        }
                    }
                }
            }

            foreach (SensorCommand command in commands)
            {
                adapterCommands.Add(command);
            }
            try
            {
                _callback.AvailableSensorCommandsCallbackReceived(adapterCommands);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<Entities.SensorCommandParameter> getSensorCommandParameters(string sensorCommandValue, Guid sensorCommandID)
        {
            List<Entities.SensorCommandParameter> sensorCommandParameters = new List<Entities.SensorCommandParameter>();
            string[] splitSensorCommandParameters = sensorCommandValue.Split(',');
            foreach (string sensorCommandParameter in splitSensorCommandParameters)
            {
                sensorCommandParameters.Add(new Entities.SensorCommandParameter { ID = Guid.NewGuid(), Key = sensorCommandParameter, Value = string.Empty, SensorCommandID = sensorCommandID });
            }
            return sensorCommandParameters;
        }
    }
}
