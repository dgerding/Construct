using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Construct.MessageBrokering
{
    public class Broker
    {
        public event System.Action<object, string> OnCommandReceived;
        public event System.Action<object, string> OnTelemetryReceived;
        public event System.Action<object, string> OnItemReceived;

        private List<IPeer> peers = new List<IPeer>();
        public List<IPeer> Peers
        {
            get
            {
                return peers;
            }
        }

        private Guid? brokerID;
        public Guid BrokerID
        {
            get
            {
                return brokerID.Value;
            }
        }

        public Broker(IPeer[] myInitialPeers, Guid? id = null)
        {
            brokerID = id ?? Guid.NewGuid();
            if ((CheckHasValue(myInitialPeers) == false))
            {
                throw new ApplicationException("Broker must be provided at least 1 initial inbox or outbox");
            }

            PrepareInitialPeers(myInitialPeers);
            IsReady = true;
        }

        public Broker(IPeer initialPeer) : this(new IPeer[] { initialPeer })
        {
        }

        public Broker()
        {
            brokerID = Guid.NewGuid();
            IsReady = false;
        }

        private void PrepareInitialPeers(IPeer[] myInitialPeers)
        {
            if (CheckHasValue(myInitialPeers))
            {
                arePeersReady = AddPeers(myInitialPeers);
            }
        }

        private bool CheckHasValue(IPeer[] boxCollection)
        {
            return boxCollection != null && boxCollection.Length != 0;
        }

        private void OnCommandReceivedFromInbox(object s, string e)
        {
            if (OnCommandReceived != null)
                OnCommandReceived(this, e);
        }

        private void OnItemReceivedFromInbox(object s, string e)
        {
            if (OnItemReceived != null)
                OnItemReceived(this, e);
        }

        private void OnTelemetryReceivedFromInbox(object s, string e)
        {
            if (OnTelemetryReceived != null)
                OnTelemetryReceived(this, e);
        }

        public bool IsReady { get; private set; }
        public bool IsFaulted { get; private set; }

        #region Cache and Persistence Members

        //private Cache cache;
        //private readonly Persistor persistor = null;

        #endregion

        #region Peering Members

        private List<IPeer> formerPeers = new List<IPeer>();
        private bool arePeersReady = false;

        #endregion

        #region Add Peer methods

        public bool AddPeers(IEnumerable<IPeer> somePeers)
        {
            foreach (IPeer peer in somePeers)
            {
                if (!AddPeer(peer))
                {
                    return false;
                }
            }
            return true;
        }

        //TODO: prevent the adding of 2 inboxes with same rendezvous
        public bool AddPeer(IPeer peer)
        {
            //lock (peers)
            //{
            IEnumerable<Outbox<Telemetry>> telemetryOutboxes = peers.OfType<Outbox<Telemetry>>();
            IEnumerable<Inbox<Telemetry>> telemetryInboxes = peers.OfType<Inbox<Telemetry>>();
            IEnumerable<Outbox<Command>> commandOutboxes = peers.OfType<Outbox<Command>>();
            IEnumerable<Inbox<Command>> commandInboxes = peers.OfType<Inbox<Command>>();
            IEnumerable<Outbox<Data>> dataOutboxes = peers.OfType<Outbox<Data>>();
            IEnumerable<Inbox<Data>> dataInboxes = peers.OfType<Inbox<Data>>();

            if (peer is Inbox<Command> && commandInboxes.Count() == 0)
            {
                (peer as Inbox<Command>).OnObjectReceived += OnCommandReceivedFromInbox;
                peers.Add(peer);
            }
            if (peer is Inbox<Telemetry> && telemetryInboxes.Count() == 0)
            {
                (peer as Inbox<Telemetry>).OnObjectReceived += OnTelemetryReceivedFromInbox;
                peers.Add(peer);
            }
            if (peer is Inbox<Data>)
            {
                bool addFlag = true;
                foreach (Inbox<Data> inbox in dataInboxes)
                {
                    if ((peer as Inbox<Data>).CurrentRendezvous == inbox.CurrentRendezvous)
                    {
                        addFlag = false;
                        break;
                    }
                }
                if (addFlag)
                {
                    (peer as Inbox<Data>).OnObjectReceived += OnItemReceivedFromInbox;
                    peers.Add(peer);
                }
            }

            if (peer is Outbox<Command> && commandOutboxes.Count() == 0)
                peers.Add(peer);
            else if (peer is Outbox<Command> && commandOutboxes.Count() > 0)
            {
                foreach (Rendezvous<Command> rendezvous in (peer as Outbox<Command>).AllRendezvous)
                {
                    if (!commandOutboxes.Single().AllRendezvous.Contains(rendezvous))
                    {
                        commandOutboxes.Single().AddRendezvous(rendezvous);
                    }
                }
            }

            if (peer is Outbox<Command> && commandOutboxes.Count() == 0)
                peers.Add(peer);
            else if (peer is Outbox<Command> && commandOutboxes.Count() > 0)
            {
                foreach (Rendezvous<Command> rendezvous in (peer as Outbox<Command>).AllRendezvous)
                {
                    if (!commandOutboxes.Single().AllRendezvous.Contains(rendezvous))
                    {
                        commandOutboxes.Single().AddRendezvous(rendezvous);
                    }
                }
            }

            if (peer is Outbox<Telemetry> && telemetryOutboxes.Count() == 0)
                peers.Add(peer);
            else if (peer is Outbox<Telemetry> && telemetryOutboxes.Count() > 0)
            {
                foreach (Rendezvous<Telemetry> rendezvous in (peer as Outbox<Telemetry>).AllRendezvous)
                {
                    if (!telemetryOutboxes.Single().AllRendezvous.Contains(rendezvous))
                    {
                        telemetryOutboxes.Single().AddRendezvous(rendezvous);
                    }
                }
            }

            //TODO: lock implementation, come back to this when we fix SensorList and SensorHostList threading
            //if (peer is Outbox<Telemetry> && telemetryOutboxes.Count() == 0)
            //{
            //    peers.Add(peer);
            //}
            //else if (peer is Outbox<Telemetry> && telemetryOutboxes.Count() == 1)
            //{
            //    IEnumerable<Rendezvous<Telemetry>> rendezvousCollection = null;
            //    Outbox<Telemetry> telemetryOutbox = null;
            //    lock (telemetryOutbox = peer as Outbox<Telemetry>)
            //    {
            //        lock (rendezvousCollection = telemetryOutbox.AllRendezvous)
            //        {
            //            System.Threading.ThreadSafe.ForEach<Rendezvous<Telemetry>>(rendezvousCollection, (Rendezvous<Telemetry> rendezvous) =>
            //            {
            //                if (!telemetryOutbox.AllRendezvous.Contains(rendezvous))
            //                {
            //                    telemetryOutbox.AddRendezvous(rendezvous);
            //                }
            //            });
            //        }
            //    }
            //}
            //else
            //{
            //    throw new ApplicationException("More than one outbox of type \"Telemetry\" exists within the broker.");
            //}

            //TODO: This forces all outbox<Item> instances to merge in a broker because I cant think of a way to distinguish
            // between wanting to output the same item type to a new place, and wanting to output a new item type to a place
            if (peer is Outbox<Data> && peers.OfType<Outbox<Data>>().Count() == 0)
                peers.Add(peer);
            else if (peer is Outbox<Data> && peers.OfType<Outbox<Data>>().Count() > 0)
            {
                foreach (Rendezvous<Data> rendezvous in (peer as Outbox<Data>).AllRendezvous)
                {
                    if (!peers.OfType<Outbox<Data>>().Single().AllRendezvous.Contains(rendezvous))
                    {
                        peers.OfType<Outbox<Data>>().Single().AddRendezvous(rendezvous);
                    }
                }
            }
            return true;
        }

        #endregion

        #region Publish overloads
        public void Publish(Data theData, string dataHeader)
        {
            //hack in brokerID string
            theData.BrokerID = this.BrokerID;
			//	Extra }} (escaped brace) to close the open brace in dataHeader
            string theDataString = String.Format("{0}{1}", dataHeader, PackageObjectAsJson(theData));

			//	Complex (non-primitive) types will be a JSON object rather than a value, which require an extra closing brace
			//		to be correct JSON. (Otherwise there will be a missing close brace)
	        if (!theData.Payload.GetType().IsPrimitive)
		        theDataString += "}";

            List<Outbox<Data>> tempPeers = peers.OfType<Outbox<Data>>().ToList();
            foreach (Outbox<Data> outbox in tempPeers)
            {
                outbox.AddObject(theDataString);
            }
        }

        public void Publish(Telemetry theTelemetry)
        {
            theTelemetry.BrokerID = this.BrokerID;
            string theTelemetryString = PackageObjectAsJson(theTelemetry);

            List<Outbox<Telemetry>> tempPeers = peers.OfType<Outbox<Telemetry>>().ToList();
            foreach (Outbox<Telemetry> outbox in tempPeers)
            {
                outbox.AddObject(theTelemetryString);
            }
        }

        public void Publish(Command theCommand)
        {
            theCommand.BrokerID = this.BrokerID;
            string theCommandString = PackageObjectAsJson(theCommand);

            List<Outbox<Command>> tempPeers = peers.OfType<Outbox<Command>>().ToList();
            foreach (Outbox<Command> outbox in tempPeers)
            {
                outbox.AddObject(theCommandString);
            }
        }

        #endregion

        #region PublishToInbox overloads
        public void PublishToInbox(Rendezvous<Data> targetRendezvous, Data theData, string dataHeader)
        {
            // hack in brokerID string
            theData.BrokerID = this.BrokerID;
            string theDataString = String.Format("{0}{1}", dataHeader, PackageObjectAsJson(theData));

            List<Outbox<Data>> tempPeers = peers.OfType<Outbox<Data>>().ToList();
            foreach (Outbox<Data> outbox in tempPeers)
            {
                List<Rendezvous<Data>> tempRendezvous = new List<Rendezvous<Data>>(outbox.AllRendezvous);
                foreach (Rendezvous<Data> rendezvous in tempRendezvous)
                {
                    if (rendezvous == targetRendezvous)
                    {
                        outbox.AddObjectTo(targetRendezvous, theDataString);
                    }
                }
            }
        }

        public void PublishToInbox(Rendezvous<Telemetry> targetRendezvous, Telemetry theTelemetry)
        {
            theTelemetry.BrokerID = this.BrokerID;
            string theTelemetryString = PackageObjectAsJson(theTelemetry);

            List<Outbox<Telemetry>> tempPeers = peers.OfType<Outbox<Telemetry>>().ToList();
            foreach (Outbox<Telemetry> outbox in tempPeers)
            {
                List<Rendezvous<Telemetry>> tempRendezvous = new List<Rendezvous<Telemetry>>(outbox.AllRendezvous);
                foreach (Rendezvous<Telemetry> rendezvous in tempRendezvous)
                {
                    if (rendezvous == targetRendezvous)
                    {
                        outbox.AddObjectTo(targetRendezvous, theTelemetryString);
                    }
                }
            }
        }

        public void PublishToInbox(Rendezvous<Command> targetRendezvous, Command theCommand)
        {
            theCommand.BrokerID = this.BrokerID;
            string theCommandString = PackageObjectAsJson(theCommand);

            List<Outbox<Command>> tempPeers = peers.OfType<Outbox<Command>>().ToList();
            foreach (Outbox<Command> outbox in tempPeers)
            {
                List<Rendezvous<Command>> tempRendezvous = new List<Rendezvous<Command>>(outbox.AllRendezvous);
                foreach (Rendezvous<Command> rendezvous in tempRendezvous)
                {
                    if (rendezvous == targetRendezvous)
                    {
                        outbox.AddObjectTo(targetRendezvous, theCommandString);
                    }
                }
            }
        }

        #endregion

        public void PersistJson(string json)
        {
            OnItemReceived(this, json);
        }

        #region PackageObjectAsJsons overloads
        private string PackageObjectAsJson<T>(T theData)
            where T : Message
        {
            string serializedMessage = JsonConvert.SerializeObject(theData);
            return serializedMessage;
        }

        #endregion

        #region Not Implemented broker functionality

        public void PublishTypeAndIdentity(Type aType, Object anIdentity)
        {
            throw new NotImplementedException();
        }

        public void PublishRendezvous<T>(Rendezvous<T> aRendezvous)
            where T : Message
        {
            throw new NotImplementedException();
        }

        public object GetDataFromInbox()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}