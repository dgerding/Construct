using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	/// <summary>
	/// Interaction logic for TextPropertyPreview.xaml
	/// </summary>
	public partial class TextPropertyPreview : PropertyVisualization
	{
		private ConcurrentDictionary<DataSubscription, VisualizedDataSet> SubscriptionVisualizations = new ConcurrentDictionary<DataSubscription, VisualizedDataSet>();
		private SubscriptionTranslator Translator;

		class VisualizedDataSet
		{
			public ChartDataSource Visualizer;
			public PropertyDataStore DataSource;
			public ObservableCollection<SimplifiedPropertyValue> VisualizedData = new ObservableCollection<SimplifiedPropertyValue>();

			private DateTime lastTime = DateTime.MinValue;

			public VisualizedDataSet(PropertyDataStore dataSource)
			{
				DataSource = dataSource;

				var sortedData = DataSource.Data.OrderBy(spv => spv.TimeStamp).ToList();
				foreach (var existingData in sortedData)
					VisualizedData.Add(new SimplifiedPropertyValue()
					{
						PropertyId = existingData.PropertyId,
						SensorId = existingData.SensorId,
						TimeStamp = existingData.TimeStamp,
						Value = 1
					});

				DataSource.OnNewData += dataSource_OnNewData;
			}

			public void Dispose()
			{
				DataSource.OnNewData -= dataSource_OnNewData;
			}

			void dataSource_OnNewData(SimplifiedPropertyValue obj)
			{
				var statisticalValue = new SimplifiedPropertyValue()
				{
					PropertyId = obj.PropertyId,
					SensorId = obj.SensorId,
					TimeStamp = obj.TimeStamp,
					Value = 1
				};

				if ((obj.TimeStamp - lastTime).TotalSeconds > 0.0)
					Visualizer.Dispatcher.BeginInvoke(new Action(() => VisualizedData.Add(statisticalValue)));
			}
		}


		public override IEnumerable<Type> VisualizableTypes
		{
			get { return new List<Type> {typeof (String)}; }
		}


		public TextPropertyPreview(ClientDataStore dataStore, SubscriptionTranslator translator, SessionInfo sessionInfo) :
			base(dataStore)
		{
			InitializeComponent();

			Translator = translator;

			sessionInfo.PropertyChanged += sessionInfo_PropertyChanged;
			if (sessionInfo.StartTime.HasValue)
				HorizontalAxis.Minimum = sessionInfo.StartTime.Value;

			this.OnSubscriptionAdded += AddNewVisualization;
			this.OnSubscriptionRemoved += RemoveVisualization;
		}

		void sessionInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StartTime")
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					HorizontalAxis.Minimum = (sender as SessionInfo).StartTime.Value;
				}));
			}
		}

		public override void ChangeVisualizationArea(SessionInfo newSessionRange)
		{
			base.ChangeVisualizationArea(newSessionRange);

			var chartInfo = new ChartVisualizationInfo()
			{
				VisSize = new Size(ChartView.PlotAreaClip.Width, ChartView.PlotAreaClip.Height),
				PanOffset = ChartView.PanOffset,
				Zoom = ChartView.Zoom
			};

			var chartToSessionConverter = new ChartToSessionConverter();
			if (chartToSessionConverter.UpdateChartToSession(chartInfo, newSessionRange))
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					ChartView.PanOffset = chartInfo.PanOffset;
					ChartView.Zoom = chartInfo.Zoom;
				}));
			}
		}

		public override void ChangeRealTimeRangeEnd(DateTime endTime)
		{
			base.ChangeRealTimeRangeEnd(endTime);

			Dispatcher.BeginInvoke(new Action(() =>
			{
				HorizontalAxis.Maximum = endTime;
			}));
		}


		void AddNewVisualization(DataSubscription subscriptionToVisualize)
		{
			var subscriptionTranslation = Translator.GetTranslation(subscriptionToVisualize);

			ChartDataSource newDataSource = new ChartDataSource();
			newDataSource.SamplingUnit = SamplingTimeUnit.Second;
			newDataSource.SamplingUnitInterval = 5;
			SumAreaSeries areaSeries = new SumAreaSeries();
			areaSeries.CombineMode = ChartSeriesCombineMode.Stack;
			areaSeries.ItemsSource = newDataSource;
			areaSeries.CategoryBinding = new GenericDataPointBinding<SimplifiedPropertyValue, DateTime>() { ValueSelector = spv => spv.TimeStamp };
			areaSeries.ValueBinding = new GenericDataPointBinding<SimplifiedPropertyValue, int>() { ValueSelector = spv => (int)spv.Value };

			areaSeries.LegendSettings = new SeriesLegendSettings()
			{
				Title = String.Format("{0} {1} - {2}", subscriptionTranslation.SourceName, subscriptionTranslation.SourceTypeName, subscriptionTranslation.PropertyName)
			};

			VisualizedDataSet newDataSet = new VisualizedDataSet(this.DataStore.GenerateDataStore(subscriptionToVisualize));
			newDataSet.Visualizer = newDataSource;
			newDataSource.ItemsSource = newDataSet.VisualizedData;
			SubscriptionVisualizations.TryAdd(subscriptionToVisualize, newDataSet);

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Add(areaSeries)));
		}

		void RemoveVisualization(DataSubscription subscriptionToRemove)
		{
			VisualizedDataSet dataSet;
			SubscriptionVisualizations.TryRemove(subscriptionToRemove, out dataSet);
			dataSet.Dispose();

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Remove(ChartView.Series.Single(s => s.ItemsSource == dataSet.Visualizer))));
		}
	}
}
