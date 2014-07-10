using System;
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
	public class VideoFrameSamplerDelegate : AVCaptureVideoDataOutputSampleBufferDelegate 
	{ 	
		public override void DidOutputSampleBuffer
		(
			AVCaptureOutput captureOutput, 
			CMSampleBuffer sampleBuffer, 
			AVCaptureConnection connection
		)
		{
			try 
			{
				// render the image into the debug preview pane
				UIImage image = getImageFromSampleBuffer(sampleBuffer);

				// TODO - do something with the image
				
				// The AVFoundation has a fixed number of buffers and if it runs out of free buffers, it will stop
				// delivering frames. So make sure to call Dispose !!!
				sampleBuffer.Dispose ();
			} 
			catch (Exception e)
			{
				if ( Settings.DebugEnabled == true )
				{
					DebugUtils.ShowMessage( "Caught exception in sample buffer delegate call to DidOutputSampleBuffer", e.Message );
				}
			}
		}
		
		private UIImage getImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
		{
			// Get the CoreVideo image
			using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
			{
				// Lock the base address
				pixelBuffer.Lock (0);
				
				// Get the number of bytes per row for the pixel buffer
				var baseAddress = pixelBuffer.BaseAddress;
				int bytesPerRow = pixelBuffer.BytesPerRow;
				int width = pixelBuffer.Width;
				int height = pixelBuffer.Height;
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
				
				// Create a CGImage on the RGB colorspace from the configured parameter above
				using (var cs = CGColorSpace.CreateDeviceRGB ())
				using (var context = new CGBitmapContext (baseAddress,width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo) flags))
				using (var cgImage = context.ToImage ())
				{
					pixelBuffer.Unlock (0);
					return UIImage.FromImage (cgImage);
				}
			}
		}
	}
}

