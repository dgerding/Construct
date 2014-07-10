using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IGuidPropertyValueService
    {
        [OperationContract]
        IEnumerable<GuidPropertyValue> GetAll();
        [OperationContract]
        IEnumerable<GuidPropertyValue> GetAfter(DateTime time);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GuidPropertyValueService : PropertyService<GuidPropertyValue, Guid>, IGuidPropertyValueService
    {
        public GuidPropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public GuidPropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override Guid GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.GetGuid(4);
        }
    }
}
