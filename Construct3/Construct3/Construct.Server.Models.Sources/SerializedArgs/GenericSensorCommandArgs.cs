using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Sources
{
    [DataContract]
    public class GenericSensorCommandArgs
    {
        public GenericSensorCommandArgs()
        {
        }

        public GenericSensorCommandArgs(string sensorRendezvous, string commandName, Dictionary<string, string> argsList)
        {
            SensorRendezvous = sensorRendezvous;
            CommandName = commandName;
            ArgsList = argsList;
        }

        [DataMember]
        public string SensorRendezvous
        {
            get;
            set;
        }


        [DataMember]
        public string CommandName
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, string> ArgsList
        {
            get;
            set;
        }
    }
}