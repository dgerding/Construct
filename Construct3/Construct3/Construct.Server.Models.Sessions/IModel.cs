using System;
using System.Linq;
using System.ServiceModel;
using Construct.Server.Entities.Adapters;
using System.Collections.Generic;

namespace Construct.Server.Models.Sessions
{
    [ServiceContract(CallbackContract = typeof(Sessions.ICallback))]
    public interface IModel : Models.IModel
    {
        [OperationContract(Name = "AddSession")]
        bool Add(Session session);

        [OperationContract(Name = "AddSessionSource")]
        bool Add(SessionSource sessionSource);

        [OperationContract]
        IEnumerable<Entities.Adapters.Session> GetSessions();

        [OperationContract]
        IEnumerable<Entities.Adapters.Source> GetSources();

        [OperationContract]
        IEnumerable<Entities.Adapters.SessionSource> GetSessionSources();   
    }
}