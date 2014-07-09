using SMFramework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * The purpose of this tool is just to retrieve data from the FaceLAB cameras and persist
 *	the data directly to disk. This is for development purposes, to provide a set of data
 *	to work from without requiring someone to sit in front of the cameras. The data saved
 *	is raw and untransformed, preventing it from being contaminated by incorrect measurements
 *	or algorithms that are being debugged.
 */

/* ParallelDatabaseWriter needs an interface similar to that of PairedDatabase in order to
 * be of any use
 */

namespace RawDataCollection
{
	class DataController
	{
		private Task m_DataCollectionTask;
		private volatile bool m_IsCollectingData = false;

		public SensorClusterConfiguration Config;

		AsyncPairedDatabase m_DBWriter = new AsyncPairedDatabase();

		private void CollectData()
		{
			FaceLabConnection[] signals = new FaceLabConnection[Config.SensorConfigurations.Count];
			for (int i = 0; i < signals.Length; i++)
			{
#if DEBUG
				signals[i] = new FaceLabConnection(Config.SensorConfigurations[i]);
#else
				signals[i] = new FaceLabConnection(Config.SensorConfigurations[i]);
#endif
			}

			int frames = 0;
			DateTime startTime = DateTime.Now;
			DateTime prevTime = DateTime.Now;

			m_DBWriter.Start("test.csv");

			while (m_IsCollectingData)
			{
				if ((DateTime.Now - startTime).TotalSeconds >= 1.0F)
				{
					String consoleTitle = "FPS: " + frames;
					consoleTitle += " | Memory Usage: ";
					consoleTitle += Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
					consoleTitle += "MB";
					Console.Title = consoleTitle;
					frames = 0;
					startTime = DateTime.Now;
				}

				frames++;

				int msDifference = 10 - (int)(DateTime.Now - prevTime).TotalMilliseconds;
				if (msDifference > 0)
					System.Threading.Thread.Sleep(msDifference);
				prevTime = DateTime.Now;

				lock (m_DBWriter)
				{
					m_DBWriter.BeginNextSnapshot();

					foreach (FaceLabConnection data in signals)
					{
						data.RetrieveNewData();

						FaceDataSerialization.WriteFaceDataToSnapshot(data.CurrentData, m_DBWriter.CurrentSnapshot);
					}
				}
			}

			m_DBWriter.Stop();
		}

		public void CollectDataAsync()
		{
			if (m_IsCollectingData)
				return;

			m_IsCollectingData = true;

			m_DataCollectionTask = Task.Factory.StartNew((System.Action)CollectData);
		}

		public void EndCollectionAsync()
		{
			m_IsCollectingData = false;
			m_DataCollectionTask.Wait();
		}
	}

	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
#if DEBUG
			Console.WriteLine("Warning: In Debug mode, no data will be collected. (Blame SeeingMachines.)");
#endif

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Cluster Configuration|*.cfg";
			DialogResult result = ofd.ShowDialog();

			if (result != DialogResult.OK)
				return;

			if (!Directory.Exists("Data"))
				Directory.CreateDirectory("Data");

			Console.Title = "FPS: <Invalid>";

			DataController controller = new DataController();
			controller.Config = new SensorClusterConfiguration(ofd.FileName, SensorClusterConfiguration.FileLoadResponse.FailIfMissing);
			controller.CollectDataAsync();

			bool runApplication = true;

			while (runApplication)
			{
				Console.WriteLine("Commands: esc [cls], c [clear db], f [show output folder], s [save], e [end]");
				Console.Write("> ");
				var key = Console.ReadKey(false);
				Console.WriteLine();

				switch (key.Key)
				{
					case ConsoleKey.Escape:
						{
							Console.Clear();
							continue;
						}

					//case ConsoleKey.C:
					//	{
					//		Console.WriteLine("Clearing database contents...");
					//		lock (controller.TargetDatabase)
					//		{
					//			controller.TargetDatabase.ClearDatabase();
					//		}
					//		Console.WriteLine("Done.");
					//		break;
					//	}

					case ConsoleKey.E:
						{
							Console.WriteLine("Ending application.");
							runApplication = false;
							break;
						}

					case ConsoleKey.F:
						{
							String target = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Data";
							System.Diagnostics.Process.Start("explorer.exe", target);
							break;
						}

					//case ConsoleKey.S:
					//	{
					//		Console.Write("Enter the file name: ");
					//		lock (controller.TargetDatabase)
					//		{
					//			controller.TargetDatabase.WriteToDisk("Data/" + Console.ReadLine());
					//		}
					//		Console.WriteLine("Done.");
					//		break;
					//	}

					default:
						{
							Console.WriteLine("Unrecognized command.");
							break;
						}
				}

				Console.WriteLine();
			}

			controller.EndCollectionAsync();
		}
	}
}
