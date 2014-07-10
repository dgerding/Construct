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

		[OperationContract]
		DateTime? GetEarliestItemForTypeAndSource(Guid dataTypeId, Guid sourceId);

		[OperationContract]
		DateTime? GetLatestItemForTypeAndSource(Guid dataTypeId, Guid sourceId);

		[OperationContract]
		uint GetNumberOfItemsInTimespan(DateTime startTime, DateTime endTime, Guid? itemTypeId, Guid? sourceId);


		//	List of identifiers of properties contained in the specified item types
		[OperationContract]
		Guid[] GenerateConstructHeaders(Guid[] itemTypeIds);

		[OperationContract]
		String[] GenerateConstructHeaderNames(Guid[] constructHeader);

		//	Returns list of values from the associated source ids for the requested headers
		[OperationContract]
		List<object[]> GenerateConstruct(DateTime startTime, DateTime endTime, TimeSpan interval, Guid[] constructHeaders, Guid[] sourceIds);
    }
}
