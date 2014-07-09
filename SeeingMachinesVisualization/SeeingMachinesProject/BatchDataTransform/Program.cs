using SMFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchDataTransform
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			String clusterFile;
			Console.WriteLine("Enter the cluster file (leave blank for OpenFileDialog): ");
			clusterFile = Console.ReadLine();

			if (clusterFile.Length == 0)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "Cluster Config|*.cfg";
				DialogResult clusterResult = ofd.ShowDialog();
				if (clusterResult != DialogResult.OK)
					return;

				clusterFile = ofd.FileName;
			}

			SensorClusterConfiguration config = new SensorClusterConfiguration(clusterFile, SensorClusterConfiguration.FileLoadResponse.FailIfMissing);

			Console.WriteLine("Select the files to be transformed (multiple windows will pop up, cancel out of one of them to begin transforming)");

			List<String> fileNames = new List<String>();

			OpenFileDialog selectDialog = new OpenFileDialog();
			DialogResult filesResult;
			do
			{
				selectDialog.FileName = "";
				selectDialog.Filter = "CSV Files|*.csv";
				selectDialog.Multiselect = true;
				filesResult = selectDialog.ShowDialog();

				if (filesResult == DialogResult.OK)
					fileNames.AddRange(selectDialog.FileNames);

			} while (filesResult == DialogResult.OK);

			if (fileNames.Count == 0)
				return;

			DialogResult transformResult = MessageBox.Show("Would you like to transform data to global space?", "Transform Method", MessageBoxButtons.YesNo);
			bool useInverse = (transformResult == DialogResult.No);

			List<Task> tasks = new List<Task>();

			/*
			 * 
			 * 
			 * Warning: Recordings thrown through the transform tool tend to have their timestamps messed up
			 * 
			 * 
			 */

			foreach (String file in fileNames)
			{
				tasks.Add(Task.Run((Action)delegate() {
					Console.WriteLine("Processing {0}", file);

					PairedDatabase currentDatabase = new PairedDatabase();
					PlaybackDataStream fileData = new PlaybackDataStream(file, config, FaceData.CoordinateSystemType.Local);
					fileData.AutoGenerateMissingData = false;
					for (int i = 0; i < fileData.NumberOfSnapshots; i++)
					{
						FaceData[] currentData = fileData.CurrentDataInterpretation;
						currentDatabase.BeginNextSnapshot(fileData.CurrentSnapshot.TimeStamp);
						foreach (FaceData face in currentData)
						{
							if (face == null)
								continue;

							FaceLabSignalConfiguration currentConfig = config.FindConfigurationForLabel(face.SignalLabel);
							if (currentConfig == null)
							{
								Console.WriteLine("Warning: Could not find signal configuration {0} in the specified cluster. Skipping current FaceData...", face.SignalLabel);
								continue;
							}
							FaceDataSerialization.WriteFaceDataToDatabase(SeeingModule.EvaluateCameraData(currentConfig, face, useInverse), currentDatabase);
						}
						fileData.BeginNextSnapshot();
					}

					String destinationFile = Path.GetDirectoryName(file) + "\\Transformed";
					if (!Directory.Exists(destinationFile))
						Directory.CreateDirectory(destinationFile);
					destinationFile += "\\" + Path.GetFileNameWithoutExtension(file) + ".csv";
					if (File.Exists(destinationFile))
						File.Delete(destinationFile);
					currentDatabase.WriteToDisk(destinationFile);
				}));
			}

			bool allTasksCompleted = false;
			while (!allTasksCompleted)
			{
				allTasksCompleted = true;
				foreach (Task task in tasks)
				{
					if (task.Status != TaskStatus.RanToCompletion)
						allTasksCompleted = false;
				}

				System.Threading.Thread.Sleep(100);
			}
		}
	}
}
