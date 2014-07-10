using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Construct.Server.Models.Data.MsSql;

namespace Construct.Server.Models.Data
{
    public class PersistenceEventManager
    {

        public void AddPropertyValuePersistenceSupport(string connectionString, string propertyValueName)
        {
            //TODO:In future use connectionString as metadata to resolve perstence type and destination, not just for SqlServer

            SqlServerPersistenceEventSupport.AddPropertyValueTrigger(connectionString, propertyValueName);

        }


       

    }
}
