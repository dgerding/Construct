using Construct.Server.Entities;
using Construct.Server.Models;
using Construct.Server.Models.Data.MsSql;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Utilities.Shared;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

					ConcurrentQueue<ServiceHost> propertyValueServices = new ConcurrentQueue<ServiceHost>();
					Dictionary<PropertyType, DataType> allPropertyTypes = new Dictionary<PropertyType, DataType>();

					foreach (DataType dataType in context.DataTypes)
					{
						foreach (PropertyType propertyType in dataType.PropertyParents)
						{
							allPropertyTypes.Add(propertyType, dataType);
							//	For some reason, some data retrieval results in null values if we do continuous queries
							System.Threading.Thread.Sleep(0);
						}
					}

					Parallel.ForEach(allPropertyTypes, (propertyDataTypePair) =>
					{
						DataType dataType = propertyDataTypePair.Value;
						PropertyType propertyType = propertyDataTypePair.Key;
						ServiceHost host = PropertyServiceManager.StartService(serverServiceUri, dataType, propertyType, connectionString);
						propertyValueServices.Enqueue(host);
					});

					propertyValueServices.ToList().ForEach((serviceHost) => Services.Add(serviceHost));

                    dbSchemaStatusLabel.Content = modelsHost.IsDatabaseSchemaCurrent.ToString();
                    dbConnectionStatusLabel.Content = modelsHost.IsDatabaseReachable.ToString();
                }
                else
                {
                    MessageBox.Show("Construct Server did NOT start.  Check database connection and schema status");

                    dbSchemaStatusLabel.Content = modelsHost.IsDatabaseSchemaCurrent.ToString();
                    dbConnectionStatusLabel.Content = modelsHost.IsDatabaseReachable.ToString();

                    if (!modelsHost.IsDatabaseSchemaCurrent) 
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

                DropAndAddStoredProceduresToDB(connectionString);

                WipeDatabaseAndAddCoreData(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update schema failed.  " + ex.Message);
                throw;
            }
            //get contenxt and do that thing
        }

        private void WipeDatabaseAndAddCoreData(string connectionString)
        {
            var path = new Uri(
            System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "DeleteItemZTables.sql"));

            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "WipeDatabaseAndResetToDefaults.sql"));

            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "ValidateCoreEntities.sql"));
        }

        private void DropAndAddStoredProceduresToDB(string connectionString)
        {
            //Construct.Server.Models.Data.MsSql.ExecuteMsSqlScript();
            var path = new Uri(
                System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

            //TODO: All SQL scripts should be stored as application resources rather than external files
            //Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","ValidateCoreEntities.sql")
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddBooleanPropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddByteArrayPropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddDateTimePropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddDoublePropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddGuidPropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddIntPropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddItemHeader.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddSinglePropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "AddStringPropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "Create_Login.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateBooleanPropertyValueTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateByteArrayPropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateDateTimePropertyValue.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateDoublePropertyValueTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateGuidPropertyValueTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateInsertProcedureFromByteArrayTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateInt32PropertyValueTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateSinglePropertyValueTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateStringPropertyValueTable.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "CreateTestItem_zTableData.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "DeleteAllRows.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "GetAllPropertyValues.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "GetTypes.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "Insert_Test_Item.sql"));
            ExecuteMsSqlScript.Go(connectionString, Path.Combine(path, "MsSql", "JoinDataPropertyToDataType.sql"));
            
            
            
            

            
        }
    }
}