
#pragma once

#include <sm_api.h>
#include <list>
#include <mutex>

//	Note: This is a singleton because right now every instance of FaceRecognizer
//		stands-up and tears-down the FaceAPI framework at every Start() and Stop().
//		If you want to have multiple FaceRecognizers, decouple the API startup from
//		the engine code. (Check FaceRecognizer::Start.)
class FaceRecognizer
{
public:
	static FaceRecognizer * Instance( );

	void Start( );
	void Stop( );
	void RestartTracking( );

	void SetMinimumConfidence( float minimumConfidence );
	float GetMinimumConfidence( ) const;

	bool HeadPoseDataAvailable( );
	smEngineHeadPoseData GetNextHeadPose( );

	bool IsCommercial( ) const;

	bool HasError( ) const;
	std::string GetErrorString( );

protected:
	std::list <smEngineHeadPoseData> m_HeadPoseQueue;
	std::mutex m_HeadPoseQueueMutex;

	std::string m_LastError;

	smEngineHandle m_EngineHandle;

	float m_MinimumConfidence;

	void SetErrorString( const std::string & text );

	friend void __stdcall OnNewHeadPose( void *, smEngineHeadPoseData, smCameraVideoFrame );

private:
	FaceRecognizer( );
	~FaceRecognizer( );
};