using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using Construct.UX.ViewModels.Sources;
using Construct.UX.ViewModels.Sources.SourcesServiceReference;

namespace Construct.UX.Views.Sources
{
    /// <summary>
    /// Interaction logic for SensorHostsPanel.xaml
    /// </summary>
    public partial class SensorHostsPanel : UserControl
    {
        public ViewModel TheViewModel;
        private bool loadedHasRun = false;
        private Dictionary<Guid, PendingAction> pendingActionList;

        public SensorHostsPanel()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            pendingActionList = new Dictionary<Guid, PendingAction>();
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //can we put this in a constructor?
            if (loadedHasRun == false)
            {
                AddNewSensorHostIDTextBox.Text = Guid.NewGuid().ToString();
                TheViewModel.SensorLoadedCallbackReceived += LoadSensorCompleted;
                TheViewModel.SensorInstalledCallbackReceived += InstallSensorCompleted;    
                loadedHasRun = true;
            }
        }

        private void SensorGridView_Loaded(object sender, RoutedEventArgs e)
        {
            var childGrid = (RadGridView)sender;
            var parentRow = childGrid.ParentRow;

            SensorHost parentRowHost = parentRow.Item as SensorHost;
            //TODO: Deal with Null references to ParentRowHost ?
            if ((parentRowHost != null) && (parentRowHost.ID != null))
            {
                FilterDescriptor descriptor = new FilterDescriptor();
                descriptor.Member = "SensorHostID";
                descriptor.Operator = FilterOperator.IsEqualTo;
                descriptor.Value = parentRowHost.ID;
                childGrid.FilterDescriptors.Add(descriptor);

                lock (TheViewModel.ObservableHumanReadableSensors)
                {
                    childGrid.ItemsSource = TheViewModel.ObservableHumanReadableSensors;
                }
            }
        }

