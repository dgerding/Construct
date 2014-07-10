using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class PropertyType : PropertyParent
    {
        private Guid _propertyDataTypeID;
        [DataMember]
        public virtual Guid PropertyDataTypeID
        {
            get
            {
                return this._propertyDataTypeID;
            }
            set
            {
                this._propertyDataTypeID = value;
            }
        }

        public static implicit operator Adapters.PropertyType(Entities.PropertyType propertyType)
        {
            return new Adapters.PropertyType()
            {
                ID = propertyType.ID,
                Name = propertyType.Name,
                ParentDataTypeID = propertyType.ParentDataTypeID,
                PropertyDataTypeID = propertyType.PropertyDataTypeID
            };
        }

        public static implicit operator Entities.PropertyType(Adapters.PropertyType propertyType)
        {
            return new Entities.PropertyType()
            {
                ID = propertyType.ID,
                Name = propertyType.Name,
                ParentDataTypeID = propertyType.ParentDataTypeID,
                PropertyDataTypeID = propertyType.PropertyDataTypeID
            };
        }
    }
}