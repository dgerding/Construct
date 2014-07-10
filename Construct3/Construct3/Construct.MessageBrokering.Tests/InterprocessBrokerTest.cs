using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Construct.MessageBrokering;
using System.Threading;

namespace Construct.MessageBrokering.Tests
{
    [TestClass]
    public class InterprocessBrokerTest
    {
        private static Guid testSenderProcessID = Guid.Parse("9A2A3E44-25AF-489E-8EE5-6CDE7C7506DD");
        private static Guid johnny5ProcessID = Guid.Parse("741009C1-B8C0-433E-95F7-C75F8995828D");
        private static Guid commandGuid = Guid.Parse("AE9E3C31-09C8-4835-8E2D-286922ADB3F6");
        private static Guid telemetryGuid = Guid.Parse("F721F879-9F84-412F-AE00-632CFEA5A1F3");

        private static Rendezvous<Command>
        commandInboxRend,
        commandOutboxRend;

        private static Rendezvous<Telemetry>
        telemetryInboxRend,
        telemetryOutboxRend;

        private static IPeer
        commandInbox,
        commandOutbox,
        telemetryInbox,
        telemetryOutbox;
        private static IPeer[] peers;
        private static Broker broker;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            commandInboxRend = new Rendezvous<Command>(Protocol.HTTP, System.Net.Dns.GetHostName(), commandGuid, testSenderProcessID);
            commandInbox = new Inbox<Command>(commandInboxRend);

            telemetryInboxRend = new Rendezvous<Telemetry>(Protocol.HTTP, System.Net.Dns.GetHostName(), telemetryGuid, testSenderProcessID);
            telemetryInbox = new Inbox<Telemetry>(telemetryInboxRend);

            commandOutboxRend = new Rendezvous<Command>(Protocol.HTTP, "johnny5", commandGuid, johnny5ProcessID);
            commandOutbox = new Outbox<Command>(commandOutboxRend);

            telemetryOutboxRend = new Rendezvous<Telemetry>(Protocol.HTTP, "johnny5", telemetryGuid, johnny5ProcessID);
            telemetryOutbox = new Outbox<Telemetry>(telemetryOutboxRend);
            peers = new IPeer[] 
            {
                commandInbox, commandOutbox,
                telemetryInbox, telemetryOutbox
            };
            broker = new Broker(peers);
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }

        [TestMethod]
        public void InterprocessCommunicate()
        {
            string telemetryReceived = null;
            string telemetryMessage = "Telemetry received";
            broker.OnTelemetryReceived += (obj, telemetry) =>
            {
                telemetryReceived = telemetryMessage;
            };

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("Uri", telemetryInboxRend.Uri.ToString());

            Command AddTelemetryOutboxCommand = new Command("AddTelemetryOutbox", args);
            broker.PublishToInbox(commandOutboxRend, AddTelemetryOutboxCommand);

            Thread.Sleep(5000);

            Assert.AreEqual(telemetryMessage, telemetryReceived);

            broker.OnTelemetryReceived -= (obj, telemetry) =>
            {
                telemetryReceived = telemetryMessage;
            };
        }
    }
}