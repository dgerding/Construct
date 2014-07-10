using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Construct.Server.Entities;

namespace Construct.Server.Services.PropertyValue
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PropertyValueService : Construct.Server.Models.Data.PropertyValue.PropertyService<Int32>
    {
        public PropertyValueService(Uri serverUri, DataType type, Property property, string connectionString)
            : base(serverUri, type, property, connectionString)
        {
        }
    }

}
