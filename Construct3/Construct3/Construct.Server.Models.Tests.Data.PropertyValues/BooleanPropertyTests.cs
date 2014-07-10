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
    public class BooleanPropertyTests : PropertyValuesTests
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        #region Boolean Property Tests
        private void BooleanPropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theBoolProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }
        private void BooleanPropertyValueInserts()
        {
            Random random = new Random();

            BooleanPropertyValue value = new BooleanPropertyValue()
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
                Value = random.Next(int.MinValue, int.MaxValue) > (int.MaxValue >> 1) ? true : false
            };

            dataModelTests.model.AddBooleanPropertyValue(Guid.NewGuid(), "TestTEMP", "TheBool", value);
        }
        private void BooleanPropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<IBooleanPropertyValueService> factory = new ChannelFactory<IBooleanPropertyValueService>(binding, address);
            IBooleanPropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<BooleanPropertyValue> BooleanPropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                BooleanPropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(BooleanPropertyValues);
            Assert.IsTrue(BooleanPropertyValues.Count() > 0);
        }
        private void BooleanPropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            BooleanPropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theBoolProperty.ID).Single();
            serviceModel = new BooleanPropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                PropertyValuesTests.connectionString
            );

            IEnumerable<BooleanPropertyValue> BooleanPropertyValues;
            try
            {
                BooleanPropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(BooleanPropertyValues);
            Assert.IsTrue(BooleanPropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _1_1_S1_SetBooleanPropertyServiceUri()
        {
            _1_1_S1_SetBooleanPropertyServiceUriLogic();
        }

        private void _1_1_S1_SetBooleanPropertyServiceUriLogic()
        {
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "localhost", "TestTEMP", "TheBool");
        }

        [TestMethod]
        public void _1_1_S2_BooleanPropertyServiceStart()
        {
            _1_1_S2_BooleanPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S2_BooleanPropertyServiceStartLogic()
        {
            _1_1_S1_SetBooleanPropertyServiceUriLogic();
            BooleanPropertyServiceStart();
        }

        [TestMethod]
        public void _1_1_S3_BooleanPropertyValueInserts()
        {
            _1_1_S3_BooleanPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S3_BooleanPropertyValueInsertsLogic()
        {
            _1_1_S2_BooleanPropertyServiceStartLogic();
            BooleanPropertyValueInserts();
        }

        [TestMethod]
        public void _1_1_S4_BooleanPropertyValueServiceGet()
        {
            _1_1_S4_BooleanPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_BooleanPropertyValueServiceGetLogic()
        {
            _1_1_S3_BooleanPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            BooleanPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_1_S4_BooleanPropertyValueModelGet()
        {
            _1_1_S4_BooleanPropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_BooleanPropertyValueModelGetLogic()
        {
            _1_1_S3_BooleanPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            BooleanPropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _1_2_S1_SetBooleanPropertyServiceUri()
        {
            _1_2_S1_SetBooleanPropertyServiceUriLogic();
        }

        private void _1_2_S1_SetBooleanPropertyServiceUriLogic()
        {
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy", "TestTEMP", "TheBool");
        }

        [TestMethod]
        public void _1_2_S2_BooleanPropertyServiceStart()
        {
            _1_2_S2_BooleanPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S2_BooleanPropertyServiceStartLogic()
        {
            _1_2_S1_SetBooleanPropertyServiceUriLogic();
            BooleanPropertyServiceStart();
        }

        [TestMethod]
        public void _1_2_S3_BooleanPropertyValueInserts()
        {
            _1_2_S3_BooleanPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S3_BooleanPropertyValueInsertsLogic()
        {
            _1_2_S2_BooleanPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            BooleanPropertyValueInserts();
        }

        [TestMethod]
        public void _1_2_S4_BooleanPropertyValueGet()
        {
            _1_2_S4_BooleanPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S4_BooleanPropertyValueGetLogic()
        {
            _1_2_S3_BooleanPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            BooleanPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_3_S1_SetBooleanPropertyServiceUri()
        {
            _1_3_S1_SetBooleanPropertyServiceUriLogic();
        }

        private void _1_3_S1_SetBooleanPropertyServiceUriLogic()
        {
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy.colum.edu", "TestTEMP", "TheBool");
        }

        [TestMethod]
        public void _1_3_S2_BooleanPropertyServiceStart()
        {
            _1_3_S2_BooleanPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S2_BooleanPropertyServiceStartLogic()
        {
            _1_3_S1_SetBooleanPropertyServiceUriLogic();
            BooleanPropertyServiceStart();
        }

        [TestMethod]
        public void _1_3_S3_BooleanPropertyValueInserts()
        {
            _1_3_S3_BooleanPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S3_BooleanPropertyValueInsertsLogic()
        {
            _1_3_S2_BooleanPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            BooleanPropertyValueInserts();
        }

        [TestMethod]
        public void _1_3_S4_BooleanPropertyValueGet()
        {
            _1_3_S4_BooleanPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S4_BooleanPropertyValueGetLogic()
        {
            _1_3_S3_BooleanPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            BooleanPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S1_SetBooleanPropertyServiceUri()
        {
            _2_1_S1_SetBooleanPropertyServiceUriLogic();
        }

        private void _2_1_S1_SetBooleanPropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "TheBool");
        }

        [TestMethod]
        public void _2_1_S2_BooleanPropertyServiceStart()
        {
            _2_1_S2_BooleanPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S2_BooleanPropertyServiceStartLogic()
        {
            _2_1_S1_SetBooleanPropertyServiceUriLogic();
            BooleanPropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S3_BooleanPropertyValueInserts()
        {
            _2_1_S3_BooleanPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S3_BooleanPropertyValueInsertsLogic()
        {
            _2_1_S2_BooleanPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            BooleanPropertyValueInserts();
        }

        [TestMethod]
        public void _2_1_S4_BooleanPropertyValueServiceGet()
        {
            _2_1_S4_BooleanPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_BooleanPropertyValueServiceGetLogic()
        {
            _2_1_S3_BooleanPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            BooleanPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S4_BooleanPropertyValueModelGet()
        {
            _2_1_S4_BooleanPropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_BooleanPropertyValueModelGetLogic()
        {
            _2_1_S3_BooleanPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            BooleanPropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _2_2_S1_SetBooleanPropertyServiceUri()
        {
            _2_2_S1_SetBooleanPropertyServiceUriLogic();
        }

        private void _2_2_S1_SetBooleanPropertyServiceUriLogic()
        {
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy", "TestTEMP", "TheBool");
        }

        [TestMethod]
        public void _2_2_S2_BooleanPropertyServiceStart()
        {
            _2_2_S2_BooleanPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S2_BooleanPropertyServiceStartLogic()
        {
            _2_2_S1_SetBooleanPropertyServiceUriLogic();
            BooleanPropertyServiceStart();
        }

        [TestMethod]
        public void _2_2_S3_BooleanPropertyValueInserts()
        {
            _2_2_S3_BooleanPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S3_BooleanPropertyValueInsertsLogic()
        {
            _2_2_S2_BooleanPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            BooleanPropertyValueInserts();
        }

        [TestMethod]
        public void _2_2_S4_BooleanPropertyValueGet()
        {
            _2_2_S4_BooleanPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S4_BooleanPropertyValueGetLogic()
        {
            _2_2_S3_BooleanPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            BooleanPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_3_S1_SetBooleanPropertyServiceUri()
        {
            _2_3_S1_SetBooleanPropertyServiceUriLogic();
        }

        private void _2_3_S1_SetBooleanPropertyServiceUriLogic()
        {
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy.colum.edu", "TestTEMP", "TheBool");
        }

        [TestMethod]
        public void _2_3_S2_BooleanPropertyServiceStart()
        {
            _2_3_S2_BooleanPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S2_BooleanPropertyServiceStartLogic()
        {
            _2_3_S1_SetBooleanPropertyServiceUriLogic();
            BooleanPropertyServiceStart();
        }

        [TestMethod]
        public void _2_3_S3_BooleanPropertyValueInserts()
        {
            _2_3_S3_BooleanPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S3_BooleanPropertyValueInsertsLogic()
        {
            _2_3_S2_BooleanPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            BooleanPropertyValueInserts();
        }

        [TestMethod]
        public void _2_3_S4_BooleanPropertyValueGet()
        {
            _2_3_S4_BooleanPropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S4_BooleanPropertyValueGetLogic()
        {
            _2_3_S3_BooleanPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            BooleanPropertyValueServiceGet(binding);
        }

        #endregion
    }
}
