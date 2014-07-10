using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Sources
{
    [DataContract]
    public class LoadSensorArgs
    {
        public LoadSensorArgs()
        {
        }

        public LoadSensorArgs(string sensorTypeSourceID, string version, string sourceID, string hostID, string startupArgs)
        {
            SensorTypeSourceID = sensorTypeSourceID;
            Version = version;
            SourceID = sourceID;
            HostID = hostID;
            StartupArgs = startupArgs;
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

        [DataMember]
        public string StartupArgs
        {
            get;
            set;
        }
    }
}