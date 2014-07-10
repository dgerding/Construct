using System;
using System.Linq;
using Construct.Server.Models.Visualizations;
using System.ServiceModel;

namespace Construct.Server.Services.Visualizations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : Model
    {
        public Service(Uri serverServiceUri, string connectionString, Models.IServer server)
            : base(serverServiceUri, connectionString, server)
        {
        }
    }
}