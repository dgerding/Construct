using Microsoft.Xna.Framework;
using System;

namespace SMFramework
{
	/// <summary>
	/// Generic interface for data streams. The Visualization takes a FaceLabDataStream as a source, which
	/// can be set to a PlaybackDataStream instance or a PairedDatabase instance.
	/// </summary>
	public interface FaceLabDataStream
	{
		DataSnapshot CurrentSnapshot
		{
			get;
		}

		//	Requests that the stream advance to its next snapshot
		void BeginNextSnapshot();
		//	Requests that the stream advance to the nearest snapshot with the specified timestamp (which can be forward or backwards in the stream, relative to current)
		void BeginNextSnapshot(DateTime snapshotTimestamp);

		void AddToCurrentSnapshot(string key, double value);

		void AddToCurrentSnapshot(string key, Vector2 value);

		void AddToCurrentSnapshot(string key, Vector3 value);

		int NumberOfSnapshots
		{
			get;
		}
	}
}
