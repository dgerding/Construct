using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

using Construct.UX.ViewModels.Data;
using Construct.UX.ViewModels.Data.DataServiceReference;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Construct.UX.ViewModels.Sources.SourcesServiceReference;
using System.Text.RegularExpressions;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Data;
using System.IO;
using System.Threading;

/* Note from developer: I did NOT make use of WPF MVVM utilities here as was used in the rest of this application. I programmed this utility mostly as though
 * I were working in WinForms. Bad, bad dev.
 */

namespace Construct.UX.Views.Data
{
	/// <summary>
	/// Interaction logic for ExportDataWindow.xaml
	/// </summary>
	public partial class ExportDataWindow : RadWindow
	{
		Construct.UX.ViewModels.Data.ViewModel dataViewModel;
		Construct.UX.ViewModels.Sources.ViewModel sourcesViewModel;
		BackgroundWorker exportWorker;

		List<object> checkedDataTypes = new List<object>();
		List<object> checkedSources = new List<object>();

		int totalExportItemCount = -1; // > 0 when export is in progress

		SpecificTimeSpan? totalCurrentDataWindow = null;

		
		

		//	<SourceGuid,TimeSpan>; NOTE: Assumes a source will only ever emit a single datatype
		Dictionary<Guid, SpecificTimeSpan?> sourcesTimeSpansCache = new Dictionary<Guid,SpecificTimeSpan?>();

		Dictionary<String, HumanReadableSensor> readableSensorNamesMapping = new Dictionary<String, HumanReadableSensor>();

		struct ExportOperationData
		{
			public SpecificTimeSpan ExportTimeSpan;
			public TimeSpan SampleInterval;
			public String TargetFile;
			public Guid[] SourceIds;
			public Guid[] DataTypeIds;
		}

		public ExportDataWindow(Construct.UX.ViewModels.Data.ViewModel dataViewModel, Construct.UX.ViewModels.Sources.ViewModel sourcesViewModel)
		{
			this.HideMinimizeButton = true;

			InitializeComponent();

			progressLabel.Visibility = System.Windows.Visibility.Hidden;
			this.dataViewModel = dataViewModel;
			this.sourcesViewModel = sourcesViewModel;

			var dataTypes = this.dataViewModel.GetAllTypes();
			foreach (var type in dataTypes)
			{
				if (type.IsCoreType)
					continue;

				//	Ignore data types that aren't emitted by a sensor (for now)
				if (!this.sourcesViewModel.ObservableSensors.Any((sensor) => sensor.SensorTypeSourceID == type.DataTypeSourceID))
					continue;

				dataTypesList.Items.Add(type);
			}

			this.PreviewClosed += ExportDataWindow_PreviewClosed;
		}

		void ExportDataWindow_PreviewClosed(object sender, WindowPreviewClosedEventArgs e)
		{
			if (exportWorker != null)
			{
				var parameters = new DialogParameters();
				parameters.Content = new TextBlock() {
					Width = 300,
					TextWrapping = System.Windows.TextWrapping.Wrap,
					Text = "There is a data export operation currently underway. Are you sure you want to cancel and close the window?",
					Foreground = Brushes.White
				};
				parameters.OkButtonContent = "Yes";
				parameters.CancelButtonContent = "No";
				parameters.Closed += delegate(object s, WindowClosedEventArgs args)
				{
					if (args.DialogResult.HasValue)
					{
						e.Cancel = !args.DialogResult.Value;

						if (!e.Cancel.Value)
						{
							exportWorker.CancelAsync();
						}
					}
					else
					{
						//	They X'ed out
						e.Cancel = true;
					}
				};

				RadWindow.Confirm(parameters);
			}
		}

		private bool ValidateForms()
		{
			if (checkedDataTypes.Count == 0)
			{
				MessageBox.Show("At least one data type must be checked for export.");
				return false;
			}

			if (checkedSources.Count == 0)
			{
				MessageBox.Show("At least one source must be checked.");
				return false;
			}

			float interval;
			if (!float.TryParse(sampleIntervalTextbox.Text, out interval))
			{
				MessageBox.Show("The sample interval must be a number.");
				return false;
			}

			if (interval <= 0)
			{
				MessageBox.Show("The sample interval must be greater than 0.");
				return false;
			}

			return true;
		}

