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

namespace Construct.UX.Views.Visualizations
{
    public partial class VisualizationWindow : UserControl, INotifyPropertyChanged
    {
		public StreamDataRouter DataRouter { get; set; }
		public SubscriptionTranslator SubscriptionTranslator { get; set; }

		public SessionInfo DataSession { get; set; }

	    private bool routerIsInitialized = false;
	    private void UpdateTimeBars(DateTime startTime, DateTime endTime)
	    {
		    foreach (var visualizationContainer in VisualizationsDock.Items)
		    {
			    Dispatcher.BeginInvoke(new Action(delegate
			    {
				    var splitContainer = visualizationContainer as RadSplitContainer;
				    var paneGroup = splitContainer.Items[0] as RadPaneGroup;
				    if (paneGroup.Items.Count == 0)
					    return;

				    var radPane = paneGroup.Items[0] as RadPane;
				    var visualization = radPane.Content as PropertyVisualization;

				    visualization.TimeBar.PeriodStart = startTime;
				    visualization.TimeBar.PeriodEnd = endTime;
			    }));
		    }

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
	    }
	    public void AddVisualization(UserControl visualizationControl)
	    {
		    var dock = this.VisualizationsDock;
		    var splitContainer = new RadSplitContainer();
		    var paneGroup = new RadPaneGroup();
		    var pane = new RadPane();
		    pane.Content = visualizationControl;
		    if (visualizationControl is PropertyVisualization)
			    pane.Header = (visualizationControl as PropertyVisualization).VisualizationName;
		    else
			    pane.Header = "Data Visualization";
		    paneGroup.Items.Add(pane);
		    splitContainer.Items.Add(paneGroup);
		    splitContainer.InitialPosition = DockState.DockedTop;

		    dock.Items.Add(splitContainer);
	    }

	    public void AddVisualization(String visualizationType)
	    {
		    if (!routerIsInitialized)
		    {
				DataRouter.DataSource.OnData += DataSource_OnData;
			    routerIsInitialized = true;
		    }

		    switch (visualizationType)
		    {
			    case ("Numeric"):
					AddVisualization(new NumericPropertyVisualization(DataRouter, SubscriptionTranslator, DataSession));
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
			    var visualizer = pane.Content as PropertyVisualization;
			    if (visualizer == null)
				    continue;

				visualizer.Close();
		    }
	    }
    }
}
