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
				if (SourceDataStore != null)
					SourceDataStore.OnNewData -= SourceDataStore_OnNewData;
			}

			public void ChangeDataStore(PropertyDataStore dataStore)
			{
				if (SourceDataStore != null)
					SourceDataStore.OnNewData -= SourceDataStore_OnNewData;
				SourceDataStore = dataStore;

				//	Remove ItemsSource binding while refilling data
				Visualizer.ItemsSource = null;

				var sortedData = SourceDataStore.Data.OrderBy(spv => spv.TimeStamp).ToList();
				VisualizedData.Clear();
				foreach (var data in sortedData)
					VisualizedData.Add(new SimplifiedPropertyValue()
					{
						PropertyId = data.PropertyId,
						SensorId = data.SensorId,
						TimeStamp = data.TimeStamp,
						Value = Convert.ToDouble(data.Value)
					});

				Visualizer.ItemsSource = VisualizedData;

				SourceDataStore.OnNewData += SourceDataStore_OnNewData;
			}

			void SourceDataStore_OnNewData(SimplifiedPropertyValue obj)
			{
				var doubleData = new SimplifiedPropertyValue()
				{
					PropertyId = obj.PropertyId,
					SensorId = obj.SensorId,
					TimeStamp = obj.TimeStamp,
					Value = Convert.ToDouble(obj.Value)
				};
				var dataList = VisualizedData.ToList();
				var insertIndex = dataList.FindIndex(spv => spv.TimeStamp.Ticks > obj.TimeStamp.Ticks);
				if (insertIndex < 0)
					VisualizedData.Add(doubleData);
				else
					VisualizedData.Insert(insertIndex, doubleData);
			}
		}

		public override IEnumerable<Type> VisualizableTypes
		{
			get { return new List<Type>() {typeof (double), typeof (float), typeof (int)}; }
		}

		public override void ChangeVisualizedDataRange(SessionInfo sessionInfo)
		{
			base.ChangeVisualizedDataRange(sessionInfo);

			Dispatcher.BeginInvoke(new Action(() =>
			{
				this.HorizontalAxis.Minimum = sessionInfo.SelectedStartTime.Value;
				this.HorizontalAxis.Maximum = sessionInfo.SelectedEndTime.Value;
			}));

			foreach (var subscriptionVisualization in SubscriptionVisualizations)
			{
				var subscription = subscriptionVisualization.Key;
				var dataVis = subscriptionVisualization.Value;

				dataVis.Unbind();
				if (dataVis.SourceDataStore != null)
					DataStore.ReleaseDataStore(dataVis.SourceDataStore);

				dataVis.ChangeDataStore(DataStore.GenerateDataStore(subscription, sessionInfo.SelectedStartTime.Value, sessionInfo.SelectedEndTime.Value));
			}
		}

		public NumericPropertyDetails(ClientDataStore dataStore, SubscriptionTranslator translator)
			: base(dataStore)
		{
			InitializeComponent();

			Translator = translator;

			this.OnSubscriptionAdded += AddNewVisualization;
			this.OnSubscriptionRemoved += RemoveVisualization;
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
			if (visualization.SourceDataStore != null)
			{
				visualization.Unbind();
				DataStore.ReleaseDataStore(visualization.SourceDataStore);
			}
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
