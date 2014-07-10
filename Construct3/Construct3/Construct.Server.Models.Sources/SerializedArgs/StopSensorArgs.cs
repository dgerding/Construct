using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Sources
{
    [DataContract]
    public class StopSensorArgs
    {
        public StopSensorArgs()
        {
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