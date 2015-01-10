using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface ISinglePropertyValueService
    {
        [OperationContract]
        IEnumerable<SinglePropertyValue> GetAll();
        [OperationContract]
        IEnumerable<SinglePropertyValue> GetAfter(DateTime time);
	    [OperationContract]
	    IEnumerable<SinglePropertyValue> GetBetween(DateTime start, DateTime end);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SinglePropertyValueService : PropertyService<SinglePropertyValue, Single>, ISinglePropertyValueService
    {
        public SinglePropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public SinglePropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override Single GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return reader.GetFloat(5);
        }
    }
}
