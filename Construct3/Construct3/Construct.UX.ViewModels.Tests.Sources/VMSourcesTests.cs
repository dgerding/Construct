using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.MessageBrokering;
using Construct.Server.Models.Sources;
using Construct.UX.ViewModels.Sources.SourcesServiceReference;
using System.ServiceModel;
using System.Threading;
using Construct.Utilities.Shared;

namespace Construct.UX.ViewModels.Tests.Sources
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class VMSourcesTests
    {
        private static Construct.UX.ViewModels.Sources.SourcesServiceReference.ModelClient modelClient;
        private static string connectionString;
        private static Construct.Server.Entities.EntitiesModel context;
        private static InstanceContext instanceContext;

        private static Guid testDataTypeSourceID, testDataTypeID;
        private static string testHostName;
        private static string defaultDatabase = "Construct3";
        private static string testTempSensorDefinitionXML = @"<?xml version='1.0' encoding='utf-16'?>
<SensorTypeSource Name='TestTEMPSensor' ID='38196E9E-A581-4326-B6F2-FFFFFFFFFFFF' ParentID='5C11FBBD-9E36-4BEA-A8BE-06E225250EF8' SensorHostTypeID='EDA0FF3E-108B-45D5-BF58-F362FABF2EFE' Version= '1000'>
	<DataType Name='TestTEMP' ID='0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF' >
		<DataTypeProperty Name='TheInt' Type='int' ID='14F0E115-3EF3-4D59-882C-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='TheBool' Type='bool' ID='388A0BC9-6010-40BD-BAD0-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='TheString' Type='string' ID='92EBA461-1872-4504-A26E-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='TheBytes' Type='byte[]' ID='715E510E-D7E9-439E-BF6A-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='TheGuid' Type='Guid' ID='12E166B9-8DB7-4EF6-BAC7-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='intOne' Type='int' ID='B919F2B4-1BFC-4A2E-A9E6-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='doubleOne' Type='double' ID='D7535E8F-A3B3-43CA-B6D5-FFFFFFFFFFFF'/>
		<DataTypeProperty Name='name' Type='string' ID='3994D895-998D-49F6-A0AB-FFFFFFFFFFFF'/>
	</DataType>
</SensorTypeSource>";

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
            connectionString = String.Format("data source=daisy.colum.edu;initial catalog={0};User ID=Construct;Password=ConstructifyConstructifyConstructify", defaultDatabase);

            instanceContext = new InstanceContext(new ViewModels.Sources.CallbackImplementation());
            modelClient = new Construct.UX.ViewModels.Sources.SourcesServiceReference.ModelClient(instanceContext);

            context = new Construct.Server.Entities.EntitiesModel(connectionString);

            testDataTypeSourceID = Guid.Parse("38196e9e-a581-4326-b6f2-ffffffffffff");
            testDataTypeID = Guid.Parse("0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF");
            testHostName = "TempHost";
        }
        
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{ 
        //}
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
        public void InstallSensorRuntimeTest()
        {
            Guid sensorRuntimeID = InstallSensorRuntimeLogic();

            int postInsertCount = context.SensorRuntimes.Where(r => r.SensorTypeSource.ID == testDataTypeSourceID).Count();
            Assert.AreEqual(1, postInsertCount);

            context.Delete(context.SensorRuntimes.Where(r => r.SensorTypeSource.ID == testDataTypeSourceID));
            context.SaveChanges();

            var propertyParents = context.PropertyParents.Where(p => p.ParentDataTypeID == testDataTypeID);
            context.Delete(propertyParents);
            context.SaveChanges();

            context.Delete(context.DataTypes.Where(d => d.ID == testDataTypeID));
            context.SaveChanges();

            context.Delete(context.DataTypeSources.Where(ds => ds.ID == testDataTypeSourceID));
            context.SaveChanges();
        }

        [TestMethod]
        public void InstallSensorHostTest()
        {
            Guid sensorHostID = InstallSensorHostLogic();

            int postInsertCount = context.SensorHosts.Where(r => r.HostName == testHostName).Count();
            Assert.AreEqual(1, postInsertCount);

            context.Delete(context.SensorHosts.Where(r => r.HostName == testHostName));
            context.SaveChanges();
        }

        [TestMethod]
        public void InstallSensorTest()
        {
            Guid runtimeID = InstallSensorRuntimeLogic();
            Guid sensorHostID = InstallSensorHostLogic();
            Guid sensorID = InstallSensorLogic(sensorHostID);

            int postInsertSensorCount = context.Sensors.Where(s => s.SensorTypeSourceID == testDataTypeSourceID).Count();
            int postInsertSourcesCount = context.Sources.Where(s => s.DataTypeSourceID == testDataTypeSourceID).Count();

            Assert.AreEqual(1, postInsertSensorCount);
            Assert.AreEqual(1, postInsertSourcesCount);

            context.Delete(context.Sensors.Where(s => s.SensorTypeSourceID == testDataTypeSourceID));
            context.Delete(context.Sources.Where(s => s.DataTypeSourceID == testDataTypeSourceID));
            context.SaveChanges();

            var propertyParents = context.PropertyParents.Where(p => p.ParentDataTypeID == testDataTypeID);

            context.Delete(propertyParents);
            context.SaveChanges();

            context.Delete(context.SensorRuntimes.Where(r => r.SensorTypeSource.ID == testDataTypeSourceID));
            context.Delete(context.DataTypes.Where(d => d.ID == testDataTypeID));
            context.SaveChanges();

            context.Delete(context.DataTypeSources.Where(ds => ds.ID == testDataTypeSourceID));
            context.Delete(context.SensorHosts.Where(r => r.HostName == testHostName));
            context.SaveChanges();
        }

        private Guid InstallSensorRuntimeLogic()
        {
            SensorRuntime sensorRuntime = new SensorRuntime();

            Guid runtimeID = Guid.NewGuid();
            sensorRuntime.ID = runtimeID;
            sensorRuntime.InstallerUri = @"C:\Construct\Sensors\TestSensor.38196e9e-a581-4326-b6f2-c4120f89d4cc.1000.zip";
            sensorRuntime.CacheUri = "Not Implemented";
            sensorRuntime.InstallerXml = testTempSensorDefinitionXML;
            sensorRuntime.InstallerZip = new byte[1] { 1 };
            sensorRuntime.RecCreationDate = DateTime.Now;
            sensorRuntime.SensorTypeSourceID = Guid.Parse("38196e9e-a581-4326-b6f2-ffffffffffff");

            modelClient.AddSensorDefinition(sensorRuntime);
            return runtimeID;
        }

        private Guid InstallSensorHostLogic()
        {
            SensorHost sensorHost = new SensorHost();

            Guid sensorHostID = Guid.NewGuid();
            sensorHost.ID = sensorHostID;
            sensorHost.SensorHostTypeID = Guid.Parse("EDA0FF3E-108B-45D5-BF58-F362FABF2EFE");
            sensorHost.HostName = "TempHost";
            sensorHost.HostUri = String.Format(@"{0}://{1}/{2}/{3}", "http", sensorHost.HostName,"F721F879-9F84-412F-AE00-632CFEA5A1F3", sensorHostID.ToString());
            sensorHost.IsHealthy = true;

            modelClient.AddSensorHost(sensorHost);
            return sensorHostID;
        }

        private Guid InstallSensorLogic(Guid sensorHostID)
        {
            Sensor sensor = new Sensor();
            
            Guid sensorID = Guid.NewGuid();
            sensor.ID = sensorID;
            sensor.SensorHostID = sensorHostID;
            sensor.IsHealthy = true;
            sensor.InstalledFromServerDate = DateTime.Now;
            sensor.CurrentRendezvous = "tempRendezvous";
            sensor.SensorTypeSourceID = testDataTypeSourceID;
            sensor.DataTypeSourceID = testDataTypeSourceID;

            Construct.UX.ViewModels.Sources.SourcesServiceReference.AddSensorArgs args = new Construct.UX.ViewModels.Sources.SourcesServiceReference.AddSensorArgs();
            args.DownloadUri = "temp_Download_Uri";
            args.ZippedFileName = "TestSensor.38196e9e-a581-4326-b6f2-c4120f89d4cc.1000.zip";
            args.HumanName = "TestSensor";
            args.Version = "1000";
            args.Overwrite = "true";

            modelClient.AddSensor(sensor, args);
            return sensorID;
        }
    }
}
