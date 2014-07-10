using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.Server.Models.Data.PropertyValue;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Construct.Server.Models.Data.PropertyValues.Services;
using System.Configuration;
using Construct.Server.Entities;
using Construct.Server.Entities.Tests;

namespace Construct.Server.Models.Tests.Data.PropertyValue
{
    [TestClass]
    public class SinglePropertyTests : PropertyValuesTests
    {
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        /*

        #region Single Property Tests
        private void SinglePropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theSingleProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }
        private void SinglePropertyValueInserts()
        {
            Random random = new Random();

            SinglePropertyValue value = new SinglePropertyValue()
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

            dataModelTests.model.AddSinglePropertyValue(Guid.NewGuid(), "TestTEMP", "TheSingle", value);
        }
        private void SinglePropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<ISinglePropertyValueService> factory = new ChannelFactory<ISinglePropertyValueService>(binding, address);
            IIntPropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<IntPropertyValue> SinglePropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                SinglePropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(SinglePropertyValues);
            Assert.IsTrue(SinglePropertyValues.Count() > 0);
        }
        private void SinglePropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            SinglePropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theIntProperty.ID).Single();
            serviceModel = new SinglePropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                PropertyValuesTests.connectionString
            );

            IEnumerable<SinglePropertyValue> SinglePropertyValues;
            try
            {
                SinglePropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(SinglePropertyValues);
            Assert.IsTrue(SinglePropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _1_1_S1_SetSinglePropertyServiceUri()
        {
            _1_1_S1_SetSinglePropertyServiceUriLogic();
        }

        private void _1_1_S1_SetSinglePropertyServiceUriLogic()
        {
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "localhost", "TestTEMP", "TheSingle");
        }

        [TestMethod]
        public void _1_1_S2_SinglePropertyServiceStart()
        {
            _1_1_S2_SinglePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S2_SinglePropertyServiceStartLogic()
        {
            _1_1_S1_SetSinglePropertyServiceUriLogic();
            SinglePropertyServiceStart();
        }

        [TestMethod]
        public void _1_1_S3_SinglePropertyValueInserts()
        {
            _1_1_S3_SinglePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S3_SinglePropertyValueInsertsLogic()
        {
            _1_1_S2_SinglePropertyServiceStartLogic();
            SinglePropertyValueInserts();
        }

        [TestMethod]
        public void _1_1_S4_SinglePropertyValueServiceGet()
        {
            _1_1_S4_SinglePropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_SinglePropertyValueServiceGetLogic()
        {
            _1_1_S3_SinglePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            SinglePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_1_S4_SinglePropertyValueModelGet()
        {
            _1_1_S4_SinglePropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_SinglePropertyValueModelGetLogic()
        {
            _1_1_S3_SinglePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            SinglePropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _1_2_S1_SetSinglePropertyServiceUri()
        {
            _1_2_S1_SetSinglePropertyServiceUriLogic();
        }

        private void _1_2_S1_SetSinglePropertyServiceUriLogic()
        {
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy", "TestTEMP", "TheSingle");
        }

        [TestMethod]
        public void _1_2_S2_SinglePropertyServiceStart()
        {
            _1_2_S2_SinglePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S2_SinglePropertyServiceStartLogic()
        {
            _1_2_S1_SetSinglePropertyServiceUriLogic();
            SinglePropertyServiceStart();
        }

        [TestMethod]
        public void _1_2_S3_SinglePropertyValueInserts()
        {
            _1_2_S3_SinglePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S3_SinglePropertyValueInsertsLogic()
        {
            _1_2_S2_SinglePropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            SinglePropertyValueInserts();
        }

        [TestMethod]
        public void _1_2_S4_SinglePropertyValueGet()
        {
            _1_2_S4_SinglePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S4_SinglePropertyValueGetLogic()
        {
            _1_2_S3_SinglePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            SinglePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_3_S1_SetSinglePropertyServiceUri()
        {
            _1_3_S1_SetSinglePropertyServiceUriLogic();
        }

        private void _1_3_S1_SetSinglePropertyServiceUriLogic()
        {
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy.colum.edu", "TestTEMP", "TheSingle");
        }

        [TestMethod]
        public void _1_3_S2_SinglePropertyServiceStart()
        {
            _1_3_S2_SinglePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S2_SinglePropertyServiceStartLogic()
        {
            _1_3_S1_SetSinglePropertyServiceUriLogic();
            SinglePropertyServiceStart();
        }

        [TestMethod]
        public void _1_3_S3_SinglePropertyValueInserts()
        {
            _1_3_S3_SinglePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S3_SinglePropertyValueInsertsLogic()
        {
            _1_3_S2_SinglePropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            SinglePropertyValueInserts();
        }

        [TestMethod]
        public void _1_3_S4_SinglePropertyValueGet()
        {
            _1_3_S4_SinglePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S4_SinglePropertyValueGetLogic()
        {
            _1_3_S3_SinglePropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            SinglePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S1_SetSinglePropertyServiceUri()
        {
            _2_1_S1_SetSinglePropertyServiceUriLogic();
        }

        private void _2_1_S1_SetSinglePropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "TheSingle");
        }

        [TestMethod]
        public void _2_1_S2_SinglePropertyServiceStart()
        {
            _2_1_S2_SinglePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S2_SinglePropertyServiceStartLogic()
        {
            _2_1_S1_SetSinglePropertyServiceUriLogic();
            SinglePropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S3_SinglePropertyValueInserts()
        {
            _2_1_S3_SinglePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S3_SinglePropertyValueInsertsLogic()
        {
            _2_1_S2_SinglePropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            SinglePropertyValueInserts();
        }

        [TestMethod]
        public void _2_1_S4_SinglePropertyValueServiceGet()
        {
            _2_1_S4_SinglePropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_SinglePropertyValueServiceGetLogic()
        {
            _2_1_S3_SinglePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            SinglePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S4_SinglePropertyValueModelGet()
        {
            _2_1_S4_SinglePropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_SinglePropertyValueModelGetLogic()
        {
            _2_1_S3_SinglePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            SinglePropertyValueModelGet(binding);
        }

        [TestMethod]
        public void _2_2_S1_SetSinglePropertyServiceUri()
        {
            _2_2_S1_SetSinglePropertyServiceUriLogic();
        }

        private void _2_2_S1_SetSinglePropertyServiceUriLogic()
        {
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy", "TestTEMP", "TheSingle");
        }

        [TestMethod]
        public void _2_2_S2_SinglePropertyServiceStart()
        {
            _2_2_S2_SinglePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S2_SinglePropertyServiceStartLogic()
        {
            _2_2_S1_SetSinglePropertyServiceUriLogic();
            SinglePropertyServiceStart();
        }

        [TestMethod]
        public void _2_2_S3_SinglePropertyValueInserts()
        {
            _2_2_S3_SinglePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S3_SinglePropertyValueInsertsLogic()
        {
            _2_2_S2_SinglePropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            SinglePropertyValueInserts();
        }

        [TestMethod]
        public void _2_2_S4_SinglePropertyValueGet()
        {
            _2_2_S4_SinglePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S4_SinglePropertyValueGetLogic()
        {
            _2_2_S3_SinglePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            SinglePropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_3_S1_SetSinglePropertyServiceUri()
        {
            _2_3_S1_SetSinglePropertyServiceUriLogic();
        }

        private void _2_3_S1_SetSinglePropertyServiceUriLogic()
        {
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy.colum.edu", "TestTEMP", "TheSingle");
        }

        [TestMethod]
        public void _2_3_S2_SinglePropertyServiceStart()
        {
            _2_3_S2_SinglePropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S2_SinglePropertyServiceStartLogic()
        {
            _2_3_S1_SetSinglePropertyServiceUriLogic();
            SinglePropertyServiceStart();
        }

        [TestMethod]
        public void _2_3_S3_SinglePropertyValueInserts()
        {
            _2_3_S3_SinglePropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S3_SinglePropertyValueInsertsLogic()
        {
            _2_3_S2_SinglePropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            SinglePropertyValueInserts();
        }

        [TestMethod]
        public void _2_3_S4_SinglePropertyValueGet()
        {
            _2_3_S4_SinglePropertyValueGetLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S4_SinglePropertyValueGetLogic()
        {
            _2_3_S3_SinglePropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            SinglePropertyValueServiceGet(binding);
        }

        #endregion

        */
    }
}
