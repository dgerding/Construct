using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using System.Collections.ObjectModel;

namespace Construct.UX.Views.Visualizations
{
    public partial class Labeler : UserControl
    {
        public readonly ObservableCollection<Taxonomy> Taxonomies = new ObservableCollection<Taxonomy>();
        public ObservableCollection<TaxonomyLabel> TaxonomyLabels = new ObservableCollection<TaxonomyLabel>();
        public readonly ObservableCollection<TaxonomyLabel> SelectedTaxonomyLabels = new ObservableCollection<TaxonomyLabel>();

        public Action<LabeledItemAdapter> LabelApplied;

        public ObservableCollection<Source> Sources
        {
            get { return ViewModel.SelectedSources; }
            set
            {
                ViewModel.SelectedSources = value;
                this.sourcesListBox.ItemsSource = ViewModel.SelectedSources;
            }
        }

        public ObservableCollection<PropertyType> PropertyTypes
        {
            get { return ViewModel.SelectedProperties; }
            set
            {
                ViewModel.SelectedProperties.Clear();
                this.propertiesComboBox.ItemsSource = ViewModel.SelectedSources;
            }
        }
        
        public DateTime? EndTime
        {
            get
            {
                return ViewModel.EndTime;
            }
            set
            {
                ViewModel.EndTime = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return ViewModel.StartTime;
            }
            set
            {
                ViewModel.StartTime = value;
            }
        }

        
        public LabelerViewModel ViewModel
        {
            get { return (LabelerViewModel)this.Resources["viewModel"]; }
        }

        public Labeler()
        {
            InitializeComponent();
        }

        private void TaxonomySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.SelectedTaxonomy = (Taxonomy)this.taxonomiesComboBox.SelectedItem;

            if (this.ViewModel.SelectedTaxonomy != null)
            {
                SelectedTaxonomyLabels.Clear();

                foreach (var taxonomyLabel in TaxonomyLabels.Where(target => target.TaxonomyID == this.ViewModel.SelectedTaxonomy.ID))
                {
                    SelectedTaxonomyLabels.Add(taxonomyLabel);
                }
                labelsComboBox.ItemsSource = SelectedTaxonomyLabels;

                this.labelsComboBox.IsEnabled = true;
            }
        }

        private void LabelSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.SelectedTaxonomyLabel = (TaxonomyLabel)this.labelsComboBox.SelectedItem;

            if (this.ViewModel.SelectedTaxonomyLabel != null)
            {
                this.applyBtn.IsEnabled = true;
            } 
        }

        private void CollectionSelectionChanged<T>(ObservableCollection<T> sender, SelectionChangedEventArgs e)
        {
            foreach (var addedItem in e.AddedItems)
            {
                sender.Add((T)addedItem);
            }
            foreach (var removedItem in e.RemovedItems)
            {
                sender.Remove((T)removedItem);
            }
        }

        private void SourcesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionSelectionChanged<Source>(this.ViewModel.SelectedSources, e);
        }
        private void PropertiesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionSelectionChanged<PropertyType>(this.ViewModel.SelectedProperties, e);
        }
        private void IntervalsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionSelectionChanged<int>(this.ViewModel.SelectedIntervals, e);
        }
        
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //TODO: MAJOR TODO:
            //Make sure source id is not null so you can insert into Items_Item table.
            //Probably use Supervised_Machine_Learning_Value source ID.

            if (sourcesListBox.SelectedItems.Count != 0)
            {
                foreach (var source in sourcesListBox.SelectedItems)
                {
                    if (propertiesComboBox.SelectedItems.Count != 0)
                    {
                        foreach (var property in propertiesComboBox.SelectedItems)
                        {
                            LabeledItemAdapter labeledItem = new LabeledItemAdapter()
                            {
                                LabeledItemID = Guid.NewGuid(),
                                LabeledSourceID = Guid.Parse("064a944c-9347-4e0a-9642-744f80d7bd8f"),
                                LabeledPropertyID = ((Source)property).ID,
                                LabeledStartTime = this.StartTime,
                                LabeledInterval = this.EndTime.HasValue ? (this.EndTime - this.StartTime).Value.Milliseconds : 0,
                                TaxonomyLabelID = Guid.NewGuid(), 
                                SessionID = Guid.NewGuid()                                
                            };
                            ApplyLabel(labeledItem);
                        }
                    }
                    else
                    {
                        LabeledItemAdapter labeledItem = new LabeledItemAdapter()
                        {
                            LabeledItemID = Guid.NewGuid(),
                            LabeledSourceID = ((Source)source).ID,
                            LabeledStartTime = this.StartTime,
                            LabeledInterval = this.EndTime.HasValue ? (this.EndTime - this.StartTime).Value.Milliseconds : 0,
                            TaxonomyLabelID = Guid.NewGuid(), 
                            SessionID = Guid.NewGuid() 
                        };
                        ApplyLabel(labeledItem);
                    }
                }
            }
            else
            {
                if (propertiesComboBox.SelectedItems.Count != 0)
                {
                    foreach (var property in propertiesComboBox.SelectedItems)
                    {
                        LabeledItemAdapter labeledItem = new LabeledItemAdapter()
                        {
                            LabeledItemID = Guid.NewGuid(),
                            LabeledPropertyID = ((PropertyType)property).ID,
                            LabeledStartTime = this.StartTime,
                            LabeledInterval = this.EndTime.HasValue ? (this.EndTime - this.StartTime).Value.Milliseconds : 0
                        };
                        ApplyLabel(labeledItem);
                    }
                }
                else
                {
                    LabeledItemAdapter labeledItem = new LabeledItemAdapter()
                    {
                        LabeledItemID = Guid.NewGuid(),
                        LabeledStartTime = this.StartTime,
                        LabeledInterval = this.EndTime.HasValue ? (this.EndTime - this.StartTime).Value.Milliseconds : 0,
                        TaxonomyLabelID = Guid.NewGuid(), 
                        SessionID = Guid.NewGuid() 
                    };
                    ApplyLabel(labeledItem);
                }
            }
            this.applyBtn.IsEnabled = false;
        }

        private void ApplyLabel(LabeledItemAdapter labeledItem)
        {
            if (LabelApplied != null)
            {
                LabelApplied(labeledItem);
            }
        }
    }
}
