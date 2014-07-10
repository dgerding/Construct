using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace RemoteSensor
{
	public class RootViewController : UINavigationController
	{
		private RemoteSensorViewController remoteSensorViewController;

		#region events
		public class ViewControllerPoppedEventArgs : EventArgs
		{
			public UIViewController Controller = null;	
		}
		public event EventHandler<ViewControllerPoppedEventArgs> ViewControllerPopped;
		private void onViewControllerPopped( UIViewController controller )
		{
			if ( ViewControllerPopped != null )
			{
				ViewControllerPoppedEventArgs args = new ViewControllerPoppedEventArgs();
				args.Controller = controller;
				ViewControllerPopped( this, args );
			}
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
			
			if ( Settings.DebugEnabled == true)
			{
				DebugUtils.ShowLastErrorLog();
			}
			
			remoteSensorViewController = new RemoteSensorViewController();
			PushViewController( remoteSensorViewController, true );
		}
		
		public override UIViewController PopViewControllerAnimated (bool animated)
		{
			UIViewController controller = base.PopViewControllerAnimated (animated);
			onViewControllerPopped( controller );
			return controller;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		
	}
}

