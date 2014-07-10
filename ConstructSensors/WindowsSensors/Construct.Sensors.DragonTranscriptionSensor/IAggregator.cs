using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Construct.Sensors.DragonTranscriptionSensor.AggregatorService;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    [ServiceContract]
    public interface IAggregator
    {
        [OperationContract]
        Guid GetID();

        [OperationContract]
        void AddItem(Guid sourceID, DateTime createdTime, SqlGeography createdLocation, Object blob);

        [OperationContract]
        void AddTelemetry(Guid sourceID, string theTelemetry);

        [OperationContract]
        void AddStream(Guid sourceID);
    }
}