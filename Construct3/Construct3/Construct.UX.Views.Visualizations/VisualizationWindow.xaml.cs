using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Construct.MessageBrokering.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Telerik.Windows.Controls.Docking;
using System.Collections.Generic;
using Construct.UX.Views.Visualizations.Visualizations;

namespace Construct.UX.Views.Visualizations
{
    public partial class VisualizationWindow : UserControl, INotifyPropertyChanged
    {
		List<SplitVisualizationContainer> VisualizationControls = new List<SplitVisualizationContainer>();
	    private double CurrentGlobalSplitPosition = 0.5;

	    private ClientDataStore dataStore;

	    public ClientDataStore DataStore
	    {
		    get { return dataStore; }
		    set
		    {
			    if (dataStore != null)
				    dataStore.DataSource.OnData -= DataSource_OnData;

			    dataStore = value;
			    dataStore.DataSource.OnData += DataSource_OnData;
		    }
	    }
		public SubscriptionTranslator SubscriptionTranslator { get; set; }

		public SessionInfo DataSession { get; set; }

	    private bool routerIsInitialized = false;
	    private void UpdateTimeBars(DateTime startTime, DateTime endTime)
	    {
			//foreach (var visualizationContainer in VisualizationsDock.Items)
			//{
			//	Dispatcher.BeginInvoke(new Action(delegate
			//	{
			//		var splitContainer = visualizationContainer as RadSplitContainer;
			//		var paneGroup = splitContainer.Items[0] as RadPaneGroup;
			//		if (paneGroup.Items.Count == 0)
			//			return;

			//		var radPane = paneGroup.Items[0] as RadPane;
			//		var visualization = radPane.Content as PropertyVisualization;

			//		visualization.TimeBar.PeriodStart = startTime;
			//		visualization.TimeBar.PeriodEnd = endTime;
			//	}));
			//}

		    Dispatcher.BeginInvoke(new Action(() =>
		    {
			    TimespanDisplayLabel.Content = String.Format("Recorded Timespan: {0:h:mm:ss tt} - {1:h:mm:ss tt}", startTime, endTime);
		    }));
	    }

	    public VisualizationWindow()
	    {
		    this.InitializeComponent();

		    this.TimespanDisplayLabel.DataContext = this;
			this.DataSession = new SessionInfo();

			GlobalTimeBar.DataContext = this.DataSession;
			GlobalTimeBar.VisiblePeriodChanged += GlobalTimeBar_VisiblePeriodChanged;
			GlobalTimeBar.SelectionChanged += GlobalTimeBar_SelectionChanged;
	    }

		void GlobalTimeBar_SelectionChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			DataSession.SelectedStartTime = GlobalTimeBar.SelectionStart;
			DataSession.SelectedEndTime = GlobalTimeBar.SelectionEnd;

			if (DataSession.SelectedStartTime == DataSession.SelectedEndTime)
				return;

			foreach (var container in VisualizationControls)
			{
				var detailsVis = container.DetailsVisualization;
				if (detailsVis != null)
					detailsVis.ChangeVisualizationArea(DataSession);
			}
		}

		void GlobalTimeBar_VisiblePeriodChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			DataSession.ViewStartTime = GlobalTimeBar.VisiblePeriodStart;
			DataSession.ViewEndTime = GlobalTimeBar.VisiblePeriodEnd;

