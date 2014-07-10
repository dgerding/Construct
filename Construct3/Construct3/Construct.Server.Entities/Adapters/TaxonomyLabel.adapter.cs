using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class TaxonomyLabel
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

        private Guid taxonomyID;
        [DataMember]
        public virtual Guid TaxonomyID
        {
            get
            {
                return this.taxonomyID;
            }
            set
            {
                this.taxonomyID = value;
            }
        }

        private Guid labelID;
        [DataMember]
        public virtual Guid LabelID
        {
            get
            {
                return this.labelID;
            }
            set
            {
                this.labelID = value;
            }
        }

        public static implicit operator Entities.TaxonomyLabel(Adapters.TaxonomyLabel adapter)
        {
            Entities.TaxonomyLabel result = new Entities.TaxonomyLabel();

            result.ID = adapter.ID;
            result.LabelID = adapter.LabelID;
            result.TaxonomyID = adapter.TaxonomyID;

            return result;
        }
        public static implicit operator Adapters.TaxonomyLabel(Entities.TaxonomyLabel entity)
        {
            Adapters.TaxonomyLabel result = new Adapters.TaxonomyLabel();

            result.ID = entity.ID;
            result.LabelID = entity.LabelID;
            result.TaxonomyID = entity.TaxonomyID;

            return result;
        }
    }
}