		private void DisableInputForms()
		{
			dataTypesList.IsEnabled = false;
			sourcesList.IsEnabled = false;

			timeFrameBar.IsEnabled = false;
			startTimePicker.IsEnabled = false;
			endTimePicker.IsEnabled = false;

			sampleIntervalTextbox.IsEnabled = false;
		}

		private void ResetInputForms()
		{
			dataTypesList.IsEnabled = true;
			sourcesList.IsEnabled = true;

			sampleIntervalTextbox.IsEnabled = true;

			UpdateTimeInputsToTimeRange(totalCurrentDataWindow);
		}

		private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			if (!this.ValidateForms())
				return;

			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.Filter = "*.csv|CSV Files|*.*|All Files";
			saveDialog.DefaultExt = "csv";
			saveDialog.OverwritePrompt = true;

			if (saveDialog.ShowDialog() != true)
				return;

			exportWorker = new BackgroundWorker();
			exportWorker.WorkerReportsProgress = true;
			exportWorker.WorkerSupportsCancellation = true;

			exportWorker.RunWorkerCompleted += exportWorker_RunWorkerCompleted;
			exportWorker.ProgressChanged += exportWorker_ProgressChanged;
			exportWorker.DoWork += RunExportJob;

			ExportOperationData operationData = new ExportOperationData();

			DisableInputForms();

			SpecificTimeSpan selectedTimeSpan = new SpecificTimeSpan(startTimePicker.SelectedValue.Value, endTimePicker.SelectedValue.Value);

			operationData.DataTypeIds = checkedDataTypes.Cast<Construct.UX.ViewModels.Data.DataServiceReference.DataType>().Select(dataType => dataType.ID).ToArray();
			operationData.SourceIds = checkedSources.Cast<HumanReadableSensor>().Select(source => source.ID).ToArray();
			operationData.TargetFile = saveDialog.FileName;
			operationData.ExportTimeSpan = selectedTimeSpan;
			operationData.SampleInterval = TimeSpan.FromMilliseconds(float.Parse(sampleIntervalTextbox.Text));

			progressBar.IsIndeterminate = true;

			progressLabel.Visibility = System.Windows.Visibility.Visible;

			exportWorker.RunWorkerAsync(operationData);
			ExportButton.IsEnabled = false;

			AbortButton.IsEnabled = true;
		}

