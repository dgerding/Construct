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

		private SubscriptionTranslator Translator;
		private SessionInfo DataSession;


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
					VisualizedData.Add(existingData);

				DataSource.OnNewData += dataSource_OnNewData;
			}

			public void Dispose()
			{
				DataSource.OnNewData -= dataSource_OnNewData;
			}

			void dataSource_OnNewData(SimplifiedPropertyValue obj)
			{
				if ((obj.TimeStamp - lastTime).TotalSeconds > 0.0)
					Visualizer.Dispatcher.BeginInvoke(new Action(() => VisualizedData.Add(obj)));
			}
		}

		public override int MaxProperties
		{
			get { return 10; }
		}

		public override IEnumerable<Type> VisualizableTypes
		{
			get { return new List<Type>() {typeof (double), typeof (float), typeof (int)}; }
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
					ChartView.PanOffset = chartInfo.PanOffset;
					ChartView.Zoom = chartInfo.Zoom;
				}));
			}
		}

		public NumericPropertyPreview(ClientDataStore dataSource, SubscriptionTranslator translator, SessionInfo sessionInfo)
			: base(dataSource)
		{
			InitializeComponent();

			VisualizationName = "Numeric Visualization";
			Translator = translator;

			DataSession = sessionInfo;

			ChartView.PanOffsetChanged += ChartView_PanOffsetChanged;
			ChartView.ZoomChanged += ChartView_ZoomChanged;

			this.OnSubscriptionAdded += NumericPropertyPreview_OnSubscriptionAdded;
			this.OnSubscriptionRemoved += NumericPropertyPreview_OnSubscriptionRemoved;
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

		void NumericPropertyPreview_OnSubscriptionRemoved(DataSubscription obj)
		{
			//Legend.RemoveLegendItem(obj);
			RemoveVisualization(obj);
		}

		void NumericPropertyPreview_OnSubscriptionAdded(DataSubscription obj)
		{
			//Color sparklineColor = Legend.AddLegendItem(obj, Translator).Value;
			AddNewVisualization(obj, Color.FromRgb(255, 0, 0));
		}

		void AddNewVisualization(DataSubscription subscriptionToVisualize, Color visualizationColor)
		{
			var subscriptionTranslation = Translator.GetTranslation(subscriptionToVisualize);

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

			VisualizedDataSet newDataSet = new VisualizedDataSet(this.DataStore.GenerateDataStore(subscriptionToVisualize));
			newDataSet.Visualizer = newDataSource;
			newDataSource.ItemsSource = newDataSet.VisualizedData;
			SubscriptionVisualizations.TryAdd(subscriptionToVisualize, newDataSet);

			ChartView.Dispatcher.BeginInvoke(new Action(() => ChartView.Series.Add(lineSeries)));
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
