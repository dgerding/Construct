using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class Taxonomy
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


        public static implicit operator Entities.Taxonomy(Adapters.Taxonomy adapter)
        {
            Entities.Taxonomy result = new Entities.Taxonomy();
            
            result.ID = adapter.ID;
            result.Name = adapter.Name;
            
            return result;
        }
        public static implicit operator Adapters.Taxonomy(Entities.Taxonomy entity)
        {
            Adapters.Taxonomy result = new Adapters.Taxonomy();

            result.ID = entity.ID;
            result.Name = entity.Name;

            return result;
        }
    }
}
