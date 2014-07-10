using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.IO;
using SensorSharedTypes;

namespace Clients
{ 
    public class BaseClient
    {
        protected string baseUrl;

        protected BaseClient() { }

        public BaseClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public bool CanConnect()
        {
            string guid = Guid.NewGuid().ToString();
            return (Ping(guid) == guid);
        }

        // GET: /SensorAPI/Ping?val={xyz}
        public string Ping( string text )
        {
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/Ping");
            request.Method = Method.GET;
            request.AddParameter("val", text);
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute<BoolMessage>(request);
            if ( response == null || response.Data == null )
            {
                return null;
            }
            return response.Data.Message;
        }

        protected RestClient RestClient
        {
            get
            {
                return new RestClient( this.baseUrl );
            }
        }
    }
}
