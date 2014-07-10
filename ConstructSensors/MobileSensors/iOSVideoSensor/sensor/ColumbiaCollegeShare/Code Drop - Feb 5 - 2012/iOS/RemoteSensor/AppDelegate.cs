using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace RemoteSensor
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// windows and views
		UIWindow window;
		private RootViewController rootView = null;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Settings.Load();
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			rootView = new RootViewController();
			window.AddSubview( rootView.View );
			window.MakeKeyAndVisible ();
			return true;
		}
		
		public override void WillTerminate (UIApplication application)
		{
			Settings.Save();
		}
		
		
		
	}
}

