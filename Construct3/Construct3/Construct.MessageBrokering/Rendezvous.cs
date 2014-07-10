using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Construct.MessageBrokering
{
    public class Rendezvous<T>
        where T : Message
    {
        public static int DEFAULT_PORT = 8086;

        public readonly Guid SensorID;
        public readonly Binding Binding;

        private DateTime lastVisited;
        private bool isFaulted;

        public Rendezvous(Protocol protocol, string hostName, Guid typeSourceID, Guid processID) // Please add TypeSourceID and box identity string requirements
        {
            Protocol tempProtocol = protocol;
            string tempHostName = hostName;
            SensorID = processID;
            Type theType = typeof(T);
            if (IsRendezvousLocal(protocol, hostName))
            {
                tempProtocol = Protocol.NetNamedPipes;
                tempHostName = "localhost";
            }
            switch (tempProtocol)
            {
                case Protocol.HTTP:
                    Binding = SelfServiceHostConfigurator.MakeBinding<BasicHttpBinding>();
                    this.Uri = new Uri("http://" + tempHostName + "/" + typeSourceID + "/" + SensorID.ToString());
                    break;
                case Protocol.NetNamedPipes:
                    Binding = SelfServiceHostConfigurator.MakeBinding<NetNamedPipeBinding>();
                    this.Uri = new Uri("net.pipe://" + tempHostName + "/" + typeSourceID + "/" + SensorID.ToString());
                    break;
                case Protocol.TCP:
                    Binding = SelfServiceHostConfigurator.MakeBinding<NetTcpBinding>();
                    this.Uri = new Uri("net.tcp://" + tempHostName + ":" + DEFAULT_PORT + "/" + typeSourceID + "/" + SensorID.ToString());
                    break;
                default:
                    break;
            }
        }

        public Rendezvous(string uri) : this(Rendezvous<T>.TryParseProtocol(uri), Rendezvous<T>.TryParseHostName(uri), Rendezvous<T>.TryParseSource(uri), Rendezvous<T>.TryParseProcessID(uri))
        {
        }

        protected Rendezvous()
        {
        }

        public Uri Uri { get; private set; }

        public DateTime LastVisited
        {
            get
            {
                return lastVisited;
            }
            set
            {
                lastVisited = value;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return isFaulted;
            }
            set
            {
                isFaulted = value;
            }
        }

        public static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Rendezvous<T> rendezvous = obj as Rendezvous<T>;
            if ((System.Object)rendezvous == null)
                return false;

            return (rendezvous.Uri == Uri);
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }

        public bool Equals(Rendezvous<T> rendezvous)
        {
            if ((object)rendezvous == null)
                return false;
            return (rendezvous.Uri == Uri);
        }

        private static Protocol TryParseProtocol(string value)
        {
            Protocol result = (Protocol)int.MaxValue;
            switch (value.Split(':')[0])
            {
                case "http":
                    result = Protocol.HTTP;
                    break;
                case "net.pipe":
                    result = Protocol.NetNamedPipes;
                    break;
                case "net.tcp":
                    result = Protocol.TCP;
                    break;
                default:
                    break;
            }
            return result;
        }

        private static string TryParseHostName(string value)
        {
            string withoutProtocol = value.Split(new string[] { "://" }, StringSplitOptions.None)[1];
            string host = withoutProtocol.Split('/')[0];
            return host;
        }

        private static Guid TryParseSource(string value)
        {
            string[] potentials = value.Split('/');
            Guid result = new Guid();

            foreach (string potential in potentials)
            {
                if (Guid.TryParse(potential, out result))
                {
                    break;
                }
            }
            return result;
        }

        private static Guid TryParseProcessID(string value)
        {
            string[] uriParts = value.Split('/');
            Guid result = new Guid();
            Guid.TryParse(uriParts[4], out result);
            return result;
        }

        private bool IsRendezvousLocal(Protocol protocol, string hostName)
        {
            return ((protocol == Protocol.NetNamedPipes) || (protocol == Protocol.HTTP && hostName.ToLower() == "localhost"));
        }

        public static bool operator ==(Rendezvous<T> rendezvous1, Rendezvous<T> rendezvous2)
        {
            return rendezvous1.Uri.Equals(rendezvous2.Uri);
        }

        public static bool operator !=(Rendezvous<T> rendezvous1, Rendezvous<T> rendezvous2)
        {
            return !(rendezvous1.Uri.Equals(rendezvous2.Uri));
        }

        public static implicit operator Uri(Rendezvous<T> rendezvous)
        {
            return rendezvous.Uri;
        }
    }
}