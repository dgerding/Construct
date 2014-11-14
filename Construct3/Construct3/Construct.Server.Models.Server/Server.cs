using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using Construct.Utilities.Shared;
using NLog;

namespace Construct.Server.Models
{
    public class Server : Models.IServer
    {
        private Uri serverUri;
        private Guid serverProcessID;
        private string publicHostName;
        private int hostPortBase;

        private Uri serverServiceUri = null;
        private Dictionary<Models.IModel, ServiceHost> serviceHostDictionary = new Dictionary<Models.IModel, ServiceHost>();

        public IEnumerable<ServiceHost> Hosts
        {
            get
            {
                return serviceHostDictionary.Values;
            }
        }

        private string connectionString = null;

        public MessageBrokering.Broker Broker { get; private set; }

        public bool IsRunning { get; private set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool isDataServiceReady = false;

        public bool IsDataServiceReady
        {
            get
            {
                return isDataServiceReady;
            }
        }

        public Server(Uri serverServiceUri, int hostPortBase, string connStringLookupKey)
        {
            logger.Trace("instantiating Server model");
            this.serverServiceUri = serverServiceUri;
            this.hostPortBase = hostPortBase;
            this.connectionString = ConfigurationManager.ConnectionStrings[connStringLookupKey].ConnectionString;
            this.Broker = new MessageBrokering.Broker();
        }

        private bool isDatabaseReachable = false;

        public bool IsDatabaseReachable
        {
            get
            {
                return isDatabaseReachable;
            }
        }

        private bool isDatabaseSchemaCurrent = false;

        public bool IsDatabaseSchemaCurrent
        {
            get
            {
                return isDatabaseSchemaCurrent;
            }
        }

        



        public void Start()
        {
            Services.Data.Service dataService = new Services.Data.Service(serverServiceUri, connectionString, this);

            //dateService

            isDatabaseReachable = dataService.IsDatabaseReachable;
            isDatabaseSchemaCurrent = dataService.IsDatabaseSchemaCurrent;

            if (!dataService.IsModelFaultedOnConstruction)
            {
                isDataServiceReady = true;
                Start(dataService, typeof(Models.Data.IModel), true);
                Start(new Services.Learning.Service(serverServiceUri, connectionString, this, dataService), typeof(Models.Learning.IModel));
                Start(new Services.Meaning.Service(serverServiceUri, connectionString, this), typeof(Models.Meaning.IModel), true);
                Start(new Services.Questions.Service(serverServiceUri, connectionString, this), typeof(Models.Questions.IModel), true);
                Start(new Services.Sessions.Service(serverServiceUri, connectionString, this), typeof(Models.Sessions.IModel), true);
                Start(new Services.Sources.Service(serverServiceUri, connectionString, this, dataService), typeof(Models.Sources.IModel), true);
                Start(new Services.Visualizations.Service(serverServiceUri, connectionString, this), typeof(Models.Visualizations.IModel), true);
            }
        }

        public void Start(Models.IModel model, Type contractType, bool isWSDualHTTPRequired = false)
        {
            this.serverProcessID = UriUtility.GetServerProcessIdFromServiceUri(serverServiceUri);

            Models.IModel service = model;
            ServiceHostConfigurator configurator = new ServiceHostConfigurator(ServiceEndpointGenerator(model.Name), service, contractType, true, isWSDualBindingRequiredHost:isWSDualHTTPRequired);
            ServiceHost host = configurator.ConfiguredServiceHost;
            host.Open();
            serviceHostDictionary.Add(service, host);
        }

        public void Stop(Models.IModel theService)
        {
            try
            {
                serviceHostDictionary[theService].Close();
            }
            catch
            {
                //TODO LOGGER
            }
        }

        public void Stop()
        {
            try
            {
                foreach (Models.IModel service in serviceHostDictionary.Keys)
                {
                    Stop(service);
                }
            }
            catch
            {
                //TODO: LOG EXCEPTION
            }
        }

        public bool AreStartParametersValid(Guid serverProcessID, string publicHostName, int hostPortBase)
        {
            if (publicHostName != string.Empty)
            {
                this.publicHostName = publicHostName;

                try
                {
                    if (hostPortBase >= 0 && hostPortBase <= 65535)
                    {
                        this.hostPortBase = hostPortBase;
                        return true;
                    }
                }
                catch
                {
                    //TODO: LOG EXCEPTION
                    return false;
                }
            }
            else
            {
                return false;
            }

            return false;
        }

        private List<string> GetValidLocalHostNames()
        {
            List<string> localHostNames = new List<string>();

            localHostNames.Add("localhost");

            try
            {
                // Get the local computer host name.
                String hostName = Dns.GetHostName();
                localHostNames.Add(hostName);
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException caught!!!");
                Debug.WriteLine("Source : " + e.Source);
                Debug.WriteLine("Message : " + e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception caught!!!");
                Debug.WriteLine("Source : " + e.Source);
                Debug.WriteLine("Message : " + e.Message);
            }

            return localHostNames;
        }

        public static string GetLocalhostFQDN()
        {
            string domainName = string.Empty;
            try
            {
                domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            }
            catch
            {
                //TODO: LOG EXCEPTION
            }
            string fqdn = "localhost";
            try
            {
                fqdn = System.Net.Dns.GetHostName();
                if (!string.IsNullOrEmpty(domainName))
                {
                    if (!fqdn.ToLowerInvariant().EndsWith("." + domainName.ToLowerInvariant()))
                    {
                        fqdn += "." + domainName;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("DNS fault?");
                Debug.WriteLine("Source : " + e.Source);
                Debug.WriteLine("Message : " + e.Message);
            }
            return fqdn;
        }

        private Uri[] ServiceEndpointGenerator(string domainModelTier)
        {
            Uri[] theList = new Uri[2];
            theList[0] = UriUtility.CreateStandardConstructServiceEndpointUri("net.pipe", domainModelTier, GetValidLocalHostNames()[0], serverProcessID, hostPortBase);
            theList[1] = UriUtility.CreateStandardConstructServiceEndpointUri("http", domainModelTier, GetValidLocalHostNames()[0], serverProcessID, hostPortBase);

            return theList;
        }
    }
}