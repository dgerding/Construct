using System;
using System.Linq;
using System.Data;
using Construct.MessageBrokering;
using NLog;

namespace Construct.Server.Models
{
    /// <summary>
    /// The abstract base model for a model tier.
    /// </summary>
    public abstract class Model : IModel
    {
        public string Name { get; protected set; }

        protected static Logger logger = LogManager.GetCurrentClassLogger();
      
        public Model()
        {
        }
    }
}