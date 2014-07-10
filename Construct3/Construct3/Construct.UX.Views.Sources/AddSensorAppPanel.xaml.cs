using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using Construct.UX.ViewModels.Sources;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Sources
{
    /// <summary>
    /// Interaction logic for AddSensorAppPanel.xaml
    /// </summary>
    /// 
    public partial class AddSensorAppPanel : UserControl
    {
        public ViewModel TheViewModel;
        const string defaultAddPackageMessage = @"Click here to add sensor app...";

        public AddSensorAppPanel()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            InitializeComponent();
            this.SensorInstallationFilePath.Text = defaultAddPackageMessage;
        }

        private void SensorInstallationFilePathAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.SensorInstallationFilePath.Text) == false &&
                this.SensorInstallationFilePath.Text != defaultAddPackageMessage)
            {
                dynamic parameter = new ExpandoObject();
                parameter.InstallationPath = SensorInstallationFilePath.Text;

                string results = TheViewModel.AddSensorDefinition(parameter);
                SensorInstallationFilePath.Text = defaultAddPackageMessage;

                if (results != String.Empty)
                {
                    ResultsTextBox.Text += String.Format("{0}\n", results);
                }
            }
        }

        private void SensorInstallationFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void SensorInstallationFilePath_GotFocus(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".zip"; // Default file extension
            dlg.Filter = "Zip Files (.zip)|*.zip"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                SensorInstallationFilePath.Text = dlg.FileName;
            }
        }
    }
}