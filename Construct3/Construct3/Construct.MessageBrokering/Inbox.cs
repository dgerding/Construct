using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Construct.MessageBrokering
{
    public interface IInbox : IPeer
    {
        CommunicationState State { get; }
    }

    public class Inbox<T> : Peer<T>, IInbox
        where T : Message
    {
        public TransponderHost<T> Host;//TODO: Do we really want to expose this?
        //TODO: The intention of exposing the host was to gain relevant information about the state of the hosting service
        //This gives us the ability to see if the host is "created, opening, opened, closing, closed, or faulted".
        //There is no other way to sensibly obtain this information from the host, without at least exposing those properties from the host.
        //
        //Granted, we don't need to expose the actual host - just the relevant properties.

        public CommunicationState State { get { return Host.configurator.serviceHost.State; } }
        private List<Rendezvous<T>> rendezvousHistory = new List<Rendezvous<T>>();//TODO: Why not publicly expose if property doesn't do anything?
        public Inbox(Rendezvous<T> aRendezvous)
        {
            Host = InitializeTransponder(aRendezvous, PassObjectUpToBroker);
        }

        public event System.Action<object, string> OnObjectReceived;

        //private Data ProcessItemMessage(string jsonData)
        //{
        //    // Replace this with SensorSerializationAssistant call to GetItem()
        //    Data deserializedItem = JsonConvert.DeserializeObject<Data>(jsonData);
        //    return deserializedItem;
        //}
        //private Command ProcessCommandMessage(string jsonData)
        //{
        //    Command deserializedCommand = JsonConvert.DeserializeObject<Command>(jsonData);
        //    return deserializedCommand;
        //}
        //private Telemetry ProcessTelemetryMessage(string jsonData)
        //{
        //    Telemetry deserializedTelemtry = JsonConvert.DeserializeObject<Telemetry>(jsonData);
        //    return deserializedTelemtry;
        //}
        public Rendezvous<T> CurrentRendezvous
        {
            get
            {
                return Host.CurrentRendezvous;
            }
        }

        public List<Rendezvous<T>> RendezvousHistory
        {
            get
            {
                return rendezvousHistory;
            }
            set
            {
                rendezvousHistory = value;
            }
        }

        public void SetCurrentRendezvous(Rendezvous<T> value)
        {
            if (value.Uri != Host.CurrentRendezvous)
            {
                rendezvousHistory.Add(Host.CurrentRendezvous);
                Host = InitializeTransponder(value, PassObjectUpToBroker);
            }
        }

        public Guid GetProcessIDFromRendezvous()
        {
            return Guid.Parse(CurrentRendezvous.Uri.ToString().Split('/')[4]);
        }

        public Guid GetTypeSourceIDFromRendezvous()
        {
            return Guid.Parse(CurrentRendezvous.Uri.ToString().Split('/')[3]);
        }

        private TransponderHost<T> InitializeTransponder(Rendezvous<T> hostRendezvous, System.Action<object, string> onObjectReceivedEventHandler)
        {
            return new TransponderHost<T>(hostRendezvous, onObjectReceivedEventHandler);
        }

        private void PassObjectUpToBroker(object s, string jsonData)
        {
            OnObjectReceived(this, jsonData);
        }
        //private void OnObjectReceivedFromTransponder(object s, string jsonData)
        //{
        //    Type theType = typeof(T);
        //    switch (theType.FullName)
        //    {
        //        case "Construct.Dataflow.Brokering.Messaging.Command":
        //            OnObjectReceived(this, ProcessCommandMessage(jsonData) as T);
        //            break;
        //        case "Construct.Dataflow.Brokering.Messaging.Telemetry":
        //            OnObjectReceived(this, ProcessTelemetryMessage(jsonData) as T);
        //            break;
        //        case "Construct.Dataflow.Brokering.Messaging.Item":
        //            OnObjectReceived(this, ProcessItemMessage(jsonData) as T);
        //            break;
        //    }
        //}
    }
}