
#pragma once

#include <string>
#include <map>

struct StreamInstanceSettings
{
	int Port;
	std::string TargetHost;
};

class StreamLoggingSettings
{
public:
	StreamLoggingSettings( const std::string & sourceFile );

	void AddStreamSettings( int cameraIndex, const std::string & targetHost, int port );

	int GetStreamPort( int cameraIndex );
	std::string GetStreamTargetHost( int cameraIndex );

private:
	std::map<int, StreamInstanceSettings> m_Settings;
	std::string m_ConfigFile;

	void FillMissingSettingsForCameraIndex( int cameraIndex );

	void LoadFromFile( const std::string & sourceFile );
	void SaveToFile( const std::string & targetFile );
};