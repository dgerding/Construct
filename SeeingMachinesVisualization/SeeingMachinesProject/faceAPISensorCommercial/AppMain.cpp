
#include <sm_api.h>
#include <SFML/Network.hpp>
#include <string>
#include <iostream>
#include <sstream>
#include "FaceRecognizer.h"

//	Note that this application is 64-bit ONLY.

/* http://support.microsoft.com/kb/99261 */
 void cls( HANDLE hConsole, COORD startPos, int count )
 {
	COORD coordScreen = startPos;    /* here's where we'll home the
										cursor */ 
	DWORD cCharsWritten;
	DWORD dwConSize;                 /* number of character cells in
										the current buffer */ 

	dwConSize = count;

	FillConsoleOutputCharacter( hConsole, (TCHAR) ' ',
	   dwConSize, coordScreen, &cCharsWritten );

	SetConsoleCursorPosition( hConsole, coordScreen );
	return;
 }

template <typename T>
T WithPrecision( T value, int decimalPrecision )
{
	return value;
	//return (int)(value * decimalPrecision) / (T)(decimalPrecision);
}

std::ostream & operator<<( std::ostream & stream, const smEngineData & data )
{
	//	Use this to prevent flushing to screen during output
	std::ostringstream bufferStream;

	stream << "numPeople: " << data.num_people << "\n\n";
	for( int i = 0; i < data.num_people; i++ )
	{
		stream << "--- Person " << i + 1 << "\n";
		char outputBuffer[1024];
		smEnginePersonData * currentPerson = data.people + i;

		int prec = 3;

		if( currentPerson->head_pose_data != nullptr )
		{
			smEngineHeadPoseData * headPose = currentPerson->head_pose_data;
			sprintf_s( outputBuffer,
				"confidence: %f\n"
				"headPos: (%f, %f, %f)\n"
				"leftEye: (%f, %f, %f)\n"
				"rightEye: (%f, %f, %f)\n"
				"headRot: (%f, %f, %f)\n",
				headPose->confidence,
				headPose->head_pos.x,		headPose->head_pos.y,		headPose->head_pos.z,
				headPose->left_eye_pos.x,	headPose->left_eye_pos.y,	headPose->left_eye_pos.z,
				headPose->right_eye_pos.x,	headPose->right_eye_pos.y,	headPose->right_eye_pos.z,
				headPose->head_rot.x_rads,	headPose->head_rot.y_rads,	headPose->head_rot.z_rads
				);

			bufferStream << outputBuffer;
		}

		if( currentPerson->eye_data != nullptr )
		{
			smEngineEyeData * currentEyeData = currentPerson->eye_data;

			if( currentEyeData->closure_data != nullptr )
			{
				smEngineEyeClosureData * closureData = currentEyeData->closure_data;
				sprintf_s( outputBuffer,
					"isBlinking: %s\n"
					"eyeClosures: (%f, %f)\n"
					"confidence: %f\n",
					closureData->blinking ? "true" : "false",
					closureData->closure[0],
					closureData->closure[1]
					);

				bufferStream << outputBuffer;
			}

			//	These are just checks - no such data from faceAPI
			if( currentEyeData->gaze_data != nullptr ) {
				smEngineGazeData * gazeData = currentEyeData->gaze_data;
				bufferStream << "Got Gaze Data!\n";
			}
			if( currentEyeData->pupil_data != nullptr ) {
				smEnginePupilData * pupilData = currentEyeData->pupil_data;
				bufferStream << "Got Pupil Data!\n";
			}
		}

		if( currentPerson->face_data != nullptr )
		{
			smEngineFaceData * faceData = currentPerson->face_data;

			std::string wearingGlassesString;
			switch( faceData->wearing_glasses ) {
				case( SM_API_WEARING_GLASSES_NO ): {
					wearingGlassesString = "No"; break;
				}
			
				case( SM_API_WEARING_GLASSES_YES ): {
					wearingGlassesString = "Yes"; break;
				}

				case( SM_API_WEARING_GLASSES_UNKNOWN ): {
					wearingGlassesString = "Unknown"; break;
				}
			}

			sprintf_s( outputBuffer,
				"wearingGlasses: %s\n"
				"numLandmarks: %i\n",
				wearingGlassesString.c_str( ),
				faceData->num_landmarks
				);
			bufferStream << outputBuffer;
		}
	}

	stream << bufferStream.str( );
	return stream;
}

#define MSG_QUIT 1000
#define MSG_CHANGE_DATA_PORT 1001

int SensorMain(int argc, char * argv[])
{
	std::cout << "SensorMain" << std::endl;

	/* First arg is logging port, second arg is comms port. */
	int logPort, communicationPort;
	logPort = atoi( argv[1] );
	communicationPort = atoi( argv[2] );

	if( logPort == 0 || communicationPort == 0 )
		return EXIT_FAILURE;

	sf::SocketUDP commsSocket;
	if( !commsSocket.Bind( communicationPort ) )
		std::cin.get( );


	FaceRecognizer * recognizer = FaceRecognizer::Instance( );
	recognizer->SetDataQueueing( false );
	recognizer->Start( );
	recognizer->StartLogging( logPort, "127.0.0.1" );

	//	Only ever pass around ints
	char messageBuffer[4];

	bool continueProcessing = true;
	while( continueProcessing )
	{
		std::size_t receivedSize;
		unsigned short remotePort;
		if( sf::Socket::Done == commsSocket.Receive( messageBuffer, sizeof(messageBuffer), receivedSize, sf::IPAddress(), remotePort ) )
		{
			int message = *(int *)messageBuffer;
			switch( message )
			{
			case( MSG_QUIT ):
				{
					continueProcessing = false;
					break;
				}

			default:
				{
					std::cout << "Warning: Message ID " << message << " is not supported" << std::endl;
					break;
				}
			}
		}
		Sleep(100);
	}

	recognizer->Stop();

	std::cin.get( );

	return EXIT_SUCCESS;
}

int DebugMain( int argc, char * argv[] )
{
	std::cout << "DebugMain" << std::endl;

	HWND consoleWindow = GetConsoleWindow( );

	auto fr = FaceRecognizer::Instance( );
	fr->SetDataQueueing( true );
	fr->Start( );
	std::cout << "\n\n\n\n\n";

	HANDLE stdOut = GetStdHandle( STD_OUTPUT_HANDLE );
	CONSOLE_SCREEN_BUFFER_INFO csbi;
	GetConsoleScreenBufferInfo( stdOut, &csbi );
	COORD start = csbi.dwCursorPosition;

	bool prevRState = false;
	while( !GetAsyncKeyState( VK_ESCAPE ) )
	{
		if( fr->HasError( ) )
			std::cout << fr->GetErrorString( ) << std::endl;

		while( fr->HeadPoseDataAvailable( ) )
		{
			cls( stdOut, start, 100 );
			smEngineData newHeadPose = fr->GetNextHeadPose( );
			std::cout << newHeadPose << std::endl;
			smEngineDataDestroy( &newHeadPose );
			continue;
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

/* Yes, this sensor is done strangely (C++ and C# apps for faceAPI even though it all could've been done
	in C#) and it's for strange reasons as well. Capability-wise, there's nothing wrong with an all-C#
	rewrite of this. C# would be even better.
	*/
int main( int argc, char * argv[] )
{
	std::cout << "ArgCount: " << argc << std::endl;

	if( argc == 3 )
		return SensorMain( argc, argv );
	else
		return DebugMain( argc, argv );
}