        private void AddNewSensorHostButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.AddNewSensorHostTypeComboBox.SelectedIndex > -1)
            {
                SensorHostType newSensorHostType = TheViewModel.ObservableSensorHostTypes
                                                               .Single(s => s.SensorHostTypeName == (this.AddNewSensorHostTypeComboBox.SelectedItem as SensorHostType).SensorHostTypeName);

                dynamic parameter = new ExpandoObject();
                //TODO: Add field validation
                parameter.HostID = AddNewSensorHostIDTextBox.Text;
                parameter.Protocol = AddNewSensorHostPreferedProtcolComboBox.Text;
                parameter.HostName = AddNewSensorHostNameTextBox.Text;
                parameter.SensorHostTypeID = newSensorHostType.ID;

                TheViewModel.AddSensorHost(parameter);
                AddNewSensorHostIDTextBox.Text = Guid.NewGuid().ToString();
            }
            else
            {
                throw new ApplicationException("The specified Sensor Host was not found");
            }
        }

        private void AddExistingSensorHostButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.AddExistingSensorHostTypeComboBox.SelectedIndex > -1)
            {
                SensorHostType newSensorHostType = TheViewModel.ObservableSensorHostTypes
                                                               .Single(s => s.SensorHostTypeName == (this.AddExistingSensorHostTypeComboBox.SelectedItem as SensorHostType).SensorHostTypeName);

                dynamic parameter = new ExpandoObject();
                //TODO: Add field validation
                parameter.HostID = AddExistingSensorHostIDTextBox.Text;
                parameter.Protocol = AddExistingSensorHostPreferedProtcolComboBox.Text;
                parameter.HostName = AddExistingSensorHostNameTextBox.Text;
                parameter.SensorHostTypeID = newSensorHostType.ID;

                TheViewModel.AddSensorHost(parameter);
            }
            else
            {
                throw new ApplicationException("The specified Sensor Host was not found");
            }
        }

        private void InstallSensorButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstallFetchURI.Text != String.Empty && ZippedFileName.Text != String.Empty)
            {
                dynamic parameter = new ExpandoObject();

                parameter.UriToDownloadFrom = InstallFetchURI.Text;
                parameter.ZippedFileName = ZippedFileName.Text;
                parameter.Overwrite = OverwriteSensorToggle.IsChecked;
                parameter.HostID = TheViewModel.CurrentSensorHost.ID;

                Guid newSensorID = TheViewModel.AddSensorToSensorHost(parameter);

                if (newSensorID == Guid.Empty)
                {
                    // sensor was not added, handle here
                }
                else
                {
                    // sensor was added, handle here
                }
            }
        }

        private void OnSensorRowAdded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow)
            {
                //((GridViewRow)e.Row).IsEnabled = false;
            }
        }

        private void InstallSensorCompleted(Guid sensorID)
        {
            IEnumerable<GridViewRow> sensorRows = SensorHostsGridView.ChildrenOfType<GridViewRow>()
                                                                     .Where(s => s.Item is HumanReadableSensor);
            Button loadButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
                                          .First()
                                          .ChildrenOfType<Button>()
                                          .Where(s => s.Name == "LoadButton")
                                          .SingleOrDefault();

            loadButton.IsEnabled = true;
            loadButton.Content = "Load";
        }

        private void LoadSensor_Click(object sender, RoutedEventArgs e)
        {
            RadButton button = (RadButton)sender;
            HumanReadableSensor rowSensor = (HumanReadableSensor)button.GetVisualParent<RadRowItem>().Item;

            dynamic parameter = new ExpandoObject();
            var dispatcher = button.Dispatcher;
            parameter.SourceID = rowSensor.ID;
            parameter.SensorTypeSourceID = rowSensor.SensorTypeSourceID;
            parameter.CommandLineArgs = CommandLineArgs.Text;
            parameter.HostID = TheViewModel.CurrentSensorHost.ID;

            TheViewModel.LoadSensor(parameter);
            button.IsEnabled = false;
            button.Content = "Loading...";

            PendingAction loadAction = new PendingAction(rowSensor.ID, HandleLoadSensorTimeout);
            pendingActionList.Add(rowSensor.ID, loadAction);
        }

        private void LoadSensorCompleted(Guid sensorID)
        {
			if (!this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.BeginInvoke(new Action(() => LoadSensorCompleted(sensorID)));
				return;
			}

			IEnumerable<GridViewRow> sensorRows = SensorHostsGridView.ChildrenOfType<GridViewRow>()
																		.Where(s => s.Item is HumanReadableSensor);


			Button loadButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
											.First()
											.ChildrenOfType<Button>()
											.Where(s => s.Name == "LoadButton")
											.SingleOrDefault();
			loadButton.Content = "Load";
			loadButton.IsEnabled = false;

			Button unloadButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
											.First()
											.ChildrenOfType<Button>()
											.Where(s => s.Name == "UnloadButton")
											.SingleOrDefault();
			unloadButton.IsEnabled = true;

			Button startButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
											.First()
											.ChildrenOfType<Button>()
											.Where(s => s.Name == "StartButton")
											.SingleOrDefault();
			startButton.IsEnabled = true;

			Button stopButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
								.First()
								.ChildrenOfType<Button>()
								.Where(s => s.Name == "StopButton")
								.SingleOrDefault();
			stopButton.IsEnabled = false;

			pendingActionList[sensorID].timeoutTimer.Stop();
			pendingActionList.Remove(sensorID);
        }

        private void HandleLoadSensorTimeout(Guid sensorID)
        {
            SensorHostsGridView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IEnumerable<GridViewRow> sensorRows = SensorHostsGridView.ChildrenOfType<GridViewRow>()
                                                                 .Where(s => s.Item is HumanReadableSensor);


                    Button loadButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
                                                  .First()
                                                  .ChildrenOfType<Button>()
                                                  .Where(s => s.Name == "LoadButton")
                                                  .SingleOrDefault();

                    loadButton.Content = "Load";
                    loadButton.IsEnabled = true;

                    Button unloadButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
                                                    .First()
                                                    .ChildrenOfType<Button>()
                                                    .Where(s => s.Name == "UnloadButton")
                                                    .SingleOrDefault();
                    unloadButton.IsEnabled = false;

                    Button startButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
                                                   .First()
                                                   .ChildrenOfType<Button>()
                                                   .Where(s => s.Name == "StartButton")
                                                   .SingleOrDefault();
                    startButton.IsEnabled = false;

                    Button stopButton = sensorRows.Where(s => (s.Item as HumanReadableSensor).ID == sensorID)
                                       .First()
                                       .ChildrenOfType<Button>()
                                       .Where(s => s.Name == "StopButton")
                                       .SingleOrDefault();
                    stopButton.IsEnabled = false;

                    pendingActionList.Remove(sensorID);
                }));

        }

        private void UnloadSensor_Click(object sender, RoutedEventArgs e)
        {
			if (!this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.BeginInvoke(new Action(() => UnloadSensor_Click(sender, e)));
				return;
			}

            RadButton button = (RadButton)sender;
            HumanReadableSensor rowSensor = (HumanReadableSensor)button.GetVisualParent<RadRowItem>().Item;

            dynamic parameter = new ExpandoObject();
            parameter.SourceID = rowSensor.ID;
            parameter.SensorTypeSourceID = rowSensor.SensorTypeSourceID;
            parameter.HostID = TheViewModel.CurrentSensorHost.ID;

            TheViewModel.UnloadSensor(parameter);

            RadButton loadButton = button.GetVisualParent<RadRowItem>()
                                         .ChildrenOfType<RadButton>()
                                         .Where(b => b.Name == "LoadButton")
                                         .SingleOrDefault();
            RadButton startButton = button.GetVisualParent<RadRowItem>()
                                          .ChildrenOfType<RadButton>()
                                          .Where(b => b.Name == "StartButton")
                                          .SingleOrDefault();
            RadButton stopButton = button.GetVisualParent<RadRowItem>()
                                         .ChildrenOfType<RadButton>()
                                         .Where(b => b.Name == "StopButton")
                                         .SingleOrDefault();
            button.IsEnabled = false;
            loadButton.IsEnabled = true;
            startButton.IsEnabled = false;
            stopButton.IsEnabled = false;
        }

        private void StartSensor_Click(object sender, RoutedEventArgs e)
        {
			if (!this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.BeginInvoke(new Action(() => StartSensor_Click(sender, e)));
				return;
			}

            RadButton button = (RadButton)sender;
            HumanReadableSensor rowSensor = (HumanReadableSensor)button.GetVisualParent<RadRowItem>().Item;

            dynamic parameter = new ExpandoObject();
            parameter.SourceID = rowSensor.ID;
            parameter.SensorTypeSourceID = rowSensor.SensorTypeSourceID;
            parameter.HostID = TheViewModel.CurrentSensorHost.ID;

            TheViewModel.StartSensor(parameter);

            RadButton stopButton = button.GetVisualParent<RadRowItem>()
                                         .ChildrenOfType<RadButton>()
                                         .Where(b => b.Name == "StopButton")
                                         .SingleOrDefault();
            button.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        private void StopSensor_Click(object sender, RoutedEventArgs e)
        {
			if (!this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.BeginInvoke(new Action(() => StopSensor_Click(sender, e)));
				return;
			}

            RadButton button = (RadButton)sender;
            HumanReadableSensor rowSensor = (HumanReadableSensor)button.GetVisualParent<RadRowItem>().Item;

            dynamic parameter = new ExpandoObject();
            parameter.SourceID = rowSensor.ID;
            parameter.SensorTypeSourceID = rowSensor.SensorTypeSourceID;
            parameter.HostID = TheViewModel.CurrentSensorHost.ID;

            TheViewModel.StopSensor(parameter);

            RadButton startButton = button.GetVisualParent<RadRowItem>()
                                          .ChildrenOfType<RadButton>()
                                          .Where(b => b.Name == "StartButton")
                                          .SingleOrDefault();
            button.IsEnabled = false;
            startButton.IsEnabled = true;
        }

        private void SensorHostsGridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (AddSensorButton.IsEnabled == false)
            {
                AddSensorButton.IsEnabled = true;
            }

            GridViewRow sensorHostRow = (GridViewRow)((RadGridView)sender).ItemContainerGenerator.ContainerFromItem(((RadGridView)sender).SelectedItem);
            RadGridView sensorGrid = null;
            if (sensorHostRow != null)
            {
                sensorGrid = sensorHostRow.ChildrenOfType<RadGridView>().SingleOrDefault();
            }
            if (sensorGrid != null)
            {
                ChangeSensorSelection(sensorGrid);
            }
        }

        private void SensorGridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            ChangeSensorSelection((RadGridView)sender);
        }
  
        private void ChangeSensorSelection(RadGridView sender)
        {
            if (TheViewModel.CurrentSensorCommandList.Count != 0)
            {
                Telerik.Windows.Controls.CollectionExtensions.RemoveAll<SensorCommand>(TheViewModel.CurrentSensorCommandList);
            }
            var selectedSensorRow = sender.SelectedItem;
            TheViewModel.CurrentHumanReadableSensor = (HumanReadableSensor)selectedSensorRow;
            if (TheViewModel.ObservableSensorCommands != null)
            {
                var x = TheViewModel.ObservableSensorCommands
                                    .Where(s => s.SensorTypeSourceID == (TheViewModel.CurrentHumanReadableSensor.SensorTypeSourceID))
                                    .ToList<SensorCommand>();
                foreach (SensorCommand sensorCommand in x)
                {
                    if (TheViewModel.CurrentSensorCommandList.Contains(sensorCommand) == false)
                    {
                        TheViewModel.CurrentSensorCommandList.Add(sensorCommand);
                    }
                }
                if (TheViewModel.CurrentSensorCommandList.Count != 0)
                {
                    TheViewModel.SetDefaultSensorCommand();
                    TheViewModel.CurrentSensorCommand = TheViewModel.CurrentSensorCommandList[0];
                }
            }
        }

        void parentRow_IsExpandedChanged(object sender, RoutedEventArgs e)
        {
            SensorHostsGridView.SelectedItem = ((GridViewRow)sender).DataContext;
        }

        private class PendingAction
        {
            public Timer timeoutTimer;
            Guid sourceID;
            Action<Guid> timeoutDelegate;

            public PendingAction(Guid sourceID, Action<Guid> timeoutDelegate)
            {
                this.sourceID = sourceID;
                this.timeoutDelegate = timeoutDelegate;

                timeoutTimer = new Timer(120000);
                timeoutTimer.Elapsed += new ElapsedEventHandler(timeoutTimer_Elapsed);
                timeoutTimer.AutoReset = false;
                timeoutTimer.Start();
            }

            private void timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
            {
                if (timeoutDelegate != null)
                {
                    timeoutDelegate.Invoke(sourceID);
                }
            }
        }
    }
}