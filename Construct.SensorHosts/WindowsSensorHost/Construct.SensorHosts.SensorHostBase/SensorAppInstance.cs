using System;
using System.Diagnostics;

namespace Construct.SensorManagers.SensorManagerBase
{
    public class SensorAppInstance 
    {
        public SensorAppInstance(Guid theSourceTypeID, Guid theSourceID)
        {
            SensorUri = null;
            ProcessRef = null;
            TypeSourceID = theSourceTypeID;
            SourceID = theSourceID;
            IsSensorProcessLoaded = false;
            IsWcfServiceRunning = false;
        }
        public string SensorUri { get; set; }
        public Process ProcessRef { get; set; }
        public Guid TypeSourceID { get; set; }
        public Guid SourceID { get; set; }
        public bool IsSensorProcessLoaded { get; set; }
        public bool IsWcfServiceRunning { get; set; }
    }
}
