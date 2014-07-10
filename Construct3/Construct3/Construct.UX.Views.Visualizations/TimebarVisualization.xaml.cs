using System;
using System.Linq;
using Telerik.Windows.Controls.TimeBar;
using Telerik.Windows.Controls.Sparklines;
using System.Collections.Specialized;
using Telerik.Windows.Controls;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using System.Collections.ObjectModel;

namespace Construct.UX.Views.Visualizations
{
    public partial class TimebarVisualization : RadSplitContainer
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



        public TimebarVisualization()
        {
            InitializeComponent();

            this.DataContext = this;
            this.ContentGrid.DataContext = this;
            this.timeBar.SelectionChanged += TimeBarSelectionChanged;
        }

        private void TimeBarSelectionChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DateTime start = this.timeBar.SelectionStart;
            DateTime end = this.timeBar.SelectionEnd;

            this.labeler.IsEnabled = true;
            this.labeler.StartTime = start;
            this.labeler.EndTime = end;
        }

        public ObservableCollection<RadSparklineBase> Sparklines = new ObservableCollection<RadSparklineBase>();

        public void Add(RadSparklineBase series)
        {
            this.Sparklines.Add(series);
            //this.ContentGrid.ItemsSource = this.Sparklines;
        }

        DateTime? earlyTime = null;

        public void TimesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    dynamic element = item;

                    if (earlyTime.HasValue)
                    {
                        if (element.StartTime < earlyTime.Value)
                        {
                            earlyTime = element.StartTime;
                            this.timeBar.PeriodStart = earlyTime.Value;
                        }
                    }
                    else
                    {
                        earlyTime = element.StartTime;
                        this.timeBar.PeriodStart = earlyTime.Value;
                    }

                    if (element.StartTime > this.timeBar.PeriodEnd)
                    {
                        this.timeBar.PeriodEnd = element.StartTime;
                        TimeSpan difference = this.timeBar.PeriodEnd - this.timeBar.PeriodStart;
                        GetIntervals(difference);
                    }
                }
            }
        }

        private void SetInterval<T>(int set, int comparer, int? baseValue = null)
            where T : Telerik.Windows.Controls.TimeBar.IntervalBase, new()
        {
            if (set > 0)
            {
                this.timeBar.Intervals.Clear();

                T interval = new T();

                int ratio = 0;

                if (baseValue.HasValue)
                {
                    ratio = baseValue.Value / comparer;
                }
                else
                {
                    ratio = comparer;
                }

                float temp = ratio;
                while (temp > 1)
                {
                    interval.IntervalSpans.Add((int)temp);
                    temp = (float)temp / (float)comparer;
                }
                    
                this.timeBar.Intervals.Add(interval);
            }
        }

        private void GetIntervals(TimeSpan difference)
        {
            SetInterval<MillisecondInterval>(difference.Milliseconds, 10, 1000);
            SetInterval<SecondInterval>(difference.Seconds, 2, 60);
            SetInterval<MinuteInterval>(difference.Minutes, 2, 60);
            SetInterval<HourInterval>(difference.Hours, 2, 24);
            SetInterval<DayInterval>(difference.Days, 2, 7);
            //SetInterval<WeekInterval>(difference.Days, 2, 4);
            //SetInterval<MonthInterval>(difference.Days, 2, 30);
            //SetInterval<YearInterval>(difference.Days, 2, 365);
            //SetInterval<DecadeInterval>(difference.Days, 2, 3650);
            //SetInterval<CenturyInterval>(difference.Days, 2, 36500);
        }
    }
}