			foreach (var container in VisualizationControls)
			{
				var previewVis = container.PreviewVisualization;
				if (previewVis != null)
				{
					previewVis.ChangeVisualizationArea(DataSession);
				}
			}
		}

	    public void AddVisualization(UserControl previewVisualization, UserControl detailsVisualization)
	    {
		    var dock = this.VisualizationsDock;
		    var splitContainer = new RadSplitContainer();
		    var paneGroup = new RadPaneGroup();
		    var pane = new RadPane();
			var visContainer = new SplitVisualizationContainer(SubscriptionTranslator);
		    if (previewVisualization is PropertyVisualization)
			    pane.Header = (previewVisualization as PropertyVisualization).VisualizationName;
		    else
			    pane.Header = "Data Visualization";

			visContainer.PreviewContainer.Children.Add(previewVisualization);
		    visContainer.DetailsContainer.Children.Add(detailsVisualization);
			pane.Content = visContainer;
		    paneGroup.Items.Add(pane);
		    splitContainer.Items.Add(paneGroup);
		    splitContainer.InitialPosition = DockState.DockedTop;

		    visContainer.SplitterPosition = CurrentGlobalSplitPosition;
			visContainer.SplitPositionChanged += visContainer_SplitPositionChanged;

		    if (previewVisualization is PropertyVisualization)
		    {
			    var propertyVisualization = previewVisualization as PropertyVisualization;

				propertyVisualization.OnVisualizationRangeChanged += propertyVisualization_OnVisualizationRangeChanged;
		    }

			VisualizationControls.Add(visContainer);
		    dock.Items.Add(splitContainer);
	    }

	    private bool _IsUpdating = false;
		void propertyVisualization_OnVisualizationRangeChanged(object sender, ChartVisualizationInfo obj)
		{
			if (_IsUpdating)
				return;

			var chartToSessionConverter = new ChartToSessionConverter();
			if (chartToSessionConverter.UpdateSessionToChart(this.DataSession, obj))
			{

				Dispatcher.BeginInvoke(new Action(() =>
				{
					_IsUpdating = true;
					GlobalTimeBar.VisiblePeriodStart = DataSession.ViewStartTime.Value;
					GlobalTimeBar.VisiblePeriodEnd = DataSession.ViewEndTime.Value;
					_IsUpdating = false;
				}));

				//	Update other visualizations

				//	[Currently causes application hang]
				//foreach (var visualization in VisualizationControls)
				//{
				//	var previewVisualization = visualization.PreviewContainer.Children[0] as PropertyVisualization;

				//	if (previewVisualization != null && previewVisualization != sender)
				//		previewVisualization.ChangeVisualizationArea(DataSession);
				//}
			}
		}

		void visContainer_SplitPositionChanged(object sender, double splitPosition)
		{
			var visualizations = VisualizationControls;

			foreach (var vis in visualizations)
			{
				if (vis == sender)
					continue;

				vis.SplitterPosition = splitPosition;
			}

			CurrentGlobalSplitPosition = splitPosition;

			DataPreviewColumn.Width = new GridLength(splitPosition, GridUnitType.Star);
			DataDetailsColumn.Width = new GridLength(1.0 - splitPosition, GridUnitType.Star);
		}

	    public void AddVisualization(String visualizationType)
	    {
		    switch (visualizationType)
		    {
			    case ("Numeric"):
					AddVisualization(
						new NumericPropertyPreview(DataStore, SubscriptionTranslator, DataSession),
						new NumericPropertyDetails(DataStore, SubscriptionTranslator)
						);
				    break;

				default:
				    throw new Exception("Unknown visualization type: " + visualizationType);
		    }
	    }

	    private void DataSource_OnData(SimplifiedPropertyValue simplifiedPropertyValue)
	    {
		    var localTime = simplifiedPropertyValue.TimeStamp;

		    if (DataSession.StartTime == null)
		    {
			    DataSession.StartTime = simplifiedPropertyValue.TimeStamp;
			    DataSession.EndTime = simplifiedPropertyValue.TimeStamp;
		    }

		    if ((localTime - DataSession.EndTime).Value.TotalMilliseconds > 0.0)
		    {
			    DataSession.EndTime = localTime;
				UpdateTimeBars(DataSession.StartTime.Value, DataSession.EndTime.Value);
		    }
	    }

	    public event PropertyChangedEventHandler PropertyChanged;

	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChangedEventHandler handler = PropertyChanged;
		    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
	    }

	    private void VisualizationsDock_OnPreviewClose(object sender, StateChangeEventArgs e)
	    {
		    foreach (RadPane pane in e.Panes)
		    {
			    var visualizer = pane.Content as SplitVisualizationContainer;
			    if (visualizer == null)
				    continue;

			    var previewPropertyVis = visualizer.PreviewContainer.Children[0] as PropertyVisualization;
				if (previewPropertyVis != null)
					previewPropertyVis.Close();

			    VisualizationControls.Remove(visualizer);
		    }
	    }
    }
}
