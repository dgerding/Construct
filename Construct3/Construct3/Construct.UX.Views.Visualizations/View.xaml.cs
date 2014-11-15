using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels;
using System.ComponentModel;
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

        private DataType dataType = null;
        public DataType DataType
        {
            get { return dataType; }
            set
            {
                dataType = value;
                NotifyPropertyChanged("DataType");
            }
        }
        private IEnumerable<Visualizer> visualizers = null;
        public IEnumerable<Visualizer> Visualizers
        {
            get { return visualizers; }
            set
            {
                visualizers = value;
                NotifyPropertyChanged("Visualizers");
            }
        }

        private Visualizer visualizer = null;
        public Visualizer Visualizer
        {
            get { return visualizer; }
            set
            {
                visualizer = value;
                NotifyPropertyChanged("Visualizer");
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

        private Source source = null;
        public Source Source
        {
            get { return source; }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
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


        public View(ApplicationSessionInfo sessionInfo)
            : base()
        {
            InitializeComponent();
            InitializeViewModel(sessionInfo);

			streamDataSource = new SignalRStreamDataSource(sessionInfo.HostName);

            this.DataContext = this;
            InitializeMembers();

            serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", Guid.Empty, 8000);

            Visualizations = ((viewModel)ViewModel).GetVisualizations();
            this.VisualizationWindow.ItemLabeled += new Action<LabeledItemAdapter>(VisualizationWindow_ItemLabeled);

            this.Sources = InitializeSources().Distinct();
            this.Properties = InitializeProperties().Distinct();

            this.VisualizationWindow.Labels = new System.Collections.ObjectModel.ObservableCollection<TaxonomyLabel>(((viewModel)ViewModel).GetAllTaxonomyLables());
            this.VisualizationWindow.Taxonomies = new System.Collections.ObjectModel.ObservableCollection<Taxonomy>(((viewModel)ViewModel).GetAllTaxonomies());
            this.VisualizationWindow.Sources = new System.Collections.ObjectModel.ObservableCollection<Source>(this.Sources);
            this.VisualizationWindow.Properties = new System.Collections.ObjectModel.ObservableCollection<PropertyType>(this.Properties);
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

        void VisualizationWindow_ItemLabeled(LabeledItemAdapter obj)
        {
            viewModel viewModel = ((viewModel)ViewModel);
            viewModel.AddLabeledItem(obj);
        }

        private void InitializeMembers()
        {
            viewModel viewmodel = ((viewModel)ViewModel);
            DataTypes = viewmodel.GetAllDataTypes().Where(target => target.IsCoreType == false);
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

        public void CreateClicked(object sender, RoutedEventArgs e)
        {
            this.nameTextBox.IsEnabled = false;
            this.dataTypeComboBox.IsEnabled = false;
            this.propertiesComboBox.IsEnabled = false;
            this.visualizerComboBox.IsEnabled = false;
            this.visualizerComboBox.IsEnabled = false;
            this.createButton.IsEnabled = false;

            CreateVisuals(visualizer.Name, dataType.Name, propertyType.Name);


            Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization entity;
            entity = new ViewModels.Visualizations.VisualizationsServiceReference.Visualization();
            entity.DataTypeID = DataType.ID;
            entity.PropertyID = PropertyType.ID;
            entity.Description = "";
            entity.ID = Guid.NewGuid();
            entity.Name = this.nameTextBox.Text;
            entity.VisualizerID = visualizer.ID;

            ((viewModel)ViewModel).AddVisualization(entity);

            this.sourcesComboBox.SelectedItem = null;
            this.visualizerComboBox.SelectedItem = null;
            this.propertiesComboBox.SelectedItem = null;
            this.dataTypeComboBox.SelectedItem = null;
            this.nameTextBox.Text = string.Empty;

            this.nameTextBox.IsEnabled = true;
            this.dataTypeComboBox.IsEnabled = false;
            this.propertiesComboBox.IsEnabled = false;
            this.visualizerComboBox.IsEnabled = false;
            this.sourcesComboBox.IsEnabled = false;
            this.createButton.IsEnabled = false;
        }


        public void PropertySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PropertyType = (PropertyType)propertiesComboBox.SelectedItem;
            if (PropertyType != null)
            {
                this.Visualizers = ((viewModel)ViewModel).GetAssociatedVisualizers(PropertyType);

                this.visualizerComboBox.IsEnabled = true;
                this.createButton.IsEnabled = false;
            }
        }

        public void DataTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DataType = ((DataType)dataTypeComboBox.SelectedItem);
            if (DataType != null)
            {
                this.Properties = ((viewModel)ViewModel).GetAllProperties(dataType.Name);
                this.Sources = ((viewModel)ViewModel).GetAssociatedSources(dataType);

                this.propertiesComboBox.IsEnabled = true;
                this.visualizerComboBox.IsEnabled = false;
                this.sourcesComboBox.IsEnabled = false;
                this.createButton.IsEnabled = false;
            }
        }

        private void SourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Source = (Source)sourcesComboBox.SelectedItem;

            if (this.source != null)
            {
                this.createButton.IsEnabled = true;
            }
        }

        public void VisualizerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Visualizer = (Visualizer)visualizerComboBox.SelectedItem;
            
            if (Visualizer != null)
            {
                this.Sources = ((viewModel)ViewModel).GetAssociatedSources(dataType);

                this.sourcesComboBox.IsEnabled = true;
                this.createButton.IsEnabled = false;
            }
        }

        private void NameChanged(object sender, TextChangedEventArgs e)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(nameTextBox.Text);
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(nameTextBox.Text);

            if (isNullOrEmpty == false && isNullOrWhiteSpace == false)
            {
                this.dataTypeComboBox.IsEnabled = true;
                this.propertiesComboBox.IsEnabled = false;
                this.visualizerComboBox.IsEnabled = false;
                this.sourcesComboBox.IsEnabled = false;
                this.createButton.IsEnabled = false;
            }
            else
            {
                this.dataTypeComboBox.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox.SelectedIndex != -1)
            {
                Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization entity;
                entity = (Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference.Visualization)this.listBox.SelectedItem;

                Visualizer visualizer = ((viewModel)ViewModel).GetAssociatedVisualizer(entity);
                DataType tempDataType = ((viewModel)ViewModel).GetAllDataTypes().Single(target => target.ID == entity.DataTypeID);
                Property tempProperty = ((viewModel)ViewModel).GetAllProperties(tempDataType.Name).Single(target => target.ID == entity.PropertyID);

                CreateVisuals(visualizer.Name, tempDataType.Name, tempProperty.Name);
            }
        }

        private void CreateVisuals(string visualizerName, string dataTypeName, string propertyTypeName)
        {
            Uri propertyServiceUri = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverServiceUri, dataTypeName, propertyTypeName);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;

            switch (visualizerName)
            {
                case "IntVisualizer":
                    IntVisualizer intVisualizer = new IntVisualizer(this.Dispatcher, propertyServiceUri, binding);
                    this.VisualizationWindow.Add(intVisualizer);

                    break;
                case "BooleanVisualizer":
                    BooleanVisualizer boolVisualizer = new BooleanVisualizer(this.Dispatcher, propertyServiceUri, binding);
                    this.VisualizationWindow.Add(boolVisualizer);

                    break;
                default:
                    //TODO: Add logic to load visualizer from dll with name.
                    break;
            }       
        }

        private void DataTypeComboBoxDropDownOpened(object sender, EventArgs e)
        {
            InitializeMembers();
        }

		private void StreamButton_Click(object sender, RoutedEventArgs e)
		{
			streamDataSource.Start();
		}

    }
}