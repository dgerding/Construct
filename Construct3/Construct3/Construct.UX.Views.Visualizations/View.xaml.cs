using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels;
using System.ComponentModel;
using Construct.UX.Views.Helper;
using viewModel = Construct.UX.ViewModels.Visualizations.ViewModel;
using Construct.Utilities.Shared;
using System.ServiceModel;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;

namespace Construct.UX.Views.Visualizations
{
    public partial class View : Views.View, INotifyPropertyChanged
    {
        private Random random = new Random();
        public event PropertyChangedEventHandler PropertyChanged;

        private IEnumerable<DataType> dataTypes = null;
        public IEnumerable<DataType> DataTypes
        {
            get { return dataTypes; }
            set
            {
                dataTypes = value;
                NotifyPropertyChanged("DataTypes");
            }
        }

        private PropertyType propertyType = null;
        public PropertyType PropertyType
        {
            get { return propertyType; }
            set
            {
                propertyType = value;
                NotifyPropertyChanged("PropertyType");
            }
        }
        private IEnumerable<PropertyType> properties = null;
        public IEnumerable<PropertyType> Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                NotifyPropertyChanged("Properties");
            }
        }
        private IEnumerable<Source> sources = null;
        public IEnumerable<Source> Sources
        {
            get { return sources; }
            set
            {
                sources = value;
                NotifyPropertyChanged("Sources");
            }
        }



        private IEnumerable<Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization> visualizations = null;
        public IEnumerable<Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization> Visualizations
        {
            get { return visualizations; }
            set
            {
                visualizations = value;
                NotifyPropertyChanged("Visualizations");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Uri serverServiceUri = null;
	    private IStreamDataSource streamDataSource;
	    private StreamDataRouter streamDataRouter;
	    private SubscriptionTranslator subscriptionTranslator;


        public View(ApplicationSessionInfo sessionInfo)
            : base()
        {
            InitializeComponent();
            InitializeViewModel(sessionInfo);

            this.DataContext = this;
            InitializeMembers();

            serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.Empty, 8000);

            Visualizations = ((viewModel)ViewModel).GetVisualizations();

            this.Sources = InitializeSources().Distinct().ToList();
            this.Properties = InitializeProperties().Distinct().ToList();

			//((viewModel)ViewModel)

			InitializeSubscriptionTranslator();

			streamDataSource = new SignalRStreamDataSource(sessionInfo.HostName);
			streamDataSource.Start();

	        streamDataRouter = new StreamDataRouter(streamDataSource);

	        VisualizationWindow.DataRouter = streamDataRouter;
	        VisualizationWindow.SubscriptionTranslator = subscriptionTranslator;

	        PropertyVisualizationsOptions.ItemsSource = new List<String>()
	        {
				"Numeric"
	        };
	        PropertyVisualizationsOptions.SelectedIndex = 0;
        }

        private IEnumerable<PropertyType> InitializeProperties()
        {
            foreach (var dataType in ((viewModel)ViewModel).GetAllDataTypes())
            {
                foreach(var property in ((viewModel)ViewModel).GetAllProperties(dataType.Name))
                {
                    yield return property;
                }
            }
        }

        private IEnumerable<Source> InitializeSources()
        {
            foreach (var dataType in ((viewModel)ViewModel).GetAllDataTypes())
            {
                foreach (var source in ((viewModel)ViewModel).GetAssociatedSources(dataType))
                {
                    yield return source;
                }
            }
        }

        private void InitializeMembers()
        {
            viewModel viewmodel = ((viewModel)ViewModel);
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
			    var dataProperties = Properties.Cast<PropertyParent>().Where((p) => p.ParentDataTypeID == dataType.ID).ToList();
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
					    newTranslation.SourceName = rendezvousHostnameExtractor.Match(sensor.CurrentRendezvous).Groups[1].Captures[0].Value;
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
                base.ViewModel = new ViewModels.Visualizations.ViewModel(theApplicationSessionInfo);
            }
            this.ViewModel.SessionInfo = theApplicationSessionInfo;
            if (this.ViewModel.SessionInfo.IsAuthenticated == false)
            {
                this.Visibility = System.Windows.Visibility.Hidden;
            }
        }

		private void AddPropertyVisualizationButton_Click(object sender, RoutedEventArgs e)
		{
			VisualizationWindow.AddVisualization(PropertyVisualizationsOptions.Text);
		}
    }
}