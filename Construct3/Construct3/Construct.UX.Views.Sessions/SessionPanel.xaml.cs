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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using System.Dynamic;
using Construct.UX.ViewModels.Sessions;
using Construct.UX.ViewModels.Sessions.SessionsServiceReference;

namespace Construct.UX.Views.Sessions
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SessionPanel : UserControl
    {
        public ViewModel TheViewModel;

        public SessionPanel()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //can we put this in a constructor?
        }

        private void AddSession_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(AddSessionTextBox.Text))
            {
                MessageBox.Show("Please enter a name for the Session.");
            }
            else if (SessionStartPicker.SelectedDate == null || SessionStartPicker.SelectedTime == null)
            {
                MessageBox.Show("Please select a Date and Time from the \"Pick Session Start\" Selector.");
            }
            else if (SessionEndPicker.SelectedDate == null || SessionEndPicker.SelectedTime == null)
            {
                MessageBox.Show("Please select a Date and Time from the \"Pick Session End\" Selector.");
            }
            else 
            {
                dynamic parameter = new ExpandoObject();
                parameter.ID = Guid.NewGuid();
                parameter.FriendlyName = AddSessionTextBox.Text;
                parameter.StartTime = SessionStartPicker.SelectedValue;
                parameter.Interval = (SessionEndPicker.SelectedValue.Value - SessionStartPicker.SelectedValue.Value).Ticks;
                TheViewModel.AddSession(parameter);
            }
        }

        private void AddSessionSource_Click(object sender, RoutedEventArgs e)
        {
            Session selectedSession = (Session)SessionComboBox.SelectedValue;
            Source selectedSource = (Source)SourceComboBox.SelectedValue;

            if (selectedSession != null && selectedSource != null)
            {
                dynamic parameter = new ExpandoObject();
                parameter.ID = Guid.NewGuid();
                parameter.SessionID = selectedSession.ID;
                parameter.SourceID = selectedSource.ID;
                TheViewModel.AddSessionSource(parameter);
            }
        }

    }
}
