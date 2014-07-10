using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Server.Entities.Tests;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Construct.Server.Entities.Adapters;
using Construct.Utilities.Shared;
using System.Configuration;

namespace Construct.Server.Models.Tests.Data
{
    /// <summary>
    /// Underscores follow by a number represent sequence
    /// "S" followed by a number represents steps
    /// </summary>
    [TestClass]
    public class DataModelTests
    {
        protected Construct.Server.Entities.Tests.EntityTests entityTests = new Entities.Tests.EntityTests();
        protected Construct.Utilities.Tests.Shared.SharedUtilitiesTests utilitiesTests = new Utilities.Tests.Shared.SharedUtilitiesTests();

        public Construct.Server.Models.Data.Model model;
        public static Construct.Server.Models.Server server;
        public static string connectionString;
        private static Entities.EntitiesModel context;

        public Guid testSensorTypeSourceID;
        public Guid testDataTypeID;
        public Guid testSourceID;

        public DataModelTests() : base()
        {
            testSensorTypeSourceID = Guid.Parse("38196e9e-a581-4326-b6f2-ffffffffffff");
            testDataTypeID = Guid.Parse("0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF");
        }

        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://construct1.colum.edu:8080/tfs"), new UICredentialsProvider());
            tfs.EnsureAuthenticated();
            VersionControlServer versionControl = tfs.GetService<VersionControlServer>();

            string loggedInUser = versionControl.AuthorizedUser;
            EntityTests.connectionStringName = loggedInUser;

            int hostPortBase = 8000;
            Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.NewGuid(), hostPortBase);
            server = new Server(serverServiceUri, hostPortBase, EntityTests.connectionStringName);

