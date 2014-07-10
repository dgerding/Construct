using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using NLog;
using Telerik.Windows.Zip;

namespace Construct.Credentials.Model
{
    public class ServerValidator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //static private SourcesServiceReference.ModelClient GetModel()
        //{
        //    CallbackImplementation callback = new CallbackImplementation();
        //    callback.OnSensorLoadedCallbackReceived += SensorLoadedCallbackReceived;
        //    callback.OnSensorInstalledCallbackReceived += SensorInstalledCallbackReceived;
        //    InstanceContext context = new InstanceContext(callback);
        //    SourcesServiceReference.ModelClient client = new SourcesServiceReference.ModelClient(context);
        //    client.Open();
        //    return client;
        //}

        static public bool IsValidationServerAvailable()
        {
            try
            {
                logger.Log(LogLevel.Trace, "In IsValidationServerAvailable()");
                EntitiesModel testServerConnection = new EntitiesModel("data source=mc7t5hoo37.database.windows.net;initial catalog=ConstructServers;user ID=ConstructServerLogin;password=!!TheWoodsAreLovelyDarkAndDeep??");

                if (testServerConnection.Connection.State == System.Data.ConnectionState.Open)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.LogException(LogLevel.Warn, "Could not connect to credentials server.", e);
                //TODO: This is an example of where logger can be used to provide details obliterated by friendly ui
                Debug.Write(e.Message);
                return false;
            }
            return false;
        }

        static public bool AreCredentialsAuthentic(string serverName, string userName, string password)
        {
            UserAuthenticator theUserAuthenticator = new UserAuthenticator();

            string connectionString = theUserAuthenticator.IsUserAuthentic(serverName, userName, password);

            if (connectionString.Length > 0)
            {
                return true;
            }

            return false;
        }

        static public bool IsConstructServerAvailable(string connectionString)
        {
            try
            {
                
                //ConstructContext testConstruct3Connection = new ConstructContext(connectionString);
                //if (testConstruct3Connection.Connection.State == System.Data.ConnectionState.Open)
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                logger.LogException(LogLevel.Warn, "Could not connect to Construct Server.", e);
                
                return false;
            }
            //return false;
        }

        static public bool AreConstructServerCoreEntitiesValid(string connectionString)
        {
            try
            {

                EnsureCoreEntitiesExist(connectionString);
                return true;
            }
            catch(Exception e)
            {
                logger.LogException(LogLevel.Warn, "Construct Server entities validation failed.", e);
                
                return false;
            }
            //return false;
        }

        public static string GetConstructServerConnectionString(string theServerName, string theUserName, string thePassword)
        {
            try
            {
                EntitiesModel UserEntities = new EntitiesModel(@"server=tcp:mc7t5hoo37.database.windows.net;database=ConstructServers;user id=ConstructServerLogin;password=!!TheWoodsAreLovelyDarkAndDeep??;trusted_connection=False;encrypt=True");

                var theUser = from User aUser in UserEntities.Users where aUser.UserName == theUserName select aUser;

                if (theUser.Count() == 1)
                {
                    Guid id = theUser.First().ID;

                    var somePassword = from PasswordCredential aPassword in UserEntities.PasswordCredentials where aPassword.UserID == id select aPassword;//theUser.First().ID select aPassword;

                    if (somePassword.Count() > 0)
                    {
                        if (somePassword.First().Password == thePassword)
                        {
                            var theServer = from Server aServer in UserEntities.Servers where aServer.ServerName == theServerName select aServer;

                            if (theServer.Count() > 0)
                            {
                                return theServer.First().ConnectionString;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        static public void EnsureCoreEntitiesExist(string connectionString)
        {
            //string workingDir = "C:\\Construct";
            ExecuteScript.Go(connectionString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","ValidateCoreEntities.sql"));
        }

        static public void EnsureValidUnzip(string folderPath, string zipFile)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (FileStream fileStream = new FileStream(Path.Combine(folderPath, zipFile), FileMode.OpenOrCreate))
                {
                    using (ZipPackage package = ZipPackage.Open(fileStream))
                    {
                        foreach (var entry in package.ZipPackageEntries)
                        {
                            if (entry.Attributes != FileAttributes.Directory)
                            {
                                SaveFile(entry, folderPath);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw new ApplicationException("EnsureValidWorkingDirectory threw an exception.");
            }
        }

        static public void LoadTestItemData(string connectionString)
        {
            string correctedString = connectionString;
            correctedString = correctedString.Replace("User ID=Construct", "User ID=Construct");

            ExecuteScript.Go(correctedString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","CreateItemScratch.sql"));
            ExecuteScript.Go(correctedString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","CreateTestItemData.sql"));
            ExecuteScript.Go(correctedString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","CreateTestItem_zData.sql"));
        }

        //Save the unzipped files
        private static void SaveFile(ZipPackageEntry entry, string folderPath)
        {
            Stream reader = entry.OpenInputStream();
            var fileName = folderPath + "\\" + entry.FileNameInZip;

            var directoryName = Path.GetDirectoryName(fileName);

            if (directoryName != null)
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                FileStream writer = File.Create(folderPath + "\\" + entry.FileNameInZip);

                int size = 2048;
                byte[] data = new byte[2048];
                try
                {
                    while (true)
                    {
                        size = reader.Read(data, 0, data.Length);
                        if (size > 0)
                            writer.Write(data, 0, size);
                        else
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message + ":" + e.InnerException.Message);
                }
                writer.Close();
            }
        }

        private static Guid GetSensorHostTypeIDFromXML(string theXml)
        {
            XDocument document = XDocument.Parse(theXml);

            foreach (XElement externalAndInternalSource in document.Root.Nodes())
            {
                string sensorHostTypeIDstring = externalAndInternalSource.Attributes("SensorHostTypeID").First().Value;
                return Guid.Parse(sensorHostTypeIDstring);
            }
            return Guid.Empty;
        }
    }
}