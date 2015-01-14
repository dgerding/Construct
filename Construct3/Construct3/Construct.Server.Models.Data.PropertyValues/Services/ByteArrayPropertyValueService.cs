using System;
using System.Linq;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValues.Services
{
    [ServiceContract]
    public interface IByteArrayPropertyValueService
    {
        [OperationContract]
        IEnumerable<ByteArrayPropertyValue> GetAll();
        [OperationContract]
        IEnumerable<ByteArrayPropertyValue> GetAfter(DateTime time);
		[OperationContract]
		IEnumerable<ByteArrayPropertyValue> GetBetween(DateTime start, DateTime end);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ByteArrayPropertyValueService : PropertyService<ByteArrayPropertyValue, byte[]>, IByteArrayPropertyValueService
    {
        public ByteArrayPropertyValueService(Uri serverUri, string type, string property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
        public ByteArrayPropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }

        protected override byte[] GetValue(System.Data.SqlClient.SqlDataReader reader)
        {
            return (byte[])reader[5];
        }
    }
}
