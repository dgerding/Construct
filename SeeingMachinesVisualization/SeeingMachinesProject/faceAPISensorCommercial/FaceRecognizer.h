
#pragma once

#include <sm_api.h>
#include <list>
#include <mutex>
#include <thread>

//	Note: This is a singleton because right now every instance of FaceRecognizer
//		stands-up and tears-down the FaceAPI framework at every Start() and Stop().
//		If you want to have multiple FaceRecognizers, decouple the API startup from
//		the engine code. (Check FaceRecognizer::Start.) Note that a single FaceRecognizer
//		is not restricted in the number of faces it can track at a time; you can
//		track i.e. 3 faces at the same time with a single FaceRecognizer.
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
	smEngineData GetNextHeadPose( );

	bool IsCommercial( ) const;

	bool HasError( ) const;
	std::string GetErrorString( );

	void StartLogging( int targetPort, const std::string & target );
	void StopLogging( );

	void SetDataQueueing( bool shouldQueue );

protected:
	bool m_QueueData;
	std::list <smEngineData> m_HeadDataQueue;
	std::mutex m_HeadDataQueueMutex;

	std::string m_LastError;

	bool m_ContinueQuery;
	std::thread * m_QueryThread;

	smEngineHandle m_EngineHandle;

	float m_MinimumConfidence;

	void SetErrorString( const std::string & text );

	friend void HeadPoseQueryThread( FaceRecognizer * );

private:
	FaceRecognizer( );
	~FaceRecognizer( );
};