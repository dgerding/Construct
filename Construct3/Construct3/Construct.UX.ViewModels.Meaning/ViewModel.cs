using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Construct.UX.ViewModels.Meaning.MeaningServiceReference;

namespace Construct.UX.ViewModels.Meaning
{
    public class ViewModel : ViewModels.ViewModel
    {
        private CallbackImplementation callback = new CallbackImplementation();
        private InstanceContext instanceContext = null;
        private ModelClient client = null;

        private ModelClient GetModel()
        {
            if (client == null || client.State == CommunicationState.Closed || client.State == CommunicationState.Closing || client.State == CommunicationState.Faulted)
            {
                instanceContext = new InstanceContext(callback);
                client = new ModelClient(instanceContext, "WsDualHttpBinding", RemoteAddress);
                client.Open();
            }
            return client;
        }
        [ImportingConstructor]
        public ViewModel()
            : base("Meaning")
        {
        }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo)
            : base(applicationSessionInfo, "Meaning")
        {
            //callback.AddSemanticObjectCallbackReceived += ??
            //callback.AddSemanticPredicateCallbackReceived += ??
            //callback.AddSemanticSubjectCallbackReceived += ??
        }

        public override void Load()
        {
            ObservableTaxonomies = new ObservableCollection<Taxonomy>(GetTaxonomies());
            ObservableLabels = new ObservableCollection<Label>(GetLabels());
            ObservableTaxonomyLabels = new ObservableCollection<TaxonomyLabel>(GetTaxonomyLabels());
        }

        private ObservableCollection<Taxonomy> observableTaxonomies;
        public ObservableCollection<Taxonomy> ObservableTaxonomies
        {
            get
            {
                return observableTaxonomies;
            }
            set
            {
                observableTaxonomies = value;
            }
        }

        private ObservableCollection<Construct.UX.ViewModels.Meaning.MeaningServiceReference.Label> observableLabels;
        public ObservableCollection<Construct.UX.ViewModels.Meaning.MeaningServiceReference.Label> ObservableLabels
        {
            get
            {
                return observableLabels;
            }
            set
            {
                observableLabels = value;
            }
        }

        private ObservableCollection<TaxonomyLabel> observableTaxonomyLabels;
        public ObservableCollection<TaxonomyLabel> ObservableTaxonomyLabels
        {
            get
            {
                return observableTaxonomyLabels;
            }
            set
            {
                observableTaxonomyLabels = value;
            }
        }

        public Guid AddTaxonomy(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid ID = typed.ID;
            string name = typed.Name;

            Taxonomy taxonomy = new Taxonomy();
            taxonomy.ID = ID;
            taxonomy.Name = name;

            client = GetModel();
            if (client.AddTaxonomy(taxonomy))
            {
                ObservableTaxonomies.Add(taxonomy);
                return taxonomy.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid AddLabel(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid ID = typed.ID;
            string name = typed.Name;

            Label label = new Label();
            label.ID = ID;
            label.Name = name;

            client = GetModel();
            if (client.AddLabel(label))
            {
                ObservableLabels.Add(label);
                return label.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid AddTaxonomyLabel(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid ID = typed.ID;
            Guid taxonomyID = typed.TaxonomyID;
            Guid labelID = typed.LabelID;

            TaxonomyLabel taxonomyLabel = new TaxonomyLabel();
            taxonomyLabel.ID = ID;
            taxonomyLabel.LabelID = labelID;
            taxonomyLabel.TaxonomyID = taxonomyID;

            client = GetModel();
            if (client.AddTaxonomyLabel(taxonomyLabel))
            {
                observableTaxonomyLabels.Add(taxonomyLabel);
                return taxonomyLabel.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public List<Taxonomy> GetTaxonomies()
        {
            client = GetModel();
            List<Taxonomy> ret = client.GetTaxonomies();
            return ret;
        }

        public List<Label> GetLabels()
        {
            client = GetModel();
            List<Label> ret = client.GetLabels();
            return ret;
        }

        public List<TaxonomyLabel> GetTaxonomyLabels()
        {
            client = GetModel();
            List<TaxonomyLabel> ret = client.GetTaxonomyLabels();
            return ret;
        }
    }
}