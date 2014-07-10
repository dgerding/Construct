using System;
using System.Collections.Generic;
using System.Linq;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public class DataTypeCreationInfo
    {
        public Guid DataTypeID { get; set; }

        public string TypeName { get; set; }

        public string SourceName { get; set; }

        public Guid DataTypeSourceID { get; set; }

        public Guid DataTypeSourceParentID { get; set; }

        public IList<DataTypePropertyCreationInfo> Properties { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeCreationInfo"/> class.
        /// </summary>
        public DataTypeCreationInfo()
        {
            Properties = new List<DataTypePropertyCreationInfo>();
        }
    }
}