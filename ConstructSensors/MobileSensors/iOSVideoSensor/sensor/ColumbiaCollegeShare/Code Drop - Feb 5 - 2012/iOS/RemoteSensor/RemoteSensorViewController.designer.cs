// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RemoteSensor
{
	[Register ("RemoteSensorViewController")]
	partial class RemoteSensorViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIToolbar toolBar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (toolBar != null) {
				toolBar.Dispose ();
				toolBar = null;
			}
		}
	}
}
