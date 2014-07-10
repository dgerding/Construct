using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Construct.MessageBrokering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Construct.Server.Entities;
using Construct.Server.Models.Sessions;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Utilities.Shared;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Configuration;

namespace Construct.Server.Models.Tests.Sessions
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ModelSessionsTests
    {
        private static Entities.EntitiesModel context;
        private static Models.Sessions.Model model;
        private static Guid testSensorTypeSourceID, testDataTypeID, testSourceID;
        private static string testItemDefinitionXml;
        private static string testItemXml;
        private static string connectionString;
        private static string defaultDatabase = "Construct3";
        private static Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("net.pipe", "Server", "localhost", Guid.Empty, 8000);
        
        public ModelSessionsTests()
        {
           
        }

        #region Additional test attributes
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://construct1.colum.edu:8080/tfs"), new UICredentialsProvider());
            tfs.EnsureAuthenticated();
            VersionControlServer versionControl = tfs.GetService<VersionControlServer>();
            string loggedInUser = versionControl.AuthorizedUser;

            int hostPortBase = 8000;
            Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.NewGuid(), hostPortBase);

            Models.Server server = new Server(serverServiceUri, hostPortBase, loggedInUser);
            
            connectionString = ConfigurationManager.ConnectionStrings[loggedInUser].ConnectionString;
            context = new Entities.EntitiesModel(connectionString);
            
            model = new Models.Sessions.Model(serverServiceUri, connectionString, server);
        }

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void CreateSessionModelInstance()
        {
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void CreateSessionDesignEntity()
        {
            SessionDesign testSession = new SessionDesign();

            testSession.Description = "This is a test session.";
            testSession.IsComplete = false;
            testSession.IsReadOnly = false;
            testSession.Name = "TestSession";

            try
            {
                context.Add(testSession);

                context.SaveChanges();

                Guid sessionID = testSession.ID; 

                Assert.IsNotNull(context.SessionDesigns.First(s => s.ID==sessionID));

                context.Delete(context.SessionDesigns.First(s => s.ID == sessionID));

            }
            catch(Exception e)
            {
                Assert.Fail("Context threw exception during add or save:" + e.Message);
            }

        }

        [TestMethod]
        public void CreateSessionDesignEntityAndChildSessionDesignNode()
        {

            Guid sessionID = Guid.NewGuid();
            SessionDesign testSession = new SessionDesign();

            testSession.Description = "This is a test session.";
            testSession.IsComplete = false;
            testSession.IsReadOnly = false;
            testSession.Name = "TestSession";

            try
            {
                context.Add(testSession);

                context.SaveChanges();

                 sessionID= testSession.ID;

                Assert.IsNotNull(context.SessionDesigns.First(s => s.ID == sessionID));

            }
            catch (Exception e)
            {
                Assert.Fail("Context threw exception during add or save:" + e.Message);
            }

            SessionDesignNode testSessionDesignNode = new SessionDesignNode();

            testSessionDesignNode.SessionDesignID = sessionID;
            testSessionDesignNode.DataTypeID = context.DataTypes.First(d => d.IsCoreType==false).ID;
            // testSessionDesignNode.SourceID =  context.SourcestestSessionDesignNode.DataType.DataTypeSourceID

            context.Add(testSessionDesignNode);
            context.SaveChanges();

            context.Delete(context.SessionDesignNodes.First(sd => sd.ID == testSessionDesignNode.ID));
            context.Delete(context.SessionDesigns.First(s => s.ID == sessionID));
        }

    }
}
