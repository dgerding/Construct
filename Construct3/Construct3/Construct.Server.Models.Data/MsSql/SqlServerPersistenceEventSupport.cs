using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Server.Models.Data.MsSql
{
    public static class SqlServerPersistenceEventSupport
    {
        public static void AddPropertyValueTrigger(string connectionString, string propertyValueName)
        {
            string x = sqlTriggerString;
            //This method ...
            
            //creates CLR Trigger c# code for targeting the table and propertyValue table

            //Compiles the dll

            //Ensures the dll is at a file path accessible to SqlServer instance

            //Writes and invokes the SQL required to create an Assembly from the dll at the SqlServer instance

            //Writes and invokes the SQL to create the trigger

            //Adds the propertyValueName to the dictionary of supported persistence events
        }

        private static string sqlTriggerString = @"";             
    }

 
                        
}
