using System;
using System.Linq;

namespace Construct.Utilities.Shared
{
    public static class UriUtility
    {
        public static Guid GetServerProcessIdFromServiceUri(Uri theUri)
        {
            char separator = '/';
            string processID = theUri.ToString().Split(separator)[3];
            return Guid.Parse(processID);
        }

        public static Uri CreatePropertyValueServiceEndpointFromServerEndpoint(Uri serverServiceUri, string datatypeName, string propertyName, int port=8000)
        {
            UriBuilder builder = new UriBuilder(serverServiceUri);
            string[] paths = builder.Path.Split('/');
            paths[2] = datatypeName;
            paths[3] = propertyName;
            builder.Path = paths.Aggregate((previous, current) => previous + "/" + current);

            if(builder.Scheme != "net.pipe") builder.Port = port;

            Uri uri = builder.Uri;
            return uri;
        }

        public static Uri CreateModelServiceEndpointFromServerEndpoint(Uri serverServiceUri, string domainModelTier)
        {
            string hostName;
            int port;
            char separator = '/';
            string[] uriParts = serverServiceUri.ToString().Split(separator);

            if ((uriParts[0] == "http:") || (uriParts[0] == "https:") || (uriParts[0] == "net.tcp:"))
            {
                char portSeparator = ':';
                string[] nameAndPort = uriParts[2].Split(portSeparator);

                hostName = nameAndPort[0];
                port = int.Parse(nameAndPort[1]);

                return CreateStandardConstructServiceEndpointUri(uriParts[0], domainModelTier, hostName, GetServerProcessIdFromServiceUri(serverServiceUri), port);

            }
            else if (uriParts[0] == "net.pipe:")
            {
                return CreateStandardConstructServiceEndpointUri(uriParts[0], domainModelTier, uriParts[2], GetServerProcessIdFromServiceUri(serverServiceUri), 0);
            }
            else
                throw new InvalidOperationException(String.Format("Scheme {0} is not supported. Can not form a uri using {0}.", uriParts[0]));
        }

        public static Uri CreateStandardConstructServiceEndpointUri(string endpointScheme, string domainModelTier, string hostName, Guid processID, int portBase)
        {
            if (endpointScheme.EndsWith(":") == false)
            {
                endpointScheme += ":";
            }
            if (endpointScheme.ToLower() == "http:" || endpointScheme.ToLower() == "https:")
            {
                string uriPart = string.Format(@"{0}//{1}{2}/{3}/{4}/{4}Service.svc", endpointScheme,
                                                                                      hostName,
                                                                                      GetPortFromDomailModelTier(domainModelTier, portBase),
                                                                                      processID.ToString(),
                                                                                      domainModelTier);

                return new Uri(uriPart);
            }
            #region old https
            //if (endpointScheme.ToLower() == "https:")
            //{
            //    string uriPart = string.Format(@"{0}//{1}{2}{3}/{4}/{4}Service.svc", endpointScheme,
            //                                                                         hostName,
            //                                                                         GetPortFromDomailModelTier(domainModelTier, portBase),
            //                                                                         processID.ToString(),
            //                                                                         domainModelTier);

            //    return new Uri(uriPart);
            //}
            #endregion

            if (endpointScheme.ToLower() == "net.tcp:")
            {
                string uriPart = string.Format(@"{0}//{1}{2}/{3}/{4}/{4}Service.svc", endpointScheme,
                                                                                      hostName,
                                                                                      GetPortFromDomailModelTier(domainModelTier, portBase),
                                                                                      processID.ToString(),
                                                                                      domainModelTier);

                return new Uri(uriPart);
            }

            if (endpointScheme.ToLower() == "net.pipe:")
            {
                string uriPart = string.Format(@"{0}//{1}/{2}/{3}/{3}Service.svc", endpointScheme,
                                                                                   hostName,
                                                                                   processID,
                                                                                   domainModelTier);

                return new Uri(uriPart);
            }
            return null;
        }

        private static string GetPortFromDomailModelTier(string domainModelTier, int portbase)
        {
            switch (domainModelTier.ToLower())
            {
                case ("server"):
                    return ":" + portbase.ToString();
                    //break;
                case("sources"):
                    return ":" + (portbase + (int)ServicePortOffsets.Sources).ToString();
                    //break;
                case("meaning"):
                    return ":" + (portbase + (int)ServicePortOffsets.Meaning).ToString();
                    //break;
                case("questions"):
                    return ":" + (portbase + (int)ServicePortOffsets.Questions).ToString();
                    //break;
                case("visualizations"):
                    return ":" + (portbase + (int)ServicePortOffsets.Visualizations).ToString();
                    //break;
                case("sessions"):
                    return ":" + (portbase + (int)ServicePortOffsets.Sessions).ToString();
                    //break;
                case ("learning"):
                    return ":" + (portbase + (int)ServicePortOffsets.Learning).ToString();
                    //break;
                case ("data"):
                    return ":" + (portbase + (int)ServicePortOffsets.Data).ToString();
                    //break;
                default:
                    return string.Empty;
                    //break;
            }
        }

        public enum ServicePortOffsets
        {
            Server = 0,
            Sources = 1000,
            Meaning = 2000,
            Questions = 3000,
            Visualizations = 4000,
            Sessions = 5000,
            Learning = 6000,
            Data = 7000
        }
    }
}