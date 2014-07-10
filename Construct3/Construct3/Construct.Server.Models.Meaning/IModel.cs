using System;
using System.Linq;
using System.ServiceModel;
using Construct.Server.Entities.Adapters;
using System.Collections.Generic;

namespace Construct.Server.Models.Meaning
{
    [ServiceContract(CallbackContract = typeof(Meaning.ICallback))]
    public interface IModel : Models.IModel
    {
        [OperationContract(Name = "AddTaxonomy")]
        bool Add(Taxonomy taxonomy);

        [OperationContract(Name = "AddLabel")]
        bool Add(Label label);

        [OperationContract(Name = "AddTaxonomyLabel")]
        bool Add(TaxonomyLabel taxonomyLabel);

        [OperationContract]
        IEnumerable<Entities.Adapters.Taxonomy> GetTaxonomies();

        [OperationContract]
        IEnumerable<Entities.Adapters.Label> GetLabels();

        [OperationContract]
        IEnumerable<Entities.Adapters.TaxonomyLabel> GetTaxonomyLabels();
    }
}