using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Sources
{
    [DataContract]
    public class StartSensorArgs
    {
        public StartSensorArgs()
        {
        }

        public StartSensorArgs(string sensorTypeSourceID, string version, string sourceID, string hostID)
        {
            SensorTypeSourceID = sensorTypeSourceID;
            Version = version;
            SourceID = sourceID;
            HostID = hostID;
        }

        [DataMember]
        public string SensorTypeSourceID
        {
            get;
            set;
        }

        [DataMember]
        public string Version
        {
            get;
            set;
        }

        [DataMember]
        public string SourceID
        {
            get;
            set;
        }

        [DataMember]
        public string HostID
        {
            get;
            set;
        }
    }
}