using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using Construct.MessageBrokering;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

namespace Construct.Server.Models
{
    public class ServiceHostConfigurator
    {
        public ServiceHost ConfiguredServiceHost;
        private Type typeOfServiceObject = null;
        private IEnumerable<Uri> requestedServiceAddresses = null;
        
        private bool isMexEndpointRequested = false;
        private bool isMexEndpointCreated = false;

        private bool isForNonLocalUse = false;
        private bool isOpen;
        private bool isFaulted;


        private int maxBufferSize;
        
        private List<Uri> serviceAddresses = new List<Uri>();
        //private List<Uri> optimalServiceUris = new List<Uri>();

        public ServiceHostConfigurator(IEnumerable<Uri> theRequestedServiceAddresses, object theService, Type theServiceType, bool theIsForNonLocalUse = true, int theMaxBufferSize = 20000000, bool aMexEndpointIsRequested = true, bool isWSDualBindingRequiredHost = false)
        {
            isForNonLocalUse = theIsForNonLocalUse;
            typeOfServiceObject = theServiceType;
            maxBufferSize = theMaxBufferSize;
            isMexEndpointRequested = aMexEndpointIsRequested;
            requestedServiceAddresses = theRequestedServiceAddresses;

            AreServiceAddressesOk(theRequestedServiceAddresses);

            AddRequestedServiceAddresses();

            
            ConfiguredServiceHost = new ServiceHost(theService, serviceAddresses.ToArray());
            ConfiguredServiceHost.OpenTimeout = TimeSpan.FromMinutes(10);
            ConfiguredServiceHost.CloseTimeout = TimeSpan.FromMinutes(10);

            ConfiguredServiceHost.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);

            PrepareBindingsForRequestedEndpoints(isWSDualBindingRequiredHost);

            if (isMexEndpointRequested)
            {
                EnableMetadataExchange = true;
                AddAllMexEndPoints();
            }
        }
  
        private void PrepareBindingsForRequestedEndpoints(bool isWSDualHTTPRequired = false)
        {
            foreach (Uri endpointUri in serviceAddresses)
            {
                switch (endpointUri.Scheme)
                {
                    case ("http"):
                        Uri serviceAddress = serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeHttp);
                        if (isWSDualHTTPRequired == false)
                        {
                            ConfigureHTTPBinding(serviceAddress);
                        }
                        else
                        {
                            ConfigureWSDualHTTPBinding(serviceAddress);
                        }
                        //ConfigureWebHTTPBinding(new Uri(serviceAddresses.First(webHttpAddress => webHttpAddress.Scheme == Uri.UriSchemeHttp).AbsoluteUri + "/web"));
                        break;
                    case ("https"):
                        //TODO: check validity of https binding
                        ConfigureHTTPBinding(serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeHttps));
                        //ConfigureWebHTTPBinding(new Uri(serviceAddresses.First(webHttpAddress => webHttpAddress.Scheme == Uri.UriSchemeHttp).AbsoluteUri + "/web"));
                        break;
                    case ("net.tcp"):
                        ConfigureTcpBinding(serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeNetTcp));
                        break;

                    case ("net.pipe"):
                        ConfigureNetNamedPipeBinding(serviceAddresses.First(httpAddress => httpAddress.Scheme == Uri.UriSchemeNetPipe));
                        break;

                    default:
                        break;
                }

                
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

        public bool EnableMetadataExchange
        {
            set
            {
                if (ConfiguredServiceHost.State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }
                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior = ConfiguredServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    metadataBehavior = new ServiceMetadataBehavior();
                    metadataBehavior.HttpGetEnabled = value;

                    ConfiguredServiceHost.Description.Behaviors.Add(metadataBehavior);
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
                metadataBehavior = ConfiguredServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
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
                ServiceMetadataBehavior smb = ConfiguredServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
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
                ConfiguredServiceHost.Open();

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

        public void AddAllMexEndPoints()
        {
            foreach (Uri baseAddress in ConfiguredServiceHost.BaseAddresses)
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
                    ConfiguredServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), binding, "MEX");
                }
            }
        }

        protected void AddRequestedServiceAddresses()
        {
            foreach (Uri uri in requestedServiceAddresses)
            {
                serviceAddresses.Add(uri);
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
            ServiceEndpoint endpoint = ConfiguredServiceHost.AddServiceEndpoint(typeOfServiceObject, binding, address);
            endpoint.ListenUriMode = ListenUriMode.Unique;
            //ConfiguredServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            //ConfiguredServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
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
            ServiceEndpoint httpEndpoint = ConfiguredServiceHost.AddServiceEndpoint(typeOfServiceObject, theBasicHttpBinding, address);
        }

        private void ConfigureWSDualHTTPBinding(Uri address)
        {
            WSDualHttpBinding theWSDualHTTPBinding = new WSDualHttpBinding();
            //theWSDualHTTPBinding.TransferMode = TransferMode.Buffered;
            //theWSDualHTTPBinding.MaxBufferSize = maxBufferSize;
            theWSDualHTTPBinding.MaxReceivedMessageSize = maxBufferSize;
            SetTimeout(theWSDualHTTPBinding);

            theWSDualHTTPBinding.MaxReceivedMessageSize = int.MaxValue;
            //theWSDualHTTPBinding.MaxBufferSize = int.MaxValue;
            theWSDualHTTPBinding.MaxBufferPoolSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            theWSDualHTTPBinding.ReaderQuotas = readerQuotas;
            ServiceEndpoint httpEndpoint = ConfiguredServiceHost.AddServiceEndpoint(typeOfServiceObject, theWSDualHTTPBinding, address);
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

            ServiceEndpoint webHttpEndpoint = ConfiguredServiceHost.AddServiceEndpoint(typeOfServiceObject, theWebHttpBinding, address);
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
            ServiceEndpoint endpoint = ConfiguredServiceHost.AddServiceEndpoint(typeOfServiceObject, netNamedPipeBinding, address);
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