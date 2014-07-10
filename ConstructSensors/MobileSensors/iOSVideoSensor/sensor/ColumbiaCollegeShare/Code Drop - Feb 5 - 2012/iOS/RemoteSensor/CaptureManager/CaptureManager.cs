using System;
using System.Collections.Concurrent;
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
	public class CaptureManager
	{
		// capture session
		private AVCaptureSession session = null;
		private bool isCapturing = false;
		private Resolution? currentResolution = null;

		// camera input objects
		private AVCaptureDevice videoCaptureDevice = null;
		private AVCaptureDeviceInput videoInput = null;
		
		// microphone input objects
		private AVCaptureDevice audioCaptureDevice = null;
		private AVCaptureDeviceInput audioInput = null;
		
		// frame grabber objects
		private AVCaptureVideoDataOutput frameGrabberOutput = null;
		private VideoFrameSamplerDelegate videoFrameSampler = null;
		private DispatchQueue queue = null;
		
		// movie recorder objects
		private AVCaptureMovieFileOutput movieFileOutput = null;
		private MovieFragmentWriterDelegate movieFragmentWriter = null;
		private string currentFragmentFile = null;
		private DateTime currentFragmentStartedAt;
		private static uint fragmentDuration = 2000;
		
		public EventHandler<MediaFragmentCapturedEventArgs> MediaFragmentCaptured;
		private void onMediaFragmentCaptured( MediaFragmentCapturedEventArgs args )
		{
			if (  MediaFragmentCaptured != null )
			{
				MediaFragmentCaptured( this, args );
			}
		}

		public static uint FragmentDuration 
		{
			get 
			{
				return fragmentDuration;
			}
		}

		public bool IsCapturing 
		{
			get 
			{
				return this.isCapturing;
			}
			set 
			{
				isCapturing = value;
			}
		}		
		
		public bool StartCapture( Resolution resolution, out string message)
		{
			message = "";
			if ( isCapturing == true )
			{
				message = "already capturing";
				return true;
			}
			isCapturing = true;
			
			if ( currentResolution == null )
			{
				if ( setupCaptureSessionInternal( resolution, out message ) == false )
				{
					return false;	
				}
			}
			else if ( currentResolution.Value != resolution )
			{
				StopCapture();
				if ( setupCaptureSessionInternal( resolution, out message ) == false )
				{
					return false;	
				}
			}
			currentResolution = resolution;
			
			// start the capture
			session.StartRunning();
			
			// start recording (if configured)
			startMovieWriter();
			
			return true;
		}

		private void startMovieWriter()
		{
			if ( movieFileOutput == null )
			{
				return;
			}
			
			// generate a random file fragment name
			currentFragmentFile = System.IO.Path.Combine( Settings.VideoDataPath, string.Format("video_fragment_{0}.mov", Guid.NewGuid().ToString()) );
			NSUrl fragmentUrl = NSUrl.FromFilename( currentFragmentFile );
			
			// start recording
			movieFileOutput.StartRecordingToOutputFile( fragmentUrl, movieFragmentWriter);
		}
		
		public void StopCapture()
		{
			if ( isCapturing == false )
			{
				return;
			}
			
			isCapturing = false;
			
			// stop recording (if configured)
			stopMovieWriter();
			
			// stop the capture
			session.StopRunning();
		}

		private void stopMovieWriter()
		{
			if ( movieFileOutput == null )
			{
				return;
			}
			movieFileOutput.StopRecording();
		}
		
		private bool setupCaptureSessionInternal( Resolution resolution, out string errorMessage )
		{
			errorMessage = "";
			
			// create the capture session
			session = new AVCaptureSession(); 
			if ( resolution == Resolution.Medium )
			{
				session.SessionPreset = AVCaptureSession.PresetMedium;
			}
			else
			{
				session.SessionPreset = AVCaptureSession.PresetLow;
			}
			
			// configure the camera input
			if ( addCameraInput( out errorMessage ) == false )
			{
				return false;	
			}

			// configure the microphone input
			if ( addAudioInput( out errorMessage ) == false )
			{
				return false;	
			}

			// conditionally configure the sample buffer output
//			if ( Settings.DebugEnabled == true )
//			{
//				if ( addVideoSampleOutput( out errorMessage ) == false )
//				{
//					return false;	
//				}
//			}
			
			// configure the movie file output
			if ( addMovieFileOutput( out errorMessage ) == false )
			{
				return false;	
			}
			
			return true;
		}

		private bool addCameraInput( out string errorMessage )
		{
			errorMessage = "";
			videoCaptureDevice = getCamera( Settings.Camera );
			videoInput = AVCaptureDeviceInput.FromDevice(videoCaptureDevice);
			if (videoInput == null)
			{
				errorMessage = "No video capture device";
				return false;
			}
			session.AddInput (videoInput);
			return true;
		}

		private bool addAudioInput( out string errorMessage )
		{
			errorMessage = "";
			audioCaptureDevice = getMicrophone();
			audioInput = AVCaptureDeviceInput.FromDevice(audioCaptureDevice);
			if (audioInput == null)
			{
				errorMessage = "No audio capture device";
				return false;
			}
			session.AddInput (audioInput);
			return true;
		}
		
		private bool addMovieFileOutput( out string errorMessage )
		{
			errorMessage = "";
			
			// create a movie file output and add it to the capture session
			movieFileOutput = new AVCaptureMovieFileOutput();
			movieFileOutput.MaxRecordedDuration = new CMTime( fragmentDuration, 1000 );

			// setup the delegate that handles the writing
			movieFragmentWriter = new MovieFragmentWriterDelegate();
			
			// subscribe to the delegate events
			movieFragmentWriter.FileFragmentRecordingStarted += new EventHandler<FileFragmentRecordingStartedEventArgs>( handleFileFragmentRecordingStarted );
			movieFragmentWriter.FileFragmentRecordingComplete += new EventHandler<FileFragmentRecordingCompleteEventArgs>( handleFileFragmentRecordingComplete );
			
			session.AddOutput (movieFileOutput);

			return true;
		}
		
		private void handleFileFragmentRecordingStarted( object sender, FileFragmentRecordingStartedEventArgs args )
		{
			currentFragmentStartedAt = DateTime.Now;
		}
		
		private void handleFileFragmentRecordingComplete(object sender, FileFragmentRecordingCompleteEventArgs args )
		{
			try
			{
				// grab the pertinent event data 
				MediaFragmentCapturedEventArgs captureInfo = new MediaFragmentCapturedEventArgs();
				captureInfo.StartedAt = currentFragmentStartedAt;
				captureInfo.DurationMilliSeconds = fragmentDuration;
				captureInfo.File = args.Path;
				
				// start recording the next fragment - generate a random file fragment name
				if ( args.ErrorOccured == false )
				{
					currentFragmentFile = System.IO.Path.Combine( Settings.VideoDataPath, string.Format("video_fragment_{0}.mov", Guid.NewGuid().ToString()) );
					NSUrl fragmentUrl = NSUrl.FromFilename( currentFragmentFile );
					movieFileOutput.StartRecordingToOutputFile( fragmentUrl, movieFragmentWriter);
				}
				
				// raise the capture event to external listeners
				onMediaFragmentCaptured( captureInfo );
			}
			catch
			{
			}
		}
		
		private bool addVideoSampleOutput( out string errorMessage )
		{
			errorMessage = "";
			
			// create a VideoDataOutput and add it to the capture session
			frameGrabberOutput = new AVCaptureVideoDataOutput();
			frameGrabberOutput.VideoSettings = new AVVideoSettings (CVPixelFormatType.CV32BGRA);
			
			// set up the output queue and delegate
			queue = new MonoTouch.CoreFoundation.DispatchQueue ("captureQueue");
			videoFrameSampler = new VideoFrameSamplerDelegate();
			frameGrabberOutput.SetSampleBufferDelegateAndQueue (videoFrameSampler, queue);
			
			// add the output to the session
			session.AddOutput (frameGrabberOutput);
			
			return true;
		}
		
		private AVCaptureDevice getCamera( Settings.CameraType cameraType )
		{
			// TODO - if this code ever becomes serious enough to use in production then the localized strings will need to be replaced
			string localizedDeviceName = cameraType == Settings.CameraType.FrontFacing ? "Front Camera" : "Rear Camera";
			
			foreach ( AVCaptureDevice device in AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video) )
			{
				if ( string.Compare( device.LocalizedName, localizedDeviceName, true ) == 0 )
				{
					return device;
				}
			}
			return AVCaptureDevice.DefaultDeviceWithMediaType( AVMediaType.Video );
		}

		private AVCaptureDevice getMicrophone()
		{
			foreach ( AVCaptureDevice device in AVCaptureDevice.DevicesWithMediaType(AVMediaType.Audio) )
			{
				if ( device.LocalizedName.ToLower().Contains("microphone") == true )
				{
					return device;
				}
			}
			return AVCaptureDevice.DefaultDeviceWithMediaType( AVMediaType.Audio );
		}
	
	}
	
	public enum Resolution
	{
		Medium,
		Low
	}
		
	
	
}

