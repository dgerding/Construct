using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace RemoteSensor
{
	public class Application
	{
		static void Main (string[] args)
		{
			try
			{
				AppDomain.CurrentDomain.UnhandledException += HandleGlobalException;
				UIApplication.Main (args, null, "AppDelegate");
			}
			catch (Exception ex)
			{
				if ( Settings.DebugEnabled == true )
				{
					DebugUtils.LogUnhandledException(ex);
				}
			}
		}

		static void HandleGlobalException (object sender, UnhandledExceptionEventArgs e)
		{
			DebugUtils.LogUnhandledException(e.ExceptionObject as Exception);
		}
		
		
		
	}
}
