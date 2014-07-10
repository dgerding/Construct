using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public class DataTypePropertyCreationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypePropertyCreationInfo"/> class.
        /// </summary>
        public DataTypePropertyCreationInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypePropertyCreationInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public DataTypePropertyCreationInfo(string name, string type, Guid id)
        {
            Name = name;
            Type = type;
            ID = id;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        public Guid ID { get; set; }
    }
}