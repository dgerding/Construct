using System;

namespace SMFramework.Testing
{
	public delegate void CollectionCompletedHandler();

	/* IMPORTANT NOTE:
	 * 
	 *	All data collection sources assume that they are the owner of the PairedDatabase that
	 *		is passed to them; modifications should *NOT* be made to the CollectedData property
	 *		in between BeginCollection/StopCollection calls. PairedDatabases are NOT thread-safe.
	 */

	public interface DataCollectionSource
	{
		event CollectionCompletedHandler CollectionCompleted;

		PairedDatabase CollectedData { get; }

		/* Requested name of the file that would be generated upon persistence to disk. */
		String PersistenceFileName { get; }

		/* A DataCollectionSource is allowed to end collection on its own, i.e. during a timed
		 *	collection period.
		 */
		bool CollectionIsDone { get; }

		void BeginCollection(SensorClusterConfiguration cluster);
		void StopCollection();
	}

	public class FileStreamCollectionSource : DataCollectionSource
	{
		public PairedDatabase CollectedData { get; private set; }
		public bool CollectionIsDone { get; private set; }

		public FaceData.CoordinateSystemType CoordinateSystem { get; private set; }
		public String PersistenceFileName { get; private set; }

		public event CollectionCompletedHandler CollectionCompleted;

		public FileStreamCollectionSource(String sourceFile, FaceData.CoordinateSystemType coordinateSystem)
		{
			CoordinateSystem = coordinateSystem;
			PersistenceFileName = sourceFile;
			CollectionIsDone = false;
		}

		public void BeginCollection(SensorClusterConfiguration cluster)
		{
			CollectedData = new PairedDatabase(PersistenceFileName);
			CollectionIsDone = true;

			if (CollectionCompleted != null)
				CollectionCompleted();
		}

		public void StopCollection()
		{
		}
	}

	/* Note: This class doesn't do any data transformations. It is advised that raw data streams be recorded to disk first, and then
	 * transformed later on.
	 */
	public class ConstantCollectionSource : DataCollectionSource
	{
		public PairedDatabase CollectedData { get; private set; }

		public bool CollectionIsDone { get; private set; }

		public String PersistenceFileName { get; private set; }

		public bool AutoTransformData = false;

		public event CollectionCompletedHandler CollectionCompleted;

		private IsolatedDataCaptureThread m_CaptureThread;

		public ConstantCollectionSource()
		{
			CollectedData = new PairedDatabase();
			CollectionIsDone = false;
			PersistenceFileName = "";
		}

		public ConstantCollectionSource(PairedDatabase target, String fileTarget)
		{
			CollectedData = target;
			CollectionIsDone = false;
			PersistenceFileName = fileTarget;
		}

		public void BeginCollection(SensorClusterConfiguration cluster)
		{
			CollectionIsDone = false;
			m_CaptureThread = IsolatedDataCaptureThread.StartNew(cluster, CollectedData, AutoTransformData);
		}

		public void StopCollection()
		{
			m_CaptureThread.StopCapture(true);
			m_CaptureThread = null;
			GC.Collect();
			CollectionIsDone = true;

			if (CollectionCompleted != null)
				CollectionCompleted();
		}
	}

	public class TimedCollectionSource : DataCollectionSource, IDisposable
	{
		public void Dispose()
		{
			if (m_CollectionTimer != null)
				m_CollectionTimer.Dispose();
		}

		public PairedDatabase CollectedData { get; private set; }

		public bool CollectionIsDone { get; private set; }

		public float Duration { get; private set; }

		public String PersistenceFileName { get; private set; }

		public event CollectionCompletedHandler CollectionCompleted;

		public bool AutoTransformData = false;

		System.Threading.Timer m_CollectionTimer;
		IsolatedDataCaptureThread m_CaptureThread;

		public TimedCollectionSource(float durationSeconds)
		{
			CollectedData = new PairedDatabase();
			Duration = durationSeconds;
			CollectionIsDone = false;
		}

		public TimedCollectionSource(float durationSeconds, PairedDatabase targetDatabase, String targetFile)
		{
			CollectedData = targetDatabase;
			Duration = durationSeconds;

			CollectionIsDone = false;

			PersistenceFileName = targetFile;
		}

		public void BeginCollection(SensorClusterConfiguration cluster)
		{
			CollectedData.ClearDatabase();

			/* NOTE: The thread-safety of this sort of thing is *really* questionable. (aka
						 * non-existent, not entirely sure of how much of a problem that will be.) */
			m_CollectionTimer = new System.Threading.Timer(delegate(object unusedState)
			{
				StopCollection();
			}, null, System.Threading.Timeout.Infinite, 0);

			m_CollectionTimer.Change((int)(Duration * 1000), System.Threading.Timeout.Infinite);
			m_CaptureThread = IsolatedDataCaptureThread.StartNew(cluster, CollectedData, AutoTransformData);
		}

		public void StopCollection()
		{
			m_CaptureThread.StopCapture(true);
			m_CaptureThread = null;
			GC.Collect();
			m_CollectionTimer = null;
			CollectionIsDone = true;

			/* Note: It is possible that these events will be raised on the timer thread instead of the
			 * invoker thread.
			 */
			if (CollectionCompleted != null)
				CollectionCompleted();
		}
	}
}
