using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.Server.Models.Data.PropertyValue;
using System.Configuration;
using Construct.Server.Entities.Tests;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValues.Services;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Construct.Server.Models.Tests.Data.PropertyValue
{
    [TestClass]
    public class StringPropertyTests : PropertyValuesTests
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        #region String Property Tests
        private void StringPropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theStringProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }
        private void StringPropertyValueInserts()
        {
            Random random = new Random();

            StringPropertyValue value = new StringPropertyValue()
            {
                Interval = random.Next(0, 500),
                ItemID = Guid.NewGuid(),
                Latitude = random.Next(-100, 100).ToString(),
                Longitude = random.Next(-100, 100).ToString(),
                StartTime = DateTime.Now
                    .AddMinutes(random.Next(0, 60))
                    .AddHours(random.Next(0, 24))
                    .AddDays(random.Next(0, 30))
                    .AddMonths(random.Next(0, 12))
                    .AddYears(random.Next(0, 10)),
                Value = "String with value: " + random.Next(0, 100).ToString()
            };

            dataModelTests.model.AddStringPropertyValue(Guid.NewGuid(), "TestTEMP", "TheString", value);
        }
        private void StringPropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<IStringPropertyValueService> factory = new ChannelFactory<IStringPropertyValueService>(binding, address);
            IStringPropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<StringPropertyValue> stringPropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                stringPropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(stringPropertyValues);
            Assert.IsTrue(stringPropertyValues.Count() > 0);
        }
        private void StringPropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            StringPropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theStringProperty.ID).Single();
            serviceModel = new StringPropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                PropertyValuesTests.connectionString
            );

            IEnumerable<StringPropertyValue> stringPropertyValues;
            try
            {
                stringPropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(stringPropertyValues);
            Assert.IsTrue(stringPropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _1_1_S1_SetStringPropertyServiceUri()
        {
            _1_1_S1_SetStringPropertyServiceUriLogic();
        }
  
        private void _1_1_S1_SetStringPropertyServiceUriLogic()
        {
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "localhost", "TestTEMP", "TheString");
        }

        [TestMethod]
        public void _1_1_S2_StringPropertyServiceStart()
        {
            _1_1_S2_StringPropertyServiceStartLogic();
            host.Close();
            Clean();
        }
  
        private void _1_1_S2_StringPropertyServiceStartLogic()
        {
            _1_1_S1_SetStringPropertyServiceUriLogic();
            StringPropertyServiceStart();
        }

        [TestMethod]
        public void _1_1_S3_StringPropertyValueInserts()
        {
            _1_1_S3_StringPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }
  
        private void _1_1_S3_StringPropertyValueInsertsLogic()
        {
            _1_1_S2_StringPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            StringPropertyValueInserts();
        }

        [TestMethod]
        public void _1_1_S4_StringPropertyValueServiceGet()
        {
            _1_1_S4_StringPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_StringPropertyValueServiceGetLogic()
        {
            _1_1_S3_StringPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            StringPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_2_S1_SetStringPropertyServiceUri()
        {
            _1_2_S1_SetStringPropertyServiceUriLogic();
        }
  
        private void _1_2_S1_SetStringPropertyServiceUriLogic()
        {
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy", "TestTEMP", "TheString");
        }

        [TestMethod]
        public void _1_2_S2_StringPropertyServiceStart()
        {
            _1_2_S2_StringPropertyServiceStartLogic();
            host.Close();
            Clean();
        }
  
        private void _1_2_S2_StringPropertyServiceStartLogic()
        {
            _1_2_S1_SetStringPropertyServiceUriLogic();
            StringPropertyServiceStart();
        }

        [TestMethod]
        public void _1_2_S3_StringPropertyValueInserts()
        {
            _1_2_S3_StringPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }
  
        private void _1_2_S3_StringPropertyValueInsertsLogic()
        {
            _1_2_S2_StringPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            StringPropertyValueInserts();
        }

        [TestMethod]
        public void _1_2_S4_StringPropertyValueServiceGet()
        {
            _1_2_S4_StringPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S4_StringPropertyValueServiceGetLogic()
        {
            _1_2_S3_StringPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            StringPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_3_S1_SetStringPropertyServiceUri()
        {
            _1_3_S1_SetStringPropertyServiceUriLogic();
        }
  
        private void _1_3_S1_SetStringPropertyServiceUriLogic()
        {
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy.colum.edu", "TestTEMP", "TheString");
        }

        [TestMethod]
        public void _1_3_S2_StringPropertyServiceStart()
        {
            _1_3_S2_StringPropertyServiceStartLogic();
            host.Close();
            Clean();
        }
  
        private void _1_3_S2_StringPropertyServiceStartLogic()
        {
            _1_3_S1_SetStringPropertyServiceUriLogic();
            StringPropertyServiceStart();
        }

        [TestMethod]
        public void _1_3_S3_StringPropertyValueInserts()
        {
            _1_3_S3_StringPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }
  
        private void _1_3_S3_StringPropertyValueInsertsLogic()
        {
            _1_3_S2_StringPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            StringPropertyValueInserts();
        }

        [TestMethod]
        public void _1_3_S4_StringPropertyValueServiceGet()
        {
            _1_3_S4_StringPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S4_StringPropertyValueServiceGetLogic()
        {
            _1_3_S3_StringPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            StringPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S1_SetStringPropertyServiceUri()
        {
            _2_1_S1_SetStringPropertyServiceUriLogic();
        }
  
        private void _2_1_S1_SetStringPropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "TheString");
        }

        [TestMethod]
        public void _2_1_S2_StringPropertyServiceStart()
        {
            _2_1_S2_StringPropertyServiceStartLogic();
            host.Close();
            Clean();
        }
  
        private void _2_1_S2_StringPropertyServiceStartLogic()
        {
            _2_1_S1_SetStringPropertyServiceUriLogic();
            StringPropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S3_StringPropertyValueInserts()
        {
            _2_1_S3_StringPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }
  
        private void _2_1_S3_StringPropertyValueInsertsLogic()
        {
            _2_1_S2_StringPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            StringPropertyValueInserts();
        }

        [TestMethod]
        public void _2_1_S4_StringPropertyValueServiceGet()
        {
            _2_1_S4_StringPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_StringPropertyValueServiceGetLogic()
        {
            _2_1_S3_StringPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            StringPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S4_StringPropertyValueModelGet()
        {
            _2_1_S4_StringPropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_StringPropertyValueModelGetLogic()
        {
            _2_1_S3_StringPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            StringPropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _2_2_S1_SetStringPropertyServiceUri()
        {
            _2_2_S1_SetStringPropertyServiceUriLogic();
        }
  
        private void _2_2_S1_SetStringPropertyServiceUriLogic()
        {
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy", "TestTEMP", "TheString");
        }

        [TestMethod]
        public void _2_2_S2_StringPropertyServiceStart()
        {
            _2_2_S2_StringPropertyServiceStartLogic();
            host.Close();
            Clean();
        }
  
        private void _2_2_S2_StringPropertyServiceStartLogic()
        {
            _2_2_S1_SetStringPropertyServiceUriLogic();
            StringPropertyServiceStart();
        }

        [TestMethod]
        public void _2_2_S3_StringPropertyValueInserts()
        {
            _2_2_S3_StringPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }
  
        private void _2_2_S3_StringPropertyValueInsertsLogic()
        {
            _2_2_S2_StringPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            StringPropertyValueInserts();
        }

        [TestMethod]
        public void _2_2_S4_StringPropertyValueServiceGet()
        {
            _2_2_S4_StringPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S4_StringPropertyValueServiceGetLogic()
        {
            _2_2_S3_StringPropertyValueInsertsLogic();
            Binding binding = new BasicHttpBinding();
            StringPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_3_S1_SetStringPropertyServiceUri()
        {
            _2_3_S1_SetStringPropertyServiceUriLogic();
        }
  
        private void _2_3_S1_SetStringPropertyServiceUriLogic()
        {
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy.colum.edu", "TestTEMP", "TheString");
        }

        [TestMethod]
        public void _2_3_S2_StringPropertyServiceStart()
        {
            _2_3_S2_StringPropertyServiceStartLogic();
            host.Close();
            Clean();
        }
  
        private void _2_3_S2_StringPropertyServiceStartLogic()
        {
            _2_3_S1_SetStringPropertyServiceUriLogic();
            StringPropertyServiceStart();
        }

        [TestMethod]
        public void _2_3_S3_StringPropertyValueInserts()
        {
            _2_3_S3_StringPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }
  
        private void _2_3_S3_StringPropertyValueInsertsLogic()
        {
            _2_3_S2_StringPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            StringPropertyValueInserts();
        }

        [TestMethod]
        public void _2_3_S4_StringPropertyValueServiceGet()
        {
            _2_3_S4_StringPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S4_StringPropertyValueServiceGetLogic()
        {
            _2_3_S3_StringPropertyValueInsertsLogic();
            Binding binding = new BasicHttpBinding();
            StringPropertyValueServiceGet(binding);
        }

        #endregion

    }
}
