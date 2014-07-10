using System;
using System.Linq;
using Construct.Server.Models.Sources;
using System.ServiceModel;

namespace Construct.Server.Services.Sources
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : Model
    {
        public Service(Uri serverServiceUri, string connectionString, Models.IServer server, Server.Models.Data.Model dataModel)
            : base(serverServiceUri, connectionString, server, dataModel)
        {
        }
    }
}
