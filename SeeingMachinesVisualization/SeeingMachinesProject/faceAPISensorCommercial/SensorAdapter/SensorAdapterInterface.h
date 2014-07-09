
#pragma once

#ifdef _DEBUG
# define LIBRARY_INTERFACE
#else
# define LIBRARY_INTERFACE extern "C" __declspec(dllexport)
#endif

class FaceRecognizer;

LIBRARY_INTERFACE void FaceRecognizer_Start( );
LIBRARY_INTERFACE void FaceRecognizer_Stop( );

LIBRARY_INTERFACE void FaceRecognizer_RestartTracking( );

LIBRARY_INTERFACE bool FaceRecognizer_HeadPoseDataAvailable( );
LIBRARY_INTERFACE void FaceRecognizer_AdvanceCurrentHeadPose( );

#pragma region Head Pose Accessors
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_HeadX( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_HeadY( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_HeadZ( );

LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_LeftEyeX( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_LeftEyeY( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_LeftEyeZ( );

LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_RightEyeX( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_RightEyeY( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_RightEyeZ( );

LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansX( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansY( );
LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansZ( );

LIBRARY_INTERFACE float FaceRecognizer_GetCurrentHeadPose_Confidence( );
#pragma endregion