            connectionString = ConfigurationManager.ConnectionStrings[loggedInUser].ConnectionString;
            context = new Entities.EntitiesModel(connectionString);
        }

        private void CreateDataModel()
        {
            try
            {
                model = new Models.Data.Model(utilitiesTests.serverEndpoint, entityTests.connectionString, server);
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
        public void _1_1_S1_CreateDataModel()
        {
            _1_1_S1_CreateDataModelLogic();
        }
  
        public void _1_1_S1_CreateDataModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelStateLogic();
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            CreateDataModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy
        /// </summary>
        [TestMethod]
        public void _1_2_S1_CreateDataModel()
        {
            _1_2_S1_CreateDataModelLogic();
        }
  
        private void _1_2_S1_CreateDataModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelStateLogic();
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            CreateDataModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _1_3_S1_CreateDataModel()
        {
            _1_3_S1_CreateDataModelLogic();
        }
  
        private void _1_3_S1_CreateDataModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelStateLogic();
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            CreateDataModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _2_1_S1_CreateDataModel()
        {
            _2_1_S1_CreateDataModelLogic();
        }
  
        public void _2_1_S1_CreateDataModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelStateLogic();
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            CreateDataModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _2_2_S1_CreateDataModel()
        {
            _2_2_S1_CreateDataModelLogic();
        }
  
        private void _2_2_S1_CreateDataModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelStateLogic();
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            CreateDataModel();
        }

        /// <summary>
        /// <br />Scheme: net.pipe
        /// <br />Hostname: daisy.colum.edu
        /// </summary>
        [TestMethod]
        public void _2_3_S1_CreateDataModel()
        {
            _2_3_S1_CreateDataModelLogic();
        }
  
        private void _2_3_S1_CreateDataModelLogic()
        {
            entityTests._1_S3_ValidateEntityModelStateLogic();
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            CreateDataModel();
        }

        private void CheckForNonCoreTypeDllInExecutingDirectory()
        {
            string assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(assemblyFilePath));

            IEnumerable<Entities.DataType> dataTypes = entityTests.context.DataTypes.Where(d => d.IsCoreType == false);
            foreach (Entities.DataType dataType in dataTypes)
            {
                string fileName = String.Format("Construct.Types.{0}.dll", dataType.Name);
                string filepath = Path.Combine(dirInfo.FullName, fileName);

                if (File.Exists(filepath) == false)
                {
                    Assert.Fail(String.Format("{0} was not found", filepath));
                }
            }
        }
        [TestMethod]
        public void _1_1_S2_CheckForNonCoreTypeDllInExecutingDirectory()
        {
            _1_1_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
        }
  
        private void _1_1_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic()
        {
            _1_1_S1_CreateDataModelLogic();
            CheckForNonCoreTypeDllInExecutingDirectory();
        }

        [TestMethod]
        public void _1_2_S2_CheckForNonCoreTypeDllInExecutingDirectory()
        {
            _1_2_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
        }
  
        private void _1_2_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic()
        {
            _1_2_S1_CreateDataModelLogic();
            CheckForNonCoreTypeDllInExecutingDirectory();
        }

        [TestMethod]
        public void _1_3_S2_CheckForNonCoreTypeDllInExecutingDirectory()
        {
            _1_3_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
        }
  
        private void _1_3_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic()
        {
            _1_3_S1_CreateDataModelLogic();
            CheckForNonCoreTypeDllInExecutingDirectory();
        }

        [TestMethod]
        public void _2_1_S2_CheckForNonCoreTypeDllInExecutingDirectory()
        {
            _2_1_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
        }
  
        private void _2_1_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic()
        {
            _2_1_S1_CreateDataModelLogic();
            CheckForNonCoreTypeDllInExecutingDirectory();
        }

        [TestMethod]
        public void _2_2_S2_CheckForNonCoreTypeDllInExecutingDirectory()
        {
            _2_2_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
        }
  
        private void _2_2_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic()
        {
            _2_2_S1_CreateDataModelLogic();
            CheckForNonCoreTypeDllInExecutingDirectory();
        }

        [TestMethod]
        public void _2_3_S2_CheckForNonCoreTypeDllInExecutingDirectory()
        {
            _2_3_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
        }
  
        private void _2_3_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic()
        {
            _2_3_S1_CreateDataModelLogic();
            CheckForNonCoreTypeDllInExecutingDirectory();
        }

        private void VerifyAssemblyIsEnhancedByVEnhance()
        {
            string assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(assemblyFilePath));

            IEnumerable<Entities.DataType> dataTypes = entityTests.context.DataTypes.Where(d => d.IsCoreType == false);
            foreach (Entities.DataType dataType in dataTypes)
            {
                string className = String.Format("Construct.Types.{0}", dataType.Name);
                string fileName = String.Format("{0}.dll", className);
                string filepath = Path.Combine(dirInfo.FullName, fileName);

                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(filepath);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                object instance = null;
                try
                {
                    instance = assembly.CreateInstance(className);
                    Assert.IsNotNull(instance);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                string interfaceName = "PersistenceCapable";
                Type instanceType = instance.GetType();
                Assert.AreEqual(interfaceName, instanceType.GetInterface(interfaceName).Name);
            }
        }
        [TestMethod]
        public void _1_1_S3_VerifyAssemblyIsEnhancedByVEnhance()
        {
            _1_1_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
        }
  
        private void _1_1_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic()
        {
            _1_1_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
            VerifyAssemblyIsEnhancedByVEnhance();
        }

        [TestMethod]
        public void _1_2_S3_VerifyAssemblyIsEnhancedByVEnhance()
        {
            _1_2_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
        }
  
        private void _1_2_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic()
        {
            _1_2_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
            VerifyAssemblyIsEnhancedByVEnhance();
        }

        [TestMethod]
        public void _1_3_S3_VerifyAssemblyIsEnhancedByVEnhance()
        {
            _1_3_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
        }
  
        private void _1_3_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic()
        {
            _1_3_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
            VerifyAssemblyIsEnhancedByVEnhance();
        }

        [TestMethod]
        public void _2_1_S3_VerifyAssemblyIsEnhancedByVEnhance()
        {
            _2_1_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
        }
  
        private void _2_1_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic()
        {
            _2_1_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
            VerifyAssemblyIsEnhancedByVEnhance();
        }

        [TestMethod]
        public void _2_2_S3_VerifyAssemblyIsEnhancedByVEnhance()
        {
            _2_2_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
        }
  
        private void _2_2_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic()
        {
            _2_2_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
            VerifyAssemblyIsEnhancedByVEnhance();
        }

        [TestMethod]
        public void _2_3_S3_VerifyAssemblyIsEnhancedByVEnhance()
        {
            _2_3_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
        }
  
        private void _2_3_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic()
        {
            _2_3_S2_CheckForNonCoreTypeDllInExecutingDirectoryLogic();
            VerifyAssemblyIsEnhancedByVEnhance();
        }

        public DataType dataType = null;
        public DataTypeSource dataTypeSource = null;
        public PropertyType theIntProperty = null;
        public PropertyType theBoolProperty = null;
        public PropertyType theStringProperty = null;
        public PropertyType theBytesProperty = null;
        public PropertyType theGuidProperty = null;
        public PropertyType theIntOneProperty = null;
        public PropertyType theDoubleOneProperty = null;
        public PropertyType theNameProperty = null;
        public PropertyType theSingleProperty = null;

        [TestMethod]
        public void _1_1_S4_InsertTestItemDataType()
        {
            _1_1_S4_InsertTestItemDataTypeLogic();
            Clean();
        }
  
        public void _1_1_S4_InsertTestItemDataTypeLogic()
        {
            int postInsertCount = 0;
            int preInsertCount = 0;

            try
            {
                _1_1_S3_VerifyAssemblyIsEnhancedByVEnhanceLogic();
                IEnumerable<Entities.Adapters.DataType> dataTypes = model.GetAllTypes();

                dataTypeSource = new DataTypeSource();
                dataTypeSource.ID = Guid.Parse("38196E9E-A581-4326-B6F2-FFFFFFFFFFFF");
                dataTypeSource.Name = "TestTEMPSensor";
                dataTypeSource.ParentID = Guid.Parse("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8");

                dataType = new DataType();
                dataType.Name = "TestTEMP";
                dataType.FullName = "TestTEMP";
                dataType.ID = Guid.Parse("0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF");
                dataType.DataTypeSourceID = Guid.Parse("38196E9E-A581-4326-B6F2-FFFFFFFFFFFF");

                preInsertCount = model.GetAllTypes().Where(d => d.ID == dataType.ID).Count();

                theIntProperty = new PropertyType();
                theIntProperty.Name = "TheInt";
                theIntProperty.ID = Guid.Parse("14F0E115-3EF3-4D59-882C-FFFFFFFFFFFF");
                theIntProperty.ParentDataTypeID = dataType.ID;
                theIntProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "int").ID;

                theBoolProperty = new PropertyType();
                theBoolProperty.Name = "TheBool";
                theBoolProperty.ID = Guid.Parse("388A0BC9-6010-40BD-BAD0-FFFFFFFFFFFF");
                theBoolProperty.ParentDataTypeID = dataType.ID;
                theBoolProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "bool").ID;

                theStringProperty = new PropertyType();
                theStringProperty.Name = "TheString";
                theStringProperty.ID = Guid.Parse("92EBA461-1872-4504-A26E-FFFFFFFFFFFF");
                theStringProperty.ParentDataTypeID = dataType.ID;
                theStringProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "string").ID;

                theBytesProperty = new PropertyType();
                theBytesProperty.Name = "TheBytes";
                theBytesProperty.ID = Guid.Parse("715E510E-D7E9-439E-BF6A-FFFFFFFFFFFF");
                theBytesProperty.ParentDataTypeID = dataType.ID;
                theBytesProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "byte[]").ID;

                theGuidProperty = new PropertyType();
                theGuidProperty.Name = "TheGuid";
                theGuidProperty.ID = Guid.Parse("12E166B9-8DB7-4EF6-BAC7-FFFFFFFFFFFF");
                theGuidProperty.ParentDataTypeID = dataType.ID;
                theGuidProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "Guid").ID;

                theIntOneProperty = new PropertyType();
                theIntOneProperty.Name = "intOne";
                theIntOneProperty.ID = Guid.Parse("B919F2B4-1BFC-4A2E-A9E6-FFFFFFFFFFFF");
                theIntOneProperty.ParentDataTypeID = dataType.ID;
                theIntOneProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "int").ID;

                theDoubleOneProperty = new PropertyType();
                theDoubleOneProperty.Name = "doubleOne";
                theDoubleOneProperty.ID = Guid.Parse("D7535E8F-A3B3-43CA-B6D5-FFFFFFFFFFFF");
                theDoubleOneProperty.ParentDataTypeID = dataType.ID;
                theDoubleOneProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "double").ID;

                theNameProperty = new PropertyType();
                theNameProperty.Name = "name";
                theNameProperty.ID = Guid.Parse("3994D895-998D-49F6-A0AB-FFFFFFFFFFFF");
                theNameProperty.ParentDataTypeID = dataType.ID;
                theNameProperty.PropertyDataTypeID = dataTypes.First(type => type.Name == "string").ID;

                model.AddType(dataTypeSource, dataType, new PropertyType[8]{
                                                                            theIntProperty,
                                                                            theBoolProperty,
                                                                            theStringProperty,
                                                                            theBytesProperty,
                                                                            theGuidProperty,
                                                                            theIntOneProperty,
                                                                            theDoubleOneProperty,
                                                                            theNameProperty}
                             );

                postInsertCount = model.GetAllTypes().Where(d => d.ID == dataType.ID).Count();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.AreEqual(1, postInsertCount);
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
    }
}
