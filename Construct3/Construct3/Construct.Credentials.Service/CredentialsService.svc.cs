using System;
using System.Linq;
using System.ServiceModel;
using Construct.Credentials.Model;
using NLog;

namespace Construct.Credentials.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CredentialsService : Construct.Credentials.Model.Model
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public CredentialsService() 
        {
            logger.Trace("Entered CredentialsService ctor.");
            //logger.Debug("Sample debug message");
            //logger.Info("Sample informational message");
            //logger.Warn("Sample warning message");
            //logger.Error("Sample error message");
            //logger.Fatal("Sample fatal error message");
            
            
        }

        
    }
}