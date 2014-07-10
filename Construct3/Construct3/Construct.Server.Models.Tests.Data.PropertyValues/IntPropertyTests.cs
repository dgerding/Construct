using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Construct.Server.Entities.Tests;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValue;
using System.ServiceModel.Channels;
using Construct.Server.Models.Data.PropertyValues.Services;
using System.ServiceModel;

namespace Construct.Server.Models.Tests.Data.PropertyValue
{
    [TestClass]
    public class IntPropertyTests : PropertyValuesTests
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        #region Int Property Tests
        private void IntPropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theIntProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }
        private void IntPropertyValueInserts()
        {
            Random random = new Random();

            IntPropertyValue value = new IntPropertyValue()
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
                Value = random.Next(int.MinValue, int.MaxValue)
            };

            dataModelTests.model.AddIntPropertyValue(Guid.NewGuid(), "TestTEMP", "TheInt", value);
        }
        private void IntPropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<IIntPropertyValueService> factory = new ChannelFactory<IIntPropertyValueService>(binding, address);
            IIntPropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<IntPropertyValue> intPropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                intPropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(intPropertyValues);
            Assert.IsTrue(intPropertyValues.Count() > 0);
        }
        private void IntPropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            IntPropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theIntProperty.ID).Single();
            serviceModel = new IntPropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                PropertyValuesTests.connectionString
            );

            IEnumerable<IntPropertyValue> intPropertyValues;
            try
            {
                intPropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(intPropertyValues);
            Assert.IsTrue(intPropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _1_1_S1_SetIntPropertyServiceUri()
        {
            _1_1_S1_SetIntPropertyServiceUriLogic();
        }

        private void _1_1_S1_SetIntPropertyServiceUriLogic()
        {
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "localhost", "TestTEMP", "TheInt");
        }

        [TestMethod]
        public void _1_1_S2_IntPropertyServiceStart()
        {
            _1_1_S2_IntPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S2_IntPropertyServiceStartLogic()
        {
            _1_1_S1_SetIntPropertyServiceUriLogic();
            IntPropertyServiceStart();
        }

        [TestMethod]
        public void _1_1_S3_IntPropertyValueInserts()
        {
            _1_1_S3_IntPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S3_IntPropertyValueInsertsLogic()
        {
            _1_1_S2_IntPropertyServiceStartLogic();
            IntPropertyValueInserts();
        }

        [TestMethod]
        public void _1_1_S4_IntPropertyValueServiceGet()
        {
            _1_1_S4_IntPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_IntPropertyValueServiceGetLogic()
        {
            _1_1_S3_IntPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            IntPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_1_S4_IntPropertyValueModelGet()
        {
            _1_1_S4_IntPropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_IntPropertyValueModelGetLogic()
        {
            _1_1_S3_IntPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            IntPropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _1_2_S1_SetIntPropertyServiceUri()
        {
            _1_2_S1_SetIntPropertyServiceUriLogic();
        }

        private void _1_2_S1_SetIntPropertyServiceUriLogic()
        {
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy", "TestTEMP", "TheInt");
        }

        [TestMethod]
        public void _1_2_S2_IntPropertyServiceStart()
        {
            _1_2_S2_IntPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S2_IntPropertyServiceStartLogic()
        {
            _1_2_S1_SetIntPropertyServiceUriLogic();
            IntPropertyServiceStart();
        }

        [TestMethod]
        public void _1_2_S3_IntPropertyValueInserts()
        {
            _1_2_S3_IntPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S3_IntPropertyValueInsertsLogic()
        {
            _1_2_S2_IntPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            IntPropertyValueInserts();
        }

        [TestMethod]
        public void _1_2_S4_IntPropertyValueGet()
        {
            _1_2_S4_IntPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S4_IntPropertyValueGetLogic()
        {
            _1_2_S3_IntPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            IntPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_3_S1_SetIntPropertyServiceUri()
        {
            _1_3_S1_SetIntPropertyServiceUriLogic();
        }

        private void _1_3_S1_SetIntPropertyServiceUriLogic()
        {
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy.colum.edu", "TestTEMP", "TheInt");
        }

        [TestMethod]
        public void _1_3_S2_IntPropertyServiceStart()
        {
            _1_3_S2_IntPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S2_IntPropertyServiceStartLogic()
        {
            _1_3_S1_SetIntPropertyServiceUriLogic();
            IntPropertyServiceStart();
        }

        [TestMethod]
        public void _1_3_S3_IntPropertyValueInserts()
        {
            _1_3_S3_IntPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S3_IntPropertyValueInsertsLogic()
        {
            _1_3_S2_IntPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            IntPropertyValueInserts();
        }

        [TestMethod]
        public void _1_3_S4_IntPropertyValueGet()
        {
            _1_3_S4_IntPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S4_IntPropertyValueGetLogic()
        {
            _1_3_S3_IntPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            IntPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S1_SetIntPropertyServiceUri()
        {
            _2_1_S1_SetIntPropertyServiceUriLogic();
        }

        private void _2_1_S1_SetIntPropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "TheInt");
        }

        [TestMethod]
        public void _2_1_S2_IntPropertyServiceStart()
        {
            _2_1_S2_IntPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S2_IntPropertyServiceStartLogic()
        {
            _2_1_S1_SetIntPropertyServiceUriLogic();
            IntPropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S3_IntPropertyValueInserts()
        {
            _2_1_S3_IntPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S3_IntPropertyValueInsertsLogic()
        {
            _2_1_S2_IntPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            IntPropertyValueInserts();
        }

        [TestMethod]
        public void _2_1_S4_IntPropertyValueServiceGet()
        {
            _2_1_S4_IntPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_IntPropertyValueServiceGetLogic()
        {
            _2_1_S3_IntPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            IntPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S4_IntPropertyValueModelGet()
        {
            _2_1_S4_IntPropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_IntPropertyValueModelGetLogic()
        {
            _2_1_S3_IntPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            IntPropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _2_2_S1_SetIntPropertyServiceUri()
        {
            _2_2_S1_SetIntPropertyServiceUriLogic();
        }

        private void _2_2_S1_SetIntPropertyServiceUriLogic()
        {
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy", "TestTEMP", "TheInt");
        }

        [TestMethod]
        public void _2_2_S2_IntPropertyServiceStart()
        {
            _2_2_S2_IntPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S2_IntPropertyServiceStartLogic()
        {
            _2_2_S1_SetIntPropertyServiceUriLogic();
            IntPropertyServiceStart();
        }

        [TestMethod]
        public void _2_2_S3_IntPropertyValueInserts()
        {
            _2_2_S3_IntPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S3_IntPropertyValueInsertsLogic()
        {
            _2_2_S2_IntPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            IntPropertyValueInserts();
        }

        [TestMethod]
        public void _2_2_S4_IntPropertyValueGet()
        {
            _2_2_S4_IntPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S4_IntPropertyValueGetLogic()
        {
            _2_2_S3_IntPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            IntPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_3_S1_SetIntPropertyServiceUri()
        {
            _2_3_S1_SetIntPropertyServiceUriLogic();
        }

        private void _2_3_S1_SetIntPropertyServiceUriLogic()
        {
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy.colum.edu", "TestTEMP", "TheInt");
        }

        [TestMethod]
        public void _2_3_S2_IntPropertyServiceStart()
        {
            _2_3_S2_IntPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S2_IntPropertyServiceStartLogic()
        {
            _2_3_S1_SetIntPropertyServiceUriLogic();
            IntPropertyServiceStart();
        }

        [TestMethod]
        public void _2_3_S3_IntPropertyValueInserts()
        {
            _2_3_S3_IntPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S3_IntPropertyValueInsertsLogic()
        {
            _2_3_S2_IntPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            IntPropertyValueInserts();
        }

        [TestMethod]
        public void _2_3_S4_IntPropertyValueGet()
        {
            _2_3_S4_IntPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S4_IntPropertyValueGetLogic()
        {
            _2_3_S3_IntPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            IntPropertyValueServiceGet(binding);
        }

        #endregion
    }
}
