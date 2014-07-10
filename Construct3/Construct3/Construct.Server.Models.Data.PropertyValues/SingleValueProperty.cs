using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Data.PropertyValue
{
    [DataContract]
    public class SinglePropertyValue
    {
        [DataMember]
        public Guid ItemID
        {
            get;
            set;
        }

        [DataMember]
        public Guid SourceID
        {
            get;
            set;
        }

        [DataMember]
        public Guid PropertyID
        {
            get;
            set;
        }

        [DataMember]
        public long? Interval
        {
            get;
            set;
        }

        [DataMember]
        public DateTime StartTime
        {
            get;
            set;
        }

        [DataMember]
        public Single Value
        {
            get;
            set;
        }

        [DataMember]
        public string Latitude
        {
            get;
            set;
        }

        [DataMember]
        public string Longitude
        {
            get;
            set;
        }
    }
}