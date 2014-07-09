
#include "NetworkedFaceStream.h"

NetworkedFaceStream::NetworkedFaceStream( smEngineHandle sourceHandle )
{
	m_EngineHandle = sourceHandle;

	smHTSetLipTrackingEnabled( m_EngineHandle, SM_API_TRUE );
	smHTSetEyebrowTrackingEnabled( m_EngineHandle, SM_API_TRUE );
	smHTSetEyeClosureTrackingEnabled( m_EngineHandle, SM_API_TRUE );

	smEngineSetUDPLoggingEnabled( m_EngineHandle, SM_API_TRUE );
}

NetworkedFaceStream::~NetworkedFaceStream( )
{
	smEngineDestroy( &m_EngineHandle );
}

void NetworkedFaceStream::StartStream( int targetPort, const std::string & targetHostOrIp )
{
	smStringHandle targetString;
	smStringCreate( &targetString );
	smStringWriteBuffer( targetString, (char *)targetHostOrIp.c_str( ), targetHostOrIp.length( ) + 1 );

	smEngineSetUDPOutputAddress( m_EngineHandle, targetString, targetPort );

	smEngineStart( m_EngineHandle );
}

void NetworkedFaceStream::StopStream( )
{
	smEngineStop( m_EngineHandle );
}