
#include <Windows.h>
#include <sm_api.h>
#include "FaceRecognizer.h"
#include <iostream>
#include <cstdlib>
#include <sstream>


std::ostream & operator<<( std::ostream & stream, const smImageCode & code )
{
	switch( code )
	{
	case( SM_API_IMAGECODE_ARGB_32U ):	stream << "ARGB_32U"; break;
	case( SM_API_IMAGECODE_BGRA_32U ):	stream << "BGRA_32U"; break;
	case( SM_API_IMAGECODE_BGR_24U ):	stream << "BGR_24U"; break;
	case( SM_API_IMAGECODE_GRAY_16U ):	stream << "GRAY_16U"; break;
	case( SM_API_IMAGECODE_GRAY_8U ):	stream << "GRAY_8U"; break;
	case( SM_API_IMAGECODE_I420 ):		stream << "I420"; break;
	case( SM_API_IMAGECODE_RGB_24U ):	stream << "RGB_24U"; break;
	case( SM_API_IMAGECODE_YUY2 ):		stream << "YUY2"; break;
	default:							stream << "Unknown Video Mode"; break;
	}

	return stream;
}



std::ostream & operator<<( std::ostream & stream, const smCameraVideoFormat & videoFormat )
{
	stream << videoFormat.format << " | " << videoFormat.framerate << "Hz | " << videoFormat.res.w << "x" << videoFormat.res.h;
	return stream;
}


std::ostream & operator<<( std::ostream & stream, const smCameraInfoList & infoList )
{
	stream << "smCameraInfoList:\n\tnum_cameras: " << infoList.num_cameras << std::endl;

	for( int i = 0; i < infoList.num_cameras; i++ )
	{
		auto camera = infoList.info + i;
		char stringBuffer[1024];

		smStringWriteBuffer( camera->model, stringBuffer, sizeof( stringBuffer ) );
		stream << "\nCam " << i+1 << "\tcamera->model: " << stringBuffer;
		
		//	Output supported formats
		for( int j = 0; j < camera->num_formats; j++ )
			stream << "\n\tcamera->formats[" << j << "]: " << camera->formats[j];

		stream << std::endl;
	}

	return stream;
}


typedef std::lock_guard<std::mutex> Lock;


void HeadPoseQueryThread( FaceRecognizer * faceRecognizer )
{
	smEngineData data;

	while( faceRecognizer->m_ContinueQuery )
	{
		smReturnCode result = smEngineDataWaitNext( faceRecognizer->m_EngineHandle, &data, 10 );
		if( result == SM_API_FAIL_TIMEOUT )
			continue;

		{
			Lock lock( faceRecognizer->m_HeadDataQueueMutex );
			faceRecognizer->m_HeadDataQueue.push_back( data );
		}
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

	smCameraInfoList cameraList;
	smCameraSettings settings;
	settings.approx_fov_deg = nullptr;
	settings.lens_params = nullptr;
	//settings.format_index = new int( 12 ); // Hard-coded index, gotten from my own Microsoft LifeCam after outputting the available settings
	settings.format_index = nullptr;
	smCameraCreateInfoList( &cameraList );
	std::cout << cameraList;
	smCameraHandle cameraHandle;
	smCameraCreate( cameraList.info + 0, &settings, &cameraHandle );

	if( cameraList.num_cameras == 0 )
	{
		SetErrorString( "No compatible cameras detected (is it in use?)" );
		return;
	}

	SafeAPICall(
		smEngineCreateWithCamera( SM_API_ENGINE_LATEST_HEAD_TRACKER, cameraHandle, &m_EngineHandle ),
		"Generating the head-tracking engine"
		);

	//smEngineSetUDPLoggingEnabled( m_EngineHandle, true );
	//smEngineSetUDPOutputAddress( m_EngineHandle, nullptr, nullptr ); // Fill out


	//	Disable since we want ALL data we can get
	smEngineSetRealtimeTracking( m_EngineHandle, false );

	smHTSetLipTrackingEnabled( m_EngineHandle, SM_API_TRUE );
	smHTSetEyebrowTrackingEnabled( m_EngineHandle, SM_API_TRUE );
	smHTSetEyeClosureTrackingEnabled( m_EngineHandle, SM_API_TRUE );

	SafeAPICall(
		smEngineStart( m_EngineHandle ),
		"Starting head tracking"
		);

	if( m_QueueData )
	{
		m_ContinueQuery = true;
		m_QueryThread = new std::thread( HeadPoseQueryThread, this );
	}
}

void FaceRecognizer::Stop( )
{
	if( m_EngineHandle == nullptr )
	{
		SetErrorString( "Attempted to quit face recognition when it has not been started" );
		return;
	}

	if( m_QueryThread != nullptr )
	{
		m_ContinueQuery = false;
		m_QueryThread->join( );
		delete m_QueryThread;
		m_QueryThread = nullptr;
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
		Lock lock( m_HeadDataQueueMutex );
		queueHasEntries = m_HeadDataQueue.size( ) != 0;
	}

	return queueHasEntries;
}

FaceRecognizer::FaceRecognizer( )
{
	m_MinimumConfidence = 0.05F;
	m_EngineHandle = 0;
	m_ContinueQuery = false;
	m_QueryThread = nullptr;
	m_QueueData = true;
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

void FaceRecognizer::StartLogging( int targetPort, const std::string & target )
{
	smStringHandle targetString;
	smStringCreate( &targetString );
	//	WARNING: CASTING AWAY CONSTNESS
	smStringWriteBuffer( targetString, (char *)target.c_str( ), target.length( ) + 1 );

	smEngineSetUDPOutputAddress( m_EngineHandle, targetString, targetPort );
	smEngineSetUDPLoggingEnabled( m_EngineHandle, SM_API_TRUE );
}

void FaceRecognizer::StopLogging( )
{
	smEngineSetUDPLoggingEnabled( m_EngineHandle, SM_API_FALSE );
}

void FaceRecognizer::SetDataQueueing( bool shouldQueue )
{
	//	Just data setter, needs stop/start to apply
	m_QueueData = shouldQueue;
}

void FaceRecognizer::SetErrorString( const std::string & text )
{
	m_LastError = text;
}

smEngineData FaceRecognizer::GetNextHeadPose( )
{
	smEngineData result;
	
	{
		Lock lock( m_HeadDataQueueMutex );
		result = m_HeadDataQueue.front( );
		m_HeadDataQueue.pop_front( );
	}

	return result;
}
