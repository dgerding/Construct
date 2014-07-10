using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Construct.UX.ViewModels;
using Construct.UX.ViewModels.Sources.SourcesServiceReference;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Construct.UX.Views.Sources
{
    public partial class View : Views.View
    {
        public View(ApplicationSessionInfo theApplicationSessionInfo)
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            
            InitializeComponent();

            InitializeViewModel(theApplicationSessionInfo);
        }

        public ViewModels.Sources.ViewModel SourcesViewModel
        {
            get
            {
                return ViewModel as ViewModels.Sources.ViewModel;
            }
        }

        public void InitializeViewModel(ApplicationSessionInfo theApplicationSessionInfo)
        {
            if (ViewModel == null)
            {
                ViewModel = new ViewModels.Sources.ViewModel(theApplicationSessionInfo);
            }
            if (this.ViewModel.SessionInfo.IsAuthenticated == false)
            {
                Visibility = System.Windows.Visibility.Hidden; 
            }
            else
            {
                LayoutRoot.DataContext = SourcesViewModel;

                theAddSourcePanel.TheViewModel = SourcesViewModel;
                theAddSourcePanel.DataContext = ViewModel;

                theSensorHostsPanel.TheViewModel = SourcesViewModel;
                theSensorHostsPanel.DataContext = ViewModel;

                theSensorCommandsPanel.TheViewModel = SourcesViewModel;
                theSensorCommandsPanel.DataContext = ViewModel;

                radTreeListView1.ItemsSource = SourcesViewModel.ObservableDataTypeSources;
            }
        }

        private void radTreeListView1_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row is TreeListViewRow)
            {
                TreeListViewRow row = e.Row as TreeListViewRow;
                DataTypeSource context = row.Item as DataTypeSource;

                row.IsExpandable = (from d in SourcesViewModel.ObservableDataTypeSources
                                    where d.ParentID == context.ID
                                    select d).Count() > 0;
            }
        }

        private void SaveLayoutButton_Click(object sender, System.Windows.RoutedEventArgs args)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, (object)SaveDockLayoutAsXmlString());
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        private string SaveDockLayoutAsXmlString()
        {
            MemoryStream stream = new MemoryStream();
            SourcesDock.SaveLayout(stream);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SourcesWorkflowPanelsContainer.Width = LayoutRoot.ActualWidth - SourcesGraphContainer.ActualWidth;
        }
    }
}