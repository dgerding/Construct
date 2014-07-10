using System;
using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;

namespace Construct.UX.Views.Visualizations
{
    public partial class VisualizationWindow : UserControl
    {
        private ObservableCollection<IntVisualizer> intVisualizers = new ObservableCollection<IntVisualizer>();
        private ObservableCollection<BooleanVisualizer> booleanVisualizers = new ObservableCollection<BooleanVisualizer>();

        private ObservableCollection<TaxonomyLabel> labels = new ObservableCollection<TaxonomyLabel>();
        public ObservableCollection<TaxonomyLabel> Labels 
        { 
            get { return labels; }
            set
            {
                labels = value;

                timeBar.Labels = value;
                detailsView.Labels = value;
            }
        }

        private ObservableCollection<Taxonomy> taxonomies = new ObservableCollection<Taxonomy>();
        public ObservableCollection<Taxonomy> Taxonomies
        {
            get { return taxonomies; }
            set
            {
                taxonomies = value;

                timeBar.Taxonomies.Clear();
                detailsView.Taxonomies.Clear();
                foreach (var taxonomy in value)
                {
                    timeBar.Taxonomies.Add(taxonomy);
                    detailsView.Taxonomies.Add(taxonomy);
                }
            }
        }

        private ObservableCollection<PropertyType> properties = new ObservableCollection<PropertyType>();
        public ObservableCollection<PropertyType> Properties
        {
            get { return properties; }
            set
            {
                properties = value;

                timeBar.PropertyTypes = value;
                detailsView.PropertyTypes = value;
            }
        }

        private ObservableCollection<Source> sources = new ObservableCollection<Source>();
        public ObservableCollection<Source> Sources
        {
            get { return sources; }
            set
            {
                sources = value;

                detailsView.Sources = value;
                timeBar.Sources = value;
            }
        }

        public event Action<LabeledItemAdapter> ItemLabeled
        {
            add 
            {
                timeBar.ItemLabeled += value;
                detailsView.ItemLabeled += value;
            }
            remove 
            {
                timeBar.ItemLabeled -= value;
                detailsView.ItemLabeled -= value;
            }
        }
        public VisualizationWindow()
        {
            InitializeComponent();
            timeBar.timeBar.SelectionChanged += TimeBarSelectionChanged;
        }


        private void TimeBarSelectionChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            this.detailsView.chart.Series.Clear();

            foreach (IntVisualizer visualizer in intVisualizers)
            {
                LineSeries series = new LineSeries();
                series.ItemsSource = from property in visualizer.Collection
                                     where
                                            property.StartTime > timeBar.timeBar.SelectionStart &&
                                            property.StartTime < timeBar.timeBar.SelectionEnd
                                     select property;
                    
                series.ValueBinding = new PropertyNameDataPointBinding("Value");
                series.CategoryBinding = new PropertyNameDataPointBinding("StartTime");
                series.CombineMode = Telerik.Charting.ChartSeriesCombineMode.None;

                LinearAxis axis = new LinearAxis();
                series.VerticalAxis = axis;
                series.AllowSelect = true;
                series.PointTemplate = this.detailsView.chart.Resources["normalPointTemplate"] as DataTemplate;

                this.detailsView.Add(series);
            }
        }

        public void Add(IntVisualizer visualizer)
        {
            visualizer.Collection.CollectionChanged += timeBar.TimesChanged;
            intVisualizers.Add(visualizer);

            RadLinearSparkline series = new RadLinearSparkline();
            series.DataContext = visualizer;
            series.SetBinding(RadLinearSparkline.ItemsSourceProperty, "Collection");
            //series.XValuePath = "Value";
            series.YValuePath = "StartTime";

            timeBar.Add(series);
        }

        public void Add(BooleanVisualizer visualizer)
        {
            visualizer.Collection.CollectionChanged += timeBar.TimesChanged;
            booleanVisualizers.Add(visualizer);

            RadWinLossSparkline series = new RadWinLossSparkline();
            series.DataContext = visualizer;
            series.ItemsSource = visualizer.Collection;
            series.SetBinding(RadLinearSparkline.ItemsSourceProperty, "Collection");
            series.XValuePath = "Value";
            series.YValuePath = "StartTime";

            timeBar.Add(series);
        }
    }
}
