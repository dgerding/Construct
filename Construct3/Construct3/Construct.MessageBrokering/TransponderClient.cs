using System;
using System.Linq;
using System.ServiceModel;

namespace Construct.MessageBrokering.TransponderService
{
    public partial class TransponderClient : System.ServiceModel.ClientBase<TransponderService.ITransponder>, TransponderService.ITransponder    
    {
        public void SetOpTimeout(TimeSpan timeout)
        {
            ((IContextChannel)base.Channel).OperationTimeout = timeout;
        }
    }
}