using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Data;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Construct.Server.Entities.Tests
{
    /// <summary>
    /// Underscores follow by a number represent sequence
    /// "S" followed by a number represents steps
    /// </summary>
    [TestClass]
    public class EntityTests
    {
        public EntitiesModel context = null;
        public static string connectionStringName = null;
        public string connectionString = null;
        public Guid testSensorTypeSourceID;
        public Guid testDataTypeID;

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

        public EntityTests()
        {
            testSensorTypeSourceID = Guid.Parse("38196e9e-a581-4326-b6f2-ffffffffffff");
            testDataTypeID = Guid.Parse("0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF");
        }

        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            ExtractTFSLoginConnStrName();
        }

        private static void ExtractTFSLoginConnStrName()
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://construct1.colum.edu:8080/tfs"), new UICredentialsProvider());
            tfs.EnsureAuthenticated();
            VersionControlServer versionControl = tfs.GetService<VersionControlServer>();

            string loggedInUser = versionControl.AuthorizedUser;
            connectionStringName = loggedInUser;
        }

        [TestMethod]
        public void CreateSessionEntitiesTest()
        {
            _1_S2_InitializeEntityModelLogic();

            int numPreInsertSessions = context.Sessions.Count();
            int numPreInsertSessionSources = context.SessionSources.Count();

            AddTestTempDataType();

            Session tempSession = new Session();
            Source tempSource = new Source();
            SessionSource tempSessionSource = new SessionSource();

            Guid tempSessionID = Guid.NewGuid();
            Guid tempSourceID = Guid.NewGuid();
            Guid tempSessionSourceID = Guid.NewGuid();
            
            tempSession.ID = tempSessionID;
            tempSession.StartTime = DateTime.Now;
            tempSession.Interval = 10000000;
            tempSession.FriendlyName = "tempName";

            if (context.Sessions.Where(sess => sess.ID == tempSession.ID).Count() == 0)
            {
                context.Add(tempSession);
                context.SaveChanges();
            }
            
            tempSource.ID = tempSourceID;
            tempSource.DataTypeSourceID = testSensorTypeSourceID;

            if (context.Sources.Where(source => source.ID == tempSource.ID).Count() == 0)
            {
                context.Add(tempSource);
                context.SaveChanges();
            }

            tempSessionSource.ID = tempSessionSourceID;
            tempSessionSource.SessionID = tempSessionID;
            tempSessionSource.SourceID = tempSourceID;

            if (context.SessionSources.Where(sessSource => sessSource.ID == tempSessionSource.ID).Count() == 0)
            {
                context.Add(tempSessionSource);
                context.SaveChanges();
            }

            int numPostInsertSessions = context.Sessions.Count();
            int numPostInsertSessionSources = context.SessionSources.Count();

            Assert.AreEqual(1, numPostInsertSessions);
            Assert.AreEqual(1, numPostInsertSessionSources);

            context.Delete(tempSessionSource);
            context.SaveChanges();

            context.Delete(tempSession);
            context.Delete(tempSource);
            context.SaveChanges();

            Clean();
        }

        private void AddTestTempDataType()
        {
            DataTypeSource dataTypeSource = new DataTypeSource();
            dataTypeSource.ID = testSensorTypeSourceID;
            dataTypeSource.Name = "TestTEMPSensor";
            dataTypeSource.ParentID = Guid.Parse("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8");

            if (context.DataTypeSources.Where(dts => dts.ID == dataTypeSource.ID).Count() == 0)
            {
                context.Add(dataTypeSource);
                context.SaveChanges();
            }

            DataType dataType = new DataType();
            dataType.Name = "TestTEMP";
            dataType.FullName = "TestTEMP";
            dataType.ID = testDataTypeID;
            dataType.DataTypeSourceID = testSensorTypeSourceID;

            if (context.DataTypes.Where(datatype => datatype.ID == dataType.ID).Count() == 0)
            {
                context.Add(dataType);
                context.SaveChanges();
            }

            List<Entities.DataType> dataTypes = context.DataTypes.ToList();
            List<PropertyType> properties = new List<PropertyType>();

            PropertyType itemID = new PropertyType();
            itemID.ID = Guid.NewGuid();
            itemID.Name = "ItemID";
            itemID.ParentDataTypeID = dataType.ID;
            itemID.PropertyDataTypeID = dataTypes.First(type => type.Name == "Guid").ID;
            properties.Add(itemID);

            PropertyType recordCreationDate = new PropertyType();
            recordCreationDate.ID = Guid.NewGuid();
            recordCreationDate.Name = "RecordCreationDate";
            recordCreationDate.ParentDataTypeID = dataType.ID;
            recordCreationDate.PropertyDataTypeID = dataTypes.First(type => type.Name == "DateTime").ID;
            properties.Add(recordCreationDate);

            PropertyType theIntProperty = new PropertyType();
            theIntProperty.Name = "TheInt";
            theIntProperty.ID = Guid.Parse("14F0E115-3EF3-4D59-882C-FFFFFFFFFFFF");
            theIntProperty.ParentDataTypeID = dataType.ID;
            theIntProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "int").ID;
            properties.Add(theIntProperty);

            PropertyType theBoolProperty = new PropertyType();
            theBoolProperty.Name = "TheBool";
            theBoolProperty.ID = Guid.Parse("388A0BC9-6010-40BD-BAD0-FFFFFFFFFFFF");
            theBoolProperty.ParentDataTypeID = dataType.ID;
            theBoolProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "bool").ID;
            properties.Add(theBoolProperty);

            PropertyType theStringProperty = new PropertyType();
            theStringProperty.Name = "TheString";
            theStringProperty.ID = Guid.Parse("92EBA461-1872-4504-A26E-FFFFFFFFFFFF");
            theStringProperty.ParentDataTypeID = dataType.ID;
            theStringProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "string").ID;
            properties.Add(theStringProperty);

            PropertyType theBytesProperty = new PropertyType();
            theBytesProperty.Name = "TheBytes";
            theBytesProperty.ID = Guid.Parse("715E510E-D7E9-439E-BF6A-FFFFFFFFFFFF");
            theBytesProperty.ParentDataTypeID = dataType.ID;
            theBytesProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "byte[]").ID;
            properties.Add(theBytesProperty);

            PropertyType theGuidProperty = new PropertyType();
            theGuidProperty.Name = "TheGuid";
            theGuidProperty.ID = Guid.Parse("12E166B9-8DB7-4EF6-BAC7-FFFFFFFFFFFF");
            theGuidProperty.ParentDataTypeID = dataType.ID;
            theGuidProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "Guid").ID;
            properties.Add(theGuidProperty);

            PropertyType theIntOneProperty = new PropertyType();
            theIntOneProperty.Name = "intOne";
            theIntOneProperty.ID = Guid.Parse("B919F2B4-1BFC-4A2E-A9E6-FFFFFFFFFFFF");
            theIntOneProperty.ParentDataTypeID = dataType.ID;
            theIntOneProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "int").ID;
            properties.Add(theIntOneProperty);

            PropertyType theDoubleOneProperty = new PropertyType();
            theDoubleOneProperty.Name = "doubleOne";
            theDoubleOneProperty.ID = Guid.Parse("D7535E8F-A3B3-43CA-B6D5-FFFFFFFFFFFF");
            theDoubleOneProperty.ParentDataTypeID = dataType.ID;
            theDoubleOneProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "double").ID;
            properties.Add(itemID);

            PropertyType theNameProperty = new PropertyType();
            theNameProperty.Name = "name";
            theNameProperty.ID = Guid.Parse("3994D895-998D-49F6-A0AB-FFFFFFFFFFFF");
            theNameProperty.ParentDataTypeID = dataType.ID;
            theNameProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "string").ID;
            properties.Add(theNameProperty);

            foreach (PropertyType tempProp in properties)
            {
                if (context.PropertyTypes.Where(propType => propType.ID == tempProp.ID).Count() == 0)
                {
                    context.Add(tempProp);
                }
                context.SaveChanges();
            }
        }

        private void Clean()
        {
            using (context = new Entities.EntitiesModel(connectionString))
            {
                var propertyParents = context.PropertyParents.Where(p => p.ParentDataTypeID == testDataTypeID);

                context.Delete(propertyParents);
                context.SaveChanges();

                context.Delete(context.DataTypes.Where(d => d.ID == testDataTypeID));
                context.SaveChanges();
                context.Delete(context.DataTypeSources.Where(ds => ds.ID == testSensorTypeSourceID));
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void _1_S1_CheckForAppConfigFile()
        {
            _1_S1_CheckForAppConfigFileLogic();
        }
  
        private void _1_S1_CheckForAppConfigFileLogic()
        {
            if (connectionStringName == null)
            {
                ExtractTFSLoginConnStrName();
            }

            ConnectionStringSettingsCollection connectionStrings = null;
            try
            {
                connectionStrings = ConfigurationManager.ConnectionStrings;
            }
            catch (FileNotFoundException exception)
            {
                Assert.Fail(exception.Message);
            }

            ConnectionStringSettings connectionStringSetting = null;
            connectionStringSetting = connectionStrings[connectionStringName];

            Assert.IsNotNull(connectionStringSetting.ConnectionString);

            string actualConnectionString = connectionStringSetting.ConnectionString;
            connectionString = actualConnectionString;
        }

        [TestMethod]
        public void _1_S2_InitializeEntityModel()
        {
            _1_S2_InitializeEntityModelLogic();
        }
  
        private void _1_S2_InitializeEntityModelLogic()
        {
            try
            {
                _1_S1_CheckForAppConfigFile();
                context = new EntitiesModel(connectionString);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            Assert.IsNotNull(context.Connection);
        }

        [TestMethod]
        public void _1_S3_ValidateEntityModelState()
        {
            _1_S3_ValidateEntityModelStateLogic();
        }
  
        public void _1_S3_ValidateEntityModelStateLogic()
        {
            _1_S2_InitializeEntityModel();
            Assert.AreEqual(ConnectionState.Open, context.Connection.State);
        }
    }
}