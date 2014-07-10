using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.Server.Models.Visualizations;
using Construct.MessageBrokering.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Construct.Server.Entities;

namespace Construct.Server.Models.Tests.Visualizations
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            string environment = Environment.CurrentDirectory;
            string path = this.GetType().Assembly.Location;
            string location = Assembly.GetExecutingAssembly().Location;

            Entities.EntitiesModel model = new Entities.EntitiesModel();
            ConstructSerializationAssistant assistant = new ConstructSerializationAssistant();
            
            Guid sourceID = Guid.NewGuid();
            Guid itemID = Guid.NewGuid();

            //Guid sourceInstanceID = Guid.Parse("8cc2709c-8c40-48ca-acd2-7fc844f0b1ff");

            Guid dataTypeID = Guid.Parse("1294B397-F52F-4830-A109-E1D9C2A6BE37");
            Guid dataTypeSourceID = Guid.Parse("064A944C-9347-4E0A-9642-744F80D7BD8F");
            string dataName = "Supervised_Machine_Learning_Label";

            foreach (DataType dataType in model.DataTypes)
            {
                AddPropertyIDHelper(assistant, dataType);
            }
            //Guid unknownGuid = Guid.Parse("f02b7baa-58f9-4790-99cb-4ec7015a53eb");

            if (model.Sources.Count(target => target.ID == sourceID) == 0)
            {
                model.Add(new Entities.Source
                {
                    ID = sourceID,
                    DataTypeSourceID = dataTypeSourceID
                });
                model.SaveChanges();
            }

            LabeledItemAdapter adapter = new LabeledItemAdapter
            {
                LabeledItemID = itemID,
                LabeledInterval = 0,
                LabeledPropertyID = dataTypeID,
                LabeledSourceID = sourceID,
                LabeledStartTime = DateTime.UtcNow,
                SessionID = model.Sessions.FirstOrDefault().ID,
                TaxonomyLabelID = model.TaxonomyLabels.FirstOrDefault().ID
            };

            MessageBrokering.Data dataItem = new MessageBrokering.Data
            (
                adapter,
                sourceID, 
                dataTypeSourceID, 
                dataName, 
                dataTypeID
            );

            string jsonHeaderData = "{ 'PayloadTypes' : {'LabeledInterval' : 'Int32', 'LabeledItemID' : 'Guid', 'LabeledPropertyID' : 'Guid', 'LabeledSourceID' : 'Guid', 'LabeledStartTime' : 'DateTime', 'SessionID' : 'Guid', 'TaxonomyLabelID' : 'Guid'},'Instance' :";
            string theDataString = String.Format("{0}{1}", jsonHeaderData, JsonConvert.SerializeObject(dataItem));
            Entities.Item header = assistant.GetItemHeader(theDataString, itemID);

            model.Add(header);
            model.SaveChanges();

            assistant.Persist(theDataString, itemID);
        }

        private static void AddPropertyIDHelper(ConstructSerializationAssistant assistant, DataType dataType)
        {
            var tempDict = new Dictionary<string, Guid>();
            foreach (PropertyParent prop in dataType.PropertyParents)
            {
                string capitalizedPropName = String.Format("{0}{1}", prop.Name.Substring(0, 1).ToUpper(), prop.Name.Substring(1, prop.Name.Length - 1));
                tempDict.Add(capitalizedPropName, prop.ID);
            }
            if (tempDict.Count != 0)
            {
                assistant.AddPropertyIDTable(dataType.Name, tempDict);
            }
        }
    }
}