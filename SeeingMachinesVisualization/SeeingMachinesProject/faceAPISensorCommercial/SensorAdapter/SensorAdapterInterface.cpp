
#include "../FaceRecognizer.h"
#include "SensorAdapterInterface.h"

smEngineHeadPoseData g_CurrentHeadPose;

#define FloatAlias(aliasName, aliasImpl)			\
	LIBRARY_INTERFACE float aliasName ( )			\
	{												\
		return aliasImpl;							\
	}

LIBRARY_INTERFACE void FaceRecognizer_Start( )
{
	FaceRecognizer::Instance( )->Start( );
}

LIBRARY_INTERFACE void FaceRecognizer_RestartTracking( )
{
	FaceRecognizer::Instance( )->RestartTracking( );
}

LIBRARY_INTERFACE void FaceRecognizer_Stop( )
{
	FaceRecognizer::Instance( )->Stop( );
}

LIBRARY_INTERFACE bool FaceRecognizer_HeadPoseDataAvailable( )
{
	return FaceRecognizer::Instance( )->HeadPoseDataAvailable( );
}

LIBRARY_INTERFACE void FaceRecognizer_AdvanceCurrentHeadPose( )
{
	g_CurrentHeadPose = FaceRecognizer::Instance( )->GetNextHeadPose( );
}

#pragma region HeadPose Accessors
FloatAlias( FaceRecognizer_GetCurrentHeadPose_HeadX, g_CurrentHeadPose.head_pos.x );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_HeadY, g_CurrentHeadPose.head_pos.y );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_HeadZ, g_CurrentHeadPose.head_pos.z );

FloatAlias( FaceRecognizer_GetCurrentHeadPose_LeftEyeX, g_CurrentHeadPose.left_eye_pos.x );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_LeftEyeY, g_CurrentHeadPose.left_eye_pos.y );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_LeftEyeZ, g_CurrentHeadPose.left_eye_pos.z );

FloatAlias( FaceRecognizer_GetCurrentHeadPose_RightEyeX, g_CurrentHeadPose.right_eye_pos.x );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_RightEyeY, g_CurrentHeadPose.right_eye_pos.y );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_RightEyeZ, g_CurrentHeadPose.right_eye_pos.z );

FloatAlias( FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansX, g_CurrentHeadPose.head_rot.x_rads );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansY, g_CurrentHeadPose.head_rot.y_rads );
FloatAlias( FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansZ, g_CurrentHeadPose.head_rot.z_rads );

FloatAlias( FaceRecognizer_GetCurrentHeadPose_Confidence, g_CurrentHeadPose.confidence );
#pragma endregion

#pragma region Head