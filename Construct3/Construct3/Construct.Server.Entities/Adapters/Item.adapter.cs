using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class Item
    {
        [DataMember]
        public virtual DateTime SourceTime { get; set; }
        [DataMember]
        public virtual Guid SourceId { get; set; }
        [DataMember]
        public virtual DateTime RecordCreationTime { get; set; }
        [DataMember]
        public virtual string Longitude { get; set; }
        [DataMember]
        public virtual string Latitude { get; set; }
        [DataMember]
        public virtual Guid ID { get; set; }
        [DataMember]
        public virtual Guid DataTypeID { get; set; }
        
        public static implicit operator Adapters.Item(Entities.Item item)
        {
            return new Adapters.Item()
            {
                DataTypeID = item.DataTypeID,
                ID = item.ID,
                SourceId = item.SourceId,
                Latitude = item.Latitude,
                SourceTime = item.SourceTime,
                Longitude = item.Longitude,
                RecordCreationTime = item.RecordCreationTime
            };  
        }

        public static implicit operator Entities.Item(Adapters.Item item)
        {
            return new Entities.Item()
            {
                DataTypeID = item.DataTypeID,
                ID = item.ID,
                SourceId = item.SourceId,
                Latitude = item.Latitude,
                SourceTime = item.SourceTime,
                Longitude = item.Longitude,
                RecordCreationTime = item.RecordCreationTime
            };
        } 
    }
}