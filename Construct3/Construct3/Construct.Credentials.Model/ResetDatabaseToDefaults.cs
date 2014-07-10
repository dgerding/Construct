using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Hosting;

namespace Construct.Credentials.Model
{
    static public class WipeDatabaseAndResetToDefaults
    {
        static public bool Go(string connectionString)
        {
            bool wipeResults = true;
            bool deleteItemZ = true;
            try
            {
                string correctedString = connectionString;
                correctedString = correctedString.Replace("User ID=Construct", "User ID=ConstructValidation");

                //string workingDir = "C:\\Construct";

                wipeResults = ExecuteScript.Go(connectionString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","DeleteAllRows.sql"));
                deleteItemZ = ExecuteScript.Go(connectionString, Path.Combine(HostingEnvironment.ApplicationPhysicalPath,"bin","DeleteItemZTables.sql"));

                ServerValidator.EnsureCoreEntitiesExist(connectionString);

                return (wipeResults && deleteItemZ);
            }
            catch
            {
                return false;
            }
        }
    }
}