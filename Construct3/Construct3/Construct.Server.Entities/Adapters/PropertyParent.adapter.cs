using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class PropertyParent : Property
    {
        private Guid _parentDataTypeID;
        [DataMember]
        public virtual Guid ParentDataTypeID
        {
            get
            {
                return this._parentDataTypeID;
            }
            set
            {
                this._parentDataTypeID = value;
            }
        }


        public static implicit operator Entities.PropertyParent(Adapters.PropertyParent propertyParent)
        {
            return new Entities.PropertyParent()
            {
                ID = propertyParent.ID,
                Name = propertyParent.Name,
                ParentDataTypeID = propertyParent.ParentDataTypeID
            };
        }

        public static implicit operator Adapters.PropertyParent(Entities.PropertyParent propertyType)
        {
            return new Adapters.PropertyParent()
            {
                ID = propertyType.ID,
                Name = propertyType.Name,
                ParentDataTypeID = propertyType.ParentDataTypeID
            };
        }
    }
}