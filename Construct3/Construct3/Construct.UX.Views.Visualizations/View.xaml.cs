using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Construct.Utilities.Shared;
using Construct.UX.ViewModels;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Construct.UX.Views.Helper;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using viewModel = Construct.UX.ViewModels.Visualizations.ViewModel;

namespace Construct.UX.Views.Visualizations
{
	public partial class View : Views.View, INotifyPropertyChanged
	{
		private List<DataType> dataTypes;
		private List<PropertyType> properties;
		private List<Source> sources;
		private List<Visualization> visualizations;
		private List<Visualizer> visualizers;

		private SubscriptionTranslator subscriptionTranslator;
		private readonly ISubscribableDataSource streamDataSource;
		private readonly IQueryableDataSource temporalDataSource;
		private Visualizer currentVisualizer;

		public View(ApplicationSessionInfo sessionInfo)
		{
			InitializeComponent();
			InitializeViewModel(sessionInfo);

			DataContext = this;
			InitializeMembers();

			Visualizations = ((viewModel) ViewModel).GetAllVisualizations().ToList();
			Visualizers = ((viewModel) ViewModel).GetAllVisualizers().ToList();

			Sources = InitializeSources().Distinct().ToList();
			Properties = InitializeProperties().Distinct().ToList();

			InitializeSubscriptionTranslator();

			streamDataSource = new SignalRDataSource(sessionInfo.HostName);
			streamDataSource.Connect();

			Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.Empty, 8000);
			temporalDataSource = new ConstructServerDataSource(serverServiceUri, subscriptionTranslator);

			VisualizationWindow.DataStore = new ClientDataStore(streamDataSource, temporalDataSource);
			VisualizationWindow.SubscriptionTranslator = subscriptionTranslator;

			PropertyVisualizationsOptions.ItemsSource = new List<String>
			{
				"Numeric",
				"Text"
			};
			PropertyVisualizationsOptions.SelectedIndex = 0;

			currentVisualizer = new Visualizer {ID = Guid.NewGuid()};
		}

		public List<DataType> DataTypes
		{
			get { return dataTypes; }
			set
			{
				dataTypes = value;
				NotifyPropertyChanged("DataTypes");
			}
		}

		public List<PropertyType> Properties
		{
			get { return properties; }
			set
			{
				properties = value;
				NotifyPropertyChanged("Properties");
			}
		}

		public List<Source> Sources
		{
			get { return sources; }
			set
			{
				sources = value;
				NotifyPropertyChanged("Sources");
			}
		}

		public List<Visualization> Visualizations
		{
			get { return visualizations; }
			set
			{
				visualizations = value;
				NotifyPropertyChanged("Visualizations");
			}
		}

