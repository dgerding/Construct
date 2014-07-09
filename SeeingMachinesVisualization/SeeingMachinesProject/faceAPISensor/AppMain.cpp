
#include <sm_api.h>
#include <string>
#include <iostream>
#include "FaceRecognizer.h"
#include "SensorAdapter/SensorAdapterInterface.h"

/* Built for v3.2, the faceAPI version that is currently released publicly for noncommercial
	use (there is no public-facing C# library for faceAPI 3.2 at the time of writing [11/8/2013]) */

#pragma comment (lib, "faceAPI3.2/smft32.lib")

std::ostream & operator<<( std::ostream & stream, const smEngineHeadPoseData & data )
{
	stream.precision(4);
	stream.setf(std::ios::fixed, std::ios::floatfield);
	stream <<
		"[Confidence: " << data.confidence << "] " <<
		"[HeadPos: " << data.head_pos.x << "," << data.head_pos.y << "," << data.head_pos.z << "] " <<
		"[LeftEye: " << data.left_eye_pos.x << "," << data.left_eye_pos.y << "," << data.left_eye_pos.z << "] " <<
		"[RightEye: " << data.right_eye_pos.x << "," << data.right_eye_pos.y << "," << data.right_eye_pos.z << "] ";
	stream.unsetf(std::ios::floatfield);
	return stream;
}

#ifdef _DEBUG

/* Only used for testing */
int main( )
{
	auto fr = FaceRecognizer::Instance( );
	fr->Start( );

	bool prevRState = false;
	while( !GetAsyncKeyState( VK_ESCAPE ) )
	{
		if( fr->HasError( ) )
			std::cout << fr->GetErrorString( ) << std::endl;

		while( fr->HeadPoseDataAvailable( ) )
		{
			smEngineHeadPoseData newHeadPose = fr->GetNextHeadPose( );
			std::cout << newHeadPose << "\n";
		}

		bool currentRState = GetAsyncKeyState( 'R' ) != 0;
		if( currentRState && !prevRState )
		{
			std::cout << "Attempting to restart tracking..." << std::endl;
			fr->RestartTracking( );
			std::cout << "Restarted tracking." << std::endl;
		}

		prevRState = currentRState;
		Sleep( 20 );
	}

	fr->Stop( );

	return 0;
}

#endif