using System;
using System.Linq;
using NLog;
using System.IO;
using System.Web.Hosting;

namespace Construct.Credentials.Model
{
    public class Model : ModelBase, IModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

       
        public Model() : base()
        {
            logger.Trace("Entered Credentials.Model ctor.");
            
            Name = "Credentials";
            
        }

        public bool IsCredentialsServerAvailable()
        {
            return ConstructServerConnectionTester.IsCredentialsServerAvailable();
        }

        public string GetConnectionStringUsingConnectionStringName(string name)
        {
            return ConstructServerConnectionTester.GetConnectionStringUsingConnectionStringName(name);
        }

        public bool IsExistingConnectionString(string theConnectionStringName)
        {
            return ConstructServerConnectionTester.IsExistingConnectionString(theConnectionStringName);
        }

        public bool IsValidationServerAvailable()
        {
            return ServerValidator.IsValidationServerAvailable();
        }

        public bool AreCredentialsAuthentic(string serverName, string userName, string password)
        {
            return ServerValidator.AreCredentialsAuthentic(serverName, userName, password);
        }

        public bool IsConstructServerAvailable(string connectionString)
        {
            return ServerValidator.IsConstructServerAvailable(connectionString);
        }

        public bool AreConstructServerCoreEntitiesValid(string connectionString)
        {
            return ServerValidator.AreConstructServerCoreEntitiesValid(connectionString);
        }

        public string GetConstructServerConnectionString(string theServerName, string theUserName, string thePassword)
        {
            return ServerValidator.GetConstructServerConnectionString(theServerName, theUserName, thePassword);
        }

        public void EnsureTestItem(string connectionString)
        {
//            DataServiceReference.ModelClient dataClient = new DataServiceReference.ModelClient(new BasicHttpBinding(), new System.ServiceModel.EndpointAddress(dataServiceUri));
//            string xmlText = @"<?xml version='1.0' encoding='utf-16'?>
//<TypeSource Name='TestSensor' ID='38196E9E-A581-4326-B6F2-C4120F89D4CC' ParentID='5C11FBBD-9E36-4BEA-A8BE-06E225250EF8'>
//	<SensorType Name='Test' ID='0c89f6c2-c749-4085-b0ea-163c419f5ac3' SensorHostTypeID='EDA0FF3E-108B-45D5-BF58-F362FABF2EFE'>
//		<DataType>
//			<DataTypeProperty Name='TheInt' Type='int' ID='14F0E115-3EF3-4D59-882C-5E622B8849DB'/>
//			<DataTypeProperty Name='TheBool' Type='bool' ID='388A0BC9-6010-40BD-BAD0-ACCDAFE35912'/>
//			<DataTypeProperty Name='TheString' Type='string' ID='92EBA461-1872-4504-A26E-A46C292AB623'/>
//			<DataTypeProperty Name='TheBytes' Type='byte[]' ID='715E510E-D7E9-439E-BF6A-7B4F213CA71C'/>
//			<DataTypeProperty Name='TheGuid' Type='Guid' ID='12E166B9-8DB7-4EF6-BAC7-F86ECABD4843'/>
//			<DataTypeProperty Name='intOne' Type='int' ID='B919F2B4-1BFC-4A2E-A9E6-41B692F31307'/>
//			<DataTypeProperty Name='doubleOne' Type='double' ID='D7535E8F-A3B3-43CA-B6D5-EEAD354B0B8A'/>
//			<DataTypeProperty Name='name' Type='string' ID='3994D895-998D-49F6-A0AB-29D1A3026838'/>
//		</DataType>
//	</SensorType>
//</TypeSource>";
//            if (dataClient.GetDataTypeSources().Where(ts => ts.Name == "TestItemSensor").Count() == 0)
//            {
//                SourcesServiceReference.ModelClient sourcesClient = new SourcesServiceReference.ModelClient(null, new EndpointAddress(sourcesServiceUri).Uri.AbsoluteUri);

//                SensorRuntime runtime = new SensorRuntime();

//                runtime.ID = Guid.NewGuid();
//                runtime.TypeSourceID = Guid.Parse("38196E9E-A581-4326-B6F2-C4120F89D4CC");
//                runtime.Version = "1000";
//                runtime.InstallerUri = Path.Combine("C:\\Construct", "Assemblies", String.Format("TestSensor.{0}.{1}.zip", runtime.TypeSourceID, runtime.Version));
//                runtime.CacheUri = "Not Implemented";
//                runtime.SensorHostTypeID = Guid.Parse("EDA0FF3E-108B-45D5-BF58-F362FABF2EFE");
//                runtime.InstallerXml = xmlText;
//                runtime.InstallerZip = new byte[1] { 1 };
//                runtime.RecCreationDate = DateTime.Now;

//                sourcesClient.AddSensorDefinition(runtime);
//            }
        }

        public void EnsureCoreEntitiesExist(string connectionString)
        {
            ExecuteScript.Go(connectionString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","ValidateCoreEntities.sql"));
        }

        

        public void LoadTestItemData(string connectionString)
        {
            string correctedString = connectionString;
            correctedString = correctedString.Replace("User ID=Construct", "User ID=Construct");

            ExecuteScript.Go(correctedString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","CreateItemScratch.sql"));
            ExecuteScript.Go(correctedString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","CreateTestItemData.sql"));
            ExecuteScript.Go(correctedString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","CreateTestItem_zData.sql"));
        }

        

        public void Reset(string connectionString)
        {
            if (WipeDatabaseAndResetToDefaults.Go(connectionString) == false)
            {
                throw new ApplicationException("Failed to wipe database");
            }
            EnsureTestItem(connectionString);
        }
    }
}