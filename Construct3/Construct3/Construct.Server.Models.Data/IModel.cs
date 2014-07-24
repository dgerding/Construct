using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
using Construct.Server.Entities;
using System.Collections.ObjectModel;

namespace Construct.Server.Models.Data
{
    [ServiceContract(CallbackContract=typeof(ICallback))]
    public interface IModel : Models.IModel
    {
        [OperationContract(Name = "AddTypeWithXML")]
        bool AddType(string xml);

        [OperationContract(Name = "AddTypeWithDataType")]
        bool AddType(Entities.Adapters.DataTypeSource source, Entities.Adapters.DataType dataType, Entities.Adapters.PropertyType[] properties, bool IsAggregateType = false);

        [OperationContract]
        bool SetContext(string connectionString);

        [OperationContract]
        void Add(Datum datum);

        [OperationContract]
        IEnumerable<Entities.Adapters.DataType> GetAllTypes();

        [OperationContract]
        ReadOnlyCollection<Uri> GetUris(Entities.Adapters.DataType dataType, Entities.Adapters.PropertyType propertyType);

        [OperationContract]
        Uri GetPropertyValueEndpoint(Guid propertyID);
    }
}
