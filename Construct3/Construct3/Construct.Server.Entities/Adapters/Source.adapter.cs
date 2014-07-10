using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class Source
    {
        [DataMember]
        public virtual Guid ID { get; set; }
        [DataMember]
        public virtual Guid DataTypeSourceID { get; set; }

        public static implicit operator Adapters.Source(Entities.Source source)
        {
            return new Adapters.Source()
            {
                DataTypeSourceID = source.DataTypeSourceID,
                ID = source.ID,
            };
        }

        public static implicit operator Entities.Source(Adapters.Source source)
        {
            return new Entities.Source()
            {
                DataTypeSourceID = source.DataTypeSourceID,
                ID = source.ID,
            };
        }
    }
}