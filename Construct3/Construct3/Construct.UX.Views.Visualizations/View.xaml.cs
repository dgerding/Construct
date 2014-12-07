using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Construct.Utilities.Shared;
using Construct.UX.ViewModels;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using viewModel = Construct.UX.ViewModels.Visualizations.ViewModel;

namespace Construct.UX.Views.Visualizations
{
	public partial class View : Views.View, INotifyPropertyChanged
	{
		private IEnumerable<DataType> dataTypes;
		private IEnumerable<PropertyType> properties;
		private PropertyType propertyType;
		private Random random = new Random();
		private Uri serverServiceUri;
		private IEnumerable<Source> sources;
		private SubscriptionTranslator subscriptionTranslator;
		private IEnumerable<Visualization> visualizations;
		private readonly IDataSource streamDataSource;

		public View(ApplicationSessionInfo sessionInfo)
		{
			InitializeComponent();
			InitializeViewModel(sessionInfo);

			DataContext = this;
			InitializeMembers();

			serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.Empty,
				8000);

			Visualizations = ((viewModel) ViewModel).GetVisualizations();

			Sources = InitializeSources().Distinct().ToList();
			Properties = InitializeProperties().Distinct().ToList();


			InitializeSubscriptionTranslator();

			streamDataSource = new SignalRDataSource(sessionInfo.HostName);
			streamDataSource.Connect();

			VisualizationWindow.DataStore = new ClientDataStore(streamDataSource);
			VisualizationWindow.SubscriptionTranslator = subscriptionTranslator;

			PropertyVisualizationsOptions.ItemsSource = new List<String>
			{
				"Numeric"
			};
			PropertyVisualizationsOptions.SelectedIndex = 0;
		}

		public IEnumerable<DataType> DataTypes
		{
			get { return dataTypes; }
			set
			{
				dataTypes = value;
				NotifyPropertyChanged("DataTypes");
			}
		}

		public PropertyType PropertyType
		{
			get { return propertyType; }
			set
			{
				propertyType = value;
				NotifyPropertyChanged("PropertyType");
			}
		}

		public IEnumerable<PropertyType> Properties
		{
			get { return properties; }
			set
			{
				properties = value;
				NotifyPropertyChanged("Properties");
			}
		}

		public IEnumerable<Source> Sources
		{
			get { return sources; }
			set
			{
				sources = value;
				NotifyPropertyChanged("Sources");
			}
		}

		public IEnumerable<Visualization> Visualizations
		{
			get { return visualizations; }
			set
			{
				visualizations = value;
				NotifyPropertyChanged("Visualizations");
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
	}
}