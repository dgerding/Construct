using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IDateTimePropertyValueService
    {
        [OperationContract]
        IEnumerable<DateTimePropertyValue> GetAll();
        [OperationContract]
        IEnumerable<DateTimePropertyValue> GetAfter(DateTime time);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DateTimePropertyValueService : PropertyService<DateTimePropertyValue, DateTime>, IDateTimePropertyValueService
    {
        public DateTimePropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public DateTimePropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override DateTime GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.GetDateTime(4);
        }
    }
}
