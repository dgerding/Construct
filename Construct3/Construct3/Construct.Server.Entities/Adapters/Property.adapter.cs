using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class Property
    {
        private string _tag;
        [DataMember]
        public virtual string Tag
        {
            get
            {
                return this._tag;
            }
            set
            {
                this._tag = value;
            }
        }

        private string _name;
        [DataMember]
        public virtual string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        private Guid _iD;
        [DataMember]
        public virtual Guid ID
        {
            get
            {
                return this._iD;
            }
            set
            {
                this._iD = value;
            }
        }

        public static implicit operator Adapters.Property(Entities.Property property)
        {
            return new Adapters.Property()
            {
                ID = property.ID,
                Name = property.Name,
            };
        }

        public static implicit operator Entities.Property(Adapters.Property propertyType)
        {
            return new Entities.Property()
            {
                ID = propertyType.ID,
                Name = propertyType.Name,
            };
        }
    }
}