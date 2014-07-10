using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Construct.Server.Entities;

namespace Construct.Server.Models.Sessions
{
    public interface ICallback
    {
        [OperationContract]
        void AddSessionDesignCallbackReceived();

        
    }
}