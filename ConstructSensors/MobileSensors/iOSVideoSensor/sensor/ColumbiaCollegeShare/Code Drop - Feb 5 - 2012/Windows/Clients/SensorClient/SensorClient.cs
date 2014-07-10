using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.IO;
using SensorSharedTypes;
using Clients;

namespace SensorClient
{
    public class SensorClient : BaseClient
    {
        private string sensorID;
        private string displayName;
        private Uploader uploader = null;
        private CommandMonitor commandMonitor = null;

        public EventHandler SensorConnected;
        private void onSensorConnected()
        {
            if ( SensorConnected != null )
            {
                SensorConnected(this, new EventArgs());
            }
        }

        public EventHandler SensorDisconnected;
        private void onSensorDisconnected()
        {
            if (SensorDisconnected != null)
            {
                SensorDisconnected(this, new EventArgs());
            }
        }

        public EventHandler<SensorConnectFailedEventArgs> SensorConnectFailed;
        private void onSensorConnectFailed( string message )
        {
            if (SensorConnectFailed != null)
            {
                SensorConnectFailedEventArgs args = new SensorConnectFailedEventArgs();
                args.Message = message;
                SensorConnectFailed(this, args);
            }
        }

        public EventHandler<SensorDisconnectFailedEventArgs> SensorDisconnectFailed;
        private void onSensorDisconnectFailed(string message)
        {
            if (SensorDisconnectFailed != null)
            {
                SensorDisconnectFailedEventArgs args = new SensorDisconnectFailedEventArgs();
                args.Message = message;
                SensorDisconnectFailed(this, args);
            }
        }


        public SensorClient(string baseUrl, string sensorID, string displayName, uint maxPartsPerStream, uint pollingIntervalMilliSeconds) : base(baseUrl) 
        {
            this.sensorID = sensorID;
            this.displayName = displayName;
            this.uploader = new Uploader(this, maxPartsPerStream);
            this.commandMonitor = new CommandMonitor(this, pollingIntervalMilliSeconds);
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public string SensorID
        {
            get { return sensorID; }
        }

        public CommandMonitor CommandMonitor
        {
            get { return commandMonitor; }
        }

        public Uploader Uploader
        {
            get { return uploader; }
        }

        public bool ConnectSensor( out string message )
        {
            message = "";
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/ConnectSensor");
            request.Method = Method.GET;
            request.AddParameter("sensorID", this.sensorID);
            request.AddParameter("displayName", displayName );
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute<BoolMessage>(request);
            if (response == null || response.Data == null)
            {
                onSensorConnectFailed(response.ErrorMessage);
                return false;
            }
            message = response.Data.Message;
            if (response.Data.Result == true)
            {
                onSensorConnected();
                return true;
            }
            else
            {
                onSensorConnectFailed( message );
                return false;
            }
        }

        public bool DisconnectSensor(out string message)
        {
            message = "";
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/DisconnectSensor");
            request.Method = Method.GET;
            request.AddParameter("sensorID", this.sensorID);
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute<BoolMessage>(request);
            if (response == null || response.Data == null)
            {
                onSensorDisconnectFailed(response.ErrorMessage);
                return false;
            }
            message = response.Data.Message;
            if (response.Data.Result == true)
            {
                onSensorDisconnected();
                return true;
            }
            else
            {
                onSensorDisconnectFailed(message);
                return false;
            }
        }

        // POST: /SensorAPI/UploadStreamPart
        public bool UploadStreamPart(StreamPart part, out string message)
        {
            message = "";
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/UploadStreamPart");
            request.Method = Method.POST;
            request.RequestFormat = DataFormat.Json;

            request.AddObject(new
            {
                SensorID = part.SensorID,
                StreamID = part.StreamID,
                SequenceNumber = part.SequenceNumber,
                IsLastPart = part.IsLastPart,
                StartTime = part.StartTime,
                DurationMilliSeconds = part.DurationMilliSeconds,
                FileName = Path.GetFileName(part.FileName),
                Base64Bytes = part.Base64Bytes
            });
            var response = RestClient.Execute<BoolMessage>(request);
            if (response == null || response.Data == null)
            {
                return false;
            }
            message = response.Data.Message;
            return response.Data.Result;
        }


        // GET: /SensorAPI/GetSensorCommand?sensorID=xxx
        public SensorCommand GetSensorCommand(out string message)
        {
            message = "";
            var request = new RestRequest();
            request.Resource = string.Format("/SensorAPI/GetSensorCommand");
            request.Method = Method.GET;
            request.AddParameter("sensorID", this.sensorID);
            request.RequestFormat = DataFormat.Json;
            var response = RestClient.Execute<BoolMessage>(request);
            if (response == null || response.Data == null)
            {
                return SensorCommand.None;
            }
            message = response.Data.Message;
            if ( (response.Data.Result == false) || string.IsNullOrEmpty(message) )
            {
                return SensorCommand.None;
            }

            return (SensorCommand)Enum.Parse(typeof(SensorCommand), message);
        }
    }

    public class SensorConnectFailedEventArgs : EventArgs
    {
        public string Message;
    }

    public class SensorDisconnectFailedEventArgs : EventArgs
    {
        public string Message;
    }
}
