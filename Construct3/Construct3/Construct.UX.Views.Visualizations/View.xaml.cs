﻿using System;
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

            this.Sources = InitializeSources().Distinct();
            this.Properties = InitializeProperties().Distinct();

			//((viewModel)ViewModel)

			InitializeSubscriptionTranslator();

			streamDataSource = new SignalRStreamDataSource(sessionInfo.HostName);
			streamDataSource.Start();

	        streamDataRouter = new StreamDataRouter(streamDataSource);
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
            DataTypes = viewmodel.GetAllDataTypes().Where(target => target.IsCoreType == false);
        }

	    private void InitializeSubscriptionTranslator()
	    {
		    subscriptionTranslator = new SubscriptionTranslator();
		    //	TODO: Fill in
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
    }
}