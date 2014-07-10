using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Construct.MessageBrokering.Transponder
{
    [ServiceContract]
    public interface ITransponder
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        bool AddObject(string jsonObject);
    }
}