using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Construct.Credentials.Model
{
    public static class ConstructServerConnectionTester
    {
        public static string GetConnectionStringUsingConnectionStringName(string theConnectionStringName)
        {
            if (IsExistingConnectionString(theConnectionStringName))
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[theConnectionStringName];
                return settings.ConnectionString;
            }
            return "ERROR";
        }

        public static bool IsExistingConnectionString(string theConnectionStringName)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[theConnectionStringName];

            if (settings == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsCredentialsServerAvailable()
        {
            EntitiesModel userEntities = new EntitiesModel("data source=mc7t5hoo37.database.windows.net;initial catalog=ConstructServers;user ID=ConstructServerLogin;password=!!TheWoodsAreLovelyDarkAndDeep??");

            try
            {
                if (userEntities.Connection.State == System.Data.ConnectionState.Open)
                {
                    userEntities.Dispose();
                    return true;
                }
                else
                {
                    userEntities.Dispose();
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                userEntities.Dispose();
                return false;
            }
        }
    }
}