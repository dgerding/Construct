using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Construct.Server.Entities;

namespace Construct.Server.Models.Sources
{
    public interface ICallback
    {
        [OperationContract]
        void SensorLoadedCallbackReceived(Guid guid);

        [OperationContract]
        void SensorInstalledCallbackReceived(Guid guid);

        [OperationContract]
        void AvailableSensorCommandsCallbackReceived(List<Entities.Adapters.SensorCommand> commands);
    }
}