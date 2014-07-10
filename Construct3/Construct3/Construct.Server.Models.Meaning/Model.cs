using System;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using Construct.MessageBrokering;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Entities;
using Construct.Utilities.Shared;
using Construct.Server.Entities;

namespace Construct.Server.Models.Meaning
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
            logger.Trace("instantiating Meaning model");
            
            Name = "Meaning";
            this.serverServiceUri = serverServiceUri;
            entitiesConnectionString = theEntitiesConnectionString;
            broker = server.Broker;

            serverProcessID = UriUtility.GetServerProcessIdFromServiceUri(serverServiceUri);
            Rendezvous<Telemetry> constructTelemetryRendezvous = new Rendezvous<Telemetry>(Protocol.HTTP, 
                                                                                            Rendezvous<Telemetry>.GetHostName(),
                                                                                            Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"),
                                                                                            serverProcessID);
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

        public bool Add(Entities.Adapters.Taxonomy taxonomyAdapter)
        {
            ICallback serviceOperationContext = Callback;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                try
                {
                    Taxonomy taxonomy = taxonomyAdapter;
                    model.Add(taxonomy);
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

        public bool Add(Entities.Adapters.Label labelAdapter)
        {
            ICallback serviceOperationContext = Callback;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                try
                {
                    Label label = labelAdapter;
                    model.Add(label);
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

        public bool Add(Entities.Adapters.TaxonomyLabel taxonomyLabelAdapter)
        {
            ICallback serviceOperationContext = Callback;

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                try
                {
                    TaxonomyLabel taxonomyLabel = taxonomyLabelAdapter;
                    model.Add(taxonomyLabel);
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

        public IEnumerable<Entities.Adapters.Taxonomy> GetTaxonomies()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.Taxonomy> taxonomies = new List<Entities.Adapters.Taxonomy>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Taxonomies)
                {
                    taxonomies.Add(obj);
                }
            }
            return taxonomies;
        }

        public IEnumerable<Entities.Adapters.Label> GetLabels()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.Label> labels = new List<Entities.Adapters.Label>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Labels)
                {
                    labels.Add(obj);
                }
            }
            return labels;
        }

        public IEnumerable<Entities.Adapters.TaxonomyLabel> GetTaxonomyLabels()
        {
            ICallback serviceOperationContext = Callback;

            List<Entities.Adapters.TaxonomyLabel> taxonomyLabels = new List<Entities.Adapters.TaxonomyLabel>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.TaxonomyLabels)
                {
                    taxonomyLabels.Add(obj);
                }
            }
            return taxonomyLabels;
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