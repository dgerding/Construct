using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    public struct EmotivItemData
    {
        #region RawEEGData
        // <summary>
        /// field for property LeftPrefrontal
        /// </summary>
        public  float RawLeftPrefrontal;
        /// <summary>
        /// field for property LeftFrontalA
        /// </summary>
        public float RawLeftFrontalA;
        /// <summary>
        /// field for property LeftFrontalB
        /// </summary>
        public float RawLeftFrontalB;
        /// <summary>
        /// field for property LeftCenterFrontal
        /// </summary>
        public float RawLeftCenterFrontal;
        /// <summary>
        /// field for property LeftTemporal
        /// </summary>
        public float RawLeftTemporal;
        /// <summary>
        /// field for property LeftParietal
        /// </summary>
        public float RawLeftParietal;
        /// <summary>
        /// field for property LeftOccipital
        /// </summary>
        public float RawLeftOccipital;
        /// <summary>
        /// field for property RightOccipital
        /// </summary>
        public float RawRightOccipital;
        /// <summary>
        /// field for property RightParietal
        /// </summary>
        public float RawRightParietal;
        /// <summary>
        /// field for property RightTemporal
        /// </summary>
        public float RawRightTemporal;
        /// <summary>
        /// field for property RightCenterFrontal
        /// </summary>
        public float RawRightCenterFrontal;
        /// <summary>
        /// field for property RightFrontalB
        /// </summary>
        public float RawRightFrontalB;
        /// <summary>
        /// field for property RightFrontalA
        /// </summary>
        public float RawRightFrontalA;
        /// <summary>
        /// field for property RightPrefrontal
        /// </summary>
        public float RawRightPrefrontal;
        /// <summary>
        /// field for property Counter
        /// </summary>
        public double RawCounter;
        /// <summary>
        /// field for property Interpolated
        /// </summary>
        public double RawInterpolated;
        /// <summary>
        /// field for property RawCQ
        /// </summary>
        public double RawRawCQ;
        /// <summary>
        /// field for property GyroX
        /// </summary>
        public float RawGyroX;
        /// <summary>
        /// field for property GyroY
        /// </summary>
        public float RawGyroY;
        /// <summary>
        /// field for property TimeStamp
        /// </summary>
        public float RawTimeStamp;
        /// <summary>
        /// field for property EsTimeStamp
        /// </summary>
        public float RawEsTimeStamp;
        /// <summary>
        /// field for property FuncId
        /// </summary>
        public float RawFuncId;
        /// <summary>
        /// field for property FuncValue
        /// </summary>
        public float RawFuncValue;
        /// <summary>
        /// field for property Marker
        /// </summary>
        public float RawMarker;
        /// <summary>
        /// field for property SyncSignal
        /// </summary>
        public float RawSyncSignal;

        #endregion

        #region AffectivData
        /// <summary>
        /// field for property Equal
        /// </summary>
        public bool AffectivEqual;
        /// <summary>
        /// field for property EngagementBordomLevel
        /// </summary>
        public float AffectivEngagementBordomLevel;
        /// <summary>
        /// field for property LongTermExcitement
        /// </summary>
        public float AffectivLongTermExcitement;
        /// <summary>
        /// field for property ShortTermExcitement
        /// </summary>
        public float AffectivShortTermExcitement;
        /// <summary>
        /// field for property FrustrationScore
        /// </summary>
        public float AffectivFrustrationScore;
        /// <summary>
        /// field for property MeditationScore
        /// </summary>
        public float AffectivMeditationScore;


        #endregion

        #region CognitivData

        /// <summary>
        /// field for property CurrentAction
        /// </summary>
        public string CognitivCurrentAction;
        /// <summary>
        /// field for property Equal
        /// </summary>
        public bool CognitivEqual;
        /// <summary>
        /// field for property CurrentActionPower
        /// </summary>
        public float CognitivCurrentActionPower;

        #endregion

        #region ExpressivData

        /// <summary>
        /// field for property Equal
        /// </summary>
        public bool ExpressivEqual;
        /// <summary>
        /// field for property ClenchExtent
        /// </summary>
        public float ExpressivClenchExtent;
        /// <summary>
        /// field for property EyebrowExtent
        /// </summary>
        public float ExpressivEyebrowExtent;
        /// <summary>
        /// field for property EyelidStateLeftEye
        /// </summary>
        public float ExpressivEyelidStateLeftEye;
        /// <summary>
        /// field for property EyelidStateRightEye
        /// </summary>
        public float ExpressivEyelidStateRightEye;
        /// <summary>
        /// field for property EyeLocationX
        /// </summary>
        public float ExpressivEyeLocationX;
        /// <summary>
        /// field for property EyeLocationY
        /// </summary>
        public float ExpressivEyeLocationY;
        /// <summary>
        /// field for property LowerFaceAction
        /// </summary>
        public string ExpressivLowerFaceAction;
        /// <summary>
        /// field for property LowerFaceActionPower
        /// </summary>
        public float ExpressivLowerFaceActionPower;
        /// <summary>
        /// field for property SmileExtent
        /// </summary>
        public float ExpressivSmileExtent;
        /// <summary>
        /// field for property UpperFaceAction
        /// </summary>
        public string ExpressivUpperFaceAction;
        /// <summary>
        /// field for property UpperFaceActionPower
        /// </summary>
        public float ExpressivUpperFaceActionPower;
        /// <summary>
        /// field for property Blink
        /// </summary>
        public bool ExpressivBlink;
        /// <summary>
        /// field for property EyeOpen
        /// </summary>
        public bool ExpressivEyeOpen;
        /// <summary>
        /// field for property LookDown
        /// </summary>
        public bool ExpressivLookDown;
        /// <summary>
        /// field for property LookUp
        /// </summary>
        public bool ExpressivLookUp;
        /// <summary>
        /// field for property LookLeft
        /// </summary>
        public bool ExpressivLookLeft;
        /// <summary>
        /// field for property LookRight
        /// </summary>
        public bool ExpressivLookRight;
        /// <summary>
        /// field for property LeftWink
        /// </summary>
        public bool ExpressivLeftWink;
        /// <summary>
        /// field for property RightWink
        /// </summary>
        public bool ExpressivRightWink;

        #endregion

    }
}
