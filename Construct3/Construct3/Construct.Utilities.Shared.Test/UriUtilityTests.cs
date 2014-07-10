using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Construct.Utilities.Shared.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UriUtilityTests
    {
        private static Uri testHttpUri;
        private static Uri testHttpsUri;
        private static Uri testNetTcpUri;
        private static Uri testNetPipeUri;

        public UriUtilityTests()
        {
            testHttpUri = new Uri("http://daisy.colum.edu:8000/CCD83DFB-5152-43E3-B823-427516385F38/Server/ServerService.svc");
            testHttpsUri = new Uri("https://daisy.colum.edu:8000/CCD83DFB-5152-43E3-B823-427516385F38/Server/ServerService.svc");
            testNetTcpUri = new Uri("net.tcp://daisy.colum.edu:8000/CCD83DFB-5152-43E3-B823-427516385F38/Server/ServerService.svc");
            testNetPipeUri = new Uri("net.pipe://localhost/CCD83DFB-5152-43E3-B823-427516385F38/Server/ServerService.svc");
        }

        private TestContext testContextInstance;
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
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
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
        public void CreateModelUriFromServerUriTest()
        {
            Uri httpUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "sources");
            Uri httpsUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpsUri, "sources");
            Uri NetTcpUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testNetTcpUri, "sources");
            Uri NetPipeUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testNetPipeUri, "sources");

            Uri httpMeaningUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "meaning");
            Uri httpQuestionsUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "questions");
            Uri httpVisualizationsUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "visualizations");
            Uri httpSessionsUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "sessions");
            Uri httpLearningUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "learning");
            Uri httpDataUri = UriUtility.CreateModelServiceEndpointFromServerEndpoint(testHttpUri, "data");

            Assert.AreEqual("http://daisy.colum.edu:9000/ccd83dfb-5152-43e3-b823-427516385f38/sources/sourcesService.svc", httpUri.AbsoluteUri);
            Assert.AreEqual("https://daisy.colum.edu:9000/ccd83dfb-5152-43e3-b823-427516385f38/sources/sourcesService.svc", httpsUri.AbsoluteUri);
            Assert.AreEqual("net.tcp://daisy.colum.edu:9000/ccd83dfb-5152-43e3-b823-427516385f38/sources/sourcesService.svc", NetTcpUri.AbsoluteUri);
            Assert.AreEqual("net.pipe://localhost/ccd83dfb-5152-43e3-b823-427516385f38/sources/sourcesService.svc", NetPipeUri.AbsoluteUri);

            Assert.AreEqual("http://daisy.colum.edu:10000/ccd83dfb-5152-43e3-b823-427516385f38/meaning/meaningService.svc", httpMeaningUri.AbsoluteUri);
            Assert.AreEqual("http://daisy.colum.edu:11000/ccd83dfb-5152-43e3-b823-427516385f38/questions/questionsService.svc", httpQuestionsUri.AbsoluteUri);
            Assert.AreEqual("http://daisy.colum.edu:12000/ccd83dfb-5152-43e3-b823-427516385f38/visualizations/visualizationsService.svc", httpVisualizationsUri.AbsoluteUri);
            Assert.AreEqual("http://daisy.colum.edu:13000/ccd83dfb-5152-43e3-b823-427516385f38/sessions/sessionsService.svc", httpSessionsUri.AbsoluteUri);
            Assert.AreEqual("http://daisy.colum.edu:14000/ccd83dfb-5152-43e3-b823-427516385f38/learning/learningService.svc", httpLearningUri.AbsoluteUri);
            Assert.AreEqual("http://daisy.colum.edu:15000/ccd83dfb-5152-43e3-b823-427516385f38/data/dataService.svc", httpDataUri.AbsoluteUri);
        }

        [TestMethod]
        public void CreatePropertyValueServiceEndpointFromServerEndpoint()
        {

            Uri serverUri = new Uri("http://localhost/00000000-0000-0000-0000-000000000000/Server/ServerService.svc");

            string dataTypeName = "TestTEMP";
            string propertyName = "TheInt";
            int port = 15001;

            Uri test = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, "ItemTEMP", "TheInt", 15001);

            Assert.AreEqual(new UriBuilder("http://localhost/00000000-0000-0000-0000-000000000000/TestTEMP/TheInt/PropertyValueService.svc"), test);



        }
    }


}
