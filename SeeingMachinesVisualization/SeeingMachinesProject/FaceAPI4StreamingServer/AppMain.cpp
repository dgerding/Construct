
//	Note: Needs to be built in x64 (FaceAPI 4.0 lib is only x64)

#include <sm_api.h>
#include <iostream>

#include <list>

#include "StreamLoggingSettings.h"
#include "NetworkedFaceStream.h"

std::ostream & operator<<( std::ostream & stream, smStringHandle stringHandle )
{
	char buffer[512];
	smStringWriteBuffer( stringHandle, buffer, sizeof( buffer ) );
	stream << buffer;
	return stream;
}

int GetCameraFormatIndexWithLargestResolution( smCameraInfo * sourceCamera )
{
	int largestResolutionIndex = 0;
	for( int i = 1; i < sourceCamera->num_formats; i++ )
	{
		smCameraVideoFormat & currentLargestFormat = sourceCamera->formats[largestResolutionIndex];
		smCameraVideoFormat & currentFormat = sourceCamera->formats[i];

		int largestTotalPixels = currentLargestFormat.res.w * currentLargestFormat.res.h;
		int currentTotalPixels = currentFormat.res.w * currentFormat.res.h;

		if( currentTotalPixels > largestTotalPixels )
			largestResolutionIndex = i;
	}

	return largestResolutionIndex;
}

int main( )
{
	//	When you've ran for the first time the logging-settings.cfg file will be generated for the existing cameras. Close
	//		the app, modify the cfg, and re-run. Test your settings by running the visualizer and make modifications/restart
	//		when you need to change the port/target for a camera.

	std::cout << "FaceAPI 4.0 Multi-Camera Streaming Server" << std::endl;

	StreamLoggingSettings loggingSettings( "logging-settings.cfg" );

	std::list<NetworkedFaceStream *> faceStreams;

	smAPIInit( );
	smCameraRegisterType( SM_API_CAMERA_TYPE_WDM );

	smCameraInfoList cameraList;
	smCameraSettings settings;
	settings.approx_fov_deg = nullptr;
	settings.lens_params = nullptr;
	settings.format_index = nullptr;
	smCameraCreateInfoList( &cameraList );
	
	std::cout << cameraList.num_cameras << " cameras detected" << std::endl;

	//	Create an engine for each attached camera
	for( int i = 0; i < cameraList.num_cameras; i++ )
	{
		smCameraInfo * currentInfo = cameraList.info + i;
		std::cout << i << "> " << currentInfo->model << std::endl;

		int targetFormatIndex = GetCameraFormatIndexWithLargestResolution( currentInfo );
		settings.format_index = new int( targetFormatIndex );

		smCameraHandle cameraHandle;
		smCameraCreate( currentInfo, &settings, &cameraHandle );

		smEngineHandle newEngineHandle;
		smEngineCreateWithCamera( SM_API_ENGINE_LATEST_HEAD_TRACKER, cameraHandle, &newEngineHandle );

		NetworkedFaceStream * faceStream = new NetworkedFaceStream( newEngineHandle );
		faceStream->StartStream( loggingSettings.GetStreamPort( currentInfo->instance_index ), loggingSettings.GetStreamTargetHost( currentInfo->instance_index ) );

		faceStreams.push_back( faceStream );
	}

	std::cout << "Generated camera streams, press Enter at any time to close." << std::endl;

	std::cin.get( );

	for( auto stream : faceStreams )
	{
		stream->StopStream( );
		delete stream;
	}

	smAPIQuit( );

	return 0;
}