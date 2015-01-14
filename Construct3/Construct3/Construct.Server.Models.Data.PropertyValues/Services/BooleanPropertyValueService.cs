using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IBooleanPropertyValueService
    {
        [OperationContract]
        IEnumerable<BooleanPropertyValue> GetAll();
        [OperationContract]
        IEnumerable<BooleanPropertyValue> GetAfter(DateTime time);
		[OperationContract]
		IEnumerable<BooleanPropertyValue> GetBetween(DateTime start, DateTime end);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BooleanPropertyValueService : PropertyService<BooleanPropertyValue, Boolean>, IBooleanPropertyValueService
    {
        public BooleanPropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public BooleanPropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override Boolean GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.GetBoolean(5);
        }
    }
}
