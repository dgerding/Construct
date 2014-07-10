using System;

namespace RemoteSensor
{
	public class FileFragmentRecordingStartedEventArgs : EventArgs
	{
		public string Path;	
	}
	
	public class FileFragmentRecordingCompleteEventArgs : EventArgs
	{
		public string Path;	
		public int Length;
		public bool ErrorOccured;
	}

	public class MediaFragmentCapturedEventArgs : EventArgs
	{
		public DateTime StartedAt;
		public uint DurationMilliSeconds;
		public string File;
	}
	
}

