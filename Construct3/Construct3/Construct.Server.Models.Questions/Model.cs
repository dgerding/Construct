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
using Telerik.Expressions;
using System.Linq.Expressions;
using Telerik.Windows.Data;

namespace Construct.Server.Models.Questions
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
            Name = "Questions";

            logger.Trace("instantiating Questions model");

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

            string exprString = "Interval < 1000";
            ExpressionNode exprNode = ExpressionParser.Parse(exprString);
            ExpressionTypeConverter converter = new ExpressionTypeConverter();
            NodeExpression temp1 = (NodeExpression)(converter.ConvertFrom(exprString));
            //LambdaExpression temp2 = temp1.ToLambda(typeof(NodeExpression));
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

        #region IModel Members

        public bool Add(Entities.Adapters.Question question)
        {
            using (Entities.EntitiesModel context = GetModel())
            {
                try
                {
                    context.Add(question);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    return false;
                }
            }    
        }

        public bool Add(Entities.Adapters.QuestionTypeSource questionTypeSource)
        {
            using (Entities.EntitiesModel context = GetModel())
            {
                try
                {
                    context.Add(questionTypeSource);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    logger.Trace(e.Message);
                    return false;
                }
            }
        }

        public IEnumerable<Entities.Adapters.DataType> GetDataTypes()
        {
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

        public IEnumerable<Entities.Adapters.Property> GetProperties()
        {
            List<Entities.Adapters.Property> properties = new List<Entities.Adapters.Property>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Properties)
                {
                    properties.Add(obj);
                }
            }
            return properties;
        }

        public IEnumerable<Entities.Adapters.PropertyParent> GetPropertyParents()
        {
            List<Entities.Adapters.PropertyParent> propertyParents = new List<Entities.Adapters.PropertyParent>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.PropertyParents)
                {
                    propertyParents.Add(obj);
                }
            }
            return propertyParents;
        }

        public IEnumerable<Entities.Adapters.PropertyType> GetPropertyTypes()
        {
            List<Entities.Adapters.PropertyType> propertyTypes = new List<Entities.Adapters.PropertyType>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.PropertyTypes)
                {
                    propertyTypes.Add(obj);
                }
            }
            return propertyTypes;
        }


        public IEnumerable<Entities.Adapters.Question> GetQuestions()
        {
            List<Entities.Adapters.Question> questions = new List<Entities.Adapters.Question>();
            using (Entities.EntitiesModel model = new EntitiesModel(entitiesConnectionString))
            {
                foreach (var obj in model.Questions)
                {
                    questions.Add(obj);
                }
            }
            return questions;
        }
        #endregion

        private Entities.EntitiesModel GetModel()
        {
            Entities.EntitiesModel result = new Entities.EntitiesModel(entitiesConnectionString);
            return result;
        }
    }
}