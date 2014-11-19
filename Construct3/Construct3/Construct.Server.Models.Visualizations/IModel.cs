using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Construct.Server.Models.Visualizations
{
    [ServiceContract(CallbackContract = typeof(Visualizations.ICallback))]
    [ServiceKnownType(typeof(Entities.Adapters.DataType))]
    [ServiceKnownType(typeof(Entities.Adapters.PropertyType))]
    [ServiceKnownType(typeof(Entities.Adapters.Visualization))]
	[ServiceKnownType(typeof(Entities.Adapters.HumanReadableSensor))]

    public interface IModel : Models.IModel
    {
        [OperationContract]
        void AddVisualization(Entities.Adapters.Visualization visualization);
        [OperationContract]
        IEnumerable<Entities.Adapters.DataType> GetAllDataTypes();
        [OperationContract]
        IEnumerable<Entities.Adapters.PropertyType> GetAllProperties(string dataType);
        [OperationContract]
        Entities.Adapters.DataType GetDataType(Guid id);
        [OperationContract]
        IEnumerable<Entities.Adapters.Visualizer> GetAssociatedVisualizers(Entities.Adapters.PropertyType adapter);
        [OperationContract]
        IEnumerable<Entities.Adapters.Source> GetAssociatedSources(Entities.Adapters.DataType adapter);
		[OperationContract]
	    IEnumerable<Entities.Adapters.HumanReadableSensor> GetHumanReadableSensors();
        [OperationContract]
        IEnumerable<Entities.Adapters.Visualization> GetAllVisualizations();
        [OperationContract]
        Entities.Adapters.Visualizer GetAssociatedVisualizer(Entities.Adapters.Visualization adapter);
        [OperationContract]
        IEnumerable<Entities.Adapters.Label> GetAllLabels();
        [OperationContract]
        IEnumerable<Entities.Adapters.Label> GetAssociatedLabels(Guid itemID);
        [OperationContract]
        IEnumerable<Entities.Adapters.Taxonomy> GetAllTaxonomies();
        [OperationContract]
        IEnumerable<Entities.Adapters.TaxonomyLabel> GetAllTaxonomyLables();
        [OperationContract]
        void PersistLabeledItem(LabeledItemAdapter adapter, Guid sourceID);
    }
}