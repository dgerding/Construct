using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IDoublePropertyValueService
    {
        [OperationContract]
        IEnumerable<DoublePropertyValue> GetAll();
        [OperationContract]
        IEnumerable<DoublePropertyValue> GetAfter(DateTime time);
		[OperationContract]
		IEnumerable<DoublePropertyValue> GetBetween(DateTime start, DateTime end);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DoublePropertyValueService : PropertyService<DoublePropertyValue, Double>, IDoublePropertyValueService
    {
        public DoublePropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public DoublePropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override Double GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.GetDouble(4);
        }
    }
}
