using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Construct.MessageBrokering;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Entities;
using Construct.Utilities.Shared;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Telerik.OpenAccess;


namespace Construct.Server.Models.Visualizations
{
	public class Model : Models.Model, IModel
	{
		private readonly Guid serverProcessID;
		private readonly Broker broker;
		private readonly ConstructSerializationAssistant assistant;

		public readonly Uri serverServiceUri;
		private string entitiesConnectionString;
		private ICallback _callback = null;
		private Entities.EntitiesModel context = null;

		public Model(Uri serverServiceUri, string theEntitiesConnectionString, Models.IServer server)
			: base()
		{
			Name = "Visualizations";

			logger.Trace("instantiating Visualizations model");

			serverProcessID = UriUtility.GetServerProcessIdFromServiceUri(serverServiceUri);

			Rendezvous<Telemetry> constructTelemetryRendezvous = new Rendezvous<Telemetry>(Protocol.HTTP,
				Rendezvous<Telemetry>.GetHostName(),
				Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"),
				serverProcessID);

			broker = server.Broker;
			this.serverServiceUri = serverServiceUri;
			entitiesConnectionString = theEntitiesConnectionString;
			context = new Entities.EntitiesModel(entitiesConnectionString);
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

		public void PersistLabeledItem(LabeledItemAdapter adapter, Guid sourceID)
		{
			Guid itemID = Guid.NewGuid();

			Guid dataTypeID = Guid.Parse("1294B397-F52F-4830-A109-E1D9C2A6BE37");
			Guid dataTypeSourceID = Guid.Parse("064A944C-9347-4E0A-9642-744F80D7BD8F");
			string dataName = "Supervised_Machine_Learning_Label";

			if (context.Sources.Count(target => target.ID == sourceID) == 0)
			{
				context.Add(new Source
				{
					ID = sourceID,
					DataTypeSourceID = dataTypeSourceID
				});
				context.SaveChanges();
			}

			MessageBrokering.Data dataItem = new MessageBrokering.Data
				(
				adapter,
				sourceID,
				dataTypeSourceID,
				dataName,
				dataTypeID
				);

			string jsonHeaderData =
				"{ 'PayloadTypes' : {'LabeledInterval' : 'Int32', 'LabeledItemID' : 'Guid', 'LabeledPropertyID' : 'Guid', 'LabeledSourceID' : 'Guid', 'LabeledStartTime' : 'DateTime', 'SessionID' : 'Guid', 'TaxonomyLabelID' : 'Guid'},'Instance' :";
			string theDataString = String.Format("{0}{1}", jsonHeaderData, JsonConvert.SerializeObject(dataItem));

			broker.PersistJson(theDataString);
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

		public Entities.Visualization Create(string name, Entities.Visualizer visualizer, PropertyType propertyType)
		{
			Entities.Visualization result = new Entities.Visualization();
			result.ID = Guid.NewGuid();
			result.Name = name;
			result.Visualizer = visualizer;
			result.DataTypeID = visualizer.DataType.ID;

			//TODO: persist property information to later query on the visualizer.

			return result;
		}

		public void AddVisualization(Entities.Adapters.Visualization visualization)
		{
			Entities.Visualization entity = new Visualization();
			entity.DataTypeID = visualization.DataTypeID;
			entity.Description = visualization.Description;
			entity.ID = visualization.ID;
			entity.Name = visualization.Name;
			entity.VisualizerID = visualization.VisualizerID;
			entity.PropertyID = visualization.PropertyID;

			context.Add(entity);
			context.SaveChanges();
		}

		public IEnumerable<Entities.Adapters.Label> GetAllLabels()
		{
			foreach (var label in context.Labels)
			{
				yield return (Entities.Adapters.Label) label;
			}
		}

		public IEnumerable<Entities.Adapters.Label> GetAssociatedLabels(Guid itemID)
		{
			foreach (var label in context.Labels)
			{
				yield return (Entities.Adapters.Label) label;
			}
		}

		public IEnumerable<Entities.Adapters.Taxonomy> GetAllTaxonomies()
		{
			foreach (var taxonomy in context.Taxonomies)
			{
				yield return (Entities.Adapters.Taxonomy) taxonomy;
			}
		}

		public IEnumerable<Entities.Adapters.TaxonomyLabel> GetAllTaxonomyLables()
		{
			foreach (var taxonomyLabel in context.TaxonomyLabels)
			{
				yield return (Entities.Adapters.TaxonomyLabel) taxonomyLabel;
			}
		}

		public IEnumerable<Entities.Adapters.Visualization> GetAllVisualizations()
		{
			IEnumerable<Entities.Visualization> visualizations = context.Visualizations.AsEnumerable();

			foreach (var visualization in visualizations)
			{
				yield return (Entities.Adapters.Visualization) visualization;
			}
		}

		public IEnumerable<Entities.Adapters.Source> GetAssociatedSources(Entities.Adapters.DataType adapter)
		{
			IEnumerable<Entities.Source> sources = context.Sources.AsEnumerable();
			foreach (Source source in sources)
			{
				if (source.DataTypeSource.DataTypes.FirstOrDefault(target => target.ID == adapter.ID) != null)
				{
					yield return (Entities.Adapters.Source) source;
				}
			}
		}

		public IEnumerable<Entities.Adapters.HumanReadableSensor> GetHumanReadableSensors()
		{
			foreach (var humanReadableSensor in context.HumanReadableSensors)
			{
				yield return (Entities.Adapters.HumanReadableSensor) humanReadableSensor;
			}
		}

		public System.Collections.Generic.IEnumerable<Entities.Adapters.DataType> GetAllDataTypes()
        {
            IEnumerable<Entities.DataType> dataTypes = context.DataTypes.AsEnumerable();

            foreach (var dataType in dataTypes)
            {
                yield return (Entities.Adapters.DataType)dataType;
            }
        }

        public Entities.Adapters.DataType GetDataType(Guid id)
        {
            return (Entities.Adapters.DataType)context.DataTypes.Single(target => target.ID == id);
        }


        public System.Collections.Generic.IEnumerable<Entities.Adapters.PropertyType> GetAllProperties(string dataTypeName)
        {
			DataType dataType = null;
			try
			{
				dataType = context.DataTypes.AsEnumerable().Single(target => target.Name == dataTypeName);
			}
			catch (InvalidOperationException e)
			{
				logger.Warn("Query 'Single' for datatype " + dataTypeName + " failed. Either nonexistant datatype or numerous entries for that datatype.");
				return new Entities.Adapters.PropertyType[0];
			}

            IList<Entities.Adapters.PropertyType> propertyTypes = new List<Entities.Adapters.PropertyType>();

            foreach (var propertyType in context.PropertyTypes)
            {
                if (propertyType.ParentDataTypeID == dataType.ID)
                {
                    Entities.Adapters.PropertyType adapter = (Entities.Adapters.PropertyType)propertyType;
                    propertyTypes.Add(adapter);
                }
            }

            return propertyTypes;
        }

        public IEnumerable<Entities.Adapters.Visualizer> GetAssociatedVisualizers(Entities.Adapters.PropertyType adapter)
        {
            Entities.PropertyType propertyType = context.PropertyTypes.Single(target => adapter.ID == target.ID);

            IEnumerable<Entities.Visualizer> visualizers = context.Visualizers.Where(target => target.PrimitiveDataTypeID == propertyType.PropertyDataTypeID);

            foreach (var visualizer in visualizers)
            {
                yield return (Entities.Adapters.Visualizer)visualizer;
            }
        }

        public Entities.Adapters.Visualizer GetAssociatedVisualizer(Entities.Adapters.Visualization adapter)
        {
            Entities.Visualizer visualizer = context.Visualizers.Single(target => adapter.VisualizerID == target.ID);
            
            return (Entities.Adapters.Visualizer)visualizer;
        }
    }
}