		private void RunExportJob(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			ExportOperationData operationData = (ExportOperationData)e.Argument;

			totalExportItemCount = (int)(operationData.ExportTimeSpan.TimeSpan.Ticks / operationData.SampleInterval.Ticks);

			TimeSpan sliceSize = TimeSpan.FromTicks(operationData.SampleInterval.Ticks * 100);
			List<SpecificTimeSpan> timeSlices = new List<SpecificTimeSpan>();
			for (int i = 0; i < Math.Ceiling(operationData.ExportTimeSpan.TimeSpan.TotalSeconds / sliceSize.TotalSeconds); i++)
			{
				if (timeSlices.Count == 0)
				{
					timeSlices.Add(new SpecificTimeSpan(operationData.ExportTimeSpan.Start, sliceSize));
				}
				else
				{
					var previousSlice = timeSlices[i - 1];
					var startTime = previousSlice.End;
					var endTime = startTime + sliceSize;
					if ((operationData.ExportTimeSpan.End - endTime).TotalSeconds < 0)
						endTime = operationData.ExportTimeSpan.End;

					timeSlices.Add(new SpecificTimeSpan(startTime, endTime));
				}
			}

			List<String[]> dataValues = new List<String[]>();
			int numRows = 0;

			foreach (var slice in timeSlices)
			{
				var sliceData = this.dataViewModel.GetData(operationData.DataTypeIds, operationData.SourceIds, slice.Start, slice.End, operationData.SampleInterval);

				for (int i = 0; i < sliceData.Length; i++)
				{
					var dataRow = sliceData[i];

					String[] outputRowData = new String[dataRow.Length + 1];
					//	UTC-format
					outputRowData[0] = (slice.Start + TimeSpan.FromTicks(operationData.SampleInterval.Ticks * i)).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

					for (int p = 1; p < dataRow.Length; p++)
					{
						object currentObject = dataRow[i];
						String objectString;

						if (currentObject == null)
							objectString = "";
						else if (currentObject is DateTime)
							objectString = ((DateTime)currentObject).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
						else
							objectString = currentObject.ToString();

						outputRowData[p] = objectString;
					}

					dataValues.Add(outputRowData);
				}

				numRows += sliceData.Length;

				worker.ReportProgress(0, numRows);
			}

			//	Write the CSV
			if (File.Exists(operationData.TargetFile))
				File.Delete(operationData.TargetFile);
			using (var outputFile = File.CreateText(operationData.TargetFile))
			{
				String header = "Time," + String.Join(",", dataViewModel.GetDataLabels(operationData.DataTypeIds));
				outputFile.WriteLine(header);

				foreach (var dataRow in dataValues)
				{
					for (int i = 0; i < dataRow.Length; i++)
					{
						//	Escape quotation marks
						if (dataRow[i].Contains("\""))
							dataRow[i] = dataRow[i].Replace("\"", "\"\"");

						//	Strings with ',' in them get encapsulated in quotation marks
						if (dataRow[i].Contains(','))
							dataRow[i] = "\"" + dataRow[i] + "\"";
					}

					outputFile.WriteLine(String.Join(",", dataRow));
				}
			}

			e.Cancel = worker.CancellationPending;
		}

