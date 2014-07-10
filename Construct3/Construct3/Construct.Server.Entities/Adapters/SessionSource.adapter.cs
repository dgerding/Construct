using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class SessionSource
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid SessionID { get; set; }
        [DataMember]
        public Guid SourceID { get; set; }

        public static implicit operator Adapters.SessionSource(Entities.SessionSource entity)
        {
            return new Adapters.SessionSource()
            {
                ID = entity.ID,
                SessionID = entity.SessionID,
                SourceID = entity.SourceID
            };
        }

        public static implicit operator Entities.SessionSource(Adapters.SessionSource adapter)
        {
            Entities.SessionSource result = new Entities.SessionSource()
            {
                ID = adapter.ID,
                SessionID = adapter.SessionID,
                SourceID = adapter.SourceID
            };
            return result;
        }
    }
}
