using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Construct.MessageBrokering.Serialization;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	/// <summary>
	/// Interaction logic for NumericPropertyDetails.xaml
	/// </summary>
	public partial class NumericPropertyDetails : PropertyVisualization
	{
		private ConcurrentDictionary<DataSubscription, DataVisualization> SubscriptionVisualizations =
			new ConcurrentDictionary<DataSubscription, DataVisualization>();

		private SubscriptionTranslator Translator;

		class DataVisualization
		{
			public PropertyDataStore SourceDataStore;
			public LineSeries Visualizer;

			public readonly ObservableCollection<SimplifiedPropertyValue> VisualizedData = new ObservableCollection<SimplifiedPropertyValue>();

			public DataVisualization(PropertyDataStore dataStore)
			{
				if (dataStore != null)
					ChangeDataStore(dataStore);
			}

			public void Unbind()
			{
				SourceDataStore.OnNewData -= SourceDataStore_OnNewData;
			}

			public void ChangeDataStore(PropertyDataStore dataStore)
			{
				if (SourceDataStore != null)
					SourceDataStore.OnNewData -= SourceDataStore_OnNewData;
				SourceDataStore = dataStore;

				var sortedData = SourceDataStore.Data.OrderBy(spv => spv.TimeStamp).ToList();
				VisualizedData.Clear();
				foreach (var data in sortedData)
					VisualizedData.Add(data);

				SourceDataStore.OnNewData += SourceDataStore_OnNewData;
			}

			void SourceDataStore_OnNewData(SimplifiedPropertyValue obj)
			{
				var dataList = VisualizedData.ToList();
				var insertIndex = dataList.FindIndex(spv => spv.TimeStamp.Ticks > obj.TimeStamp.Ticks);
				if (insertIndex < 0)
					VisualizedData.Add(obj);
				else
					VisualizedData.Insert(insertIndex, obj);
			}
		}

		public override IEnumerable<Type> VisualizableTypes
		{
			get { return new List<Type>() {typeof (double), typeof (float), typeof (int)}; }
		}

		public override void ChangeVisualizationArea(SessionInfo newSessionRange)
		{
			base.ChangeVisualizationArea(newSessionRange);

			foreach (var subscriptionVisualization in SubscriptionVisualizations)
			{
				var subscription = subscriptionVisualization.Key;
				var dataVis = subscriptionVisualization.Value;
				var previousDataStore = dataVis.SourceDataStore;

				dataVis.ChangeDataStore(DataStore.GenerateDataStore(subscription, newSessionRange.SelectedStartTime.Value, newSessionRange.SelectedEndTime.Value));

				if (previousDataStore != null)
					DataStore.ReleaseDataStore(previousDataStore);
			}
		}

		public NumericPropertyDetails(ClientDataStore dataStore, SubscriptionTranslator translator)
			: base(dataStore)
		{
			InitializeComponent();

			VisualizationName = "Numeric Visualization";
			Translator = translator;

			ChartView.PanOffsetChanged += ChartView_PanOffsetChanged;
			ChartView.ZoomChanged += ChartView_ZoomChanged;

			this.OnSubscriptionAdded += AddNewVisualization;
			this.OnSubscriptionRemoved += RemoveVisualization;
		}

		void ChartView_ZoomChanged(object sender, ChartZoomChangedEventArgs e)
		{
			NotifyUserChangedVisualizationRange(new ChartVisualizationInfo()
			{
				PanOffset = ChartView.PanOffset,
				Zoom = ChartView.Zoom,
				VisSize = new Size(ChartView.PlotAreaClip.Width, ChartView.PlotAreaClip.Height)
			});
		}

		void ChartView_PanOffsetChanged(object sender, ChartPanOffsetChangedEventArgs e)
		{
			NotifyUserChangedVisualizationRange(new ChartVisualizationInfo()
			{
				PanOffset = ChartView.PanOffset,
				Zoom = ChartView.Zoom,
				VisSize = new Size(ChartView.PlotAreaClip.Width, ChartView.PlotAreaClip.Height)
			});
		}

		void AddNewVisualization(DataSubscription subscriptionToVisualize)
		{
			var newVisualization = new DataVisualization(null);
			SubscriptionVisualizations.TryAdd(subscriptionToVisualize, newVisualization);

			var subscriptionTranslation = Translator.GetTranslation(subscriptionToVisualize);
			LineSeries lineSeries = new LineSeries();
			lineSeries.ItemsSource = newVisualization.VisualizedData;
			lineSeries.CategoryBinding = new GenericDataPointBinding<SimplifiedPropertyValue, DateTime>() { ValueSelector = spv => spv.TimeStamp };
			lineSeries.ValueBinding = new GenericDataPointBinding<SimplifiedPropertyValue, double>() { ValueSelector = spv => (double)spv.Value };

			lineSeries.LegendSettings = new SeriesLegendSettings()
			{
				Title = String.Format("{0} {1} - {2}", subscriptionTranslation.SourceName, subscriptionTranslation.SourceTypeName, subscriptionTranslation.PropertyName)
			};

			lineSeries.DisplayName = lineSeries.LegendSettings.Title;

			newVisualization.Visualizer = lineSeries;

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Add(lineSeries)));
		}

		void RemoveVisualization(DataSubscription subscriptionToRemove)
		{
			DataVisualization visualization;
			SubscriptionVisualizations.TryRemove(subscriptionToRemove, out visualization);
			visualization.Unbind();
			DataStore.ReleaseDataStore(visualization.SourceDataStore);

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Remove(visualization.Visualizer)));
		}

		private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
		{
			foreach (var info in e.Context.DataPointInfos)
			{
				var datum = (SimplifiedPropertyValue)info.DataPoint.DataItem;
				var translation =
					Translator.GetTranslation(new DataSubscription() {PropertyId = datum.PropertyId, SourceId = datum.SensorId});
				info.DisplayContent = String.Format("{0} {1}\nValue: {2}\n(Timestamp: {3:T})", translation.SourceName, translation.PropertyName, datum.Value, datum.TimeStamp);
			}
		}
	}
}
