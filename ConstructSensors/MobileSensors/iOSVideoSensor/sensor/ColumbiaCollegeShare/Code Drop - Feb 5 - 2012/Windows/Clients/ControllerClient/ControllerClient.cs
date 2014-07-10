using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.IO;
using SensorSharedTypes;
using Clients;

namespace ControllerClient
{
    public class ControllerClient : BaseClient
    {
        public ControllerClient(string baseUrl) : base(baseUrl)
        {
        }

        // GET: /SensorAPI/GetSensors
        public List<SensorInfo> GetSensors()
        {
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/GetSensors");
            request.Method = Method.GET;
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute<List<SensorInfo>>(request);
            if (response == null || response.Data == null)
            {
                return null;
            }
            return response.Data;
        }

        // GET: /SensorAPI/DropAllSensors
        public void DropAllSensors()
        {
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/DropAllSensors");
            request.Method = Method.GET;
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute(request);
        }

        // GET: /SensorAPI/SetSensorCommand?sensorID=xxx&sensorCommand=yyy
        public bool SetSensorCommand(SensorCommand sensorCommand, string sensorID, out string message)
        {
            message = "";
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/SetSensorCommand");
            request.Method = Method.GET;
            request.AddParameter("sensorID", sensorID);
            if (sensorCommand == SensorCommand.None)
            {
                request.AddParameter("sensorCommand", "");
            }
            else
            {
                request.AddParameter("sensorCommand", sensorCommand.ToString());
            }
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute<BoolMessage>(request);
            if (response == null || response.Data == null)
            {
                return false;
            }
            message = response.Data.Message;
            return response.Data.Result;
        }


    }
}
