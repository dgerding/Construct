using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Construct.Utilities.Shared;
using System.Collections.ObjectModel;
using Construct.Server.Models;
using Construct.Server.Entities;
using System.Configuration;
using Construct.Server.Models.Data.PropertyValue;

namespace ConstructServer.Runtime.Windows
{
    public partial class MainWindow : Window
    {
        private Construct.Server.Models.Server modelsHost;
        public ObservableCollection<ServiceHost> Services = new ObservableCollection<ServiceHost>();

        Guid serverProcessID;
        string publicHostName;
        int publicHostPortBase;

        public MainWindow()
        {
            InitializeComponent();
            servicesList.DataContext = this;
            servicesList.ItemsSource = Services;
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", serverProcessID, publicHostPortBase);
            int hostPortBase = 8000;
            string connStringLookupKey = ConnectionStringLookupKey.Text;
            string connectionString = ConfigurationManager.ConnectionStrings[connStringLookupKey].ConnectionString;
            EntitiesModel context = new EntitiesModel(connectionString);

                
            if (modelsHost == null)
            {
                if (Guid.TryParse(ServerProcessIDTextBox.Text, out serverProcessID))
                {
                    publicHostName = PublicHostNameTextBox.Text;
                    publicHostPortBase = int.Parse(PublicHostPortBaseTextBox.Text);
                }
                else
                {
                    //TODO: Alert user guid is malformed
                    return;
                }

                //Server server = new Server(serverServiceUri, hostPortBase, connStringLookupKey);
                modelsHost = new Server(serverServiceUri, hostPortBase, connStringLookupKey);

                modelsHost.Start();

                if (modelsHost.IsDataServiceReady)
                { 
                    IEnumerable<ServiceHost> hostedServices = modelsHost.Hosts;

                    foreach (ServiceHost host in hostedServices)
                    {
                        Services.Add(host);
                    }
                    //if (modelsHost.Start(serverUri, publicHostPortBase))
                    {
                        StartServerButton.IsEnabled = !StartServerButton.IsEnabled;
                        StopServerButton.IsEnabled = !StopServerButton.IsEnabled;

                        ServerProcessIDTextBox.IsEnabled = !ServerProcessIDTextBox.IsEnabled;
                        PublicHostNameTextBox.IsEnabled = !PublicHostNameTextBox.IsEnabled;
                        PublicHostPortBaseTextBox.IsEnabled = !PublicHostPortBaseTextBox.IsEnabled;
                        ConnectionStringLookupKey.IsEnabled = !ConnectionStringLookupKey.IsEnabled;
                    }
                    //else
                    //{
                    //    //logger.
                    //}
                    foreach (DataType dataType in context.DataTypes)
                    {
                        foreach (PropertyType propertyType in dataType.PropertyParents)
                        {
                            ServiceHost host = PropertyServiceManager.StartService(serverServiceUri, dataType, propertyType, connectionString);
                            Services.Add(host);
                        }
                    }

                    dbSchemaStatusLabel.Content = modelsHost.IsDatabaseSchemaCurrent.ToString();
                    dbConnectionStatusLabel.Content = modelsHost.IsDatabaseReachable.ToString();
                }
                else
                {
                    MessageBox.Show("Construct Server did NOT start.  Check database connection and schema status");

                    dbSchemaStatusLabel.Content = modelsHost.IsDatabaseSchemaCurrent.ToString();
                    dbConnectionStatusLabel.Content = modelsHost.IsDatabaseReachable.ToString();

                    if(!modelsHost.IsDatabaseSchemaCurrent) 
                    {
                        ReinitializeDatabaseButton.Visibility = Visibility.Visible;
                    }

                }
            }
            else
            {
                if (modelsHost.IsRunning == true)
                {
                    modelsHost.Stop();
                    StartServerButton.IsEnabled = !StartServerButton.IsEnabled;
                    StopServerButton.IsEnabled = !StopServerButton.IsEnabled;

                    ServerProcessIDTextBox.IsEnabled = !ServerProcessIDTextBox.IsEnabled;
                    PublicHostNameTextBox.IsEnabled = !PublicHostNameTextBox.IsEnabled;
                    PublicHostPortBaseTextBox.IsEnabled = !PublicHostPortBaseTextBox.IsEnabled;
                }
            }

            if (modelsHost.IsDatabaseSchemaCurrent)
            {
               // ADjust label color to call out exceptions

                ReinitializeDatabaseButton.Visibility = Visibility.Hidden;
                
            }
            else
            {

            }

            
        }

        private void StopServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (modelsHost != null)
            {
                try
                {
                    modelsHost.Stop();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            StartServerButton.IsEnabled = !StartServerButton.IsEnabled;
            StopServerButton.IsEnabled = !StopServerButton.IsEnabled;

            ServerProcessIDTextBox.IsEnabled = !ServerProcessIDTextBox.IsEnabled;
            PublicHostNameTextBox.IsEnabled = !PublicHostNameTextBox.IsEnabled;
            PublicHostPortBaseTextBox.IsEnabled = !PublicHostPortBaseTextBox.IsEnabled;
        }

        private void OnServerTelemetryStatusSample()
        {
            Process proc = Process.GetCurrentProcess();
            string x = string.Format("{0}kb", proc.PeakWorkingSet64 / 1024);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (modelsHost != null)
            {
                try
                {
                    modelsHost.Stop();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void ReinitializeDatabaseButton_Click(object sender, RoutedEventArgs e)
        {

            Uri serverServiceUri = UriUtility.CreateStandardConstructServiceEndpointUri("http", "Server", "localhost", serverProcessID, publicHostPortBase);
            int hostPortBase = 8000;
            string connStringLookupKey = ConnectionStringLookupKey.Text;
            string connectionString = ConfigurationManager.ConnectionStrings[connStringLookupKey].ConnectionString;

            try
            {
                EntitiesModel context = new EntitiesModel(connectionString);

                Telerik.OpenAccess.ISchemaHandler schemaHandler = context.GetSchemaHandler();
                string script = null;
                if (schemaHandler.DatabaseExists())
                {
                    script = schemaHandler.CreateUpdateDDLScript(null);
                    
                }
                else
                {
                    schemaHandler.CreateDatabase();
                    script = schemaHandler.CreateDDLScript();
                }
                if (!string.IsNullOrEmpty(script))
                {
                    //context.Connection.Close();
                    schemaHandler.ExecuteDDLScript(script);
                }

                MessageBox.Show("Update schema succeded.");

            }
            catch (Exception)
            {
                MessageBox.Show("Update schema failed.");
                throw;
            }
            

            //get contenxt and do that thing
        }
    }
}