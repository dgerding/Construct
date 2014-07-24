using System;
using System.Linq;
using Construct.Utilities.Shared;
using System.ServiceModel;
using Construct.UX.ViewModels.Data.DataServiceReference;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace Construct.UX.ViewModels.Data
{
    public class ViewModel : ViewModels.ViewModel
    {
        private ModelClient model = null;
        private CallbackImplementation callback = new CallbackImplementation();
        private InstanceContext instanceContext = null;

        public ObservableCollection<DataType> ObservableDataTypes { get; private set; }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo)
            : base(applicationSessionInfo, "Data")
        {
            model = GetModel();
            ObservableDataTypes = new ObservableCollection<DataType>(GetAllTypes());
        }

        private ModelClient GetModel()
        {
			if (model == null)
			{
				instanceContext = new InstanceContext(callback);
				model = new ModelClient(instanceContext, "duplexendpoint", RemoteAddress);
				ModelClientHelper.EnhanceModelClientBandwidth<ModelClient>(model);
			}

			if (model.State != CommunicationState.Opened)
				model.Open();
			return model;
        }

        public override void Load()
        {
        }

        public bool AddTypeWithXML(string xml)
        {
            return model.AddTypeWithXML(xml);
        }
        public bool AddTypeWithDataType(DataServiceReference.DataTypeSource dataTypeSource, DataServiceReference.DataType dataType, IEnumerable<DataServiceReference.PropertyType> propertyTypes)
        {
            return model.AddTypeWithDataType(dataTypeSource, dataType, propertyTypes.ToArray(), true);
        }

        public bool SetContext(string connectionString)
        {
            return model.SetContext(connectionString);
        }

        public void Add(DataServiceReference.Datum datum)
        {
            model.Add(datum);
        }

        public DataServiceReference.DataType[] GetAllTypes()
        {
            DataType[] dataTypes = model.GetAllTypes();
            ObservableDataTypes = new ObservableCollection<DataType>(dataTypes);
            return dataTypes;
        }

        /* todo 10/11
        public ReadOnlyCollection<Uri> GetUri(DataServiceReference.DataType dataType, DataServiceReference.PropertyType propertyType)
        {
            return model.GetUris(dataType, propertyType);
        }
         */
    }
}