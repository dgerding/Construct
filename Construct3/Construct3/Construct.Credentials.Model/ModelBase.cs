using System;
using System.Linq;
using System.Data;
using Telerik.OpenAccess;

namespace Construct.Credentials.Model
{
    /// <summary>
    /// The abstract base model for a model tier.
    /// </summary>
    public abstract class ModelBase : IModelBase
    {
        public string Name { get; protected set; }
      
        public ModelBase()
        {
        }
    }
}