using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IStringPropertyValueService
    {
        [OperationContract]
        IEnumerable<StringPropertyValue> GetAll();
        [OperationContract]
        IEnumerable<StringPropertyValue> GetAfter(DateTime time);
		[OperationContract]
		IEnumerable<StringPropertyValue> GetBetween(DateTime start, DateTime end);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class StringPropertyValueService : PropertyService<StringPropertyValue, String>, IStringPropertyValueService
    {
        public StringPropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public StringPropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override String GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            string result = reader.GetString(5);
            return result;
        }
    }
}
