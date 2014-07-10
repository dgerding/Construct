using System;
using System.Text;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace RemoteSensor
{
	public static class DebugUtils
	{
		public static string GetNSErrorItems (NSError nsError)
		{
			if ( nsError == null )
			{
				return "No Error Info Available";
			}
			
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Error Code:  {0}\r\n", nsError.Code.ToString());
				sb.AppendFormat("Description: {0}\r\n", nsError.LocalizedDescription);
				var userInfo = nsError.UserInfo;
				for ( int i = 0; i < userInfo.Keys.Length; i++ )
				{
					sb.AppendFormat("[{0}]: {1}\r\n", userInfo.Keys[i].ToString(), userInfo.Values[i].ToString() );
				}
				return sb.ToString();
			}
			catch
			{
				return "Error parsing NSError object. Ironic, is it not ?";		
			}
		}
		
		private static string debugDirectory = null;
		public static string DebugDirectory
		{
			get
			{
				if ( debugDirectory == null )
				{
					string path = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs" );
					if ( Directory.Exists(path) == false )
					{
						Directory.CreateDirectory( path );
					}
					debugDirectory = path;
				}
				return debugDirectory;
			}
		}
		
		public static string ExceptionLogFilePath
		{
			get
			{
				return Path.Combine( DebugDirectory, "ExceptionLog.txt" );
			}
		}
		
		public static void ShowLastErrorLog()
		{
			if ( File.Exists( ExceptionLogFilePath ) == true )
			{
				string exceptionText = File.ReadAllText( ExceptionLogFilePath );
				File.Delete( ExceptionLogFilePath );
				ShowMessage( "Logged Exception", exceptionText );
			}
		}
		
		public static void ShowMessage( String title, string message )
		{
			using(var alert = new UIAlertView(title, message, null, "OK", null))
			{
				alert.Show();  
			}
		}		
		
		public static void LogUnhandledException( Exception ex)
		{
			StringBuilder sb = new StringBuilder();
			Exception currentException = ex;
			while ( currentException != null )
			{
				sb.AppendFormat("----------------------\r\n{0}-------------------------\r\n{1}\r\n", currentException.Message, currentException.StackTrace);
				sb.AppendFormat("-----Source: {0}\r\n", currentException.Source);
				if ( currentException.TargetSite != null )
				{
					sb.AppendFormat("-----Target Site: {0}\r\n", currentException.TargetSite.Name);
				}
				currentException = currentException.InnerException;
			}
			string text = sb.ToString();
			if ( File.Exists(ExceptionLogFilePath) == true )
			{
				File.Delete(ExceptionLogFilePath);
			}
			File.WriteAllText( ExceptionLogFilePath, text );
		}
		
		
	}
}

