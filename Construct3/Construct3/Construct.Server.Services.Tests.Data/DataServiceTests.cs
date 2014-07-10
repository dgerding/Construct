using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using Construct.Utilities.Shared;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Construct.Server.Services.Tests.Data
{
    [TestClass]
    public class DataServiceTests : Construct.Server.Models.Tests.Data.DataModelTests
    {
        private Services.Data.Service service = null;
        private ServiceHost host = null;
           

        public DataServiceTests()
        {
        }

        [TestMethod]
        public void OpenServiceHost()
        {
            base._1_1_S3_VerifyAssemblyIsEnhancedByVEnhance();

            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://construct1.colum.edu:8080/tfs"), new UICredentialsProvider());
            tfs.EnsureAuthenticated();
            VersionControlServer versionControl = tfs.GetService<VersionControlServer>();
            string loggedInUser = versionControl.AuthorizedUser;

            int hostPortBase = 8000;
            Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.NewGuid(), hostPortBase);

            Models.Server server = new Models.Server(serverServiceUri, hostPortBase, loggedInUser);
            service = new Services.Data.Service(base.utilitiesTests.serverEndpoint, base.entityTests.connectionString, server);

            host = new ServiceHost(service, base.utilitiesTests.serverEndpoint);

            try
            {
                host.Open();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}