using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    [ServiceContract]
    public interface ISensor
    {
        [OperationContract]
        string Start();
        [OperationContract]
        string Stop();
        [OperationContract]
        string Exit();

        void SendItem(Object itemPayload);
    }
}

