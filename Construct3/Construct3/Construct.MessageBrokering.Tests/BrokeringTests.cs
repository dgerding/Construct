using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Construct.MessageBrokering.Tests
{
    [TestClass]
    public class BrokeringTests
    {
        public Broker broker = null;

        public Inbox<Data> itemInbox = null;
        public Outbox<Data> itemOutbox = null;
        public Rendezvous<Data> itemRendezvous = null;

        public Inbox<Command> commandInbox = null;
        public Outbox<Command> commandOutbox = null;
        public Rendezvous<Command> commandRendezvous = null;

        public Inbox<Telemetry> telemetryInbox = null;
        public Outbox<Telemetry> telemetryOutbox = null;
        public Rendezvous<Telemetry> telemetryRendezvous = null;

        Guid testProcessID = Guid.NewGuid();
        public IPeer[] peers = null;

        private void _1_1_1_S1_SetCommunicationChannels<T>(string guid, out Rendezvous<T> rendezvous, out Outbox<T> outbox, out Inbox<T> inbox)
            where T : Message
        {
            rendezvous = new Rendezvous<T>(Protocol.HTTP, "localhost", Guid.Parse(guid), testProcessID);
            inbox = new Inbox<T>(rendezvous);
            outbox = new Outbox<T>(rendezvous);
        }
        private void _1_1_2_S1_SetCommunicationChannels<T>(string guid, out Rendezvous<T> rendezvous, out Outbox<T> outbox, out Inbox<T> inbox)
            where T : Message
        {
            rendezvous = new Rendezvous<T>(Protocol.HTTP, "daisy", Guid.Parse(guid), testProcessID);
            inbox = new Inbox<T>(rendezvous);
            outbox = new Outbox<T>(rendezvous);
        }
        private void _1_1_3_S1_SetCommunicationChannels<T>(string guid, out Rendezvous<T> rendezvous, out Outbox<T> outbox, out Inbox<T> inbox)
            where T : Message
        {
            rendezvous = new Rendezvous<T>(Protocol.HTTP, "daisy.colum.edu", Guid.Parse(guid), testProcessID);
            inbox = new Inbox<T>(rendezvous);
            outbox = new Outbox<T>(rendezvous);
        }
        private void _1_2_1_S1_SetCommunicationChannels<T>(string guid, out Rendezvous<T> rendezvous, out Outbox<T> outbox, out Inbox<T> inbox)
            where T : Message
        {
            rendezvous = new Rendezvous<T>(Protocol.NetNamedPipes, "localhost", Guid.Parse(guid), testProcessID);
            inbox = new Inbox<T>(rendezvous);
            outbox = new Outbox<T>(rendezvous);
        }
        private void _1_2_2_S1_SetCommunicationChannels<T>(string guid, out Rendezvous<T> rendezvous, out Outbox<T> outbox, out Inbox<T> inbox)
            where T : Message
        {
            rendezvous = new Rendezvous<T>(Protocol.NetNamedPipes, "daisy", Guid.Parse(guid), testProcessID);
            inbox = new Inbox<T>(rendezvous);
            outbox = new Outbox<T>(rendezvous);
        }
        private void _1_2_3_S1_SetCommunicationChannels<T>(string guid, out Rendezvous<T> rendezvous, out Outbox<T> outbox, out Inbox<T> inbox)
            where T : Message
        {
            rendezvous = new Rendezvous<T>(Protocol.NetNamedPipes, "daisy.colum.edu", Guid.Parse(guid), testProcessID);
            inbox = new Inbox<T>(rendezvous);
            outbox = new Outbox<T>(rendezvous);
        }

        private void InitializePeers()
        {
            peers = new IPeer[] 
            {
                itemInbox, itemOutbox,
                commandInbox, commandOutbox,
                telemetryInbox, telemetryOutbox
            };

            foreach (IInbox inbox in peers.OfType<IInbox>())
            {
                Assert.IsFalse(inbox.State == CommunicationState.Faulted);
                Assert.IsFalse(inbox.State == CommunicationState.Closing);
                Assert.IsFalse(inbox.State == CommunicationState.Closed);
            }
        }
        [TestMethod]
        public void _1_1_1_S1_InitializePeers()
        {
            _1_1_1_S1_InitializePeersLogic();
        }

        private void _1_1_1_S1_InitializePeersLogic()
        {
            _1_1_1_S1_SetCommunicationChannels("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8", out itemRendezvous, out itemOutbox, out itemInbox);
            _1_1_1_S1_SetCommunicationChannels("AE9E3C31-09C8-4835-8E2D-286922ADB3F6", out commandRendezvous, out commandOutbox, out commandInbox);
            _1_1_1_S1_SetCommunicationChannels("F721F879-9F84-412F-AE00-632CFEA5A1F3", out telemetryRendezvous, out telemetryOutbox, out telemetryInbox);
            InitializePeers();
        }
        [TestMethod]
        public void _1_1_2_S1_InitializePeers()
        {
            _1_1_2_S1_InitializePeersLogic();
        }

        private void _1_1_2_S1_InitializePeersLogic()
        {
            _1_1_2_S1_SetCommunicationChannels("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8", out itemRendezvous, out itemOutbox, out itemInbox);
            _1_1_2_S1_SetCommunicationChannels("AE9E3C31-09C8-4835-8E2D-286922ADB3F6", out commandRendezvous, out commandOutbox, out commandInbox);
            _1_1_2_S1_SetCommunicationChannels("F721F879-9F84-412F-AE00-632CFEA5A1F3", out telemetryRendezvous, out telemetryOutbox, out telemetryInbox);
            InitializePeers();
        }
        [TestMethod]
        public void _1_1_3_S1_InitializePeers()
        {
            _1_1_3_S1_InitializePeersLogic();
        }
  
        private void _1_1_3_S1_InitializePeersLogic()
        {
            _1_1_3_S1_SetCommunicationChannels("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8", out itemRendezvous, out itemOutbox, out itemInbox);
            _1_1_3_S1_SetCommunicationChannels("AE9E3C31-09C8-4835-8E2D-286922ADB3F6", out commandRendezvous, out commandOutbox, out commandInbox);
            _1_1_3_S1_SetCommunicationChannels("F721F879-9F84-412F-AE00-632CFEA5A1F3", out telemetryRendezvous, out telemetryOutbox, out telemetryInbox);
            InitializePeers();
        }

        [TestMethod]
        public void _1_2_1_S1_InitializePeers()
        {
            _1_2_1_S1_InitializePeersLogic();
        }
  
        private void _1_2_1_S1_InitializePeersLogic()
        {
            _1_2_1_S1_SetCommunicationChannels("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8", out itemRendezvous, out itemOutbox, out itemInbox);
            _1_2_1_S1_SetCommunicationChannels("AE9E3C31-09C8-4835-8E2D-286922ADB3F6", out commandRendezvous, out commandOutbox, out commandInbox);
            _1_2_1_S1_SetCommunicationChannels("F721F879-9F84-412F-AE00-632CFEA5A1F3", out telemetryRendezvous, out telemetryOutbox, out telemetryInbox);
            InitializePeers();
        }

        [TestMethod]
        public void _1_2_2_S1_InitializePeers()
        {
            _1_2_2_S1_InitializePeersLogic();
        }
  
        private void _1_2_2_S1_InitializePeersLogic()
        {
            _1_2_2_S1_SetCommunicationChannels("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8", out itemRendezvous, out itemOutbox, out itemInbox);
            _1_2_2_S1_SetCommunicationChannels("AE9E3C31-09C8-4835-8E2D-286922ADB3F6", out commandRendezvous, out commandOutbox, out commandInbox);
            _1_2_2_S1_SetCommunicationChannels("F721F879-9F84-412F-AE00-632CFEA5A1F3", out telemetryRendezvous, out telemetryOutbox, out telemetryInbox);
            InitializePeers();
        }

        [TestMethod]
        public void _1_2_3_S1_InitializePeers()
        {
            _1_2_3_S1_InitializePeersLogic();
        }
  
        private void _1_2_3_S1_InitializePeersLogic()
        {
            _1_2_3_S1_SetCommunicationChannels("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8", out itemRendezvous, out itemOutbox, out itemInbox);
            _1_2_3_S1_SetCommunicationChannels("AE9E3C31-09C8-4835-8E2D-286922ADB3F6", out commandRendezvous, out commandOutbox, out commandInbox);
            _1_2_3_S1_SetCommunicationChannels("F721F879-9F84-412F-AE00-632CFEA5A1F3", out telemetryRendezvous, out telemetryOutbox, out telemetryInbox);
            InitializePeers();
        }

        private void InitializeBroker()
        {
            Assert.IsNotNull(peers);
            broker = new Broker(peers);

            Assert.IsNotNull(broker.BrokerID);
            Assert.IsFalse(broker.IsFaulted);
            Assert.IsTrue(broker.IsReady);

            foreach (IPeer peer in peers)
            {
                Assert.IsTrue(broker.Peers.Contains(peer));
            }

        }
        [TestMethod]
        public void _1_1_1_S2_InitializeBroker()
        {
            _1_1_1_S2_InitializeBrokerLogic();
        }
  
        private void _1_1_1_S2_InitializeBrokerLogic()
        {
            _1_1_1_S1_InitializePeersLogic();
            InitializeBroker();
        }

        [TestMethod]
        public void _1_1_2_S2_InitializeBroker()
        {
            _1_1_2_S2_InitializeBrokerLogic();
        }
  
        private void _1_1_2_S2_InitializeBrokerLogic()
        {
            _1_1_2_S1_InitializePeersLogic();
            InitializeBroker();
        }

        [TestMethod]
        public void _1_1_3_S2_InitializeBroker()
        {
            _1_1_3_S2_InitializeBrokerLogic();
        }
  
        private void _1_1_3_S2_InitializeBrokerLogic()
        {
            _1_1_3_S1_InitializePeersLogic();
            InitializeBroker();
        }

        [TestMethod]
        public void _1_2_1_S2_InitializeBroker()
        {
            _1_2_1_S2_InitializeBrokerLogic();
        }
  
        private void _1_2_1_S2_InitializeBrokerLogic()
        {
            _1_2_1_S1_InitializePeersLogic();
            InitializeBroker();
        }

        [TestMethod]
        public void _1_2_2_S2_InitializeBroker()
        {
            _1_2_2_S2_InitializeBrokerLogic();
        }
  
        private void _1_2_2_S2_InitializeBrokerLogic()
        {
            _1_2_2_S1_InitializePeersLogic();
            InitializeBroker();
        }

        [TestMethod]
        public void _1_2_3_S2_InitializeBroker()
        {
            _1_2_3_S2_InitializeBrokerLogic();
        }
  
        private void _1_2_3_S2_InitializeBrokerLogic()
        {
            _1_2_3_S1_InitializePeersLogic();
            InitializeBroker();
        }

    }
}
