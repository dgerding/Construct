using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class Session
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public long Interval { get; set; }
        [DataMember]
        public string FriendlyName { get; set; }

        public static implicit operator Adapters.Session(Entities.Session entity)
        {
            return new Adapters.Session()
            {
                ID = entity.ID,
                StartTime = entity.StartTime,
                Interval = entity.Interval,
                FriendlyName = entity.FriendlyName
            };
        }

        public static implicit operator Entities.Session(Adapters.Session adapter)
        {
            Entities.Session result = new Entities.Session()
            {
                ID = adapter.ID,
                StartTime = adapter.StartTime,
                Interval = adapter.Interval,
                FriendlyName = adapter.FriendlyName
            };
            return result;
        }
    }
}
