using System;
using System.Linq;
using System.ServiceModel;
using Construct.Server.Entities.Adapters;
using System.Collections.Generic;

namespace Construct.Server.Models.Sources
{
    [ServiceContract(CallbackContract = typeof(Sources.ICallback))]
    public interface IModel : Models.IModel
    {
        [OperationContract(Name = "AddSensor")]
        bool Add(Sensor sensor, AddSensorArgs addArgs);

        [OperationContract(Name = "AddQuestion")]
        bool Add(Question question, AddQuestionArgs addArgs);

        [OperationContract(Name = "AddSensorHost")]
        bool Add(SensorHost sensorHost);

        [OperationContract(Name = "AddSensorDefinition")]
        bool Add(SensorRuntime sensorRuntime);

        [OperationContract]
        void LoadSensor(LoadSensorArgs loadArgs);

        [OperationContract]
        void UnloadSensor(UnloadSensorArgs unloadArgs);

        [OperationContract]
        void StartSensor(StartSensorArgs startArgs);

        [OperationContract]
        void StopSensor(StopSensorArgs stopArgs);

        [OperationContract]
        void InvokeGenericCommand(GenericSensorCommandArgs commandArgs);

        [OperationContract]
        IEnumerable<Sensor> GetSensors();

        [OperationContract]
        IEnumerable<HumanReadableSensor> GetHumanReadableSensors();

        [OperationContract]
        IEnumerable<SensorCommand> GetSensorCommands();

        [OperationContract]
        IEnumerable<DataType> GetDataTypes();

        [OperationContract]
        IEnumerable<SensorHost> GetSensorHosts();

        [OperationContract]
        IEnumerable<SensorHostType> GetSensorHostTypes();

        [OperationContract]
        IEnumerable<DataTypeSource> GetDataTypeSources();

        [OperationContract]
        IEnumerable<SensorTypeSource> GetSensorTypeSources();

        [OperationContract]
        IEnumerable<SensorRuntime> GetSensorRuntimes();

        [OperationContract]
        IEnumerable<Source> GetSources();

		[OperationContract]
		IEnumerable<Sensor> GetSensorsEmittingType(DataType dataType);
    }
}