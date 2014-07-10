using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    struct TelemetryData
    {
        #region AffectivData

        /// <summary>
        /// field for property isOn
        /// </summary>
        public bool AffectivIsOn;


        #endregion

        #region CognitivData

        /// <summary>
        /// field for property IsOn
        /// </summary>
        public bool CognitivIsOn;

        #endregion

        #region ExpressivData

        /// <summary>
        /// field for property IsOn
        /// </summary>
        public bool ExpressivIsOn;

        #endregion

        #region HardwareData

        /// <summary>
        /// field for property BatterChargeLevel
        /// </summary>
        public int HardwareBatteryChargeLevel;
        /// <summary>
        /// field for property ContactQuality
        /// </summary>
        public string HardwareContactQuality;
        /// <summary>
        /// field for property AllContactQuality
        /// </summary>
        public string HardwareAllContactQuality;
        /// <summary>
        /// field for property DuplicateState
        /// </summary>
        public object HardwareDuplicateState;
        /// <summary>
        /// field for property EngineEqual
        /// </summary>
        public bool HardwareEngineEqual;
        /// <summary>
        /// field for property EmotivEquals
        /// </summary>
        public bool HardwareEmotivEquals;
        /// <summary>
        /// field for property GetHandle
        /// </summary>
        public int HardwareGetHandle;
        /// <summary>
        /// field for property HeadsetIsOn
        /// </summary>
        public int HardwareHeadsetIsOn;
        /// <summary>
        /// field for property NumContactQualityChannels
        /// </summary>
        public int HardwareNumContactQualityChannels;
        /// <summary>
        /// field for property TimeFromStart
        /// </summary>
        public float HardwareTimeFromStart;
        /// <summary>
        /// field for property WirelessSignalStatus
        /// </summary>
        public string HardwareWirelessSignalStatus;
        /// <summary>
        /// field for property MaxBatteryChargeLevel
        /// </summary>
        public int HardwareMaxBatteryChargeLevel;

        #endregion
    }
}
