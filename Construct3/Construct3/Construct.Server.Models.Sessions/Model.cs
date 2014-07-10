using System;
using System.Configuration;
using System.Linq;
using Construct.Server.Entities;
using Construct.MessageBrokering;
using System.Collections.Generic;
using System.ServiceModel;
using Construct.MessageBrokering;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Entities;
using Construct.Utilities.Shared;
using NLog;


namespace Construct.Server.Models.Sessions
{
    public class Model : Models.Model, IModel
    {
        private readonly Guid serverProcessID;
        private readonly Broker broker;
        private readonly ConstructSerializationAssistant assistant;

        public readonly Uri serverServiceUri;
        private string entitiesConnectionString;
        private ICallback _callback = null;

        public Model(Uri serverServiceUri, string theEntitiesConnectionString, Models.IServer server)
            : base()
        {
            Name = "Sessions";

            logger.Trace("instantiating Session model");

            serverProcessID = UriUtility.GetServerProcessIdFromServiceUri(serverServiceUri);

            Rendezvous<Telemetry> constructTelemetryRendezvous = new Rendezvous<Telemetry>(Protocol.HTTP, Rendezvous<Telemetry>.GetHostName(),
                Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"),
                serverProcessID);
            broker = server.Broker;
            this.serverServiceUri = serverServiceUri;
            entitiesConnectionString = theEntitiesConnectionString;
            broker.AddPeer(new Inbox<Telemetry>(constructTelemetryRendezvous));
            using (Entities.EntitiesModel model = new EntitiesModel(theEntitiesConnectionString))
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

        public bool Add(Entities.Adapters.Session sessionAdapter)
        {
            ICallback serviceOperationContext = Callback;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                try
                {
                    Session session = sessionAdapter;
                    model.Add(session);
                    model.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    return false;
                }
            }
        }

        public bool Add(Entities.Adapters.SessionSource sessionSourceAdapter)
        {
            ICallback serviceOperationContext = Callback;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                try
                {
                    SessionSource sessionSource = sessionSourceAdapter;
                    model.Add(sessionSource);
                    model.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    return false;
                }
            }
        }

        public IEnumerable<Entities.Adapters.Session> GetSessions()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.Session> sessions = new List<Entities.Adapters.Session>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Sessions)
                {
                    sessions.Add(obj);
                }
            }
            return sessions;
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

        public IEnumerable<Entities.Adapters.SessionSource> GetSessionSources()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.SessionSource> sessionSources = new List<Entities.Adapters.SessionSource>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.SessionSources)
                {
                    sessionSources.Add(obj);
                }
            }
            return sessionSources;
        }

        private void HandleTelemetry(object sender, string telemetryString)
        {
            Telemetry telemetry = assistant.GetTelemetry(telemetryString);

            switch (telemetry.Name)
            {
                case "AnnounceCommandInbox":
                   
                    break;
                case "AnnounceInstallComplete":
                    break;
                case "AnnounceTelemetryInbox":
                    break;
                case "AnnounceItemInbox":
                    break;
                case "AvailableSensorCommands":
                    break;
                case "AnnounceTelemetryOutbox":
                    break;
                case "HealthReport":
                    break;
                default:
                    break;
            }
        }
    }
}