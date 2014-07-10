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
    public class DoublePropertyTests : PropertyValuesTests
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        #region Double Property Tests
        private void DoublePropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theDoubleOneProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }
        private void DoublePropertyValueInserts()
        {
            Random random = new Random();

            DoublePropertyValue value = new DoublePropertyValue()
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

            dataModelTests.model.AddDoublePropertyValue(Guid.NewGuid(), "TestTEMP", "doubleOne", value);
        }
        private void DoublePropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<IDoublePropertyValueService> factory = new ChannelFactory<IDoublePropertyValueService>(binding, address);
            IDoublePropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<DoublePropertyValue> DoublePropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                DoublePropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(DoublePropertyValues);
            Assert.IsTrue(DoublePropertyValues.Count() > 0);
        }
        private void DoublePropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            DoublePropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theDoubleOneProperty.ID).Single();
            serviceModel = new DoublePropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                PropertyValuesTests.connectionString
            );

            IEnumerable<DoublePropertyValue> DoublePropertyValues;
            try
            {
                DoublePropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(DoublePropertyValues);
            Assert.IsTrue(DoublePropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _1_1_S1_SetDoublePropertyServiceUri()
        {
            _1_1_S1_SetDoublePropertyServiceUriLogic();
        }

        private void _1_1_S1_SetDoublePropertyServiceUriLogic()
        {
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "localhost", "TestTEMP", "doubleOne");
        }

        [TestMethod]
        public void _1_1_S2_DoublePropertyServiceStart()
        {
            _1_1_S2_DoublePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S2_DoublePropertyServiceStartLogic()
        {
            _1_1_S1_SetDoublePropertyServiceUriLogic();
            DoublePropertyServiceStart();
        }

        [TestMethod]
        public void _1_1_S3_DoublePropertyValueInserts()
        {
            _1_1_S3_DoublePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S3_DoublePropertyValueInsertsLogic()
        {
            _1_1_S2_DoublePropertyServiceStartLogic();
            DoublePropertyValueInserts();
        }

        [TestMethod]
        public void _1_1_S4_DoublePropertyValueServiceGet()
        {
            _1_1_S4_DoublePropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_DoublePropertyValueServiceGetLogic()
        {
            _1_1_S3_DoublePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            DoublePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_1_S4_DoublePropertyValueModelGet()
        {
            _1_1_S4_DoublePropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_DoublePropertyValueModelGetLogic()
        {
            _1_1_S3_DoublePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            DoublePropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _1_2_S1_SetDoublePropertyServiceUri()
        {
            _1_2_S1_SetDoublePropertyServiceUriLogic();
        }

        private void _1_2_S1_SetDoublePropertyServiceUriLogic()
        {
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy", "TestTEMP", "doubleOne");
        }

        [TestMethod]
        public void _1_2_S2_DoublePropertyServiceStart()
        {
            _1_2_S2_DoublePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S2_DoublePropertyServiceStartLogic()
        {
            _1_2_S1_SetDoublePropertyServiceUriLogic();
            DoublePropertyServiceStart();
        }

        [TestMethod]
        public void _1_2_S3_DoublePropertyValueInserts()
        {
            _1_2_S3_DoublePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S3_DoublePropertyValueInsertsLogic()
        {
            _1_2_S2_DoublePropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            DoublePropertyValueInserts();
        }

        [TestMethod]
        public void _1_2_S4_DoublePropertyValueGet()
        {
            _1_2_S4_DoublePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S4_DoublePropertyValueGetLogic()
        {
            _1_2_S3_DoublePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            DoublePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_3_S1_SetDoublePropertyServiceUri()
        {
            _1_3_S1_SetDoublePropertyServiceUriLogic();
        }

        private void _1_3_S1_SetDoublePropertyServiceUriLogic()
        {
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy.colum.edu", "TestTEMP", "doubleOne");
        }

        [TestMethod]
        public void _1_3_S2_DoublePropertyServiceStart()
        {
            _1_3_S2_DoublePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S2_DoublePropertyServiceStartLogic()
        {
            _1_3_S1_SetDoublePropertyServiceUriLogic();
            DoublePropertyServiceStart();
        }

        [TestMethod]
        public void _1_3_S3_DoublePropertyValueInserts()
        {
            _1_3_S3_DoublePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S3_DoublePropertyValueInsertsLogic()
        {
            _1_3_S2_DoublePropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            DoublePropertyValueInserts();
        }

        [TestMethod]
        public void _1_3_S4_DoublePropertyValueGet()
        {
            _1_3_S4_DoublePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S4_DoublePropertyValueGetLogic()
        {
            _1_3_S3_DoublePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            DoublePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S1_SetDoublePropertyServiceUri()
        {
            _2_1_S1_SetDoublePropertyServiceUriLogic();
        }

        private void _2_1_S1_SetDoublePropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "doubleOne");
        }

        [TestMethod]
        public void _2_1_S2_DoublePropertyServiceStart()
        {
            _2_1_S2_DoublePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S2_DoublePropertyServiceStartLogic()
        {
            _2_1_S1_SetDoublePropertyServiceUriLogic();
            DoublePropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S3_DoublePropertyValueInserts()
        {
            _2_1_S3_DoublePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S3_DoublePropertyValueInsertsLogic()
        {
            _2_1_S2_DoublePropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            DoublePropertyValueInserts();
        }

        [TestMethod]
        public void _2_1_S4_DoublePropertyValueServiceGet()
        {
            _2_1_S4_DoublePropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_DoublePropertyValueServiceGetLogic()
        {
            _2_1_S3_DoublePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            DoublePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S4_DoublePropertyValueModelGet()
        {
            _2_1_S4_DoublePropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_DoublePropertyValueModelGetLogic()
        {
            _2_1_S3_DoublePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            DoublePropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _2_2_S1_SetDoublePropertyServiceUri()
        {
            _2_2_S1_SetDoublePropertyServiceUriLogic();
        }

        private void _2_2_S1_SetDoublePropertyServiceUriLogic()
        {
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy", "TestTEMP", "doubleOne");
        }

        [TestMethod]
        public void _2_2_S2_DoublePropertyServiceStart()
        {
            _2_2_S2_DoublePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S2_DoublePropertyServiceStartLogic()
        {
            _2_2_S1_SetDoublePropertyServiceUriLogic();
            DoublePropertyServiceStart();
        }

        [TestMethod]
        public void _2_2_S3_DoublePropertyValueInserts()
        {
            _2_2_S3_DoublePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S3_DoublePropertyValueInsertsLogic()
        {
            _2_2_S2_DoublePropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            DoublePropertyValueInserts();
        }

        [TestMethod]
        public void _2_2_S4_DoublePropertyValueGet()
        {
            _2_2_S4_DoublePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S4_DoublePropertyValueGetLogic()
        {
            _2_2_S3_DoublePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            DoublePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_3_S1_SetDoublePropertyServiceUri()
        {
            _2_3_S1_SetDoublePropertyServiceUriLogic();
        }

        private void _2_3_S1_SetDoublePropertyServiceUriLogic()
        {
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy.colum.edu", "TestTEMP", "doubleOne");
        }

        [TestMethod]
        public void _2_3_S2_DoublePropertyServiceStart()
        {
            _2_3_S2_DoublePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S2_DoublePropertyServiceStartLogic()
        {
            _2_3_S1_SetDoublePropertyServiceUriLogic();
            DoublePropertyServiceStart();
        }

        [TestMethod]
        public void _2_3_S3_DoublePropertyValueInserts()
        {
            _2_3_S3_DoublePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S3_DoublePropertyValueInsertsLogic()
        {
            _2_3_S2_DoublePropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            DoublePropertyValueInserts();
        }

        [TestMethod]
        public void _2_3_S4_DoublePropertyValueGet()
        {
            _2_3_S4_DoublePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S4_DoublePropertyValueGetLogic()
        {
            _2_3_S3_DoublePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            DoublePropertyValueServiceGet(binding);
        }

        #endregion
    }
}
