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
		public List<SplitVisualizationContainer> VisualizationControls { get; private set; }
	    private double CurrentGlobalSplitPosition = 0.5;

	    private ClientDataStore dataStore;

	    public ClientDataStore DataStore
	    {
		    get { return dataStore; }
		    set
		    {
			    if (dataStore != null)
				    dataStore.RealtimeDataSource.OnData -= DataSource_OnData;

			    dataStore = value;
			    dataStore.RealtimeDataSource.OnData += DataSource_OnData;
		    }
	    }
		public SubscriptionTranslator SubscriptionTranslator { get; set; }

		public SessionInfo DataSession { get; set; }

		bool IsVisualizingRealTime { get { return DataSession != null && !DataSession.EndTime.HasValue; } }

	    private void UpdateTimeLabel(DateTime startTime, DateTime endTime)
	    {
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

			VisualizationControls = new List<SplitVisualizationContainer>();

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
					detailsVis.ChangeVisualizedDataRange(DataSession);
			}
		}

		void GlobalTimeBar_VisiblePeriodChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			DataSession.ViewStartTime = GlobalTimeBar.VisiblePeriodStart;
			DataSession.ViewEndTime = GlobalTimeBar.VisiblePeriodEnd;

			//	Data session to be used for visualization calculations, may be an augmented session
			//		if we're doing real-time data visualization (EndTime should be the last time
			//		that data was received)
			var effectiveSession = DataSession;
			if (IsVisualizingRealTime)
				effectiveSession = new SessionInfo()
				{
					StartTime = DataSession.StartTime,
					ViewStartTime = DataSession.ViewStartTime,
					SelectedStartTime = DataSession.SelectedStartTime,
					SelectedEndTime = DataSession.SelectedEndTime,
					ViewEndTime = DataSession.ViewEndTime,
					EndTime = lastDataTime // Assigned in DataSource_OnData
				};

			foreach (var container in VisualizationControls)
			{
				var previewVis = container.PreviewVisualization;
				if (previewVis != null)
				{
					previewVis.ChangeVisualizationArea(effectiveSession);
				}
			}
		}

	    public Guid AddVisualization(SplitVisualizationContainer visualization)
	    {
		    var dock = this.VisualizationsDock;
		    var splitContainer = new RadSplitContainer();
		    var paneGroup = new RadPaneGroup();
		    var pane = new RadPane();
		    var newVisId = Guid.NewGuid();

			//	How to set serialization tag?
		    //pane.Tag = newVisId;
			pane.Content = visualization;
		    paneGroup.Items.Add(pane);
		    splitContainer.Items.Add(paneGroup);
		    splitContainer.InitialPosition = DockState.DockedTop;

			visualization.SplitterPosition = CurrentGlobalSplitPosition;
			visualization.SplitPositionChanged += visContainer_SplitPositionChanged;

			VisualizationControls.Add(visualization);
		    dock.Items.Add(splitContainer);

			if (DataSession.StartTime.HasValue && (DataSession.EndTime.HasValue || IsVisualizingRealTime))
				visualization.PreviewVisualization.ChangeVisualizedDataRange(DataSession);

		    return newVisId;
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

	    public Guid AddVisualization(String visualizationType)
	    {
		    switch (visualizationType)
		    {
			    case ("Numeric"):
					return AddVisualization(new NumericPropertyVisualization(DataSession, DataStore, SubscriptionTranslator));
				    break;

				case ("Text"):
					return AddVisualization(new TextPropertyVisualization(DataSession, DataStore, SubscriptionTranslator));
				    break;

				default:
				    throw new Exception("Unknown visualization type: " + visualizationType);
		    }
	    }

	    public void ChangeVisualizedDataRange(DateTime startTime, DateTime endTime)
	    {
			DataSession.StartTime = startTime.ToLocalTime();
		    if (endTime == DateTime.MaxValue)
			    DataSession.EndTime = null;
		    else
			    DataSession.EndTime = endTime.ToLocalTime();

			//	Need to do weird DataBinding logic if we're doing real-time visualization (in order
			//		to get the GlobalTimeBar to show the latest time)
		    if (IsVisualizingRealTime)
		    {
			    GlobalTimeBar.DataContext = null;
			    GlobalTimeBar.PeriodStart = DataSession.StartTime.Value;
			    GlobalTimeBar.PeriodEnd = DateTime.Now;
		    }
		    else
		    {
			    //GlobalTimeBar.DataContext = DataSession;
			    GlobalTimeBar.PeriodStart = DataSession.StartTime.Value;
			    GlobalTimeBar.PeriodEnd = DataSession.EndTime.Value;
		    }

			foreach (var visualization in VisualizationControls)
			{
				if (visualization.PreviewVisualization != null)
				{
					visualization.PreviewVisualization.ChangeVisualizedDataRange(DataSession);
				}
			}
	    }

		//	Ugh, poor hack, member only used in DataSource_OnData and GlobalTimeBar_VisiblePeriodChanged
	    private DateTime? lastDataTime;
	    private void DataSource_OnData(SimplifiedPropertyValue simplifiedPropertyValue)
	    {
		    if (IsVisualizingRealTime)
		    {
				//	If we're supposed to be showing all data from StartTime to the current time,
				//		the shown end time should be the timestamp of the last received data

			    var timestamp = simplifiedPropertyValue.TimeStamp;
				//	100ms time difference - Limit refresh rate for UI components since they're the most expensive part
				if (!lastDataTime.HasValue || (timestamp - lastDataTime.Value).TotalMilliseconds > 100.0)
			    {
					lastDataTime = timestamp;

					Dispatcher.BeginInvoke(new Action(() =>
					{
						//	If the GlobalTimeBar is displaying up to the most recent data, make sure it continues to show
						//		the most recent data by advancing its VisiblePeriodEnd to the new PeriodEnd
						if ((GlobalTimeBar.PeriodEnd - GlobalTimeBar.VisiblePeriodEnd).TotalMilliseconds < 10.0)
						{
							GlobalTimeBar.PeriodEnd = timestamp;
							GlobalTimeBar.VisiblePeriodEnd = GlobalTimeBar.PeriodEnd;
						}
						else
						{
							GlobalTimeBar.PeriodEnd = timestamp;
						}

						UpdateTimeLabel(DataSession.StartTime.Value, lastDataTime.Value);
				    
						//	Update visualizations for new time range
						foreach (var visContainer in VisualizationControls)
						{
							visContainer.PreviewVisualization.ChangeRealTimeRangeEnd(lastDataTime.Value);
						}
					}));
			    }
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
