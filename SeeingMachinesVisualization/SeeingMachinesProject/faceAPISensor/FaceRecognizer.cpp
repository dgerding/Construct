
#include <Windows.h>
#include <sm_api.h>
#include "FaceRecognizer.h"
#include "smString.h"
#include <iostream>
#include <cstdlib>
#include <sstream>


typedef std::lock_guard<std::mutex> Lock;




void __stdcall OnNewHeadPose(void * userData, smEngineHeadPoseData headPose, smCameraVideoFrame videoFrame )
{
	auto faceRecognizer = (FaceRecognizer *)userData;

	if( headPose.confidence < faceRecognizer->m_MinimumConfidence )
		return;

	{
		Lock lock( faceRecognizer->m_HeadPoseQueueMutex );
		faceRecognizer->m_HeadPoseQueue.push_back( headPose );
	}
}




#define SafeAPICall(code,description)											\
{																				\
	smReturnCode result = (code);												\
	if (result < 0)																\
	{																			\
		std::stringstream s;													\
		s << "FaceAPI error occurred during: " << description;					\
		s << "\nAPI error code: " << result;									\
		SetErrorString( s.str( ) );												\
		return;																	\
	}																			\
}




FaceRecognizer * FaceRecognizer::Instance( )
{
	static FaceRecognizer * fr = nullptr;
	if( fr == nullptr )
		fr = new FaceRecognizer( );

	return fr;
}

void FaceRecognizer::Start( )
{
	//sm::String outputPath( "./log.txt" );
	//smLoggingSetPath(outputPath.GetSmString( ));
	
	if( m_EngineHandle != nullptr )
	{
		SetErrorString( "Attempted to start FaceRecognizer when FaceRecognizer was already started." );
		return;
	}

	SafeAPICall(
		smAPIInit( ),
		"Initializing FaceAPI"
		);

	//	Replace with SM_API_CAMERA_TYPE_IMAGE_PUSH if we want to just
	//		provide our own video feed
	SafeAPICall(
		smCameraRegisterType( SM_API_CAMERA_TYPE_WDM ),
		"Registering FaceAPI for WDM cameras"
		);

	SafeAPICall(
		smEngineCreate( SM_API_ENGINE_LATEST_HEAD_TRACKER, &m_EngineHandle ),
		"Generating the head-tracking engine"
		);

	SafeAPICall(
		smHTRegisterHeadPoseCallback( m_EngineHandle, FaceRecognizer::Instance( ), OnNewHeadPose ),
		"Registering for head-pose events"
		);
	
	SafeAPICall(
		smHTV2SetRestartPoorTrackingEnabled( m_EngineHandle, SM_API_TRUE ),
		"Configuring auto-restart of tracking during poor tracking conditions"
		);

	SafeAPICall(
		smEngineStart( m_EngineHandle ),
		"Starting head tracking"
		);
}

void FaceRecognizer::Stop( )
{
	if( m_EngineHandle == nullptr )
	{
		SetErrorString( "Attempted to quit face recognition when it has not been started" );
		return;
	}

	SafeAPICall(
		smEngineDestroy( &m_EngineHandle ),
		"Destroying face-tracking engine"
		);

	SafeAPICall(
		smAPIQuit( ),
		"Quitting FaceAPI"
		);
}

void FaceRecognizer::RestartTracking( )
{
	if( m_EngineHandle == nullptr )
	{
		SetErrorString( "Attempted to restart face recognition when it has not been initially started" );
		return;
	}

	SafeAPICall(
		smEngineStart( m_EngineHandle ),
		"Restarting the tracking engine"
		);
}

bool FaceRecognizer::HeadPoseDataAvailable( )
{
	bool queueHasEntries;

	{
		Lock lock( m_HeadPoseQueueMutex );
		queueHasEntries = m_HeadPoseQueue.size( ) != 0;
	}

	return queueHasEntries;
}

FaceRecognizer::FaceRecognizer( )
{
	m_MinimumConfidence = 0.05F;
	m_EngineHandle = 0;
}

FaceRecognizer::~FaceRecognizer( )
{
}

bool FaceRecognizer::IsCommercial( ) const
{
	return smAPINonCommercialLicense( ) == SM_API_FALSE;
}

bool FaceRecognizer::HasError() const
{
	return m_LastError.length( ) != 0; 
}

std::string FaceRecognizer::GetErrorString( )
{
	std::string result = m_LastError;
	m_LastError = "";
	return result;
}

void FaceRecognizer::SetErrorString( const std::string & text )
{
	m_LastError = text;
}

smEngineHeadPoseData FaceRecognizer::GetNextHeadPose( )
{
	smEngineHeadPoseData result;
	
	{
		Lock lock( m_HeadPoseQueueMutex );
		result = m_HeadPoseQueue.front( );
		m_HeadPoseQueue.pop_front( );
	}

	return result;
}
