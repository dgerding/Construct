using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using Construct.MessageBrokering;
using Construct.Server.Entities;

namespace Construct.UX.ViewModels.Tests.Data
{
    [TestClass]
    public class DataViewModelTests
    {
        private static EntitiesModel context;
        private static Guid testSensorTypeSourceID, testDataTypeID, testSourceID;
        private static string testItemDefinitionXml;
        private static string testItemXml;
        private static string connectionString;
        private static string defaultDatabase = "Construct3";

        private Construct.UX.ViewModels.Data.ViewModel viewModel;

        public DataViewModelTests()
        {
            testSensorTypeSourceID = Guid.Parse("38196e9e-a581-4326-b6f2-ffffffffffff");
            testDataTypeID = Guid.Parse("0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF");
            testSourceID = Guid.Parse("3E2B7612-FE27-46D3-9A6F-B9B020A2C32B");

            testItemDefinitionXml = @"<?xml version='1.0' encoding='utf-16'?>
            <SensorTypeSource Name='TestTEMPSensor' ID='38196E9E-A581-4326-B6F2-FFFFFFFFFFFF' ParentID='5C11FBBD-9E36-4BEA-A8BE-06E225250EF8' SensorHostTypeID='EDA0FF3E-108B-45D5-BF58-F362FABF2EFE' Version='1000'>
               <DataType Name='TestTEMP' ID='0c89f6c2-c749-4085-b0ea-FFFFFFFFFFFF'>
                  <DataTypeProperty Name='TheInt' Type='int' ID='14F0E115-3EF3-4D59-882C-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='TheBool' Type='bool' ID='388A0BC9-6010-40BD-BAD0-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='TheString' Type='string' ID='92EBA461-1872-4504-A26E-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='TheBytes' Type='byte[]' ID='715E510E-D7E9-439E-BF6A-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='TheGuid' Type='Guid' ID='12E166B9-8DB7-4EF6-BAC7-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='intOne' Type='int' ID='B919F2B4-1BFC-4A2E-A9E6-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='doubleOne' Type='double' ID='D7535E8F-A3B3-43CA-B6D5-FFFFFFFFFFFF' />
                  <DataTypeProperty Name='name' Type='string' ID='3994D895-998D-49F6-A0AB-FFFFFFFFFFFF' />
                </DataType>
            </SensorTypeSource>";
            connectionString = String.Format("data source=daisy.colum.edu;initial catalog={0};User ID=Construct;Password=ConstructifyConstructifyConstructify", defaultDatabase);

            ApplicationSessionInfo applicationSessionInfo = new ApplicationSessionInfo();
            applicationSessionInfo.ConnectionString = connectionString;
            applicationSessionInfo.HostName = "localhost";
            applicationSessionInfo.Port = 8000;

            viewModel = new ViewModels.Data.ViewModel(applicationSessionInfo);
        }

        [TestMethod]
        public void AddTypeWithXMLTest()
        {
            viewModel.AddTypeWithXML(testItemDefinitionXml);
            Clean();
        }

        private void Clean()
        {
            using (context = new EntitiesModel(connectionString))
            {
                var propertyParents = context.PropertyParents.Where(p => p.ParentDataTypeID == testDataTypeID);

                context.Delete(propertyParents);
                context.SaveChanges();

                context.Delete(context.DataTypes.Where(d => d.ID == testDataTypeID));
                context.SaveChanges();
                context.Delete(context.DataTypeSources.Where(ds => ds.ID == testSensorTypeSourceID));
                context.SaveChanges();
            }
        }
    }
}
