using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace Construct.MessageBrokering
{
    public class SelfServiceHostConfigurator
    {
        public static readonly int DEFAULTPORT = 8086;

        public ServiceHost serviceHost;
        private Type typeOfServiceObject = null;
        private IEnumerable<Uri> requestedServiceAddresses = null;

        private bool isForNonLocalUse = false;

        private int maxBufferSize;
        private bool isMexEndpointRequested = false;
        private bool isOpen;

        private bool isFaulted;

        private List<Uri> serviceAddresses = new List<Uri>();
        private bool isMexEndpointCreated = false;

        private List<Uri> optimalServiceUris = new List<Uri>();

        public SelfServiceHostConfigurator(IEnumerable<Uri> theRequestedServiceAddresses, object theService, Type theServiceType, bool theIsForNonLocalUse = true, int theMaxBufferSize = 20000000, bool aMexEndpointIsRequested = true)
        {
            isForNonLocalUse = theIsForNonLocalUse;
            typeOfServiceObject = theServiceType;
            maxBufferSize = theMaxBufferSize;
            isMexEndpointRequested = aMexEndpointIsRequested;
            requestedServiceAddresses = theRequestedServiceAddresses;

            AreServiceAddressesOk(theRequestedServiceAddresses);

            GenerateServiceBaseAddressesFromRequestedServiceAddresses();

            serviceHost = new ServiceHost(theService, serviceAddresses.ToArray());
            serviceHost.OpenTimeout = TimeSpan.FromMinutes(10);
            serviceHost.CloseTimeout = TimeSpan.FromMinutes(10);

            serviceHost.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);

            ConfigureHTTPBinding(serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeHttp));
            ConfigureWebHTTPBinding(new Uri(serviceAddresses.First(webHttpAddress => webHttpAddress.Scheme == Uri.UriSchemeHttp).AbsoluteUri + "/web"));
            ConfigureNetNamedPipeBinding(serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeNetPipe));
            ConfigureTcpBinding(serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeNetTcp));

            if (isMexEndpointRequested)
            {
                EnableMetadataExchange = true;
                //AddAllMexEndPoints();
            }
        }

        public string ServiceTypeName { get; private set; }

        public string ServiceAddressBaseHostName { get; set; }

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
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

        public List<Uri> ServiceAddresses
        {
            get
            {
                return serviceAddresses;
            }
        }

        public List<Uri> OptimalServiceUris
        {
            get
            {
                return optimalServiceUris;
            }
        }

        public bool EnableMetadataExchange
        {
            set
            {
                if (serviceHost.State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }
                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    metadataBehavior = new ServiceMetadataBehavior();
                    metadataBehavior.HttpGetEnabled = value;

                    serviceHost.Description.Behaviors.Add(metadataBehavior);
                }
                if (value == true)
                {
                    if (HasMexEndpoint == false)
                    {
                        AddAllMexEndPoints();
                    }
                }
            }
            get
            {
                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    return false;
                }
                return metadataBehavior.HttpGetEnabled;
            }
        }

        public bool HasMexEndpoint
        {
            get
            {
                // Check to see if the service host already has a ServiceMetadataBehavior
                ServiceMetadataBehavior smb = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                // If not, add one
                if (smb == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static Binding MakeBinding<T>(int maxBufferSize = 20000000) where T : Binding, new()
        {
            dynamic binding = new T();
            if (typeof(T).Equals(typeof(NetTcpBinding)))
            {
                binding.Security.Mode = SecurityMode.None;
            }
            binding.TransferMode = TransferMode.Buffered;
            binding.MaxBufferSize = maxBufferSize;
            binding.MaxReceivedMessageSize = maxBufferSize;
            SetTimeout((Binding)binding);

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            binding.ReaderQuotas = readerQuotas;
            return binding;
        }

        public bool Open()
        {
            try
            {
                serviceHost.Open();

                IsOpen = true;

                return true;
            }
            catch (CommunicationException e)
            {
                Debug.WriteLine(e.Message);
                IsOpen = false;
                IsFaulted = true;
                return false;
            }
            catch (TimeoutException e)
            {
                Debug.WriteLine(e.Message);
                IsOpen = false;
                IsFaulted = true;
                return false;
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
                IsOpen = false;
                IsFaulted = true;
                return false;
            }
            catch (Exception)
            {
                throw new ApplicationException("Service open not so good.");
            }
            finally
            {
            }
        }

        public List<Uri> GenerateOptimalServiceUrisFromProvidedUris(Uri[] theUris)
        {
            optimalServiceUris = RendezvousResolver.ConvertUrisIfLocal(theUris);

            if (isForNonLocalUse)
            {
                GenerateOptimalNonLocalServiceUris(theUris);
            }
            return OptimalServiceUris;
        }

        public void AddAllMexEndPoints()
        {
            Debug.Assert(HasMexEndpoint == false);

            foreach (Uri baseAddress in serviceHost.BaseAddresses)
            {
                BindingElement bindingElement = null;
                switch (baseAddress.Scheme)
                {
                    case "net.tcp":
                        {
                            bindingElement = new TcpTransportBindingElement();
                            break;
                        }
                    case "net.pipe":
                        {
                            bindingElement = new NamedPipeTransportBindingElement();
                            break;
                        }
                    case "http":
                        {
                            bindingElement = new HttpTransportBindingElement();
                            break;
                        }
                    case "https":
                        {
                            bindingElement = new HttpsTransportBindingElement();
                            break;
                        }
                }
                if (bindingElement != null)
                {
                    Binding binding = new CustomBinding(bindingElement);
                    serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), binding, "MEX");
                }
            }
        }

        protected void GenerateServiceBaseAddressesFromRequestedServiceAddresses()
        {
            foreach (Uri uri in requestedServiceAddresses)
            {
                AddBaseAddressEndpoints(uri);
            }
        }

        private static void SetTimeout(Binding binding)
        {
            binding.SendTimeout = TimeSpan.FromMinutes(10);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.CloseTimeout = TimeSpan.FromMinutes(10);
        }

        private void serviceHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            throw new ApplicationException("Unknown message received over service host.");
        }

        private void AreServiceAddressesOk(IEnumerable<Uri> serviceAddresses)
        {
            foreach (Uri theUri in serviceAddresses)
            {
                if (!theUri.IsWellFormedOriginalString())
                    throw new ApplicationException("SelfServiceHostConfigurator provided malformed URI");
            }
        }

        private void AddBaseAddressEndpoints(Uri uri)
        {
            string[] uriParts = uri.ToString().Split('/');
            StringBuilder sb = new StringBuilder(uriParts[3]);
            sb.Append("/" + uriParts[4]);

            string hostName = (uriParts[2] == "localhost") ? System.Net.Dns.GetHostName() : uriParts[2];
            string identifier = sb.ToString();

            CreateProtocolSpecificBaseAddress("http", hostName, identifier);
            CreateProtocolSpecificBaseAddress("net.pipe", hostName, identifier);
            CreateProtocolSpecificBaseAddress("net.tcp", hostName, identifier);
        }

        private void CreateProtocolSpecificBaseAddress(string protocol, string hostName, string identifier)
        {
            StringBuilder sb = new StringBuilder(protocol);
            sb.Append("://");

            if (protocol == "net.pipe")
            {
                hostName = "localhost";
            }

            sb.Append(hostName + "/");
            sb.Append(identifier);

            Uri uriToAdd = new Uri(sb.ToString());
            serviceAddresses.Add(uriToAdd);
        }

        private void ConfigureTcpBinding(Uri address)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.Buffered;
            binding.MaxBufferSize = maxBufferSize;
            binding.MaxReceivedMessageSize = maxBufferSize;
            SetTimeout(binding);

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            binding.ReaderQuotas = readerQuotas;
            ServiceEndpoint endpoint = serviceHost.AddServiceEndpoint(typeOfServiceObject, binding, address.AbsoluteUri);
            endpoint.ListenUriMode = ListenUriMode.Unique;
            //serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            //serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
        }

        private void ConfigureHTTPBinding(Uri address)
        {
            BasicHttpBinding theBasicHttpBinding = new BasicHttpBinding();
            theBasicHttpBinding.TransferMode = TransferMode.Buffered;
            theBasicHttpBinding.MaxBufferSize = maxBufferSize;
            theBasicHttpBinding.MaxReceivedMessageSize = maxBufferSize;
            SetTimeout(theBasicHttpBinding);

            theBasicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            theBasicHttpBinding.MaxBufferSize = int.MaxValue;
            theBasicHttpBinding.MaxBufferPoolSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            theBasicHttpBinding.ReaderQuotas = readerQuotas;
            ServiceEndpoint httpEndpoint = serviceHost.AddServiceEndpoint(typeOfServiceObject, theBasicHttpBinding, address);
        }

        private void ConfigureWebHTTPBinding(Uri address)
        {
            WebHttpBinding theWebHttpBinding = new WebHttpBinding();
            SetTimeout(theWebHttpBinding);

            theWebHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            theWebHttpBinding.MaxBufferSize = int.MaxValue;
            theWebHttpBinding.MaxBufferPoolSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            theWebHttpBinding.ReaderQuotas = readerQuotas;
            theWebHttpBinding.CrossDomainScriptAccessEnabled = true;

            ServiceEndpoint webHttpEndpoint = serviceHost.AddServiceEndpoint(typeOfServiceObject, theWebHttpBinding, address);
        }

        private void ConfigureNetNamedPipeBinding(Uri address)
        {
            NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding();
            netNamedPipeBinding.TransferMode = TransferMode.Buffered;
            netNamedPipeBinding.MaxBufferSize = maxBufferSize;
            netNamedPipeBinding.MaxReceivedMessageSize = maxBufferSize;
            SetTimeout(netNamedPipeBinding);

            netNamedPipeBinding.MaxReceivedMessageSize = int.MaxValue;
            netNamedPipeBinding.MaxBufferSize = int.MaxValue;
            netNamedPipeBinding.MaxBufferPoolSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            netNamedPipeBinding.ReaderQuotas = readerQuotas;
            ServiceEndpoint endpoint = serviceHost.AddServiceEndpoint(typeOfServiceObject, netNamedPipeBinding, address.AbsoluteUri);
        }

        private bool IsTcpPortAvailable(int port)
        {
            bool isAvailable = true;

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        private void GenerateOptimalNonLocalServiceUris(Uri[] theUris)
        {
            foreach (Uri uri in theUris)
            {
                if (AttemptTcpEndpointCreation(uri))
                {
                }
                else if (AttemptHttpEndpointCreation(uri))
                {
                }
                else
                {
                }
            }
        }

        private bool AttemptTcpEndpointCreation(Uri theUri)
        {
            for (int x = 8086; x <= 8286; x++)
            {
                if (IsTcpPortAvailable(x))
                {
                    string uriString = Uri.UriSchemeNetTcp.ToString();
                    uriString += @"://";
                    uriString += theUri.DnsSafeHost;
                    uriString += ":" + x.ToString();
                    uriString += @"/";
                    uriString += theUri.AbsolutePath;
                    Uri optimalTcpUri = new Uri(uriString);

                    ConfigureTcpBinding(optimalTcpUri);

                    return true;
                }
            }

            return false;
        }

        private bool AttemptHttpEndpointCreation(Uri theUri)
        {
            return false;
        }
    }
}