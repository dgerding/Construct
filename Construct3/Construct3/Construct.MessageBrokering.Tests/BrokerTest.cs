using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.MessageBrokering;
using System.Threading;

namespace Construct.MessageBrokering.Tests
{
    [TestClass()]
    public class BrokerTest
    {
        private static TestContext testContextInstance;
        private static Guid SkynetHostingAddressGUID = new Guid("1F0AB154-5E32-410A-9305-630AFFB6996C");

        private static Rendezvous<Data> itemRendezvous;
        private static Rendezvous<Command> commandRendezvous;
        private static Rendezvous<Telemetry> telemetryRendezvous;

        private static IPeer
        itemInbox,
        itemOutbox,
        commandInbox,
        commandOutbox,
        telemetryInbox,
        telemetryOutbox;
        private static IPeer[] peers;
        private static Broker broker;
        private static Data testItem;
        private static SensorSerializationAssistant assistant = new SensorSerializationAssistant();

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Guid testProcessID = Guid.NewGuid();

            itemRendezvous = new Rendezvous<Data>(Protocol.HTTP, "localhost", Guid.Parse("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8"), testProcessID);
            itemInbox = new Inbox<Data>(itemRendezvous);
            itemOutbox = new Outbox<Data>(itemRendezvous);
            commandRendezvous = new Rendezvous<Command>(Protocol.HTTP, "localhost", Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6"), testProcessID);
            commandInbox = new Inbox<Command>(commandRendezvous);
            commandOutbox = new Outbox<Command>(commandRendezvous);
            telemetryRendezvous = new Rendezvous<Telemetry>(Protocol.HTTP, "localhost", Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3"), testProcessID);
            telemetryInbox = new Inbox<Telemetry>(telemetryRendezvous);
            telemetryOutbox = new Outbox<Telemetry>(telemetryRendezvous);
            peers = new IPeer[] 
            {
                itemInbox, itemOutbox,
                commandInbox, commandOutbox,
                telemetryInbox, telemetryOutbox
            };
            broker = new Broker(peers);

            testItem = new Data(5, Guid.Empty, Guid.Empty, "IntTestItem", Guid.Empty);
            assistant.SetJsonHeader(testItem);
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }

        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void OnCommandReceivedFromInboxTest()
        {
            string testValue = null;
            string expected = "Value";
            broker.OnCommandReceived += (obj, command) =>
            {
                testValue = expected;
            };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("Test", null);
            broker.Publish(new Command("command1", args));

            Thread.Sleep(5000);
            Assert.AreEqual(testValue, expected);

            broker.OnCommandReceived -= (obj, command) =>
            {
                testValue = expected;
            };
        }

        [TestMethod()]
        public void OnItemReceivedFromInboxTest()
        {
            string testValue = null;
            string expected = "Value";
            broker.OnItemReceived += (obj, item) =>
            {
                testValue = expected;
            };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("Test", null);
            broker.Publish(testItem, assistant.GetJsonHeader(testItem.DataName));

            Thread.Sleep(5000);
            Assert.AreEqual(testValue, expected);

            broker.OnItemReceived -= (obj, item) =>
            {
                testValue = expected;
            };
        }

        [TestMethod()]
        public void OnTelemetryReceivedFromInboxTest()
        {
            string testValue = null;
            string expected = "Value";
            broker.OnTelemetryReceived += (obj, telemetry) =>
            {
                testValue = expected;
            };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("Test", null);
            broker.Publish(new Telemetry("telemetry1", args));

            Thread.Sleep(5000);
            Assert.AreEqual(testValue, expected);

            broker.OnTelemetryReceived -= (obj, telemetry) =>
            {
                testValue = expected;
            };
        }

        [TestMethod()]
        public void PublishTest()
        {
            string startItemMessage = null;
            string startCommandMessage = null;
            string startTelemetryMessage = null;

            string itemMessage = "Item Received";
            string commandMessage = "Command Received";
            string telemetryMessage = "Telemetry Received";

            broker.OnItemReceived += delegate { startItemMessage = itemMessage; };
            broker.OnCommandReceived += delegate { startCommandMessage = commandMessage; };
            broker.OnTelemetryReceived += delegate { startTelemetryMessage = telemetryMessage; };

            broker.Publish(testItem, assistant.GetJsonHeader(testItem.DataName));
            broker.Publish(new Command("command1", new Dictionary<string, string>()));
            broker.Publish(new Telemetry("telemetry2", new Dictionary<string, string>()));

            Thread.Sleep(10000);

            Assert.AreEqual(itemMessage, startItemMessage);
            Assert.AreEqual(commandMessage, startCommandMessage);
            Assert.AreEqual(telemetryMessage, startTelemetryMessage);

            broker.OnItemReceived -= delegate { startItemMessage = itemMessage; };
            broker.OnCommandReceived -= delegate { startCommandMessage = commandMessage; };
            broker.OnTelemetryReceived -= delegate { startTelemetryMessage = telemetryMessage; };
        }

        [TestMethod()]
        public void PublishToInboxTest()
        {
            string startItemMessage = null;
            string startCommandMessage = null;
            string startTelemetryMessage = null;

            string itemMessage = "Item received";
            string commandMessage = "Message received";
            string telemetryMessage = "Telemetry received";

            broker.OnItemReceived += delegate { startItemMessage = itemMessage; };
            broker.OnCommandReceived += delegate { startCommandMessage = commandMessage; };
            broker.OnTelemetryReceived += delegate { startTelemetryMessage = telemetryMessage; };

            broker.PublishToInbox(itemRendezvous, testItem, assistant.GetJsonHeader(testItem.DataName));
            broker.PublishToInbox(commandRendezvous, new Command("command1", new Dictionary<string, string>()));
            broker.PublishToInbox(telemetryRendezvous, new Telemetry("telemetry2", new Dictionary<string, string>()));

            Thread.Sleep(10000);

            Assert.AreEqual(startItemMessage, itemMessage);
            Assert.AreEqual(startCommandMessage, commandMessage);
            Assert.AreEqual(startTelemetryMessage, telemetryMessage);

            broker.OnItemReceived -= delegate { startItemMessage = itemMessage; };
            broker.OnCommandReceived -= delegate { startCommandMessage = commandMessage; };
            broker.OnTelemetryReceived -= delegate { startTelemetryMessage = telemetryMessage; };
        }
    }
}