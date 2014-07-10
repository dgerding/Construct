using System;
using System.Linq;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls;
using System.Collections.ObjectModel;

namespace Construct.UX.Views.Visualizations
{
    public partial class RadChartVisualization : RadSplitContainer
    {
        public event Action<LabeledItemAdapter> ItemLabeled
        {
            add { labeler.LabelApplied += value; }
            remove { labeler.LabelApplied -= value; }
        }

        public ObservableCollection<TaxonomyLabel> Labels
        {
            get { return labeler.TaxonomyLabels; }
            set
            {
                labeler.TaxonomyLabels = value;
            }
        }

        public ObservableCollection<Taxonomy> Taxonomies
        {
            get { return labeler.Taxonomies; }
            set
            {
                labeler.Taxonomies.Clear();

                foreach (var taxonomy in value)
                {
                    labeler.Taxonomies.Add(taxonomy);
                }
            }
        }

        public ObservableCollection<Source> Sources
        {
            get { return labeler.Sources; }
            set
            {
                labeler.Sources = value;
            }
        }

        public ObservableCollection<PropertyType> PropertyTypes
        {
            get { return labeler.PropertyTypes; }
            set
            {
                labeler.PropertyTypes = value;
            }
        }
        public RadChartVisualization()
        {
            InitializeComponent();
            this.DataContext = this;
            //this.labels.ItemLabelApplied += ItemLabelApplied;
        }

        public void Add(CartesianSeries series)
        {
            this.chart.Series.Add(series);
        }

        public void ChartSelectionBehaviorSelectionChanged(object sender, ChartSelectionChangedEventArgs e)
        {
            if (this.chart.SelectedPoints.Count > 0)
            {
                this.labeler.IsEnabled = true;
            }
        }
    }
}
