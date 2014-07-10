using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sm.eod;
using System.Collections;

namespace Construct.Sensors.FaceLabSensor
{
    struct ConstructDataType
    {
        public sbyte EngineStateOutputData_ObjectID;
        public int EngineStateOutputData_State;

        public float EyeOutputData_EyeClosureOutputData_AverageBlinkDuration;
        public float EyeOutputData_EyeClosureOutputData_BlinkFrequency;
        public bool EyeOutputData_EyeClosureOutputData_Blinking;
        public float[] EyeOutputData_EyeClosureOutputData_EyeClosure;
        public float EyeOutputData_EyeClosureOutputData_EyeClosureCalibStatus;
        public float[] EyeOutputData_EyeClosureOutputData_EyeClosureConfidence;
        public sbyte EyeOutputData_EyeClosureOutputData_ObjectID;
        public float EyeOutputData_EyeClosureOutputData_Perclos;

        public IEnumerator[] EyeOutputData_GazeOutputData_EyeballCentre_GetEnumerator;
        public bool EyeOutputData_GazeOutputData_GazeCalibrated;
        public GazeQualityLevel[] EyeOutputData_GazeOutputData_GazeQualityLevel;

        public enum GazeQualityLevel
        {
            NO_TRACKING = 0,
            HEAD_DIRECTION = 1,
            NON_IR_GAZE = 2,
            IR_GAZE = 3,
            NUM_GAZE_QUALITY_LEVELS = 4,
        }
        public IEnumerator[] EyeOutputData_GazeOutputData_GazeRotation_GetEnumerator;
        public sbyte EyeOutputData_GazeOutputData_ObjectID;
        public bool EyeOutputData_GazeOutputData_Saccade;
        public sbyte EyeOutputData_ObjectID;
        public sbyte EyeOutputData_PupilOutputData_ObjectID;
        public float[] EyeOutputData_PupilOutputData_PupilDiameter;
        public IEnumerator[] EyeOutputData_PupilOutputData_PupilPosition_GetEnumerator;
        
        public float[] FaceOutputData_FaceLandmarks_Get_fc_x;
        public float[] FaceOutputData_FaceLandmarks_Get_fc_y;
        public float[] FaceOutputData_FaceLandmarks_Get_fc_z;
        public float[] FaceOutputData_FaceLandmarks_Get_ftc_u;
        public float[] FaceOutputData_FaceLandmarks_Get_ftc_v;
        public int[] FaceOutputData_FaceLandmarks_Get_id;
        public uint FaceOutputData_FaceLandmarks_Size;

        public float[] FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_x;
        public float[] FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_y;
        public float[] FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_fc_z;
        public float[] FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_ftc_u;
        public float[] FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_ftc_v;
        public int[] FaceOutputData_FaceTexture_FaceMaskLandmarks_Get_id;
        public uint FaceOutputData_FaceTexture_FaceMaskLandmarks_Size;
        
        public uint FaceOutputData_FaceTexture_FaceTexture_BytesPerLine;
        public uint FaceOutputData_FaceTexture_FaceTexture_Height;
        public int FaceOutputData_FaceTexture_FaceTexture_ImageFormat;
        public string FaceOutputData_FaceTexture_FaceTexture_ImageIdentifier;
        public bool FaceOutputData_FaceTexture_FaceTexture_IsSharingMemory;
        public uint FaceOutputData_FaceTexture_FaceTexture_NumDataBytes;
        public sbyte FaceOutputData_FaceTexture_FaceTexture_ObjectID;
        public IntPtr FaceOutputData_FaceTexture_FaceTexture_RawData;
        public uint FaceOutputData_FaceTexture_FaceTexture_Width;

        public FaceTextureType FaceOutputData_FaceTexture_GetFaceTextureType;
        public sbyte FaceOutputData_FaceTexture_ObjectID;

        public enum FaceTextureType
        {
            UNKNOWN = -1,
            ORTHOGRAPHIC_PROJECTION = 0,
        }



        public sbyte FaceOutputData_ObjectID;

        public uint FaceOutputDataV2_FaceCount;
        
