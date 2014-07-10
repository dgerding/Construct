using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    class DataCollector
    {
        /// <summary>
        /// Provides EmoEngine for data aquisition.
        /// </summary>
        EmoEngine engine;
        /// <summary>
        /// Emotiv headset user ID.
        /// </summary>
        int userID = -1;
        /// <summary>
        /// Dictionary relates EEG Data Channels to Double Array values.
        /// </summary>
        Dictionary<EdkDll.EE_DataChannel_t, double[]> data = null;
        /// <summary>
        /// Size of headset data buffer.
        /// </summary>
        int bufferSize = 0;
        /// <summary>
        /// RawEEGData information.
        /// </summary>
        RawEEGData rawData = new RawEEGData();
        /// <summary>
        /// ExpressivData information.
        /// </summary>
        ExpressivData expressiv = new ExpressivData();
        /// <summary>
        /// CognitivData information.
        /// </summary>
        CognitivData cognitiv = new CognitivData();
        /// <summary>
        /// AffectivData information.
        /// </summary>
        AffectivData affectiv = new AffectivData();
        /// <summary>
        /// HardwareData information.
        /// </summary>
        HardwareData hardware = new HardwareData();

        public EmotivItemData emotivItemData = new EmotivItemData();
        public TelemetryData emotivTelemetryData = new TelemetryData();

        public DataCollector()
        {
            engine = EmoEngine.Instance;
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);

            engine.Connect();
            //engine.RemoteConnect("127.0.0.1", 1726);

            //Comment engine.Connect to disable live headset data, uncomment engine.RemoteConnect to allow
            //emoComposer to send data to sensor.

            engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(engine_EmoStateUpdated);
        }
        /// <summary>
        /// When user added event fires: Sets user ID; Enable data stream; Get data.
        /// </summary>
        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            userID = (int)e.userId;
            //Record the user

            //Enable data streaming for the user
            engine.DataAcquisitionEnable((uint)userID, true);

            //Ask for 1 second of buffered data
            engine.EE_DataSetBufferSizeInSec(1);
        }

        /// <summary>
        /// When engine update event fires; Get current data, Update data inside of structs.
        /// </summary>
        void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;
            ExpressivData(es);
            CognitivData(es);
            AffectivData(es);
            HardwareData(es);
        }

        /// <summary>
        /// Sets rawData values.
        /// </summary>
        public void GatherRawData()
        {
            engine.ProcessEvents();
            //Verfies that a user is connected
            if ((int)userID == -1)
                return;

            //Populate dictionary with user data
            data = engine.GetData((uint)userID);

            //If no data exists exit
            if (data == null)
                return;

            bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;
            int i = 0;
            rawData.Counter = data[EdkDll.EE_DataChannel_t.COUNTER][i];
            rawData.Interpolated = data[EdkDll.EE_DataChannel_t.INTERPOLATED][i];
            rawData.RawCQ = data[EdkDll.EE_DataChannel_t.RAW_CQ][i];
            rawData.LeftPrefrontal = (float)data[EdkDll.EE_DataChannel_t.AF3][i];
            rawData.LeftFrontalA = (float)data[EdkDll.EE_DataChannel_t.F7][i];
            rawData.LeftFrontalB = (float)data[EdkDll.EE_DataChannel_t.F3][i];
            rawData.LeftCenterFrontal = (float)data[EdkDll.EE_DataChannel_t.FC5][i];
            rawData.LeftTemporal = (float)data[EdkDll.EE_DataChannel_t.T7][i];
            rawData.LeftParietal = (float)data[EdkDll.EE_DataChannel_t.P7][i];
            rawData.LeftOccipital = (float)data[EdkDll.EE_DataChannel_t.O1][i];
            rawData.RightOccipital = (float)data[EdkDll.EE_DataChannel_t.O2][i];
            rawData.RightParietal = (float)data[EdkDll.EE_DataChannel_t.P8][i];
            rawData.RightTemporal = (float)data[EdkDll.EE_DataChannel_t.T8][i];
            rawData.RightCenterFrontal = (float)data[EdkDll.EE_DataChannel_t.FC6][i];
            rawData.RightFrontalB = (float)data[EdkDll.EE_DataChannel_t.F4][i];
            rawData.RightFrontalA = (float)data[EdkDll.EE_DataChannel_t.F8][i];
            rawData.RightPrefrontal = (float)data[EdkDll.EE_DataChannel_t.AF4][i];
            rawData.GyroX = (float)data[EdkDll.EE_DataChannel_t.GYROX][i];
            rawData.GyroY = (float)data[EdkDll.EE_DataChannel_t.GYROY][i];
            rawData.TimeStamp = (float)data[EdkDll.EE_DataChannel_t.TIMESTAMP][i];
            rawData.EsTimeStamp = (float)data[EdkDll.EE_DataChannel_t.ES_TIMESTAMP][i];
            rawData.FuncId = (float)data[EdkDll.EE_DataChannel_t.FUNC_ID][i];
            rawData.FuncValue = (float)data[EdkDll.EE_DataChannel_t.FUNC_VALUE][i];
            rawData.Marker = (float)data[EdkDll.EE_DataChannel_t.MARKER][i];
            rawData.SyncSignal = (float)data[EdkDll.EE_DataChannel_t.SYNC_SIGNAL][i];
            data = null;
            
        }
        /// <summary>
        /// Sets expressiv values.
        /// </summary>
        void ExpressivData(EmoState estate)
        {
            float leftEye = 0, rightEye = 0, eyeX = 0, eyeY = 0;
            expressiv.Equal = estate.ExpressivEqual(estate);
            expressiv.ClenchExtent = estate.ExpressivGetClenchExtent();
            expressiv.EyebrowExtent = estate.ExpressivGetEyebrowExtent();
            estate.ExpressivGetEyelidState(out leftEye, out rightEye);
            expressiv.EyelidStateLeftEye = leftEye;
            expressiv.EyelidStateRightEye = rightEye;
            estate.ExpressivGetEyeLocation(out eyeX, out eyeY);
            expressiv.EyeLocationX = eyeX;
            expressiv.EyeLocationY = eyeY;
            expressiv.LowerFaceAction = estate.ExpressivGetLowerFaceAction().ToString("G");
            expressiv.LowerFaceActionPower = estate.ExpressivGetLowerFaceActionPower();
            expressiv.SmileExtent = estate.ExpressivGetSmileExtent();
            expressiv.UpperFaceAction = estate.ExpressivGetUpperFaceAction().ToString("G");
            expressiv.UpperFaceActionPower = estate.ExpressivGetUpperFaceActionPower();
            expressiv.IsOn = estate.ExpressivIsActive(EdkDll.EE_ExpressivAlgo_t.EXP_NEUTRAL);
            expressiv.Blink = estate.ExpressivIsBlink();
            expressiv.EyeOpen = estate.ExpressivIsEyesOpen();
            expressiv.LeftWink = estate.ExpressivIsLeftWink();
            expressiv.LookDown = estate.ExpressivIsLookingDown();
            expressiv.LookLeft = estate.ExpressivIsLookingLeft();
            expressiv.LookRight = estate.ExpressivIsLookingRight();
            expressiv.LookUp = estate.ExpressivIsLookingUp();
            expressiv.RightWink = estate.ExpressivIsRightWink();
        }
        /// <summary>
        /// Sets cognitiv values.
        /// </summary>
        void CognitivData(EmoState estate)
        {
            cognitiv.CurrentAction = estate.CognitivGetCurrentAction().ToString("G");
            cognitiv.CurrentActionPower = estate.CognitivGetCurrentActionPower();
            cognitiv.Equal = estate.CognitivEqual(estate);
            cognitiv.IsOn = estate.CognitivIsActive();
        }
        /// <summary>
        /// Sets affectiv values.
        /// </summary>
        void AffectivData(EmoState estate)
        {
            affectiv.Equal = estate.AffectivEqual(estate);
            affectiv.EngagementBordomLevel = estate.AffectivGetEngagementBoredomScore();
            affectiv.LongTermExcitement = estate.AffectivGetExcitementLongTermScore();
            affectiv.ShortTermExcitement = estate.AffectivGetExcitementShortTermScore();
            affectiv.FrustrationScore = estate.AffectivGetFrustrationScore();
            affectiv.MeditationScore = estate.AffectivGetMeditationScore();
            affectiv.IsOn = estate.AffectivIsActive(EdkDll.EE_AffectivAlgo_t.AFF_ENGAGEMENT_BOREDOM);
        }
        /// <summary>
        /// Sets hardware values.
        /// </summary>
        void HardwareData(EmoState estate)
        {
            int batteryCharge = 0, maxCharge = 0;
            estate.GetBatteryChargeLevel(out batteryCharge, out maxCharge);
            hardware.BatteryChargeLevel = batteryCharge;
            hardware.MaxBatteryChargeLevel = maxCharge;
            hardware.ContactQuality = estate.GetContactQuality(3).ToString("G");
            hardware.AllContactQuality = estate.GetContactQualityFromAllChannels().ToString();
            hardware.DuplicateState = estate.Clone();
            hardware.EngineEqual = estate.EmoEngineEqual(estate);
            hardware.EmotivEquals = estate.Equals(estate);
            hardware.GetHandle = (int)estate.GetHandle().ToInt32();
            hardware.HeadsetIsOn = estate.GetHeadsetOn();
            hardware.NumContactQualityChannels = estate.GetNumContactQualityChannels();
            hardware.TimeFromStart = estate.GetTimeFromStart();
            hardware.WirelessSignalStatus = estate.GetWirelessSignalStatus().ToString("G");
        }


        
        /// <summary>
        /// Gets ExpressivData values.
        /// </summary>
        public ExpressivData Expressiv
        {
            get
            {
                return expressiv;
            }
        }
        /// <summary>
        /// Gets RawData values.
        /// </summary>
        public RawEEGData RawData
        {
            get
            {
                return rawData;
            }
        }
        /// <summary>
        /// Gets CognitivData values.
        /// </summary>
        public CognitivData Cognitiv
        {
            get
            {
                return cognitiv;
            }
        }
        /// <summary>
        /// Gets AffectivData values.
        /// </summary>
        public AffectivData Affectiv
        {
            get
            {
                return affectiv;
            }
        }
        /// <summary>
        /// Gets HardwareData values.
        /// </summary>
        public HardwareData Hardware
        {
            get
            {
                return hardware;
            }
        }

        public void PopulateEmotivItemData()
        {
            emotivItemData.RawCounter = rawData.Counter;
            emotivItemData.RawEsTimeStamp = rawData.EsTimeStamp;
            emotivItemData.RawFuncId = rawData.FuncId;
            emotivItemData.RawFuncValue = rawData.FuncValue;
            emotivItemData.RawGyroX = rawData.GyroX;
            emotivItemData.RawGyroY = rawData.GyroY;
            emotivItemData.RawInterpolated = rawData.Interpolated;
            emotivItemData.RawLeftCenterFrontal = rawData.LeftCenterFrontal;
            emotivItemData.RawLeftFrontalA = rawData.LeftFrontalA;
            emotivItemData.RawLeftFrontalB = rawData.LeftFrontalB;
            emotivItemData.RawLeftOccipital = rawData.LeftOccipital;
            emotivItemData.RawLeftParietal = rawData.LeftParietal;
            emotivItemData.RawLeftPrefrontal = rawData.LeftPrefrontal;
            emotivItemData.RawLeftTemporal = rawData.LeftTemporal;
            emotivItemData.RawMarker = rawData.Marker;
            emotivItemData.RawRawCQ = rawData.RawCQ;
            emotivItemData.RawRightCenterFrontal = rawData.RightCenterFrontal;
            emotivItemData.RawRightFrontalA = rawData.RightFrontalA;
            emotivItemData.RawRightFrontalB = rawData.RightFrontalB;
            emotivItemData.RawRightOccipital = rawData.RightOccipital;
            emotivItemData.RawRightParietal = rawData.RightParietal;
            emotivItemData.RawRightPrefrontal = rawData.RightPrefrontal;
            emotivItemData.RawRightTemporal = rawData.RightTemporal;
            emotivItemData.RawSyncSignal = rawData.SyncSignal;
            emotivItemData.RawTimeStamp = rawData.TimeStamp;

            emotivItemData.AffectivEngagementBordomLevel = affectiv.EngagementBordomLevel;
            emotivItemData.AffectivEqual = affectiv.Equal;
            emotivItemData.AffectivFrustrationScore = affectiv.FrustrationScore;
            emotivItemData.AffectivLongTermExcitement = affectiv.LongTermExcitement;
            emotivItemData.AffectivMeditationScore = affectiv.MeditationScore;
            emotivItemData.AffectivShortTermExcitement = affectiv.ShortTermExcitement;

            emotivItemData.CognitivCurrentAction = cognitiv.CurrentAction;
            emotivItemData.CognitivCurrentActionPower = cognitiv.CurrentActionPower;
            emotivItemData.CognitivEqual = cognitiv.Equal;

            emotivItemData.ExpressivBlink = expressiv.Blink;
            emotivItemData.ExpressivClenchExtent = expressiv.ClenchExtent;
            emotivItemData.ExpressivEqual = expressiv.Equal;
            emotivItemData.ExpressivEyebrowExtent = expressiv.EyebrowExtent;
            emotivItemData.ExpressivEyelidStateLeftEye = expressiv.EyelidStateLeftEye;
            emotivItemData.ExpressivEyelidStateRightEye = expressiv.EyelidStateRightEye;
            emotivItemData.ExpressivEyeLocationX = expressiv.EyeLocationX;
            emotivItemData.ExpressivEyeLocationY = expressiv.EyeLocationY;
            emotivItemData.ExpressivEyeOpen = expressiv.EyeOpen;
            emotivItemData.ExpressivLeftWink = expressiv.LeftWink;
            emotivItemData.ExpressivLookDown = expressiv.LookDown;
            emotivItemData.ExpressivLookLeft = expressiv.LookLeft;
            emotivItemData.ExpressivLookRight = expressiv.LookRight;
            emotivItemData.ExpressivLookUp = expressiv.LookUp;
            emotivItemData.ExpressivLowerFaceAction = expressiv.LowerFaceAction;
            emotivItemData.ExpressivLowerFaceActionPower = expressiv.LowerFaceActionPower;
            emotivItemData.ExpressivRightWink = expressiv.RightWink;
            emotivItemData.ExpressivSmileExtent = expressiv.SmileExtent;
            emotivItemData.ExpressivUpperFaceAction = expressiv.UpperFaceAction;
            emotivItemData.ExpressivUpperFaceActionPower = expressiv.UpperFaceActionPower;

        }

        public void PopulateEmotivTelemetryData()
        {
            emotivTelemetryData.AffectivIsOn = affectiv.IsOn;
            emotivTelemetryData.CognitivIsOn = cognitiv.IsOn;
            emotivTelemetryData.ExpressivIsOn = expressiv.IsOn;
            emotivTelemetryData.HardwareAllContactQuality = hardware.AllContactQuality;
            emotivTelemetryData.HardwareBatteryChargeLevel = hardware.BatteryChargeLevel;
            emotivTelemetryData.HardwareContactQuality = hardware.ContactQuality;
            emotivTelemetryData.HardwareDuplicateState = hardware.DuplicateState;
            emotivTelemetryData.HardwareEmotivEquals = hardware.EmotivEquals;
            emotivTelemetryData.HardwareEngineEqual = hardware.EngineEqual;
            emotivTelemetryData.HardwareGetHandle = hardware.GetHandle;
            emotivTelemetryData.HardwareHeadsetIsOn = hardware.HeadsetIsOn;
            emotivTelemetryData.HardwareMaxBatteryChargeLevel = hardware.MaxBatteryChargeLevel;
            emotivTelemetryData.HardwareNumContactQualityChannels = hardware.NumContactQualityChannels;
            emotivTelemetryData.HardwareTimeFromStart = hardware.TimeFromStart;
            emotivTelemetryData.HardwareWirelessSignalStatus = hardware.WirelessSignalStatus;
        }
    }
}
