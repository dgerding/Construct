using System;
using System.Linq;
using System.ServiceModel;

namespace Construct.Server.Models.Learning
{
    [ServiceContract]
    public interface IModel : Models.IModel
    {
        [OperationContract]
        void GeneratedLabelAttributeVectors(Guid sessionID);
    }
}