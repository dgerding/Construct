
#include "StreamLoggingSettings.h"

#include <fstream>
#include <sstream>

bool utilFileExists( const std::string & file )
{
	std::ifstream fileStream( file );
	return fileStream.is_open( );
}

StreamLoggingSettings::StreamLoggingSettings( const std::string & sourceFile )
{
	if( utilFileExists( sourceFile ) )
	{
		LoadFromFile( sourceFile );
	}
	else
	{
		//	If it doesn't exist, at least create an empty template
		SaveToFile( sourceFile );
	}

	m_ConfigFile = sourceFile;
}

void StreamLoggingSettings::AddStreamSettings( int cameraIndex, const std::string & targetHost, int port )
{
	if( m_Settings.find( cameraIndex ) != m_Settings.end( ) )
		throw;

	StreamInstanceSettings instanceSettings;
	instanceSettings.Port = port;
	instanceSettings.TargetHost = targetHost;
	m_Settings[cameraIndex] = instanceSettings;

	SaveToFile( m_ConfigFile );
}

int StreamLoggingSettings::GetStreamPort( int cameraIndex )
{
	FillMissingSettingsForCameraIndex( cameraIndex );

	return m_Settings[cameraIndex].Port;
}

std::string StreamLoggingSettings::GetStreamTargetHost( int cameraIndex )
{
	FillMissingSettingsForCameraIndex( cameraIndex );

	return m_Settings[cameraIndex].TargetHost;
}

void StreamLoggingSettings::FillMissingSettingsForCameraIndex( int cameraIndex )
{
	if( m_Settings.find( cameraIndex ) != m_Settings.end( ) )
		return;

	AddStreamSettings( cameraIndex, "localhost", 65565 );
}

void StreamLoggingSettings::LoadFromFile( const std::string & sourceFile )
{
	std::ifstream fileStream( sourceFile );
	while( !fileStream.eof( ) )
	{
		std::string currentLine;
		std::getline( fileStream, currentLine, '\n' );

		if( currentLine.size( ) == 0 )
			continue;

		if( currentLine[0] == ';' )
			continue;

		//	Parse
		int cameraIndex;
		( std::istringstream( currentLine.substr( 0, currentLine.find_first_of( ' ' ) ) ) ) >> cameraIndex;
		currentLine = currentLine.substr( currentLine.find_first_of( ' ' ) + 1 );

		std::string targetHost = currentLine.substr( 0, currentLine.find_first_of( ' ' ) );
		currentLine = currentLine.substr( currentLine.find_first_of( ' ' ) );

		int port;
		( std::istringstream( currentLine ) ) >> port;
	}
}

void StreamLoggingSettings::SaveToFile( const std::string & sourceFile )
{
	std::ofstream fileStream( sourceFile, std::ios::trunc );
	fileStream << "; CameraIndex TargetHost TargetPort (Only one space between each, no prefixed or trailing spaces)\n";
	for( auto & streamInstancePair : m_Settings )
	{
		fileStream << streamInstancePair.first << " " << streamInstancePair.second.TargetHost << " " << streamInstancePair.second.Port << "\n";
	}
	fileStream.close();
}