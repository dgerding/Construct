using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;

namespace Construct.UX.ViewModels.Visualizations
{
    [Export("VisualizationsViewModel")]
    public class ViewModel : ViewModels.ViewModel
    {
        private readonly Guid sourceID = Guid.NewGuid();

		private ModelClient client = null;
		private CallbackInstance callback = new CallbackInstance();
		private InstanceContext instanceContext = null;

		private ModelClient GetModel()
		{
			if (client == null || client.State == CommunicationState.Closed || client.State == CommunicationState.Faulted)
			{
				instanceContext = new InstanceContext(callback);
				client = new ModelClient(instanceContext);
				ModelClientHelper.EnhanceModelClientBandwidth<ModelClient>(client);
				client.Open();
			}

			return client;
		}


        [ImportingConstructor]
        public ViewModel(ApplicationSessionInfo sessionInfo)
            : base("Visualizations")
        {
        }

        public override void Load()
        {
        }

        public void AddLabeledItem(LabeledItemAdapter definition)
        {
            ModelClient client = GetModel();
            //TODO: Get an appropriate Source ID.
            client.PersistLabeledItem(definition, sourceID);
        }

        public IEnumerable<Label> GetAllLabels()
        {
			ModelClient client = GetModel();
            IEnumerable<Label> labels = client.GetAllLabels();
            return labels;
        }

        public IEnumerable<Taxonomy> GetAllTaxonomies()
        {
			ModelClient client = GetModel();
            IEnumerable<Taxonomy> taxonomies = client.GetAllTaxonomies();
            return taxonomies;
        }

        public IEnumerable<TaxonomyLabel> GetAllTaxonomyLables()
        {
			ModelClient client = GetModel();
			IEnumerable<TaxonomyLabel> taxonomies = client.GetAllTaxonomyLables();
            return taxonomies;
        }

        public IEnumerable<DataType> GetAllDataTypes()
        {
			ModelClient client = GetModel();
            return client.GetAllDataTypes();
        }

	    public IEnumerable<Visualizer> GetAllVisualizers()
	    {
		    ModelClient client = GetModel();
		    return client.GetAllVisualizers();
	    }

        public IEnumerable<PropertyType> GetAllProperties(string dataType)
        {
			ModelClient client = GetModel();
			PropertyType[] result = client.GetAllProperties(dataType);
			return result;

        }

        public IEnumerable<Visualization> GetAllVisualizations()
        {
			ModelClient client = GetModel();
            IEnumerable<Visualization> visualizations;
            visualizations = client.GetAllVisualizations();
            return visualizations;
        }

        public void AddVisualization(Visualization visualization)
        {
			ModelClient client = GetModel();
            client.AddVisualization(visualization);
        }

	    public void AddVisualizer(Visualizer visualizer)
	    {
		    ModelClient client = GetModel();
			client.AddVisualizer(visualizer);
	    }

	    public void RemoveVisualization(Visualization visualization)
	    {
		    ModelClient client = GetModel();
			client.RemoveVisualization(visualization);
	    }

	    public void RemoveVisualizer(Visualizer visualizer)
	    {
		    ModelClient client = GetModel();
			client.RemoveVisualizer(visualizer);
	    }

        public Visualizer GetAssociatedVisualizer(Visualization adapter)
        {
			ModelClient client = GetModel();
            return client.GetAssociatedVisualizer(adapter);
        }

        public IEnumerable<Source> GetAssociatedSources(DataType adapter)
        {
			ModelClient client = GetModel();
            return client.GetAssociatedSources(adapter);
        }

	    public IEnumerable<HumanReadableSensor> GetHumanReadableSensors()
	    {
		    ModelClient client = GetModel();
		    return client.GetHumanReadableSensors();
	    }
    }
}
