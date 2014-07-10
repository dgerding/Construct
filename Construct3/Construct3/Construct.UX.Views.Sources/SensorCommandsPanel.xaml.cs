using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels.Sources;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Construct.UX.ViewModels.Sources.SourcesServiceReference;

namespace Construct.UX.Views.Sources
{
    /// <summary>
    /// Interaction logic for SensorCommandsPanel.xaml
    /// </summary>
    public partial class SensorCommandsPanel : UserControl
    {
        public ViewModel TheViewModel;
        private bool loadedHasRun = false;

        public SensorCommandsPanel()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            InitializeComponent();
        }

        public void ExecuteSelectedSensorCommand_Click(object sender, RoutedEventArgs e)
        {
            TheViewModel.GenericSensorCommand();
        }

        public void CommandParametersGridView_Loaded(object sender, RoutedEventArgs e)
        {
            var childGrid = (RadGridView)sender;
            var parentRow = childGrid.ParentRow;

            if (TheViewModel.CurrentSensorCommand != null)
            {
                childGrid.ItemsSource = TheViewModel.CurrentSensorCommand.SensorCommandParameters;
            }

            if (parentRow != null)
            {
                SensorCommandGridView.SelectedItem = childGrid.DataContext;
                parentRow.IsExpandedChanged += new RoutedEventHandler(parentRow_IsExpandedChanged);
            }
        }

        //TODO: This event handler is behaving in a crazy way
        // The reason there are "temp"s in here is because I was disecting it, trying to figure out HOW the line
        // TheViewModel.CurrentSensorCommand = temp2;
        // ends up with currentsensorcommand still equalling null after assigning it to something. Whoever cares to look 
        // at it next should check out the "setter" for CurrentSensorCommand, all I can think is that it is failing to set
        // the value, so it comes back null again. 
        public void SensorCommandGridView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var temp = (RadGridView)sender;
            if (temp.SelectedItems.Count > 0)
            {
                var temp2 = (SensorCommand)(temp.SelectedItems[0]);
                TheViewModel.CurrentSensorCommand = temp2;
            }
            
        }

        public void CommandParametersGridView_CellEditEnded(object sender, RoutedEventArgs e)
        {
        }
        
        void parentRow_IsExpandedChanged(object sender, RoutedEventArgs e)
        {
            SensorCommandGridView.SelectedItem = ((GridViewRow)sender).DataContext;
        }
    }
}