        public WearingGlassesType FaceOutputDataV2_WearingGlasses;
        public enum WearingGlassesType
        {
            WEARING_GLASSES_NO = 0,
            WEARING_GLASSES_YES = 1,
            WEARING_GLASSES_UNKNOWN = 2,
        }

        public enum EyeId
        {
            RIGHT_EYE = 0,
            LEFT_EYE = 1,
            BOTH_EYES = 2,
        }

        public uint GPSOutputData_GmtTime;
        public bool GPSOutputData_IsDataValid;
        public short GPSOutputData_Latitude_degrees;
        public char GPSOutputData_Latitude_hemisphere;
        public sbyte GPSOutputData_Latitude_minutes;
        public sbyte GPSOutputData_Latitude_seconds;

        public short GPSOutputData_Longitude_degrees;
        public char GPSOutputData_Longitude_hemisphere;
        public sbyte GPSOutputData_Longitude_minutes;
        public sbyte GPSOutputData_Longitude_seconds;
        
        public sbyte GPSOutputData_ObjectID;
        public short GPSOutputData_Speed;

        public float GPSOutputDataV2_Course;
        public uint GPSOutputDataV2_GmtTime;
        public bool GPSOutputDataV2_IsDataValid;
        public short GPSOutputDataV2_Latitude_degrees;
        public char GPSOutputDataV2_Latitude_hemisphere;
        public sbyte GPSOutputDataV2_Latitude_minutes;
        public float GPSOutputDataV2_Latitude_seconds;

        public short GPSOutputDataV2_Longitude_degrees;
        public char GPSOutputDataV2_Longitude_hemisphere;
        public sbyte GPSOutputDataV2_Longitude_minutes;
        public float GPSOutputDataV2_Longitude_seconds;

        public sbyte GPSOutputDataV2_ObjectID;
        public float GPSOutputDataV2_Speed;

        public IEnumerator[] HeadOutputData_HeadEyeBallPos_GetEnumerator;
        public IEnumerator HeadOutputData_HeadPosition_GetEnumerator;
        public double HeadOutputData_HeadPositionConfidence;
        public IEnumerator HeadOutputData_HeadRotation_GetEnumerator;
        public sbyte HeadOutputData_ModelQualityLevel;
        public sbyte HeadOutputData_ObjectID;

        public IEnumerator HeadOutputDataV2_HeadRotationQuaternion_GetEnumerator;

        public uint ImageCollectionOutputData_Image_BytesPerLine;
        public uint ImageCollectionOutputData_Image_Height;
        public int ImageCollectionOutputData_Image_ImageFormat;
        public string ImageCollectionOutputData_Image_ImageIdentifier;
        public bool ImageCollectionOutputData_Image_IsSharingMemory;
        public uint ImageCollectionOutputData_Image_NumDataBytes;
        public sbyte ImageCollectionOutputData_Image_ObjectID;
        public IntPtr ImageCollectionOutputData_Image_RawData;
        public uint ImageCollectionOutputData_Image_Width;
        
        public ushort ImageCollectionOutputData_NumImages;
        public sbyte ImageCollectionOutputData_ObjectID;

        public enum ImageFormatType
        {
            RGB_24U = 21,
            GRAY_8U = 808466521,
        }

        public float InertiaOutputData_AccelX;
        public float InertiaOutputData_AccelY;
        public float InertiaOutputData_AccelZ;
        public float InertiaOutputData_AngularRateX;
        public float InertiaOutputData_AngularRateY;
        public float InertiaOutputData_AngularRateZ;
        public sbyte InertiaOutputData_ObjectID;

        public float VergencePoint_X;
        public float VergencePoint_Y;
        public float VergencePoint_Z;

        public int TimingOutputData_AnnotationLabelID;
        public float TimingOutputData_ApproxDelay;
        public double TimingOutputData_ExperimentTime;
        public ushort TimingOutputData_FrameTimeMSecs;
        public int TimingOutputData_FrameTimeSecs;
        public sbyte TimingOutputData_ObjectID;

        public sbyte WorldOutputData_HeadIntersectionOutputData_ObjectID;
        public sbyte WorldOutputData_ObjectID;
    }
}
