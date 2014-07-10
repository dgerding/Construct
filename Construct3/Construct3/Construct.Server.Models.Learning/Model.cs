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

namespace Construct.Server.Models.Learning
{
    public class Model : Models.Model, IModel
    {
        private readonly Guid serverProcessID;
        private readonly Broker broker;
        private readonly ConstructSerializationAssistant assistant;

        public readonly Uri serverServiceUri;
        private string entitiesConnectionString;
        private ICallback _callback = null;
        private Data.Model dataModel;

        public Model(Uri serverServiceUri, string theEntitiesConnectionString, Models.IServer server, Data.Model dataModel)
            : base()
        {
            Name = "Learning";

            logger.Trace("instantiating Learning model");

            serverProcessID = UriUtility.GetServerProcessIdFromServiceUri(serverServiceUri);

            Rendezvous<Telemetry> constructTelemetryRendezvous = new Rendezvous<Telemetry>(Protocol.HTTP, Rendezvous<Telemetry>.GetHostName(),
                Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"),
                serverProcessID);
            broker = server.Broker;
            this.dataModel = dataModel;

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

            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                if (model.DataTypes.Where(dt => dt.Name == "Supervised_Machine_Learning_Label").Count() == 0)
                {
                    AddSupervisedMLLabelType();
                }
            }
        }

        private void AddSupervisedMLLabelType()
        {
            Guid labelDataTypeID = Guid.Parse("1294b397-f52f-4830-a109-e1d9c2a6be37");
            Guid labelDataTypeSourceID = Guid.Parse("064A944C-9347-4E0A-9642-744F80D7BD8F");

            Entities.Adapters.DataType labelDataType = new Entities.Adapters.DataType();
            labelDataType.ID = labelDataTypeID;
            labelDataType.Name = "Supervised_Machine_Learning_Label";
            labelDataType.FullName = "Supervised_Machine_Learning_Label";
            labelDataType.IsCoreType = true;
            labelDataType.IsReadOnly = true;
            labelDataType.DataTypeSourceID = labelDataTypeSourceID;

            Entities.Adapters.DataTypeSource labelDataTypeSource = new Entities.Adapters.DataTypeSource();
            labelDataTypeSource.ID = labelDataTypeSourceID;
            labelDataTypeSource.Name = "Supervised_Machine_Learning_Labeler";
            labelDataTypeSource.IsCategory = false;
            labelDataTypeSource.IsReadOnly = true;
            labelDataTypeSource.ParentID = Guid.Parse("3BF44DE6-E03A-4E73-B5A0-277EF3F724B8");
                

            List<Entities.Adapters.PropertyType> labelDataTypeProps = new List<Entities.Adapters.PropertyType>()
            {
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("bc10d04f-be6e-4bdc-ade0-bd1b939acb66"), Name = "SessionID", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("64B84E4A-6545-405F-B760-5CA96D15EC3E")},
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("64e8590b-e554-4911-9e06-bf2edc6c7793"), Name = "LabeledPropertyID", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("64B84E4A-6545-405F-B760-5CA96D15EC3E")},
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("356f8042-5495-4ea5-b61c-aa1e1b64546b"), Name = "LabeledItemID", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("64B84E4A-6545-405F-B760-5CA96D15EC3E")},
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("5998f679-8cd2-4dbb-bd75-8f82f51cec25"), Name = "LabeledSourceID", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("64B84E4A-6545-405F-B760-5CA96D15EC3E")},
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("a4a955f9-ad15-4403-9c9b-1a0da92612a0"), Name = "TaxonomyLabelID", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("64B84E4A-6545-405F-B760-5CA96D15EC3E")},
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("fea06e28-fd9f-4763-82f4-ab83bc755508"), Name = "LabeledStartTime", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("C8D37D33-1F38-4EC5-9D49-C28DF3466B00")},
                new Entities.Adapters.PropertyType(){ID = Guid.Parse("43f4ef39-5789-454b-b11e-3c90e887d219"), Name = "LabeledInterval", ParentDataTypeID = labelDataTypeID, PropertyDataTypeID = Guid.Parse("A6FFA473-3483-43B5-A2A8-B606C565C883")}
            };

            dataModel.AddType(labelDataTypeSource, labelDataType, labelDataTypeProps.ToArray(), false);
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

        public void GeneratedLabelAttributeVectors(Guid sessionID)
        {
            //get sources and from sessionID and then all items from those sources
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