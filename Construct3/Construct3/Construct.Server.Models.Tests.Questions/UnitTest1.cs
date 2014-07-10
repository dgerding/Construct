using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Construct.Server.Entities.Tests;
using Construct.Utilities.Shared;

namespace Construct.Server.Models.Tests.Questions
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
            broker = new MessageBrokering.Broker();
        }

        private TestContext testContextInstance;
        protected Construct.Server.Entities.Tests.EntityTests entityTests = new Entities.Tests.EntityTests();
        protected Construct.Utilities.Tests.Shared.SharedUtilitiesTests utilitiesTests = new Utilities.Tests.Shared.SharedUtilitiesTests();
        public MessageBrokering.Broker broker;
        public Construct.Server.Models.Questions.Model model;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://construct1.colum.edu:8080/tfs"), new UICredentialsProvider());
            tfs.EnsureAuthenticated();
            VersionControlServer versionControl = tfs.GetService<VersionControlServer>();

            string loggedInUser = versionControl.AuthorizedUser;
            EntityTests.connectionStringName = loggedInUser;
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

        private void CreateQuestionsModel()
        {
            try
            {
                int hostPortBase = 8000;
                Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.NewGuid(), hostPortBase);
                Models.Server server = new Models.Server(serverServiceUri, hostPortBase, EntityTests.connectionStringName);
                model = new Models.Questions.Model(utilitiesTests.serverEndpoint, entityTests.connectionString, server);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: localhost
        /// </summary>
        [TestMethod]
        public void _1_1_S1_CreateQuestionsModel()
        {
            _1_1_S1_CreateQuestionsModelLogic();
        }
  
        private void _1_1_S1_CreateQuestionsModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelState();
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            CreateQuestionsModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy
        /// </summary>
        [TestMethod]
        public void _1_2_S1_CreateQuestionsModel()
        {
            _1_2_S1_CreateQuestionsModelLogic();
        }
  
        private void _1_2_S1_CreateQuestionsModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelState();
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            CreateQuestionsModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _1_3_S1_CreateQuestionsModel()
        {
            _1_3_S1_CreateQuestionsModelLogic();
        }
  
        private void _1_3_S1_CreateQuestionsModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelState();
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            CreateQuestionsModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _2_1_S1_CreateQuestionsModel()
        {
            _2_1_S1_CreateQuestionsModelLogic();
        }
  
        private void _2_1_S1_CreateQuestionsModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelState();
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            CreateQuestionsModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _2_2_S1_CreateQuestionsModel()
        {
            _2_2_S1_CreateQuestionsModelLogic();
        }
  
        private void _2_2_S1_CreateQuestionsModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelState();
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            CreateQuestionsModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _2_3_S1_CreateQuestionsModel()
        {
            _2_3_S1_CreateQuestionsModelLogic();
        }
  
        private void _2_3_S1_CreateQuestionsModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelState();
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            CreateQuestionsModel();
        }

    }
}