using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.Server.Entities.Tests;
using System.Configuration;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValue;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Construct.Server.Models.Data.PropertyValues.Services;

namespace Construct.Server.Models.Tests.Data.PropertyValue
{
    [TestClass]
    public class BytesPropertyTests : PropertyValuesTests
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        #region Byte PropertyTests
        private void ByteArrayPropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theBytesProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }
        private void ByteArrayPropertyValueInserts()
        {
            Random random = new Random();

            string str = "String with value: " + random.Next(0, 100).ToString();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            ByteArrayPropertyValue value = new ByteArrayPropertyValue()
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
                Value = bytes
            };

            dataModelTests.model.AddByteArrayPropertyValue(Guid.NewGuid(), "TestTEMP", "TheBytes", value);
        }
        private void ByteArrayPropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<IByteArrayPropertyValueService> factory = new ChannelFactory<IByteArrayPropertyValueService>(binding, address);
            IByteArrayPropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<ByteArrayPropertyValue> byteArrayPropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                byteArrayPropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(byteArrayPropertyValues);
            Assert.IsTrue(byteArrayPropertyValues.Count() > 0);
        }
        private void ByteArrayPropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            ByteArrayPropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theBytesProperty.ID).Single();
            serviceModel = new ByteArrayPropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                PropertyValuesTests.connectionString
            );

            IEnumerable<ByteArrayPropertyValue> byteArrayPropertyValues;
            try
            {
                byteArrayPropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(byteArrayPropertyValues);
            Assert.IsTrue(byteArrayPropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _1_1_S1_SetByteArrayPropertyServiceUri()
        {
            _1_1_S1_SetByteArrayPropertyServiceUriLogic();
        }

        private void _1_1_S1_SetByteArrayPropertyServiceUriLogic()
        {
            utilitiesTests._1_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "localhost", "TestTEMP", "TheBytes");
        }

        [TestMethod]
        public void _1_1_S2_ByteArrayPropertyServiceStart()
        {
            _1_1_S2_ByteArrayPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S2_ByteArrayPropertyServiceStartLogic()
        {
            _1_1_S1_SetByteArrayPropertyServiceUriLogic();
            ByteArrayPropertyServiceStart();
        }

        [TestMethod]
        public void _1_1_S3_ByteArrayPropertyValueInserts()
        {
            _1_1_S3_ByteArrayPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S3_ByteArrayPropertyValueInsertsLogic()
        {
            _1_1_S2_ByteArrayPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            ByteArrayPropertyValueInserts();
        }

        [TestMethod]
        public void _1_1_S4_ByteArrayPropertyValueServiceGet()
        {
            _1_1_S4_ByteArrayPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_1_S4_ByteArrayPropertyValueServiceGetLogic()
        {
            _1_1_S3_ByteArrayPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            ByteArrayPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_2_S1_SetByteArrayPropertyServiceUri()
        {
            _1_2_S1_SetByteArrayPropertyServiceUriLogic();
        }

        private void _1_2_S1_SetByteArrayPropertyServiceUriLogic()
        {
            utilitiesTests._1_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy", "TestTEMP", "TheBytes");
        }

        [TestMethod]
        public void _1_2_S2_ByteArrayPropertyServiceStart()
        {
            _1_2_S2_ByteArrayPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S2_ByteArrayPropertyServiceStartLogic()
        {
            _1_2_S1_SetByteArrayPropertyServiceUriLogic();
            ByteArrayPropertyServiceStart();
        }

        [TestMethod]
        public void _1_2_S3_ByteArrayPropertyValueInserts()
        {
            _1_2_S3_ByteArrayPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S3_ByteArrayPropertyValueInsertsLogic()
        {
            _1_2_S2_ByteArrayPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            ByteArrayPropertyValueInserts();
        }

        [TestMethod]
        public void _1_2_S4_ByteArrayPropertyValueServiceGet()
        {
            _1_2_S4_ByteArrayPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_2_S4_ByteArrayPropertyValueServiceGetLogic()
        {
            _1_2_S3_ByteArrayPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            ByteArrayPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _1_3_S1_SetByteArrayPropertyServiceUri()
        {
            _1_3_S1_SetByteArrayPropertyServiceUriLogic();
        }

        private void _1_3_S1_SetByteArrayPropertyServiceUriLogic()
        {
            utilitiesTests._1_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("net.pipe", "daisy.colum.edu", "TestTEMP", "TheBytes");
        }

        [TestMethod]
        public void _1_3_S2_ByteArrayPropertyServiceStart()
        {
            _1_3_S2_ByteArrayPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S2_ByteArrayPropertyServiceStartLogic()
        {
            _1_3_S1_SetByteArrayPropertyServiceUriLogic();
            ByteArrayPropertyServiceStart();
        }

        [TestMethod]
        public void _1_3_S3_ByteArrayPropertyValueInserts()
        {
            _1_3_S3_ByteArrayPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S3_ByteArrayPropertyValueInsertsLogic()
        {
            _1_3_S2_ByteArrayPropertyServiceStartLogic();
            dataModelTests._1_1_S1_CreateDataModelLogic();
            ByteArrayPropertyValueInserts();
        }

        [TestMethod]
        public void _1_3_S4_ByteArrayPropertyValueServiceGet()
        {
            _1_3_S4_ByteArrayPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _1_3_S4_ByteArrayPropertyValueServiceGetLogic()
        {
            _1_3_S3_ByteArrayPropertyValueInsertsLogic();
            Binding binding = new NetNamedPipeBinding();
            ByteArrayPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S1_SetByteArrayPropertyServiceUri()
        {
            _2_1_S1_SetByteArrayPropertyServiceUriLogic();
        }

        private void _2_1_S1_SetByteArrayPropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "TheBytes");
        }

        [TestMethod]
        public void _2_1_S2_ByteArrayPropertyServiceStart()
        {
            _2_1_S2_ByteArrayPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S2_ByteArrayPropertyServiceStartLogic()
        {
            _2_1_S1_SetByteArrayPropertyServiceUriLogic();
            ByteArrayPropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S3_ByteArrayPropertyValueInserts()
        {
            _2_1_S3_ByteArrayPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S3_ByteArrayPropertyValueInsertsLogic()
        {
            _2_1_S2_ByteArrayPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            ByteArrayPropertyValueInserts();
        }

        [TestMethod]
        public void _2_1_S4_ByteArrayPropertyValueServiceGet()
        {
            _2_1_S4_ByteArrayPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_ByteArrayPropertyValueServiceGetLogic()
        {
            _2_1_S3_ByteArrayPropertyValueInsertsLogic();
            Binding binding = new BasicHttpBinding();
            ByteArrayPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_2_S1_SetByteArrayPropertyServiceUri()
        {
            _2_2_S1_SetByteArrayPropertyServiceUriLogic();
        }

        private void _2_2_S1_SetByteArrayPropertyServiceUriLogic()
        {
            utilitiesTests._2_2_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy", "TestTEMP", "TheBytes");
        }

        [TestMethod]
        public void _2_2_S2_ByteArrayPropertyServiceStart()
        {
            _2_2_S2_ByteArrayPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S2_ByteArrayPropertyServiceStartLogic()
        {
            _2_2_S1_SetByteArrayPropertyServiceUriLogic();
            ByteArrayPropertyServiceStart();
        }

        [TestMethod]
        public void _2_2_S3_ByteArrayPropertyValueInserts()
        {
            _2_2_S3_ByteArrayPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S3_ByteArrayPropertyValueInsertsLogic()
        {
            _2_2_S2_ByteArrayPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            ByteArrayPropertyValueInserts();
        }

        [TestMethod]
        public void _2_2_S4_ByteArrayPropertyValueServiceGet()
        {
            _2_2_S4_ByteArrayPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_2_S4_ByteArrayPropertyValueServiceGetLogic()
        {
            _2_2_S3_ByteArrayPropertyValueInsertsLogic();
            Binding binding = new BasicHttpBinding();
            ByteArrayPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_3_S1_SetByteArrayPropertyServiceUri()
        {
            _2_3_S1_SetByteArrayPropertyServiceUriLogic();
        }

        private void _2_3_S1_SetByteArrayPropertyServiceUriLogic()
        {
            utilitiesTests._2_3_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "daisy.colum.edu", "TestTEMP", "TheBytes");
        }

        [TestMethod]
        public void _2_3_S2_ByteArrayPropertyServiceStart()
        {
            _2_3_S2_ByteArrayPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S2_ByteArrayPropertyServiceStartLogic()
        {
            _2_3_S1_SetByteArrayPropertyServiceUriLogic();
            ByteArrayPropertyServiceStart();
        }

        [TestMethod]
        public void _2_3_S3_ByteArrayPropertyValueInserts()
        {
            _2_3_S3_ByteArrayPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S3_ByteArrayPropertyValueInsertsLogic()
        {
            _2_3_S2_ByteArrayPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            ByteArrayPropertyValueInserts();
        }

        [TestMethod]
        public void _2_3_S4_ByteArrayPropertyValueServiceGet()
        {
            _2_3_S4_ByteArrayPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_3_S4_ByteArrayPropertyValueServiceGetLogic()
        {
            _2_3_S3_ByteArrayPropertyValueInsertsLogic();
            Binding binding = new BasicHttpBinding();
            ByteArrayPropertyValueServiceGet(binding);
        }
        #endregion
    }
}
