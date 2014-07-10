using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Construct.MessageBrokering;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Construct.Server.Entities;
using Construct.Server.Models.Data;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Utilities.Shared;
using Construct.Server.Models.Data.MsSql;

namespace Construct.Server.Models.Tests.Data
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ModelDataTests
    {
        private static Entities.EntitiesModel context;
        private static Models.Data.Model model;
        private static ConstructSerializationAssistant assistant;
        private static List<bool> persistDelegateResults = new List<bool>();

        private static Guid testSensorTypeSourceID, testDataTypeID, testSourceID;
        private static string testItemDefinitionXml;
        private static string testItemXml;
        private static string connectionString;
        private static string defaultDatabase = "Construct3";
        private static Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("net.pipe", "Server", "localhost",Guid.Empty, 8000);

        public ModelDataTests()
        {
            testItemDefinitionXml = @"<?xml version='1.0' encoding='utf-16'?>
<SensorTypeSource Name='TestTEMPSensor' ID='38196E9E-A581-4326-B6F2-FFFFFFFFFFFF' ParentID='5C11FBBD-9E36-4BEA-A8BE-06E225250EF8' SensorHostTypeID='EDA0FF3E-108B-45D5-BF58-F362FABF2EFE' Version='1000'>
  <DataType Name='TestTEMP' ID='0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF'>
      <DataTypeProperty Name='TheInt' Type='int' ID='14F0E115-3EF3-4D59-882C-FFFFFFFFFFFF' />
      <DataTypeProperty Name='TheBool' Type='bool' ID='388A0BC9-6010-40BD-BAD0-FFFFFFFFFFFF' />
      <DataTypeProperty Name='TheString' Type='string' ID='92EBA461-1872-4504-A26E-FFFFFFFFFFFF' />
      <DataTypeProperty Name='TheBytes' Type='byte[]' ID='715E510E-D7E9-439E-BF6A-FFFFFFFFFFFF' />
      <DataTypeProperty Name='TheGuid' Type='Guid' ID='12E166B9-8DB7-4EF6-BAC7-FFFFFFFFFFFF' />
      <DataTypeProperty Name='intOne' Type='int' ID='B919F2B4-1BFC-4A2E-A9E6-FFFFFFFFFFFF' />
      <DataTypeProperty Name='doubleOne' Type='double' ID='D7535E8F-A3B3-43CA-B6D5-FFFFFFFFFFFF' />
      <DataTypeProperty Name='name' Type='string' ID='3994D895-998D-49F6-A0AB-FFFFFFFFFFFF' />
    </DataType>
</SensorTypeSource>";
            testItemXml = @"{ 'PayloadTypes' : {'TheString' : 'String', 'TheBytes' : 'Byte[]', 'TheGuid' : 'Guid', 'intOne' : 'Int32', 'doubleOne' : 'Double', 'name' : 'String', 'TheInt' : 'Int32', 'TheBool' : 'Boolean'},'Instance' : {'Payload':{'TheString':'Stop right there criminal scum!','TheBytes':'AAECAwQ=','TheGuid':'1de364aa-daa1-430d-bcdb-54239d68cfa1','TheSubClass':{'intOne':5,'doubleOne':6.15,'name':'John'},'TheInt':4,'TheBool':true},'DataName':'TestTEMP','DataTypeSourceID':'38196E9E-A581-4326-B6F2-FFFFFFFFFFFF', 'SourceID':'3E2B7612-FE27-46D3-9A6F-B9B020A2C32B', 'DataTypeID':'0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF','BrokerID':'8aa6eed9-5313-4966-bb00-e20e69f97a0f','TimeStamp':'\/Date(1345493292976-0500)\/','Latitude':0.0,'Longitude':0.0}";
        }
        [TestMethod]
        public void PersistenceEventSupportForSql()
        {
            SqlClrTableTriggerDllGenerator.Compile(@"\\Daisy\triggers\", "z_TestTemp_TheString");
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://construct1.colum.edu:8080/tfs"), new UICredentialsProvider());
            tfs.EnsureAuthenticated();
            VersionControlServer versionControl = tfs.GetService<VersionControlServer>();
            string loggedInUser = versionControl.AuthorizedUser;

            int hostPortBase = 8000;
            Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.NewGuid(), hostPortBase);
            
            Models.Server server = new Models.Server(serverServiceUri, hostPortBase, loggedInUser);

            connectionString = ConfigurationManager.ConnectionStrings[loggedInUser].ConnectionString;
            context = new Entities.EntitiesModel(connectionString);

            model = new Models.Data.Model(serverServiceUri, connectionString, server);

            Action<Guid, string, string, BooleanPropertyValue> boolPersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddBooleanPropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, ByteArrayPropertyValue> byteArrayPersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddByteArrayPropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, DateTimePropertyValue> dateTimePersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddDateTimePropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, DoublePropertyValue> doublePersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddDoublePropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, GuidPropertyValue> guidPersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddGuidPropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, IntPropertyValue> intPersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddIntPropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, SinglePropertyValue> singlePersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddSinglePropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };
            Action<Guid, string, string, StringPropertyValue> stringPersistDelegate = (guid, dataTypeName, propertyName, values) => { model.AddStringPropertyValue(guid, dataTypeName, propertyName, values); persistDelegateResults.Add(true); };

            assistant = new ConstructSerializationAssistant(boolPersistDelegate,
                                                            byteArrayPersistDelegate,
                                                            dateTimePersistDelegate,
                                                            doublePersistDelegate,
                                                            guidPersistDelegate,
                                                            intPersistDelegate,
                                                            singlePersistDelegate,
                                                            stringPersistDelegate);
            try
            {
                using (EntitiesModel entityModel = new Entities.EntitiesModel(connectionString))
                {
                    foreach (DataType dataType in entityModel.DataTypes)
                    {
                        AddPropertyIDHelper(dataType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            testSensorTypeSourceID = Guid.Parse("38196e9e-a581-4326-b6f2-ffffffffffff");
            testDataTypeID = Guid.Parse("0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF");
            testSourceID = Guid.Parse("3E2B7612-FE27-46D3-9A6F-B9B020A2C32B");
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
        public void AddTypeFromXml()
        {
            AddTypeLogic(testItemDefinitionXml);

            int postInsertCount = context.DataTypes.Where(d => d.ID == testDataTypeID).Count();
            Assert.AreEqual(1, postInsertCount);

            Clean();
        }

        [TestMethod]
        public void DeserializeItem()
        {
            AddTypeLogic(testItemDefinitionXml);
            dynamic item = DeserializeItemLogic();

            Assert.IsNotNull(item);
            //Assert member wise stuff matches up

            Clean();
        }

        [TestMethod]
        public void PersistJsonItem()
        {   
            AddTypeLogic(testItemDefinitionXml);

            try
            {
                using (EntitiesModel entityModel = new Entities.EntitiesModel(connectionString))
                {
                    foreach (DataType dataType in entityModel.DataTypes)
                    {
                        AddPropertyIDHelper(dataType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            PersistItemLogic(assistant);

            Assert.AreEqual(8, persistDelegateResults.Count);
            foreach (bool flag in persistDelegateResults)
            {
                Assert.AreEqual(true, flag);
            }
 
            Clean();
            //TODO: Clean up the item rows too, some call like "CleanItems()"?
        }

        [TestMethod]
        public void CleanDataModelTests()
        {
            Clean();
        }

       

        private dynamic DeserializeItemLogic()
        {
            Guid itemID = Guid.NewGuid();
            dynamic item = assistant.GetItem(testItemXml, itemID);
            return item;
        }

        private void PersistItemLogic(ConstructSerializationAssistant assistant)
        {
            // add source
            // add sensor
            Guid itemID = Guid.NewGuid();
            assistant.Persist(testItemXml, itemID);
        }

        private void AddTypeLogic(string xml)
        {
            model.AddType(xml);
        }

        private void AddTypeLogic(DataTypeSource source, DataType dataType, params Entities.Adapters.PropertyType[] properties)
        {
            model.AddType(source, dataType, properties);
        }

        private static void AddPropertyIDHelper(DataType dataType)
        {
            var tempDict = new Dictionary<string, Guid>();
            foreach (PropertyParent prop in dataType.PropertyParents)
            {
                string capitalizedPropName = String.Format("{0}{1}", prop.Name.Substring(0, 1).ToUpper(), prop.Name.Substring(1, prop.Name.Length - 1));
                tempDict.Add(capitalizedPropName, prop.ID);
            }
            if (tempDict.Count != 0)
            {
                assistant.AddPropertyIDTable(dataType.Name, tempDict);
            }
        }

        private static void Clean()
        {
            using (context = new Entities.EntitiesModel(connectionString))
            {
                var propertyParents = context.PropertyParents.Where(p => p.ParentDataTypeID == testDataTypeID);
                var dataType = context.DataTypes.Single(d => d.ID == testDataTypeID);
                var source = context.DataTypeSources.Single(ds => ds.ID == testSensorTypeSourceID);

                context.Delete(propertyParents);
                context.SaveChanges();

                context.Delete(dataType);
                context.SaveChanges();

                context.Delete(source);
                context.SaveChanges();
            }

            //We cant delete the dynamic dll we want to while the test is running...

            //string executingAssemblyFilePath = Assembly.GetExecutingAssembly().Location;
            //string executingDir = Path.GetDirectoryName(executingAssemblyFilePath);
            //string filename = Path.Combine(executingDir, String.Format("Construct.Types.{0}.dll", sensorTypeName));

            //if (File.Exists(filename))
            //{
            //    File.Delete(filename);
            //}
            //else
            //{
            //    Assert.Fail(String.Format("File {0} does not exist", filename));
            //}
        }

        
    }
}
