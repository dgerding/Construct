
#pragma once

#include <sm_api.h>
#include <string>

class NetworkedFaceStream
{
public:
	NetworkedFaceStream( smEngineHandle sourceEngine );
	~NetworkedFaceStream( );

	void StartStream( int targetPort, const std::string & targetHostOrIp );
	void StopStream( );

private:
	smEngineHandle m_EngineHandle;
};