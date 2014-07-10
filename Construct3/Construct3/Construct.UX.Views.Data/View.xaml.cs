using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Construct.UX.ViewModels;
using Construct.UX.ViewModels.Data;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Construct.UX.Views.Data
{
    public partial class View : Views.View
    {
		//	Did my best not to hack the data exporter together, but this was somewhat necessary - exporter needs reference to sources viewmodel to gather
		//		the sources available to export data from. Intended to be assigned after initialization of all views.
		public Construct.UX.ViewModels.Sources.ViewModel SourcesViewModel;

        public View(ApplicationSessionInfo theApplicationSessionInfo)
        {
            StyleManager.ApplicationTheme = new ModernTheme();

            InitializeComponent();

            InitializeViewModel(theApplicationSessionInfo);
        }

        private ViewModels.Data.ViewModel DataViewModel
        {
            get
            {
                return ViewModel as ViewModels.Data.ViewModel;
            }
        }

        public void InitializeViewModel(ApplicationSessionInfo theApplicationSessionInfo)
        {
            if (ViewModel == null)
            {
                ViewModel = new ViewModels.Data.ViewModel(theApplicationSessionInfo);
            }
            if (this.ViewModel.SessionInfo.IsAuthenticated == false)
            {
                Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                //TODO
            }
            DataTypeTreeList.ItemsSource = DataViewModel.ObservableDataTypes;
        }

        private void RadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, (object)SaveDockLayoutAsXmlString());
            }
            catch (System.Exception)
            {

                //("Copy to clipboard failed.");
            }
        }

        private string SaveDockLayoutAsXmlString()
        {
            MemoryStream stream = new MemoryStream();
            theDock.SaveLayout(stream);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           //TODO: Anything need anything like... SourcesWorkflowPanelsContainer.Width = LayoutRoot.ActualWidth - SourcesGraphContainer.ActualWidth;
        }

		private void ExportDataButton_Click(object sender, RoutedEventArgs e)
		{
			ExportDataWindow exportDataWindow = new ExportDataWindow(this.DataViewModel, this.SourcesViewModel);
			exportDataWindow.ShowDialog();
		}
    }
}