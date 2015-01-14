using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Construct.MessageBrokering.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	/// <summary>
	/// Interaction logic for NumericPropertyVisualization.xaml
	/// </summary>
	public partial class NumericPropertyPreview : PropertyVisualization
	{
		private ConcurrentDictionary<DataSubscription, VisualizedDataSet> SubscriptionVisualizations = new ConcurrentDictionary<DataSubscription, VisualizedDataSet>();

		private SubscriptionTranslator translator;
		private SessionInfo currentDataRange;


		class VisualizedDataSet
		{
			public ChartDataSource Visualizer;
			public PropertyDataStore DataSource;
			public ObservableCollection<SimplifiedPropertyValue> VisualizedData = new ObservableCollection<SimplifiedPropertyValue>();

			private DateTime lastTime = DateTime.MinValue;

			public VisualizedDataSet(PropertyDataStore dataSource)
			{
				if (dataSource != null)
					SetDataStore(dataSource);
			}

			public void SetDataStore(PropertyDataStore dataStore)
			{
				if (DataSource != null)
					Unbind();

				DataSource = dataStore;
				DataSource.OnNewData += dataSource_OnNewData;

				//	Clear ItemsSource while refilling data to avoid event overhead
				if (Visualizer != null)
					Visualizer.ItemsSource = null;

				var sortedData = DataSource.Data.OrderBy(spv => spv.TimeStamp).ToList();
				foreach (var data in sortedData)
					VisualizedData.Add(new SimplifiedPropertyValue()
					{
						PropertyId = data.PropertyId,
						SensorId = data.SensorId,
						TimeStamp = data.TimeStamp,
						Value = Convert.ToDouble(data.Value)
					});

				if (Visualizer != null)
					Visualizer.ItemsSource = VisualizedData;
			}

			public void Unbind()
			{
				DataSource.OnNewData -= dataSource_OnNewData;

				VisualizedData.Clear();
			}

			void dataSource_OnNewData(SimplifiedPropertyValue obj)
			{
				var doubleData = new SimplifiedPropertyValue()
				{
					PropertyId = obj.PropertyId,
					SensorId = obj.SensorId,
					TimeStamp = obj.TimeStamp,
					Value = Convert.ToDouble(obj.Value)
				};

				if ((obj.TimeStamp - lastTime).TotalSeconds > 0.0)
				{
					lastTime = obj.TimeStamp;
					Visualizer.Dispatcher.BeginInvoke(new Action(() => VisualizedData.Add(doubleData)));
				}
			}
		}







		public override IEnumerable<Type> VisualizableTypes
		{
			get { return new List<Type>() {typeof (double), typeof (float), typeof (int)}; }
		}

		

		public NumericPropertyPreview(ClientDataStore dataSource, SubscriptionTranslator translator)
			: base(dataSource)
		{
			InitializeComponent();

			this.translator = translator;

			this.OnSubscriptionAdded += AddNewVisualization;
			this.OnSubscriptionRemoved += RemoveVisualization;
		}

		public override void ChangeRealTimeRangeEnd(DateTime endTime)
		{
			base.ChangeRealTimeRangeEnd(endTime);

			Dispatcher.BeginInvoke(new Action(() =>
			{
				HorizontalAxis.Maximum = endTime;
			}));
		}

		public override void ChangeVisualizationArea(SessionInfo newSessionRange)
		{
			base.ChangeVisualizationArea(newSessionRange);

			var chartInfo = new ChartVisualizationInfo()
			{
				VisSize = new Size(ChartView.PlotAreaClip.Width, ChartView.PlotAreaClip.Height)
			};
			var chartToSessionConverter = new ChartToSessionConverter();
			if (chartToSessionConverter.UpdateChartToSession(chartInfo, newSessionRange))
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					//	NOTE: MUST BE ASSIGNED IN THIS ORDER
					ChartView.Zoom = chartInfo.Zoom;
					ChartView.PanOffset = chartInfo.PanOffset;
				}));
			}
		}

		public override void ChangeVisualizedDataRange(SessionInfo sessionInfo)
		{
			base.ChangeVisualizedDataRange(sessionInfo);

			var sessionStartTime = sessionInfo.StartTime.Value;
			var sessionEndTime = sessionInfo.EndTime ?? DateTime.Now;

			currentDataRange = sessionInfo;
			Dispatcher.BeginInvoke(new Action(() =>
			{
				HorizontalAxis.Minimum = sessionStartTime;
				HorizontalAxis.Maximum = sessionEndTime;
			}));

			foreach (var dataSet in SubscriptionVisualizations)
			{
				var dataVis = dataSet.Value;
				if (dataVis.DataSource != null)
				{
					dataVis.Unbind();
					DataStore.ReleaseDataStore(dataVis.DataSource);
				}

				dataSet.Value.SetDataStore(DataStore.GenerateDataStore(dataSet.Key, sessionInfo.StartTime.Value, sessionInfo.EndTime ?? DateTime.MaxValue));
			}
		}

		void AddNewVisualization(DataSubscription subscriptionToVisualize)
		{
			var subscriptionTranslation = translator.GetTranslation(subscriptionToVisualize);

			ChartDataSource newDataSource = new ChartDataSource();
			newDataSource.SamplingUnit = SamplingTimeUnit.Second;
			newDataSource.SamplingThreshold = 0;
			LineSeries lineSeries = new LineSeries();
			lineSeries.ItemsSource = newDataSource;
			lineSeries.CategoryBinding = new GenericDataPointBinding<SimplifiedPropertyValue, DateTime>() { ValueSelector = spv => spv.TimeStamp };
			lineSeries.ValueBinding = new GenericDataPointBinding<SimplifiedPropertyValue, double>() { ValueSelector = spv => (double)spv.Value };

			lineSeries.LegendSettings = new SeriesLegendSettings()
			{
				Title = String.Format("{0} {1} - {2}", subscriptionTranslation.SourceName, subscriptionTranslation.SourceTypeName, subscriptionTranslation.PropertyName)
			};

			VisualizedDataSet newDataSet;
			if (currentDataRange != null)
				newDataSet = new VisualizedDataSet(this.DataStore.GenerateDataStore(subscriptionToVisualize, currentDataRange.StartTime.Value, currentDataRange.EndTime ?? DateTime.MaxValue));
			else
				newDataSet = new VisualizedDataSet(null);

			newDataSet.Visualizer = newDataSource;
			newDataSource.ItemsSource = newDataSet.VisualizedData;
			SubscriptionVisualizations.TryAdd(subscriptionToVisualize, newDataSet);

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Add(lineSeries)));
		}

		void RemoveVisualization(DataSubscription subscriptionToRemove)
		{
			VisualizedDataSet dataSet;
			SubscriptionVisualizations.TryRemove(subscriptionToRemove, out dataSet);
			if (dataSet.DataSource != null)
			{
				dataSet.Unbind();
				DataStore.ReleaseDataStore(dataSet.DataSource);
			}

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Remove(ChartView.Series.Single(s => s.ItemsSource == dataSet.Visualizer))));
		}
	}
}
