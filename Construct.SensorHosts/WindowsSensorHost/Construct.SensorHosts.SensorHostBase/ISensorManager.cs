using System;
using System.ServiceModel;

namespace Construct.SensorManagers.SensorManagerBase
{
    public interface ISensorManager
    {
        Guid ID { get; }
        bool InstallSensor(string installerFileUri, string installerFile, string name, Guid sourceTypeID, string version, bool overwrite);
        bool LoadSensor(Guid sourceTypeID, Guid sourceID, string version, string constructServerUri, string startUpArguments);
        bool UnloadSensor(Guid sourceTypeID, Guid sourceID, string version);
        bool StartSensor(Guid sourceTypeID, Guid sourceID, string version);
        bool StopSensor(Guid sourceTypeID, Guid sourceID, string version);
    }
}