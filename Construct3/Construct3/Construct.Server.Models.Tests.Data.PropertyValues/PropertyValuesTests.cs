using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValues.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.Utilities.Shared;
using Construct.Utilities.Tests.Shared;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Entities.Tests;
using System.ServiceModel.Channels;
using System.Collections.Generic;

namespace Construct.Server.Models.Tests.Data.PropertyValue
{
    public class PropertyValuesTests
    {
        protected DataModelTests dataModelTests = new DataModelTests();
        protected SharedUtilitiesTests utilitiesTests = new SharedUtilitiesTests();
        protected Uri propertyServiceUri = null;
        protected EndpointAddress address = null;
        protected ServiceHost host = null;
        protected DataType dataType = null;
        protected PropertyType propertyType = null;
        protected static Entities.EntitiesModel context = null;
        protected static string connectionString = null;

        protected void SetPropertyServiceUri(string scheme, string hostname, string datatypeName, string propertyName)
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

        protected void Clean()
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
