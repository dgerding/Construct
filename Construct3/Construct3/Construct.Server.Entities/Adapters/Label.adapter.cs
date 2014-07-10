using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class Label
    {
        private Guid Id;
        [DataMember]
        public virtual Guid ID
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = value;
            }
        }

        private string name;
        [DataMember]
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }


        public static implicit operator Entities.Label(Adapters.Label adapter)
        {
            Entities.Label result = new Entities.Label();

            result.ID = adapter.ID;
            result.Name = adapter.Name;

            return result;
        }
        public static implicit operator Adapters.Label(Entities.Label entity)
        {
            Adapters.Label result = new Adapters.Label();

            result.ID = entity.ID;
            result.Name = entity.Name;

            return result;
        }
    }
}
