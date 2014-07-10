using System;
using System.Linq;
using System.ServiceModel;

namespace Construct.Server.Models.Data
{
    public interface ICallback
    {
        [OperationContract]
        void HandleItem(Datum datum);
    }
}
