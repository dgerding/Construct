using System;
using System.Collections.Generic;
using System.Linq;

namespace Construct.MessageBrokering
{
    public class Message
    {
        private Guid dataTypeSourceID;
        private Guid brokerID;
        DateTime timeStamp;
        double latitude;
        double longitude;

        protected Message(Guid dataTypeSourceID)
        {
            DataTypeSourceID = dataTypeSourceID;
            TimeStamp = DateTime.Now;
        }

        protected Message(Guid dataTypeSourceID, DateTime timeStamp)
        {
            DataTypeSourceID = dataTypeSourceID;
            TimeStamp = timeStamp;
        }

        public Guid DataTypeSourceID
        {
            get
            {
                return dataTypeSourceID;
            }
            private set
            {
                dataTypeSourceID = value;
            }
        }

        public Guid BrokerID
        {
            get
            {
                return brokerID;
            }
            set
            {
                brokerID = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                timeStamp = value;
            }
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }
        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }
    }

    public class Telemetry : Message
    {
        public readonly string Name;
        public readonly Dictionary<string, string> Args;

        public Telemetry(string name, Dictionary<string, string> args) : base(GlobalRuntimeSettings.TELEMETRY_GUID)
        {
            Name = name;
            Args = args;
        }
    }

    public class Data : Message
    {
        public Data(object data, Guid sourceID, Guid dataTypeSourceID, string theDataName, Guid dataTypeID)
            : base(dataTypeSourceID)
        {
            Payload = data;
            DataName = theDataName;
            DataTypeID = dataTypeID;
            SourceID = sourceID;
        }

        public Data(object data, Guid sourceID, Guid dataTypeSourceID, string theDataName, Guid dataTypeID, DateTime messageTimeStamp)
            : base(dataTypeSourceID, messageTimeStamp)
        {
            Payload = data;
            DataName = theDataName;
            DataTypeID = dataTypeID;
            SourceID = sourceID;
        }

        private object payload;
        public object Payload
        {
            get
            {
                return payload;
            }
            set
            {
                payload = value;
            }
        }

        private string dataName;
        public string DataName
        {
            get
            {
                return dataName;
            }
            set
            {
                dataName = value;
            }
        }

        private Guid sourceID;
        public Guid SourceID
        {
            get
            {
                return sourceID;
            }
            set
            {
                sourceID = value;
            }
        }

        private Guid dataTypeID;
        public Guid DataTypeID
        {
            get
            {
                return dataTypeID;
            }
            set
            {
                dataTypeID = value;
            }
        }
    }

    public class Command : Message
    {
        public readonly string Name;
        public readonly Dictionary<string, string> Args;

        public Command(string name, Dictionary<string, string> args) : base(GlobalRuntimeSettings.COMMAND_GUID)
        {
            Name = name;
            Args = args;
        }
    }
}