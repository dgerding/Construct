using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    public struct HardwareData
    {
        /// <summary>
        /// field for property BatterChargeLevel
        /// </summary>
        private int batteryChargeLevel;
        /// <summary>
        /// field for property ContactQuality
        /// </summary>
        private string contactQuality;
        /// <summary>
        /// field for property AllContactQuality
        /// </summary>
        private string allContactQuality;
        /// <summary>
        /// field for property DuplicateState
        /// </summary>
        private object duplicateState;
        /// <summary>
        /// field for property EngineEqual
        /// </summary>
        private bool engineEqual;
        /// <summary>
        /// field for property EmotivEquals
        /// </summary>
        private bool emotivEquals;
        /// <summary>
        /// field for property GetHandle
        /// </summary>
        private int getHandle;
        /// <summary>
        /// field for property HeadsetIsOn
        /// </summary>
        private int headsetIsOn;
        /// <summary>
        /// field for property NumContactQualityChannels
        /// </summary>
        private int numContactQualityChannels;
        /// <summary>
        /// field for property TimeFromStart
        /// </summary>
        private float timeFromStart;
        /// <summary>
        /// field for property WirelessSignalStatus
        /// </summary>
        private string wirelessSignalStatus;
        /// <summary>
        /// field for property MaxBatteryChargeLevel
        /// </summary>
        private int maxBatteryChargeLevel;

        /// <summary>
        /// gets and sets the value of field batteryChargeLevel
        /// </summary>
        public int BatteryChargeLevel
        {
            get
            {
                return batteryChargeLevel;
            }
            set
            {
                batteryChargeLevel = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field contactQuality
        /// </summary>
        public string ContactQuality
        {
            get
            {
                return contactQuality;
            }
            set
            {
                contactQuality = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field allContactQuality
        /// </summary>
        public string AllContactQuality
        {
            get
            {
                return allContactQuality;
            }
            set
            {
                allContactQuality = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field duplicateState
        /// </summary>
        public object DuplicateState
        {
            get
            {
                return duplicateState;
            }
            set
            {
                duplicateState = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field engineEqual
        /// </summary>
        public bool EngineEqual
        {
            get
            {
                return engineEqual;
            }
            set
            {
                engineEqual = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field emotivEquals
        /// </summary>
        public bool EmotivEquals
        {
            get
            {
                return emotivEquals;
            }
            set
            {
                emotivEquals = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field getHandle
        /// </summary>
        public int GetHandle
        {
            get
            {
                return getHandle;
            }
            set
            {
                getHandle = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field headsetIsOn
        /// </summary>
        public int HeadsetIsOn
        {
            get
            {
                return headsetIsOn;
            }
            set
            {
                headsetIsOn = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field numContactQualityChannels
        /// </summary>
        public int NumContactQualityChannels
        {
            get
            {
                return numContactQualityChannels;
            }
            set
            {
                numContactQualityChannels = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field timeFromStart
        /// </summary>
        public float TimeFromStart
        {
            get
            {
                return timeFromStart;
            }
            set
            {
                timeFromStart = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field wirelessSignalStatus
        /// </summary>
        public string WirelessSignalStatus
        {
            get
            {
                return wirelessSignalStatus;
            }
            set
            {
                wirelessSignalStatus = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field maxBatteryChargeLevel
        /// </summary>
        public int MaxBatteryChargeLevel
        {
            get
            {
                return maxBatteryChargeLevel;
            }
            set
            {
                maxBatteryChargeLevel = value;
            }
        }
    }
}

