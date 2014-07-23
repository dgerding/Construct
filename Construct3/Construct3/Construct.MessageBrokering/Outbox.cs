using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Construct.MessageBrokering.TransponderService;

namespace Construct.MessageBrokering
{
    public interface IOutbox : IPeer
    {
    }

    public class Outbox<T> : Peer<T>, IOutbox, IDisposable
        where T : Message
    {
		//	TODO: What is an appropriate exit condition for the message-send thread? Preferably not a manual request by the user
		//	(Note: Should be moved outside of Outbox?)
		private Thread m_MessageSendThread;
		//	Double-buffered
		private ConcurrentQueue<RendezvousStruct> m_QueueDataBuffer = new ConcurrentQueue<RendezvousStruct>();
		private ConcurrentQueue<RendezvousStruct> m_ActiveDataBuffer = new ConcurrentQueue<RendezvousStruct>();
		private bool m_MessageSendThread_RequestTerminate = false;
		private int m_MinBufferSizeForDataSend = 500; // If the queue buffer hits 500 items, swap
		private int m_MinBufferDataSendWaitTimeMS = 20; // At least swap buffers every 20ms

		public int MinBufferDataSendWaitTimeMS
		{
			get { return m_MinBufferDataSendWaitTimeMS; }
			set { m_MinBufferDataSendWaitTimeMS = value; }
		}

        private readonly List<Rendezvous<T>> clientsRendezvous;
        public Outbox(Rendezvous<T> attemptedRendezvous) : this()
        {
            Uri optimalUri = RendezvousResolver.ConvertUrisIfLocal(attemptedRendezvous);
            Rendezvous<T> replacementRendezvous = new Rendezvous<T>(optimalUri.AbsoluteUri);
            clientsRendezvous.Add(replacementRendezvous);
        }

        public Outbox(Rendezvous<T>[] rendezvous) : this()
        {
            for (int i = 0, length = rendezvous.Length; i < length; i++)
            {
                ConfigureRendezvous(rendezvous[i]);
            }
        }

        private Outbox()
        {
            clientsRendezvous = new List<Rendezvous<T>>();

			m_MessageSendThread = new Thread(MessageSendThread);
			m_MessageSendThread.IsBackground = true;
			m_MessageSendThread.Name = "Construct.MessageBrokering.Outbox Thread";
			m_MessageSendThread.Start();
        }

        public List<Rendezvous<T>> AllRendezvous
        {
            get
            {
                return clientsRendezvous;
            }
        }

        public void AddRendezvous(Rendezvous<T> rendezvous)
        {
            ConfigureRendezvous(rendezvous);
        }

        public void AddRendezvous(Rendezvous<T>[] rendezvous)
        {
            foreach (Rendezvous<T> temp in rendezvous)
            {
                AddRendezvous(temp);
            }
        }

        public void AddObject(string theData)
        {
			foreach (Rendezvous<T> rendezvous in clientsRendezvous)
			{
				m_QueueDataBuffer.Enqueue(new RendezvousStruct(rendezvous.Binding, rendezvous.Uri.ToString(), theData));
			}
        }

        public void AddObjectTo(Rendezvous<T> rendezvous, string theData)
        {
			m_QueueDataBuffer.Enqueue(new RendezvousStruct(rendezvous.Binding, rendezvous.Uri.ToString(), theData));
        }

		public void Dispose()
		{
			if (m_MessageSendThread != null && m_MessageSendThread.IsAlive)
			{
				m_MessageSendThread_RequestTerminate = true;
				m_MessageSendThread.Join();
			}
		}

		private void MessageSendThread()
		{
			//	Could probably make a TransponderClientFactory that would do this caching for us
			Dictionary<String, TransponderClient> transponderCacheMap = new Dictionary<string, TransponderClient>();
			DateTime lastBufferSwapTime = DateTime.Now;

			while (!m_MessageSendThread_RequestTerminate)
			{
				DateTime startSendTime = DateTime.Now;

				foreach (var rendezvousData in m_ActiveDataBuffer)
				{
					Binding binding = rendezvousData.Binding;
					string theData = rendezvousData.Data;
					string uri = rendezvousData.Uri;
					bool success = false;

					theData = StringCompressor.CompressString(theData);

					while (!success)
					{
						TransponderClient client = null;
						try
						{
							String transponderTypeIdentifier = uri + "-" + binding.ToString();
							if (transponderCacheMap.ContainsKey(transponderTypeIdentifier))
							{
								client = transponderCacheMap[transponderTypeIdentifier];
								if (client.State != CommunicationState.Opened)
								{
									client.Close();
									client = new TransponderClient(binding, new EndpointAddress(uri));
									transponderCacheMap[transponderTypeIdentifier] = client;
									Debugger.Log(0, "", "Disposed old transponder, created new transponder");
								}
							}
							else
							{
								client = new TransponderClient(binding, new EndpointAddress(uri));
								transponderCacheMap.Add(transponderTypeIdentifier, client);
							}

							client.AddObjectAsync(theData);

							success = true;
						}
						catch (CommunicationException)
						{
							if (client != null)
							{
								client.Abort();
							}
						}
						catch (TimeoutException)
						{
							if (client != null)
							{
								client.Abort();
							}
						}
						catch (Exception e)
						{
							if (client != null)
							{
								client.Abort();
							}
						}
					}
				}

				m_ActiveDataBuffer = new ConcurrentQueue<RendezvousStruct>();

				DateTime swapTime = DateTime.Now;
				int timeSinceSwapMS = (int)(swapTime - lastBufferSwapTime).TotalMilliseconds;

				if (m_QueueDataBuffer.Count >= m_MinBufferSizeForDataSend || timeSinceSwapMS >= m_MinBufferDataSendWaitTimeMS)
				{
					var queuedDataBuffer = m_QueueDataBuffer;
					m_QueueDataBuffer = m_ActiveDataBuffer;
					m_ActiveDataBuffer = queuedDataBuffer;
					lastBufferSwapTime = swapTime;
				}

				System.Threading.Thread.Sleep(1);
			}
		}

        public bool Contains(Rendezvous<T> rendezvous)
        {
            return clientsRendezvous.Contains(rendezvous);
        }

        public void PublishData()
        {
            throw new NotImplementedException();
        }

        private void ConfigureRendezvous(Rendezvous<T> attemptedRendezvous)
        {
            Uri optimalUri = RendezvousResolver.ConvertUrisIfLocal(attemptedRendezvous);
            Rendezvous<T> replacementRendezvous = new Rendezvous<T>(optimalUri.AbsoluteUri);

            TimeSpan tenMinutes = TimeSpan.FromMinutes(10);
            replacementRendezvous.Binding.CloseTimeout = tenMinutes;
            replacementRendezvous.Binding.OpenTimeout = tenMinutes;
            replacementRendezvous.Binding.ReceiveTimeout = tenMinutes;
            replacementRendezvous.Binding.SendTimeout = tenMinutes;

            clientsRendezvous.Add(replacementRendezvous);
        }

        internal class RendezvousStruct
        {
            public Binding Binding;
            public string Uri;
            public string Data;

            public RendezvousStruct(Binding binding, string uri, string data)
            {
                Binding = binding;
                Uri = uri;
                Data = data;
            }
        }
    }
}