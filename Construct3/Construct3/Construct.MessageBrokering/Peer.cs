using System;
using System.Linq;

namespace Construct.MessageBrokering
{
    public interface IPeer
    {
    }

    public abstract class Peer<T> : IPeer
        where T : Message
    {
        protected bool isReady = false;
        protected bool isFaulted = false;
        private readonly Rendezvous<T> initialRendezvous = null;
        public bool IsReady
        {
            get
            {
                return isReady;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return isFaulted;
            }
        }

        public string MessageType
        {
            get
            {
                return typeof(T).Name;
            }
        }

        public Rendezvous<T> InitialRendezvous
        {
            get
            {
                return initialRendezvous;
            }
        }
    }
}