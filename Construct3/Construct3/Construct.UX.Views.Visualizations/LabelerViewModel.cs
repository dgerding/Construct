using System;
using System.ComponentModel;
using System.Linq;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using System.Collections.ObjectModel;

namespace Construct.UX.Views.Visualizations
{
    public class LabelerViewModel : INotifyPropertyChanged
    {
        public DateTime? EndTime
        {
            get
            {
                return this.endTime;
            }
            set
            {
                this.endTime = value;
                NotifyPropertyChanged("EndTime");
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
            set
            {
                this.startTime = value;
                NotifyPropertyChanged("StartTime");
            }
        }

        public event Action<LabeledItemAdapter> ItemLabelApplied;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ObservableCollection<object> selectedDataPoints = new ObservableCollection<object>();
        public ObservableCollection<object> SelectedDataPoints
        {
            get { return selectedDataPoints; }
        }

        private DateTime startTime;
        private DateTime? endTime;

        private Taxonomy selectedTaxonomy = null;
        public Taxonomy SelectedTaxonomy
        {
            get { return selectedTaxonomy; }
            set
            {
                selectedTaxonomy = value;
                NotifyPropertyChanged("SelectedTaxonomy");
            }
        }

        private TaxonomyLabel selectedTaxonomyLabel = null;
        public TaxonomyLabel SelectedTaxonomyLabel
        {
            get { return selectedTaxonomyLabel; }
            set
            {
                selectedTaxonomyLabel = value;
                NotifyPropertyChanged("SelectedTaxonomyLabel");
            }
        }


        private readonly ObservableCollection<Source> selectedSources = new ObservableCollection<Source>();
        public ObservableCollection<Source> SelectedSources
        {
            get { return selectedSources; }
            set
            {
                selectedSources.Clear();

                foreach (var source in value)
                {
                    selectedSources.Add(source);
                }

                NotifyPropertyChanged("SelectedSources");
            }
        }

        private readonly ObservableCollection<PropertyType> selectedProperties = new ObservableCollection<PropertyType>();
        public ObservableCollection<PropertyType> SelectedProperties
        {
            get { return selectedProperties; }
            set
            {
                selectedSources.Clear();

                foreach (var source in value)
                {
                    selectedProperties.Add(source);
                }

                NotifyPropertyChanged("SelectedProperties");
            }
        }

        private readonly ObservableCollection<int> selectedIntervals = new ObservableCollection<int>();
        public ObservableCollection<int> SelectedIntervals
        {
            get { return selectedIntervals; }
        }

        public LabelerViewModel()
        {
        }
        
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
