using System;
using System.Collections.Generic;
using System.Linq;

namespace SensorSharedTypes
{
    public enum SensorCommand
    {
        StartUpload_LowRes,
        StartUpload_MediumRes,
        StopUpload,
        None
    }
}