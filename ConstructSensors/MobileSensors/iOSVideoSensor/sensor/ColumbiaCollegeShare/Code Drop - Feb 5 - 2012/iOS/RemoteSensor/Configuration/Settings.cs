using System;
using System.IO;
using MonoTouch.Foundation;

namespace RemoteSensor
{
	public static class Settings
	{
		private static bool debugEnabled;
		private static string webServiceUrl = DefaultWebServiceUrl;
		
		public enum CameraType
		{
			FrontFacing,
			RearFacing
		}

		private static CameraType camera = CameraType.FrontFacing;
		public static CameraType Camera 
		{
			get
			{
				return camera;	
			}
		}
		
		public static string DefaultWebServiceUrl
		{
			get
			{
				return "http://192.168.83.255:8000/SensorService/";		
			}
		}
		
		private enum SettingsKeyNames
		{
			WebServiceURL,	
			DebugEnabled,
			Camera
		}
		
		public static void ToggleCamera()
		{
			camera = (camera == CameraType.FrontFacing) ? CameraType.RearFacing : CameraType.FrontFacing;	
		}
		
		public static void Load()
		{
			try
			{
				// see if any defaults have been written to the plist file
				webServiceUrl = NSUserDefaults.StandardUserDefaults.StringForKey( SettingsKeyNames.WebServiceURL.ToString() );
				if ( string.IsNullOrEmpty( webServiceUrl ) == true )
				{
					Reset();
				}
				
				debugEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey( SettingsKeyNames.DebugEnabled.ToString()  );
				webServiceUrl = NSUserDefaults.StandardUserDefaults.StringForKey( SettingsKeyNames.WebServiceURL.ToString() );
				camera = (CameraType) NSUserDefaults.StandardUserDefaults.IntForKey( SettingsKeyNames.Camera.ToString() );
				
			}
			catch
			{
				Reset();
			}
		}
		
		public static void Save()
		{
			NSUserDefaults.StandardUserDefaults.SetString( webServiceUrl, SettingsKeyNames.WebServiceURL.ToString() );
			NSUserDefaults.StandardUserDefaults.SetBool( debugEnabled, SettingsKeyNames.DebugEnabled.ToString() );
			NSUserDefaults.StandardUserDefaults.SetInt( (int)camera, SettingsKeyNames.Camera.ToString() );
			
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}
		
		public static void Reset()
		{
			debugEnabled = true;
			webServiceUrl = DefaultWebServiceUrl;
			camera = CameraType.FrontFacing;

			Save();
		}
		
		public static string WebServiceUrl
		{
			get
			{
				return webServiceUrl;		
			}
			set{
				webServiceUrl = value;	
			}
		}
		
		private static string createDirectoryIfNeeded( string directory )
		{
			if (Directory.Exists(directory) == false)
			{
				Directory.CreateDirectory( directory );
			}
			return directory;
		}
		
		private static string myDocuments = null;
		public static string MyDocuments
		{
			get
			{
				if ( myDocuments == null )
				{
					myDocuments = createDirectoryIfNeeded( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) );
				}
				return myDocuments;
			}
		}
		
		private static string configDirectory = null;
		public static string ConfigDirectory
		{
			get
			{
				if ( configDirectory == null )
				{
					configDirectory = createDirectoryIfNeeded( Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Config" ) );
				}
				return configDirectory;
			}
		}

		private static string videoDataPath = null;
		public static string VideoDataPath
		{
			get
			{
				if ( videoDataPath == null )
				{
					videoDataPath = createDirectoryIfNeeded( Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VideoData" ) );
				}
				return videoDataPath;
			}
		}

		public static bool DebugEnabled
		{
			get
			{
				return debugEnabled;
			}
			set
			{
				debugEnabled = value;	
			}
		}
		
		
		
		
	}
}

