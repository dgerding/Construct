using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Construct.Server.Models.Data.PropertyValues.Services;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities;
using Construct.Utilities.Tests.Shared;
using System.Configuration;
using Construct.Server.Entities.Tests;
using Construct.Utilities.Shared;
using Construct.Server.Models.Tests.Data.PropertyValue;

namespace Construct.Server.Models.Tests.Data.PropertyValues
{
    [TestClass]
    public class GuidPropertyValueTest
    {
        private DataModelTests dataModelTests = new DataModelTests();
        private SharedUtilitiesTests utilitiesTests = new SharedUtilitiesTests();
        private Uri propertyServiceUri = null;
        private EndpointAddress address = null;
        private ServiceHost host = null;
        private DataType dataType = null;
        private PropertyType propertyType = null;
        private static Entities.EntitiesModel context = null;
        private static string connectionString = null;

        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            DataModelTests.ClassSetup(a);

            connectionString = ConfigurationManager.ConnectionStrings[EntityTests.connectionStringName].ConnectionString;
            context = new EntitiesModel(EntityTests.connectionStringName);
        }

        [TestMethod]
        public void _2_1_S4_GuidPropertyValueServiceGet()
        {
            _2_1_S4_GuidPropertyValueServiceGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_GuidPropertyValueServiceGetLogic()
        {
            _2_1_S3_GuidPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            GuidPropertyValueServiceGet(binding);
        }

        [TestMethod]
        public void _2_1_S4_GuidPropertyValueModelGet()
        {
            _2_1_S4_GuidPropertyValueModelGetLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S4_GuidPropertyValueModelGetLogic()
        {
            _2_1_S3_GuidPropertyValueInsertsLogic();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            GuidPropertyValueModelGet(binding);
        }

        private void GuidPropertyValueModelGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            GuidPropertyValueService serviceModel;

            Entities.DataType dataType = context.DataTypes.Where(target => target.ID == dataModelTests.dataType.ID).Single();
            Entities.PropertyType propertyType = context.PropertyTypes.Where(target => target.ID == dataModelTests.theGuidProperty.ID).Single();
            serviceModel = new GuidPropertyValueService
            (
                propertyServiceUri,
                dataType,
                propertyType,
                GuidPropertyValueTest.connectionString
            );

            IEnumerable<GuidPropertyValue> guidPropertyValues;
            try
            {
                guidPropertyValues = serviceModel.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(guidPropertyValues);
            Assert.IsTrue(guidPropertyValues.Count() > 0);
        }

        private void GuidPropertyValueServiceGet(Binding binding)
        {
            Assert.IsNotNull(binding);
            address = new EndpointAddress(propertyServiceUri);

            Assert.IsNotNull(host);
            Assert.IsTrue(host.BaseAddresses.Contains(propertyServiceUri));
            Assert.IsFalse(host.State == CommunicationState.Faulted);
            Assert.IsTrue(host.State == CommunicationState.Created || host.State == CommunicationState.Opened);

            ChannelFactory<IGuidPropertyValueService> factory = new ChannelFactory<IGuidPropertyValueService>(binding, address);
            IGuidPropertyValueService serviceClient = factory.CreateChannel();
            IEnumerable<GuidPropertyValue> guidPropertyValues = null;
            try
            {
                //var temp = serviceClient.GetAll();
                guidPropertyValues = serviceClient.GetAll();
            }
            catch (Exception e)
            {
                throw;
            }
            Assert.IsNotNull(guidPropertyValues);
            Assert.IsTrue(guidPropertyValues.Count() > 0);
        }

        [TestMethod]
        public void _2_1_S3_GuidPropertyValueInserts()
        {
            _2_1_S3_GuidPropertyValueInsertsLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S3_GuidPropertyValueInsertsLogic()
        {
            _2_1_S2_GuidPropertyServiceStartLogic();
            dataModelTests._2_1_S1_CreateDataModelLogic();
            GuidPropertyValueInserts();
        }

        private void GuidPropertyValueInserts()
        {
            Random random = new Random();

            GuidPropertyValue value = new GuidPropertyValue()
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
                Value = Guid.NewGuid()
            };

            dataModelTests.model.AddGuidPropertyValue(Guid.NewGuid(), "TestTEMP", "TheGuid", value);
        }

        [TestMethod]
        public void _2_1_S2_GuidPropertyServiceStart()
        {
            _2_1_S2_GuidPropertyServiceStartLogic();
            host.Close();
            Clean();
        }

        private void _2_1_S2_GuidPropertyServiceStartLogic()
        {
            _2_1_S1_SetGuidPropertyServiceUriLogic();
            GuidPropertyServiceStart();
        }

        [TestMethod]
        public void _2_1_S1_SetGuidPropertyServiceUri()
        {
            _2_1_S1_SetGuidPropertyServiceUriLogic();
        }

        private void _2_1_S1_SetGuidPropertyServiceUriLogic()
        {
            utilitiesTests._2_1_S1_CreateStandardConstructServerEndpointUri();
            SetPropertyServiceUri("http", "localhost", "TestTEMP", "TheGuid");
        }

        private void SetPropertyServiceUri(string scheme, string hostname, string datatypeName, string propertyName)
        {
            propertyServiceUri = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(utilitiesTests.serverEndpoint, datatypeName, propertyName);

            Assert.IsNotNull(propertyServiceUri);
            Assert.AreEqual(scheme, propertyServiceUri.Scheme);
            Assert.AreEqual(hostname, propertyServiceUri.Host);
            Assert.AreEqual("/", propertyServiceUri.Segments[0]);
            Assert.AreEqual("00000000-0000-0000-0000-000000000000/", propertyServiceUri.Segments[1]);
            Assert.AreEqual("TestTEMP/", propertyServiceUri.Segments[2]);
            Assert.AreEqual(propertyName, propertyServiceUri.Segments[3]);
            Assert.IsTrue(propertyServiceUri.Segments.Count() == 4);
        }

        private void GuidPropertyServiceStart()
        {
            dataModelTests._1_1_S4_InsertTestItemDataTypeLogic();

            dataType = context.DataTypes.Single(type => type.ID == dataModelTests.dataType.ID);
            propertyType = context.PropertyTypes.Single(propType => propType.ID == dataModelTests.theGuidProperty.ID);

            host = PropertyServiceManager.StartService(utilitiesTests.serverEndpoint, dataType, propertyType, DataModelTests.connectionString);

            Assert.AreEqual(host.State, CommunicationState.Opened);
        }

        private void Clean()
        {
            var propertyParents = context.PropertyParents.Where(p => p.ParentDataTypeID == dataModelTests.testDataTypeID);

            context.Delete(propertyParents);
            context.SaveChanges();

            context.Delete(context.DataTypes.Where(d => d.ID == dataModelTests.testDataTypeID));
            context.SaveChanges();
            context.Delete(context.DataTypeSources.Where(ds => ds.ID == dataModelTests.testSensorTypeSourceID));
            context.SaveChanges();
        }
    }
}
