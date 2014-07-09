using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SMFramework;
using SMFramework.Testing;
using System.IO;
using SMFramework.Analytics;

namespace AnalyticsCompiler
{
	class MessageBoxOutputStream : SMFramework.DebugOutputStream
	{
		public override void WriteLine(string text)
		{
			MessageBox.Show(text);
		}
	}

	static class Program
	{
		static SMFramework.SensorClusterConfiguration CreateSensorConfigurations()
		{
			SMFramework.SensorClusterConfiguration config = null;

			if (File.Exists("defaultcluster.cfg"))
			{
				 config = new SMFramework.SensorClusterConfiguration("defaultcluster.cfg", SMFramework.SensorClusterConfiguration.FileLoadResponse.FailIfMissing);
			}
			else
			{
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "Sensor Configurations|*.cfg";

				DialogResult result = dialog.ShowDialog();
				if (result != DialogResult.OK)
					return null;

				config = new SensorClusterConfiguration(dialog.FileName, SensorClusterConfiguration.FileLoadResponse.FailIfMissing);
			}

			return config;
		}

		[STAThread]
		static void Main(string [] args)
		{
#if DEBUG
			Console.WriteLine("WARNING: Running in Debug mode means not capturing data for the " +
				"duration of the stages. Blame SeeingMachines. Press Enter to continue.");
			//Console.ReadLine();
#endif

			//	LabTest needs to know what sensors it's working with
			SMFramework.SensorClusterConfiguration sensors = CreateSensorConfigurations();

			if (sensors == null)
				return;

			SMFramework.Testing.LabTest mainTest = new SMFramework.Testing.LabTest(sensors);

			//OpenFileDialog ofd = new OpenFileDialog();
			//ofd.Filter = "CSV Files|*.csv";
			//DialogResult result = ofd.ShowDialog();
			//if (result != DialogResult.OK)
			//	return;
			//	

			//throw;
			foreach (FaceLabSignalConfiguration config in sensors.SensorConfigurations)
			{
				StandardQualityProtocol.GenerateStagesWithProcedures("datasamples", mainTest, config, StandardQualityProtocol.GenerationType.FromFile);
			}
			
			while (mainTest.AdvanceCurrentStage())
			{
				mainTest.CurrentStage.BeginCapture();

				Console.WriteLine("STAGE STARTED.");

				while (!mainTest.CurrentStage.IsDone)
				{
					System.Threading.Thread.Sleep(10);
				}
			}

			Console.WriteLine("Lab test complete.");
			//Console.ReadLine();

			Console.WriteLine("Generating analytics");
			mainTest.GenerateTestReport("WebSource/AnalyticsData.js", LabTest.ReportFormat.JSONP);

			Console.WriteLine("Complete.");
			//Console.ReadLine();

			//Console.WriteLine("Saving recordings to disk...");
			//mainTest.SaveStageDataToDisk();
		}
	}
}
