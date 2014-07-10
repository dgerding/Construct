using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Construct.Sensors.FaceLabSensor
{
    struct ReformattedConstructDataType
    {
        public IntPtr 
            FaceOutputData_FaceTexture_FaceTexture_RawData,
            ImageCollectionOutputData_Image_RawData,
            ImageOutputData_RawData,
            FaceTextureOutputData_FaceTexture_RawData,
            FaceOutputDataV2_FaceTexture_FaceTexture_RawData;
        public char 
            GPSOutputData_Latitude_hemisphere,
            GPSOutputData_Longitude_hemisphere,
            GPSOutputDataV2_Latitude_hemisphere,
            GPSOutputDataV2_Longitude_hemisphere;
        public double 
            HeadOutputDataV2_HeadPositionConfidence,
            TimingOutputData_ExperimentTime;
        public ushort 
            ImageCollectionOutputData_NumImages,
            TimingOutputData_FrameTimeMSecs;
        public int 
            WorldOutputData_HeadIntersectionOutputData_IntersectionObjectIndex,
            TimingOutputData_FrameTimeSecs,
            TimingOutputData_AnnotationLabelID,
            IRPodStateOutputData_State,
            IntersectionOutputBase_IntersectionObjectIndex,
            ImageOutputData_ImageFormat,
            ImageCollectionOutputData_Image_ImageFormat,
            HeadTrackerStateOutputData_State,
            HeadIntersectionOutputDataV1_IntersectionObjectIndex,
            GazeIntersectionOutputDataV2_IntersectionObjectIndex,
            FaceTextureOutputData_FaceTexture_ImageFormat,
            EngineStateOutputData_State,
            EngineStateOutputData_IrPodStateOutputData_State,
            EngineStateOutputData_GetHeadTrackerStateOutputData_State,
            FaceOutputData_FaceTexture_FaceTexture_ImageFormat,
            FaceOutputDataV2_FaceTexture_FaceTexture_ImageFormat,
            GazeIntersectionOutputDataV1_IntersectionObjectIndex;
        public uint 
            ImageOutputData_Width,
            ImageOutputData_NumDataBytes,
            ImageOutputData_BytesPerLine,
            ImageOutputData_Height,
            ImageCollectionOutputData_Image_Width,
            ImageCollectionOutputData_Image_NumDataBytes,
            ImageCollectionOutputData_Image_BytesPerLine,
            ImageCollectionOutputData_Image_Height,
            GPSOutputDataV2_GmtTime,
            GPSOutputData_GmtTime,
            FaceTextureOutputData_FaceTexture_Width,
            FaceTextureOutputData_FaceTexture_NumDataBytes,
            FaceTextureOutputData_FaceTexture_BytesPerLine,
            FaceTextureOutputData_FaceTexture_Height,
            FaceTextureOutputData_FaceMaskLandmarks_Size,
            FaceOutputDataV2_FaceTexture_FaceTexture_Width,
            FaceOutputDataV2_FaceTexture_FaceTexture_NumDataBytes,
            FaceOutputDataV2_FaceTexture_FaceTexture_BytesPerLine,
            FaceOutputDataV2_FaceTexture_FaceTexture_Height,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Size,
            FaceOutputDataV2_FaceLandmarks_Size,
            FaceOutputDataV2_FaceCount,
            FaceOutputData_FaceTexture_FaceTexture_Width,
            FaceOutputData_FaceTexture_FaceTexture_NumDataBytes,
            FaceOutputData_FaceLandmarks_Size,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Size,
            FaceOutputData_FaceTexture_FaceTexture_BytesPerLine,
            FaceOutputData_FaceTexture_FaceTexture_Height;
        public short 
            GPSOutputDataV2_Longitude_degrees,
            GPSOutputDataV2_Latitude_degrees,
            GPSOutputData_Speed,
            GPSOutputData_Longitude_degrees,
            GPSOutputData_Latitude_degrees;
        public sbyte 
            WorldOutputData_ObjectID,
            WorldOutputData_HeadIntersectionOutputData_ObjectID,
            TimingOutputData_ObjectID,
            IRPodStateOutputData_ObjectID,
            InertiaOutputData_ObjectID,
            ImageOutputData_ObjectID,
            ImageCollectionOutputData_ObjectID,
            ImageCollectionOutputData_Image_ObjectID,
            HeadTrackerStateOutputData_ObjectID,
            HeadOutputDataV2_ModelQualityLevel,
            HeadOutputDataV2_ObjectID,
            HeadOutputData_ModelQualityLevel,
            HeadOutputData_ObjectID,
            HeadOutputData_TrackingState,
            HeadIntersectionOutputDataV2_ObjectID,
            HeadIntersectionOutputDataV1_ObjectID,
            GPSOutputDataV2_ObjectID,
            GPSOutputDataV2_Longitude_minutes,
            GPSOutputDataV2_Latitude_minutes,
            GPSOutputData_ObjectID,
            GPSOutputData_Longitude_minutes,
            GPSOutputData_Longitude_seconds,
            GPSOutputData_Latitude_minutes,
            GPSOutputData_Latitude_seconds,
            GazeOutputData_ObjectID,
            GazeIntersectionOutputDataV4_ObjectID,
            GazeIntersectionOutputDataV2_ObjectID,
            GazeIntersectionOutputDataV1_ObjectID,
            FeatureOutputData_ObjectID,
            FaceTextureOutputData_ObjectID,
            FaceTextureOutputData_FaceTexture_ObjectID,
            FaceSetOutputData_ObjectID,
            FaceOutputDataV2_ObjectID,
            FaceOutputDataV2_FaceTexture_ObjectID,
            FaceOutputDataV2_FaceTexture_FaceTexture_ObjectID,
            FaceOutputData_ObjectID,
            FaceOutputData_FaceTexture_ObjectID,
            FaceOutputData_FaceTexture_FaceTexture_ObjectID,
            EyeOutputData_ObjectID,
            EyeOutputData_PupilOutputData_ObjectID,
            EyeOutputData_GazeOutputData_ObjectID,
            EyeOutputData_EyeClosureOutputData_ObjectID,
            EyeClosureOutputData_ObjectID,
            EngineStateOutputData_ObjectID,
            EngineStateOutputData_GetHeadTrackerStateOutputData_ObjectID,
            EngineStateOutputData_IrPodStateOutputData_ObjectID,
            WorldOutputDataV2_GazeIntersectionOutputData_ObjectID,
            WorldOutputDataV2_HeadIntersectionOutputData_ObjectID,
            WorldOutputDataV2_ObjectID;
        public float 
            TimingOutputData_ApproxDelay,
            InertiaOutputData_AccelX,
            InertiaOutputData_AccelY,
            InertiaOutputData_AccelZ,
            InertiaOutputData_AngularRateX,
            InertiaOutputData_AngularRateY,
            InertiaOutputData_AngularRateZ,
            HeadOutputData_HeadPositionConfidence,
            GPSOutputDataV2_Speed,
            GPSOutputDataV2_Longitude_seconds,
            GPSOutputDataV2_Latitude_seconds,
            GPSOutputDataV2_Course,
            EyeOutputData_EyeClosureOutputData_Perclos,
            EyeOutputData_EyeClosureOutputData_EyeClosureCalibStatus,
            EyeOutputData_EyeClosureOutputData_AverageBlinkDuration,
            EyeOutputData_EyeClosureOutputData_BlinkFrequency,
            EyeClosureOutputData_Perclos,
            EyeClosureOutputData_EyeClosureCalibStatus,
            EyeClosureOutputData_AverageBlinkDuration,
            EyeClosureOutputData_BlinkFrequency;
        public bool 
            WorldOutputData_HeadIntersectionOutputData_IntersectsScreen,
            IntersectionOutputBase_IntersectsScreen,
            ImageOutputData_IsSharingMemory,
            ImageCollectionOutputData_Image_IsSharingMemory,
            HeadOutputDataV2_IsTracking,
            HeadIntersectionOutputDataV1_IntersectsScreen,
            GPSOutputDataV2_IsDataValid,
            GPSOutputData_IsDataValid,
            GazeOutputData_Saccade,
            GazeOutputData_GazeCalibrated,
            GazeIntersectionOutputDataV2_IntersectsScreen,
            GazeIntersectionOutputDataV1_IntersectsScreen,
            FaceTextureOutputData_FaceTexture_IsSharingMemory,
            FaceOutputDataV2_FaceTexture_FaceTexture_IsSharingMemory,
            FaceOutputData_FaceTexture_FaceTexture_IsSharingMemory,
            EyeOutputData_GazeOutputData_Saccade,
            EyeOutputData_GazeOutputData_GazeCalibrated,
            EyeOutputData_EyeClosureOutputData_Blinking,
            EyeClosureOutputData_Blinking;
        public string 
            WorldOutputData_HeadIntersectionOutputData_IntersectionObjectName,
            IntersectionOutputBase_IntersectionObjectName,
            ImageOutputData_ImageIdentifier,
            ImageCollectionOutputData_Image_ImageIdentifier,
            HeadIntersectionOutputDataV1_IntersectionObjectName,
            GazeIntersectionOutputDataV2_IntersectionObjectName,
            GazeIntersectionOutputDataV1_IntersectionObjectName,
            FeatureOutputData_Name,
            FaceTextureOutputData_FaceTexture_ImageIdentifier,
            FaceOutputDataV2_FaceTexture_FaceTexture_ImageIdentifier,
            FaceOutputData_FaceTexture_FaceTexture_ImageIdentifier;
        public IEnumerator 
            WorldOutputData_HeadIntersectionOutputData_ScreenIntersectionPixelCoordinates_GetEnumerator,
            WorldOutputData_HeadIntersectionOutputData_ScreenIntersectionScreenCoordinates_GetEnumerator,
            WorldOutputData_HeadIntersectionOutputData_ScreenIntersectionWorldCoordinates_GetEnumerator,
            WorldOutputData_GazeIntersectionOutputData_GazeObjectIntersection_GetEnumerator,
            IntersectionOutputBase_ScreenIntersectionPixelCoordinates_GetEnumerator,
            IntersectionOutputBase_ScreenIntersectionScreenCoordinates_GetEnumerator,
            IntersectionOutputBase_ScreenIntersectionWorldCoordinates_GetEnumerator,
            HeadOutputDataV2_HeadRotation_GetEnumerator,
            HeadOutputDataV2_HeadRotationQuaternion_GetEnumerator,
            HeadOutputDataV2_HeadPosition_GetEnumerator,
            eadOutputData_HeadRotation_GetEnumerator,
            HeadOutputData_HeadPosition_GetEnumerator,
            HeadIntersectionOutputDataV1_ScreenIntersectionPixelCoordinates_GetEnumerator,
            HeadIntersectionOutputDataV1_ScreenIntersectionScreenCoordinates_GetEnumerator,
            HeadIntersectionOutputDataV1_ScreenIntersectionWorldCoordinates_GetEnumerator,
            GazeIntersectionOutputDataV2_ScreenIntersectionPixelCoordinates_GetEnumerator,
            GazeIntersectionOutputDataV1_ScreenIntersectionPixelCoordinates_GetEnumerator,
            GazeIntersectionOutputDataBase_GazeObjectIntersection_GetEnumerator,
            EngineStateOutputData_IrPodStateOutputData_Position_GetEnumerator,
            GazeIntersectionOutputDataV1_ScreenIntersectionScreenCoordinates_GetEnumerator,
            GazeIntersectionOutputDataV1_ScreenIntersectionWorldCoordinates_GetEnumerator,
            GazeIntersectionOutputDataV2_ScreenIntersectionScreenCoordinates_GetEnumerator,
            GazeIntersectionOutputDataV2_ScreenIntersectionWorldCoordinates_GetEnumerator,
            IntersectionOutputBaseV2_IntersectedItems_GetEnumerator,
            IRPodStateOutputData_Position_GetEnumerator;
        public float[] 
            FaceTextureOutputData_FaceMaskLandmarks_Get_fc_x,
            FaceTextureOutputData_FaceMaskLandmarks_Get_fc_y,
            FaceTextureOutputData_FaceMaskLandmarks_Get_fc_z,
            FaceTextureOutputData_FaceMaskLandmarks_Get_ftc_u,
            FaceTextureOutputData_FaceMaskLandmarks_Get_ftc_v,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Get_fc_x,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Get_fc_y,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Get_fc_z,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Get_ftc_u,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Get_ftc_v,
            FaceOutputDataV2_FaceLandmarks_Get_fc_x,
            FaceOutputDataV2_FaceLandmarks_Get_fc_y,
            FaceOutputDataV2_FaceLandmarks_Get_fc_z,
            FaceOutputDataV2_FaceLandmarks_Get_ftc_u,
            FaceOutputDataV2_FaceLandmarks_Get_ftc_v,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_x,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_y,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_z,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_ftc_u,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_ftc_v,
            FaceOutputData_FaceLandmarks_Get_fc_x,
            FaceOutputData_FaceLandmarks_Get_fc_y,
            FaceOutputData_FaceLandmarks_Get_fc_z,
            FaceOutputData_FaceLandmarks_Get_ftc_u,
            FaceOutputData_FaceLandmarks_Get_ftc_v,
            EyeOutputData_PupilOutputData_PupilDiameter,
            EyeOutputData_EyeClosureOutputData_EyeClosure,
            EyeOutputData_EyeClosureOutputData_EyeClosureConfidence,
            EyeClosureOutputData_EyeClosure,
            EyeClosureOutputData_EyeClosureConfidence;
        public int[] 
            FaceTextureOutputData_FaceMaskLandmarks_Get_id,
            FaceOutputDataV2_FaceTexture_FaceMaskLandmarks_Get_id,
            FaceOutputDataV2_FaceLandmarks_Get_id,
            FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_id,
            FaceOutputData_FaceLandmarks_Get_id;
        public IEnumerator[] 
            HeadOutputDataV2_HeadEyeBallPos_GetEnumerator,
            HeadOutputData_HeadEyeBallPos_GetEnumerator,
            GazeOutputData_GazeRotation_GetEnumerator,
            GazeOutputData_EyeballCentre_GetEnumerator,
            EyeOutputData_GazeOutputData_GazeRotation_GetEnumerator,
            EyeOutputData_PupilOutputData_PupilPosition_GetEnumerator,
            EyeOutputData_GazeOutputData_EyeballCentre_GetEnumerator;
        public enum GazeQualityLevel
        {
            NO_TRACKING = 0,
            HEAD_DIRECTION = 1,
            NON_IR_GAZE = 2,
            IR_GAZE = 3,
            NUM_GAZE_QUALITY_LEVELS = 4,
        }
        public GazeQualityLevel[] 
            EyeOutputData_GazeOutputData_GazeQualityLevel,
            GazeOutputData_GazeQualityLevel;
        public enum FaceTextureType
        {
            UNKNOWN = -1,
            ORTHOGRAPHIC_PROJECTION = 0,
        }
        public FaceTextureType 
            FaceOutputData_FaceTexture_GetFaceTextureType,
            FaceOutputDataV2_FaceTexture_GetFaceTextureType,
            FaceTextureOutputData_GetFaceTextureType;
        public enum WearingGlassesType
        {
            WEARING_GLASSES_NO = 0,
            WEARING_GLASSES_YES = 1,
            WEARING_GLASSES_UNKNOWN = 2,
        }
        public WearingGlassesType 
            FaceOutputDataV2_WearingGlasses;
        public enum EyeId
        {
            RIGHT_EYE = 0,
            LEFT_EYE = 1,
            BOTH_EYES = 2,
        }
        public EyeId 
            GazeIntersectionOutputDataV4_EyeId;
    }
}