		void exportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState != null)
			{
				int currentProgress = (int)e.UserState;
				this.progressLabel.Content = "Working... " + currentProgress + "/" + totalExportItemCount + " items processed";
			}
		}

		void exportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ExportButton.IsEnabled = true;
			AbortButton.IsEnabled = false;

			totalExportItemCount = -1;

			progressLabel.Visibility = System.Windows.Visibility.Hidden;
			progressBar.IsIndeterminate = false;

			ResetInputForms();

			exportWorker = null;

			if (e.Cancelled)
				return;

			if (e.Error != null)
			{
				RadWindow.Alert("An error occurred:\n" + e.Error.Message + "\n\n" + e.Error.StackTrace);
				return;
			}

			RadWindow.Alert("Export operation complete. It is now safe to close the export window.");
		}

		private Regex extractMachineFromEndpointRegex = new Regex(@":\/\/([^\/]*)\/");

		private String GetHumanReadableSensorName(HumanReadableSensor sensor)
		{
			//	Generated from the endpoint URI and the name of the sensor
			var groups = extractMachineFromEndpointRegex.Match(sensor.CurrentRendezvous).Groups;
			return groups[1] + ": " + sensor.Name;
		}

		private void RegenerateSourcesList()
		{
			var previousMapping = readableSensorNamesMapping;
			readableSensorNamesMapping = new Dictionary<string, HumanReadableSensor>();

			var enumerableDataTypes = checkedDataTypes.Cast<Construct.UX.ViewModels.Data.DataServiceReference.DataType>();

			foreach (var sensor in sourcesViewModel.ObservableHumanReadableSensors)
			{
				if (enumerableDataTypes.Any((dataType) => dataType.DataTypeSourceID == sensor.SensorTypeSourceID))
					readableSensorNamesMapping.Add(GetHumanReadableSensorName(sensor), sensor);
			}

			//	Remove sources that are no longer relevant (their datatype was unchecked)
			foreach (var previousMappingEntry in previousMapping)
			{
				if (!readableSensorNamesMapping.ContainsKey(previousMappingEntry.Key))
					sourcesList.Items.Remove(previousMappingEntry.Key);
			}

			foreach (var sourceNameMapping in readableSensorNamesMapping)
			{
				if (!previousMapping.ContainsKey(sourceNameMapping.Key))
					sourcesList.Items.Add(sourceNameMapping.Key);
			}

			//	Remove checked entries that were removed
			checkedSources.RemoveAll((sourceObject) => !readableSensorNamesMapping.Values.Contains(sourceObject));
		}

		Guid GetEmittedDataTypeForSensor(HumanReadableSensor readableSensor)
		{
			var result = checkedDataTypes.Cast<Construct.UX.ViewModels.Data.DataServiceReference.DataType>().First((dataType) => dataType.DataTypeSourceID == readableSensor.SensorTypeSourceID);
			return result.ID;
		}

		private void RegenerateTimeRangeLimit()
		{
			if (checkedSources.Count == 0)
			{
				totalCurrentDataWindow = null;
				UpdateTimeInputsToTimeRange(totalCurrentDataWindow);
				return;
			}

			DateTime? earliestStartTime = null;
			DateTime? latestEndTime = null;

			var enumerableSources = checkedSources.Cast<HumanReadableSensor>();

			foreach (var source in enumerableSources)
			{
				if (!sourcesTimeSpansCache.ContainsKey(source.ID))
					CacheSourceDataTimeRange(GetEmittedDataTypeForSensor(source), source.ID);

				var timeSpan = sourcesTimeSpansCache[source.ID];
				if (timeSpan.HasValue)
				{
					DateTime currentStart = timeSpan.Value.Start, currentEnd = timeSpan.Value.End;

					if (!earliestStartTime.HasValue)
						earliestStartTime = currentStart;
					else if (earliestStartTime.Value.Ticks > currentStart.Ticks)
						earliestStartTime = currentStart;

					if (!latestEndTime.HasValue)
						latestEndTime = currentEnd;
					else if (latestEndTime.Value.Ticks < currentEnd.Ticks)
						latestEndTime = currentEnd;
				}
			}

			if (earliestStartTime.HasValue && latestEndTime.HasValue)
			{
				//	Add a day for padding of the visualization
				totalCurrentDataWindow = new SpecificTimeSpan(earliestStartTime.Value.Subtract(TimeSpan.FromDays(1)), latestEndTime.Value.Add(TimeSpan.FromDays(1)));
			}
			else
			{
				totalCurrentDataWindow = null;
				MessageBox.Show("Warning: No data available for the selected sources. Time range selection is disabled.");
			}

			UpdateTimeInputsToTimeRange(totalCurrentDataWindow);
		}

		private Dictionary<HumanReadableSensor, IEnumerable<int>> sensorActivityCache = new Dictionary<HumanReadableSensor, IEnumerable<int>>();

		private void RegenerateItemFrequencies()
		{
			if (!totalCurrentDataWindow.HasValue || totalCurrentDataWindow.Value.TimeSpan.Ticks == 0)
			{
 				itemFrequencyChart.IsEnabled = false;
 				itemFrequencyChart.ItemsSource = null;
				return;
			}

			ThreadPool.QueueUserWorkItem((param) =>
				{
					//	For performance of the frequency viz
					const int maxSamples = 500;
					TimeSpan interval = TimeSpan.FromMinutes(30);

					if (totalCurrentDataWindow.Value.TimeSpan.Ticks / interval.Ticks > maxSamples)
						interval = TimeSpan.FromTicks(totalCurrentDataWindow.Value.TimeSpan.Ticks / maxSamples);

					var sourceIds = checkedSources.Cast<HumanReadableSensor>().Select(s => s.ID).ToArray();

					Dispatcher.Invoke((Action)(() =>
						{
							itemFrequencyChart.ItemsSource = dataViewModel.GetItemFrequencies(sourceIds, interval, totalCurrentDataWindow.Value.Start, totalCurrentDataWindow.Value.End);
						}));
				});
		}

		private void UpdateTimeInputsToTimeRange(SpecificTimeSpan? nullableTimeSpan)
		{
			if (!nullableTimeSpan.HasValue)
			{
				startTimePicker.IsEnabled = false;
				endTimePicker.IsEnabled = false;
				timeFrameBar.IsEnabled = false;
				return;
			}

			var timeSpan = nullableTimeSpan.Value;

			timeFrameBar.PeriodStart = timeSpan.Start;
			timeFrameBar.PeriodEnd = timeSpan.End;

			startTimePicker.SelectableDateStart = timeSpan.Start;
			startTimePicker.SelectableDateEnd = timeSpan.End;

			endTimePicker.SelectableDateStart = timeSpan.Start;
			endTimePicker.SelectableDateEnd = timeSpan.End;

			//	Usability
			if (startTimePicker.SelectedValue.HasValue && endTimePicker.SelectedValue.HasValue)
			{
				if (startTimePicker.SelectedValue.Value == endTimePicker.SelectedValue.Value)
				{
					startTimePicker.SelectedValue = timeSpan.Start;
					endTimePicker.SelectedValue = timeSpan.End;
				}
			}

			startTimePicker.IsEnabled = true;
			endTimePicker.IsEnabled = true;
			timeFrameBar.IsEnabled = true;
		}

		private void CacheSourceDataTimeRange(Guid dataTypeId, Guid sourceId)
		{
			SpecificTimeSpan? timeSpan = dataViewModel.GetTimeSpanForTypeAndSource(dataTypeId, sourceId);

			sourcesTimeSpansCache.Add(sourceId, timeSpan);
		}

		private void UnbindTimeInputListeners()
		{
			startTimePicker.SelectionChanged -= StartTime_SelectionChanged;
			endTimePicker.SelectionChanged -= EndTime_SelectionChanged;
			timeFrameBar.SelectionChanged -= RadTimeBar_SelectionChanged;
		}

		private void ReAddTimeInputListeners()
		{
			startTimePicker.SelectionChanged += StartTime_SelectionChanged;
			endTimePicker.SelectionChanged += EndTime_SelectionChanged;
			timeFrameBar.SelectionChanged += RadTimeBar_SelectionChanged;
		}

		private void RadTimeBar_SelectionChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			UnbindTimeInputListeners();

			startTimePicker.SelectedValue = timeFrameBar.SelectionStart;
			endTimePicker.SelectedValue = timeFrameBar.SelectionEnd;

			ReAddTimeInputListeners();
		}

		private void EndTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!endTimePicker.SelectedValue.HasValue)
				return;

			UnbindTimeInputListeners();
			timeFrameBar.SelectionEnd = endTimePicker.SelectedValue.Value;
			ReAddTimeInputListeners();
		}

		private void StartTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!startTimePicker.SelectedValue.HasValue)
				return;

			UnbindTimeInputListeners();
			timeFrameBar.SelectionStart = startTimePicker.SelectedValue.Value;
			ReAddTimeInputListeners();
		}

		private void AbortButton_Click(object sender, RoutedEventArgs e)
		{
			this.exportWorker.CancelAsync();
		}

		private void SourcesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = sender as CheckBox;
			if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value == true)
				checkedSources.Add(readableSensorNamesMapping[checkBox.DataContext as String]);
			else
				checkedSources.Remove(readableSensorNamesMapping[checkBox.DataContext as String]);

			RegenerateTimesAndFrequencies();
		}

		private void DataTypesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = sender as CheckBox;
			if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value == true)
				checkedDataTypes.Add(checkBox.DataContext);
			else
			{
				var dataType = checkBox.DataContext as Construct.UX.ViewModels.Data.DataServiceReference.DataType;
				checkedDataTypes.Remove(checkBox.DataContext);
			}

			RegenerateSourcesList();
			RegenerateTimesAndFrequencies();
		}

		private void RegenerateTimesAndFrequencies()
		{
			SpecificTimeSpan? previousWindow = totalCurrentDataWindow;
			RegenerateTimeRangeLimit();

			RegenerateItemFrequencies();
		}
	}
}
