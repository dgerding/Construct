using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IIntPropertyValueService
    {
        [OperationContract]
        IEnumerable<IntPropertyValue> GetAll();
        [OperationContract]
        IEnumerable<IntPropertyValue> GetAfter(DateTime time);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class IntPropertyValueService : PropertyService<IntPropertyValue, Int32>, IIntPropertyValueService
    {
        public IntPropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public IntPropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override Int32 GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.GetInt32(5);
        }
    }
}
