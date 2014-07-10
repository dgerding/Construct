using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class DataType
    {
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public bool IsCoreType { get; set; }
        [DataMember]
        public bool IsReadOnly { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid DataTypeSourceID { get; set; }
        //TODO: This memeber will allow us to support COMPOSITION inside our dataTypes. 
        //TODO: This way a type we create can have more than just n flat (primitive) properties, it can have DataTypes that themselves have properties.
        [DataMember]
        public Guid DataTypeParentID { get; set; }

        public static implicit operator Adapters.DataType(Entities.DataType entity)
        {
            return new Adapters.DataType()
            {
                FullName = entity.FullName,
                ID = entity.ID,
                IsCoreType = entity.IsCoreType,
                IsReadOnly = entity.IsReadOnly,
                Name = entity.Name,
                DataTypeParentID = entity.DataTypeParentID,
                DataTypeSourceID = entity.DataTypeSourceID
            };
        }

        public static implicit operator Entities.DataType(Adapters.DataType adapter)
        {
            Entities.DataType result = new Entities.DataType()
            {
                FullName = adapter.FullName,
                ID = adapter.ID,
                IsCoreType = adapter.IsCoreType,
                IsReadOnly = adapter.IsReadOnly,
                Name = adapter.Name,
                DataTypeParentID = adapter.DataTypeParentID,
                DataTypeSourceID = adapter.DataTypeSourceID
            };
            return result;
        }
    }
}