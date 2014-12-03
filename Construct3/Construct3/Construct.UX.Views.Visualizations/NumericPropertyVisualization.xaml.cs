using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Construct.MessageBrokering.Serialization;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	/// <summary>
	/// Interaction logic for NumericPropertyVisualization.xaml
	/// </summary>
	public partial class NumericPropertyVisualization : PropertyVisualization
	{
		private DateTime LatestTime = DateTime.Now;
		private ConcurrentDictionary<DataSubscription, VisualizedDataSet> SubscriptionVisualizations = new ConcurrentDictionary<DataSubscription, VisualizedDataSet>();

		private SubscriptionTranslator Translator;
		private SessionInfo DataSession;

		private double DefaultAggregationSpanMS = 500.0;

		public bool IsAggregationEnabled { get; private set; }

		class VisualizedDataSet
		{
			public RadLinearSparkline Visualizer;
			public ConcurrentQueue<SimplifiedPropertyValue> RealData = new ConcurrentQueue<SimplifiedPropertyValue>();
			private List<SimplifiedPropertyValue> QueuedAggregateData = new List<SimplifiedPropertyValue>(100);
			public ObservableCollection<SimplifiedPropertyValue> VisualizedData = new ObservableCollection<SimplifiedPropertyValue>(); 

			public DateTime? CurrentAggregationStartTime;

			public TimeSpan AggregationTimeSpan { get; set; }
			public Func<IEnumerable<decimal>, decimal> Aggregator { get; set; }

			public bool IsAggregationEnabled { get; set; }

			public VisualizedDataSet()
			{
				IsAggregationEnabled = true;
			}

			public void AddValue(SimplifiedPropertyValue value)
			{
				RealData.Enqueue(value);

				AddToAggregation(value);
			}
			public void AddToAggregation(SimplifiedPropertyValue value)
			{
				//	Doesn't happen often, but happens occasionally at first run
				if (!CurrentAggregationStartTime.HasValue)
					return;

				if (value.TimeStamp - CurrentAggregationStartTime.Value >= AggregationTimeSpan)
				{
					decimal aggregatedValue = Aggregator(QueuedAggregateData.Select((d) => Convert.ToDecimal(d.Value)));

					QueuedAggregateData.Clear();

					var encapsulatedAggregateValue = new SimplifiedPropertyValue();
					encapsulatedAggregateValue.SensorId = value.SensorId;
					encapsulatedAggregateValue.PropertyId = value.PropertyId;
					encapsulatedAggregateValue.Value = aggregatedValue;
					encapsulatedAggregateValue.TimeStamp = CurrentAggregationStartTime.Value + AggregationTimeSpan;

					//	Aggregation timestamps are set to the end of the aggregation period, need a datapoint to represent the beginning
					//		of the data vis
					if (VisualizedData.Count == 0)
						VisualizedData.Add(new SimplifiedPropertyValue() { PropertyId = value.PropertyId, SensorId = value.SensorId, TimeStamp = CurrentAggregationStartTime.Value, Value = 0.0 });

					VisualizedData.Add(encapsulatedAggregateValue);

					CurrentAggregationStartTime += AggregationTimeSpan;
					//	If the current value spills into the next aggregation (i.e. if we just aggregated batch 4 and this number
					//		would fall into batch 6) then add a 0 for the current time
					while ((value.TimeStamp - CurrentAggregationStartTime.Value - AggregationTimeSpan).Ticks > 0)
					{
						var emptyPropertyValue = new SimplifiedPropertyValue()
						{
							SensorId = value.SensorId,
							PropertyId = value.PropertyId,
							TimeStamp = CurrentAggregationStartTime.Value + AggregationTimeSpan,
							Value = 0.0
						};

						VisualizedData.Add(emptyPropertyValue);
						CurrentAggregationStartTime += AggregationTimeSpan;
					}
				}

				QueuedAggregateData.Add(value);
			}
		}

		private Func<IEnumerable<decimal>, decimal> MinAggregator;
		private Func<IEnumerable<decimal>, decimal> MaxAggregator;
		private Func<IEnumerable<decimal>, decimal> AverageAggregator;

		public override int MaxProperties
		{
			get { return Legend.MaxEntries; }
		}

		protected override IEnumerable<Type> VisualizableTypes
		{
			get { return new List<Type>() {typeof (double), typeof (float), typeof (int)}; }
		}

		public NumericPropertyVisualization(StreamDataRouter dataRouter, SubscriptionTranslator translator, SessionInfo sessionInfo)
			: base(dataRouter, translator)
		{
			InitializeComponent();

			IsAggregationEnabled = true;

			VisualizationName = "Numeric Visualization";
			TimeBar = NumericTimeBar;
			Translator = translator;

			DataSession = sessionInfo;
			DataSession.PropertyChanged += (o, args) =>
			{
				switch (args.PropertyName)
				{
					case "StartTime":
						//	Should do more when a data playback StreamDataSource has been written (reload/re-aggregate all data,
						//		for now just assume this is the first time StartTime has been assigned)
						foreach (var visData in SubscriptionVisualizations)
							visData.Value.CurrentAggregationStartTime = (o as SessionInfo).StartTime;
						break;
				}
			};

			MinAggregator = (ds) => ds.DefaultIfEmpty().Min();
			MaxAggregator = (ds) => ds.DefaultIfEmpty().Max();
			AverageAggregator = (ds) => ds.DefaultIfEmpty().Average();

			this.OnRoutedData += NumericPropertyVisualization_OnRoutedData;
			this.OnSubscriptionAdded += NumericPropertyVisualization_OnSubscriptionAdded;
			this.OnSubscriptionRemoved += NumericPropertyVisualization_OnSubscriptionRemoved;
		}

		void NumericPropertyVisualization_OnSubscriptionRemoved(DataSubscription obj)
		{
			Legend.RemoveLegendItem(obj);
			RemoveSparkline(obj);
		}

		void NumericPropertyVisualization_OnSubscriptionAdded(DataSubscription obj)
		{
			Color sparklineColor = Legend.AddLegendItem(obj, Translator).Value;
			AddNewSparkline(obj, sparklineColor);
		}

		void AddNewSparkline(DataSubscription subscriptionToVisualize, Color sparklineColor)
		{
			VisualizedDataSet newDataSet;
			if (!SubscriptionVisualizations.TryGetValue(subscriptionToVisualize, out newDataSet))
			{
				newDataSet = new VisualizedDataSet();
				SubscriptionVisualizations.TryAdd(subscriptionToVisualize, newDataSet);
			}

			newDataSet.AggregationTimeSpan = TimeSpan.FromMilliseconds(AggregationTimeSpan.Value.GetValueOrDefault(DefaultAggregationSpanMS));
			newDataSet.Aggregator = GetAggregator(AggregationType.Text);
			newDataSet.VisualizedData = new ObservableCollection<SimplifiedPropertyValue>();

			if (DataSession.StartTime.HasValue)
			{
				//	Regenerate visualized aggregate data (if data existed beforehand)
				newDataSet.CurrentAggregationStartTime = DataSession.StartTime;
				foreach (var value in newDataSet.RealData)
					newDataSet.AddToAggregation(value);
			}

			RadLinearSparkline newSparkline = new RadLinearSparkline();
			//	Manually assigned brush values (set to defaults) to remove UnsetValue exceptions

			//	http://stackoverflow.com/questions/10062376/creating-solidcolorbrush-from-hex-color-value
			var brushConverter = new BrushConverter();
			newSparkline.AxisStroke = (SolidColorBrush)brushConverter.ConvertFrom("#FF000000");
			newSparkline.NormalRangeFill = (SolidColorBrush)brushConverter.ConvertFrom("#66D2D2FF");
			newSparkline.LineStroke = new SolidColorBrush(sparklineColor);
			newSparkline.XValuePath = "TimeStamp";
			newSparkline.YValuePath = "Value";
			newSparkline.ItemsSource = newDataSet.VisualizedData;

			newDataSet.Visualizer = newSparkline;

			SparklinesContainer.Dispatcher.BeginInvoke(new Action(() => SparklinesContainer.Children.Add(newSparkline)));
		}

		void RemoveSparkline(DataSubscription subscriptionToRemove)
		{
			VisualizedDataSet dataSet;
			SubscriptionVisualizations.TryGetValue(subscriptionToRemove, out dataSet);

			SparklinesContainer.Dispatcher.BeginInvoke(new Action(() => SparklinesContainer.Children.Remove(dataSet.Visualizer)));
			dataSet.Visualizer.ItemsSource = null;
			dataSet.Visualizer = null;
		}

		void NumericPropertyVisualization_OnRoutedData(SimplifiedPropertyValue obj)
		{
			if ((LatestTime - obj.TimeStamp).TotalMilliseconds > 0.0)
				return;

			LatestTime = obj.TimeStamp;

			Dispatcher.BeginInvoke(new Action(() =>
			{
				DataSubscription subscriptionForData = new DataSubscription()
				{
					PropertyId = obj.PropertyId,
					SourceId = obj.SensorId
				};
				VisualizedDataSet visualizationForData;
				if (SubscriptionVisualizations.TryGetValue(subscriptionForData, out visualizationForData))
					visualizationForData.AddValue(obj);
			}));
		}

		Func<IEnumerable<decimal>, decimal> GetAggregator(String aggregatorName)
		{
			switch (aggregatorName.ToLowerInvariant())
			{
				case ("min"):
					return MinAggregator;
				case ("max"):
					return MaxAggregator;
				case ("average"):
					return AverageAggregator;
				default:
					return null;
			}
		}

		private void ChangeDataVisualizationsAggregation(String aggregatorName, TimeSpan aggregationLength)
		{
			var aggregator = GetAggregator(aggregatorName);
			foreach (var mapping in SubscriptionVisualizations)
			{
				var dataSet = mapping.Value;
				dataSet.Aggregator = aggregator;
				dataSet.AggregationTimeSpan = aggregationLength;

				dataSet.VisualizedData = new ObservableCollection<SimplifiedPropertyValue>();
				dataSet.CurrentAggregationStartTime = DataSession.StartTime;
				foreach (var propertyValue in dataSet.RealData)
					dataSet.AddToAggregation(propertyValue);

				dataSet.Visualizer.ItemsSource = dataSet.VisualizedData;
			}
		}
		private void ChangeAggregationButton_Click(object sender, RoutedEventArgs e)
		{
			if (!AggregationTimeSpan.Value.HasValue)
				AggregationTimeSpan.Value = DefaultAggregationSpanMS;

			//	Need > 0 aggregation span (although tiny time-spans will look strange for most datasets, limit to 50ms)
			if (AggregationTimeSpan.Value < 50)
				AggregationTimeSpan.Value = 50;

			ChangeDataVisualizationsAggregation(AggregationType.Text, TimeSpan.FromMilliseconds(AggregationTimeSpan.Value.Value));
		}

		private void OnToggleAggregation(object sender, RoutedEventArgs e)
		{
			var button = sender as RadButton;
			IsAggregationEnabled = !IsAggregationEnabled;

			button.Content = IsAggregationEnabled ? "Disable" : "Enable";

			foreach (var subscriptionVis in SubscriptionVisualizations)
			{
				subscriptionVis.Value.IsAggregationEnabled = IsAggregationEnabled;
				var currentDataVis = subscriptionVis.Value;
				if (IsAggregationEnabled)
				{
					currentDataVis.VisualizedData = new ObservableCollection<SimplifiedPropertyValue>();
					currentDataVis.CurrentAggregationStartTime = DataSession.StartTime;
					foreach (var data in currentDataVis.RealData)
						currentDataVis.AddToAggregation(data);
					currentDataVis.Visualizer.ItemsSource = currentDataVis.VisualizedData;
				}
				else
				{
					currentDataVis.VisualizedData = new ObservableCollection<SimplifiedPropertyValue>(currentDataVis.RealData);
					currentDataVis.Visualizer.ItemsSource = currentDataVis.VisualizedData;
				}
			}
		}
	}
}