		public List<Visualizer> Visualizers
		{
			get { return visualizers; }
			set
			{
				visualizers = value;
				NotifyPropertyChanged("Visualizers");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private IEnumerable<PropertyType> InitializeProperties()
		{
			foreach (var dataType in ((viewModel) ViewModel).GetAllDataTypes())
			{
				foreach (var property in ((viewModel) ViewModel).GetAllProperties(dataType.Name))
				{
					yield return property;
				}
			}
		}

		private IEnumerable<Source> InitializeSources()
		{
			foreach (var dataType in ((viewModel) ViewModel).GetAllDataTypes())
			{
				foreach (var source in ((viewModel) ViewModel).GetAssociatedSources(dataType))
				{
					yield return source;
				}
			}
		}

		private void InitializeMembers()
		{
			var viewmodel = ((viewModel) ViewModel);
			DataTypes = viewmodel.GetAllDataTypes().ToList();
		}

		//	Generates human-readable translations for sources, their emitted datatypes, and properties of those datatypes
		private void InitializeSubscriptionTranslator()
		{
			subscriptionTranslator = new SubscriptionTranslator();

			//	Matches *://(HostName)
			var rendezvousHostnameExtractor = new Regex(@"\S*:\/\/([^\/]*)");

			var model = (viewModel) ViewModel;

			var humanReadableSensors = model.GetHumanReadableSensors().ToList();
			foreach (var dataType in DataTypes)
			{
				if (dataType.IsCoreType)
					continue;

				var dataTypeName = dataType.Name;
				var dataProperties = Properties.Cast<PropertyParent>().Where(p => p.ParentDataTypeID == dataType.ID).ToList();
				var relevantSources = model.GetAssociatedSources(dataType);

				foreach (var emittingSource in relevantSources)
				{
					var sensor = humanReadableSensors.Single(s => s.ID == emittingSource.ID);
					foreach (var property in dataProperties)
					{
						var newTranslation = new SubscriptionLabel();
						var newSubscriptionType = new DataSubscription();

						newTranslation.DataTypeName = dataTypeName;
						newTranslation.PropertyName = property.Name;
						newTranslation.SourceName =
							rendezvousHostnameExtractor.Match(sensor.CurrentRendezvous).Groups[1].Captures[0].Value;
						newTranslation.SourceTypeName = sensor.Name;

						var propertyDataTypeName = DataTypes.Single(dt => dt.ID == (property as PropertyType).PropertyDataTypeID).Name;
						newSubscriptionType.PropertyId = property.ID;
						newSubscriptionType.SourceId = emittingSource.ID;
						newSubscriptionType.PropertyType = GetTypeFromName(propertyDataTypeName);

						subscriptionTranslator.AddTranslation(newSubscriptionType, newTranslation);
					}
				}
			}
		}

		private Type GetTypeFromName(String typeName)
		{
			switch (typeName)
			{
				case "bool":
				case "System.Boolean":
					return typeof (bool);

				case "string":
				case "System.String":
					return typeof (string);

				case "long":
				case "System.Int64":
					return typeof (long);

				case "byte[]":
				case "System.Byte[]":
					return typeof (byte[]);

				case "float":
				case "System.Single":
					return typeof (float);

				case "double":
				case "System.Double":
					return typeof (double);

				case "DateTime":
				case "System.DateTime":
					return typeof (DateTime);

				case "Guid":
				case "System.Guid":
					return typeof (Guid);

				case "System.Int32":
				case "int":
					return typeof (int);

				default:
					return null;
			}
		}

		public void InitializeViewModel(ApplicationSessionInfo theApplicationSessionInfo)
		{
			if (ViewModel == null)
			{
				ViewModel = new viewModel(theApplicationSessionInfo);
			}
			ViewModel.SessionInfo = theApplicationSessionInfo;
			if (ViewModel.SessionInfo.IsAuthenticated == false)
			{
				Visibility = Visibility.Hidden;
			}
		}

		private void AddPropertyVisualizationButton_Click(object sender, RoutedEventArgs e)
		{
			VisualizationWindow.AddVisualization(PropertyVisualizationsOptions.Text);
		}

		private void SaveLayoutButton_Click(object sender, RoutedEventArgs e)
		{
			SaveCurrentLayout(currentVisualizer);
		}

		private void LoadLayoutButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void SaveCurrentLayout(Visualizer targetVisualizer)
		{
			//	Embed serialization info
			//foreach (var vis in VisualizationWindow.VisualizationControls)
			//{
			//	vis.SelectedProperties
			//}

			//	Save layout info
			MemoryStream layoutStream = new MemoryStream();
			VisualizationWindow.VisualizationsDock.SaveLayout(layoutStream);
			layoutStream.Seek(0, SeekOrigin.Begin);
			using (StreamReader reader = new StreamReader(layoutStream))
				targetVisualizer.LayoutString = reader.ReadToEnd();
			



			//	Get visualizations and determine which visualizations need to be added/removed from the database
			var visualizationModels = Visualizations.Where(v => v.VisualizerID == targetVisualizer.ID);

			var serializationHelper = new VisualizationSerializationHelper();
			var currentVisualizationModels =
				VisualizationWindow.VisualizationControls.SelectMany(
					v => serializationHelper.GetVisualizationsForContainer((RadPane) v.Parent)).ToList();

			foreach (var vis in currentVisualizationModels)
				vis.VisualizerID = targetVisualizer.ID;

			var addedVisualizations = currentVisualizationModels.Where(cv => !visualizationModels.Any(v => v.ID == cv.ID)).ToList();
			var removedVisualizations = visualizationModels.Where(v => !currentVisualizationModels.Any(cv => cv.ID == v.ID)).ToList();



			//	Begin the add/remove operations

			//	Add the visualizer if necessary (when it's a new visualizer)
			if (!Visualizers.Any(v => v.ID == targetVisualizer.ID))
			{
				Visualizers.Add(targetVisualizer);
				((viewModel)ViewModel).AddVisualizer(targetVisualizer);
			}

			foreach (var newVis in addedVisualizations)
			{
				((viewModel)ViewModel).AddVisualization(newVis);
			}

			foreach (var removedVis in removedVisualizations)
			{
				((viewModel)ViewModel).RemoveVisualization(removedVis);
			}
		}

		private void ChangeTimeButton_Click(object sender, RoutedEventArgs e)
		{
			var currentStartTime = VisualizationWindow.DataSession.StartTime ?? DateTime.Now;
			var currentEndTime = VisualizationWindow.DataSession.EndTime;
			if (!currentEndTime.HasValue)
				currentEndTime = DateTime.MaxValue;

			var rangeSelectionDialog = new TimeRangeSelectionDialog(currentStartTime, currentEndTime);

			var success = rangeSelectionDialog.ShowDialog();
			if (!success.GetValueOrDefault())
				return;

			var startTime = rangeSelectionDialog.StartTime;
			var endTime = rangeSelectionDialog.EndTime;

			if (!rangeSelectionDialog.EndTimeEnabled)
				endTime = DateTime.MaxValue;

			VisualizationWindow.ChangeVisualizedDataRange(startTime, endTime);
		}
	}
}