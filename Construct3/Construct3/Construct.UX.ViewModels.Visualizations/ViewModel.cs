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
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            //TODO: Get an appropriate Source ID.
            client.PersistLabeledItem(definition, sourceID);
        }

        public IEnumerable<Label> GetAllLabels()
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            IEnumerable<Label> labels = client.GetAllLabels();
            return labels;
        }

        public IEnumerable<Taxonomy> GetAllTaxonomies()
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            IEnumerable<Taxonomy> taxonomies = client.GetAllTaxonomies();
            return taxonomies;
        }

        public IEnumerable<TaxonomyLabel> GetAllTaxonomyLables()
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            IEnumerable<TaxonomyLabel> taxonomies = client.GetAllTaxonomyLables();
            return taxonomies;
        }

        public IEnumerable<DataType> GetAllDataTypes()
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            IEnumerable<DataType> dataTypes = client.GetAllDataTypes();
            return dataTypes;
        }
        public IEnumerable<PropertyType> GetAllProperties(string dataType)
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            return client.GetAllProperties(dataType);
        }
        public IEnumerable<Visualizer> GetAssociatedVisualizers(PropertyType propertyType)
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            return client.GetAssociatedVisualizers(propertyType);
        }

        public IEnumerable<Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization> GetVisualizations()
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            IEnumerable<Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization> visualizations;
            visualizations = client.GetAllVisualizations();
            return visualizations;
        }

        public void AddVisualization(Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization visualization)
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            client.AddVisualization(visualization);
        }

        public Visualizer GetAssociatedVisualizer(Visualization adapter)
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            return client.GetAssociatedVisualizer(adapter);
        }

        public IEnumerable<Source> GetAssociatedSources(DataType adapter)
        {
            CallbackInstance callback = new CallbackInstance();
            InstanceContext context = new InstanceContext(callback);
            ModelClient client = new ModelClient(context);
            return client.GetAssociatedSources(adapter);
        }
    }
}
