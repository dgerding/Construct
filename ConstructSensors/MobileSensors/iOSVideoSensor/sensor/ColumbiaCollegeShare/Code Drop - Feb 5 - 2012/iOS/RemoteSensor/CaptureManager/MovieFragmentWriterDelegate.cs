using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreFoundation;
using System.Runtime.InteropServices;

namespace RemoteSensor
{
	public class MovieFragmentWriterDelegate : AVCaptureFileOutputRecordingDelegate 
	{ 	
		public EventHandler<FileFragmentRecordingStartedEventArgs> FileFragmentRecordingStarted;
		private void onFileFragmentRecordingStarted( string path )
		{
			if (  FileFragmentRecordingStarted != null )
			{
				try
				{
					FileFragmentRecordingStartedEventArgs args = new FileFragmentRecordingStartedEventArgs();
					args.Path = path;
					FileFragmentRecordingStarted( this, args );
				}
				catch
				{
				}
			}
		}

		public EventHandler<FileFragmentRecordingCompleteEventArgs> FileFragmentRecordingComplete;
		private void onFileFragmentRecordingComplete( string path, int length, bool errorOccurred )
		{
			if ( FileFragmentRecordingComplete != null )
			{
				try
				{
					FileFragmentRecordingCompleteEventArgs args = new FileFragmentRecordingCompleteEventArgs();	
					args.Path = path;
					args.Length = length;
					args.ErrorOccured = errorOccurred;
					FileFragmentRecordingComplete(this, args);
				}
				catch
				{
				}
			}
		}
		
		public override void DidStartRecording
		(
			AVCaptureFileOutput captureOutput, 
			NSUrl outputFileUrl, 
			NSObject[] connections
		)
		{
			string path = outputFileUrl.Path;
			onFileFragmentRecordingStarted( path );
		}

		public override void FinishedRecording
		(
			AVCaptureFileOutput captureOutput, 
			NSUrl outputFileUrl, 
			NSObject[] connections, 
			NSError nsError
		)
		{
			try
			{
				finishedRecordingInternal( captureOutput, outputFileUrl, connections, nsError );
			}
			catch (Exception ex)
			{
				int x = 1;
			}
		}
		
		private void finishedRecordingInternal
		(
			AVCaptureFileOutput captureOutput, 
			NSUrl outputFileUrl, 
			NSObject[] connections, 
			NSError nsError
		)
		{
			if ( nsError != null )
			{
				// we are hoping for an 'error' object that contains three items
				// this error is expected because we have specified a file size limit and the streaming
				// session has filled it up so we need to let the world know by raising an event.
				
				var userInfo = nsError.UserInfo;
				// handle recording stoppage due to file size limit
				if ( ( userInfo.Keys.Length == 3 ) && ( nsError.Code == -11811 ) )
				{
					var k0 = userInfo.Keys[0];
					var v0 = userInfo.Values[0];
					
					int fragmentLength;
					if ( ( k0.ToString() != "AVErrorFileSizeKey" ) || 
						 ( int.TryParse( v0.ToString(), out fragmentLength ) == false )  || 
						 ( fragmentLength == 0) )
					{
						if ( Settings.DebugEnabled == true )
						{
							DebugUtils.ShowMessage("Recording Finished with unexpected file fragment size", DebugUtils.GetNSErrorItems( nsError ));
						}
						onFileFragmentRecordingComplete( null, 0, true );
					}
					else
					{
						string path = outputFileUrl.Path;
						onFileFragmentRecordingComplete( path, fragmentLength, false );
					}
				}
				// handle recording stoppage due to file duration limit
				else if ( ( userInfo.Keys.Length == 3 ) && ( nsError.Code == -11810 ) )
				{
					var k0 = userInfo.Keys[0];
					var v0 = userInfo.Values[0];
					
					if ( k0.ToString() != "AVErrorTimeKey" )
					{
						if ( Settings.DebugEnabled == true )
						{
							DebugUtils.ShowMessage("Recording Finished with unexpected file fragment duration", DebugUtils.GetNSErrorItems( nsError ));
						}
						onFileFragmentRecordingComplete( null, 0, true );
					}
					else
					{
						string path = outputFileUrl.Path;
						onFileFragmentRecordingComplete( path, 0, false );
					}
				}
				else
				{
					// unexpected error - log it
					if ( Settings.DebugEnabled == true )
					{
						DebugUtils.ShowMessage("Recording Finished unexpectedly", DebugUtils.GetNSErrorItems( nsError ));
					}
					onFileFragmentRecordingComplete( null, 0, true );
				}
			}
		}
		
		
	}
	
		
}

