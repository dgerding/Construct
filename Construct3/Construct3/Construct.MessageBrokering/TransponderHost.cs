using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Construct.MessageBrokering.TransponderService;

namespace Construct.MessageBrokering
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TransponderHost<T> : ITransponder, IDisposable
        where T : Message
    {
        public event System.Action<object, string> OnObjectReceived;
        private List<Rendezvous<T>> rendezvous = new List<Rendezvous<T>>();

        public Rendezvous<T> CurrentRendezvous { get; private set; }
        public SelfServiceHostConfigurator configurator;
        public bool IsReady = false;

        public TransponderHost(Rendezvous<T>[] rendezvous, System.Action<object, string> addObjectHandler)
        {
            if (SetUpTransponder(rendezvous))
            {
                IsReady = true;
            }
            else
            {
                IsReady = false;
            }

            OnObjectReceived += addObjectHandler;
        }

        private bool SetUpTransponder(Rendezvous<T>[] rendezvous)
        {
            List<Uri> uris = new List<Uri>();
            foreach (Rendezvous<T> temp in rendezvous)
            {
                uris.Add(temp.Uri);
            }

            configurator = new SelfServiceHostConfigurator(uris, this, typeof(ITransponder));

            foreach (Uri realEndpoint in configurator.ServiceAddresses)
            {
                this.rendezvous.Add(new Rendezvous<T>(realEndpoint.ToString()));
            }

            CurrentRendezvous = this.rendezvous.First<Rendezvous<T>>();

            return configurator.Open();
        }

        public TransponderHost(Rendezvous<T> aRendezvous, System.Action<object, string> anAddObjectHandler) : this(new Rendezvous<T>[] { aRendezvous }, anAddObjectHandler)
        {
        }

        public bool AddObject(string jsonData)
        {
			jsonData = StringCompressor.DecompressString(jsonData);
            OnObjectReceived(this, jsonData);
            return true;
        }

        public IAsyncResult BeginAddObject(string jsonData, AsyncCallback callback, object asyncState)
        {
            bool result = AddObject(jsonData);
            return new CompletedAsyncResult<bool>(result);
        }

        public bool EndAddObject(IAsyncResult r)
        {
            CompletedAsyncResult<bool> result = r as CompletedAsyncResult<bool>;
            return result.Data;
        }

        public void Close()
        {
            configurator.serviceHost.Close();
        }

        #region IDisposable Members

        public void Dispose()
        {
            configurator.serviceHost.Close();
        }
        #endregion
    }

    class CompletedAsyncResult<T> : IAsyncResult
    {
        T data;

        public CompletedAsyncResult(T data)
        {
            this.data = data;
        }

        public T Data
        {
            get
            {
                return data;
            }
        }

        #region IAsyncResult Members
        public object AsyncState
        {
            get
            {
                return (object)data;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return true;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}