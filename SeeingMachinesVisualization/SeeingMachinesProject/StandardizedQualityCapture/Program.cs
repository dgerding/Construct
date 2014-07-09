using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMFramework;
using SMFramework.Testing;
using System.Windows.Forms;
using SMFramework.Analytics;

namespace StandardizedQualityCapture
{
	class Program
	{
		static SensorClusterConfiguration Config = null;
		static LabTest MainTest = null;

		[STAThread]
		static void Main(string[] args)
		{
			OpenFileDialog clusterDialog = new OpenFileDialog();
			clusterDialog.Filter = "Cluster Configuration Files|*.cfg";

			DialogResult result = clusterDialog.ShowDialog();
			if (result != DialogResult.OK)
				return;

			Config = new SensorClusterConfiguration(
				clusterDialog.FileName,
				SensorClusterConfiguration.FileLoadResponse.FailIfMissing
				);

			if (Config.SensorConfigurations.Count == 0)
			{
				Console.WriteLine("No signals were detected in the cluster.");
				Console.ReadLine();
				return;
			}

			Console.WriteLine("Cluster sensors:");
			for (int i = 0; i < Config.SensorConfigurations.Count; i++)
			{
				Console.WriteLine("{0}) {1}", i, Config.SensorConfigurations[i].Label);
			}

			int index = -1;
			do
			{
				Console.Write("Select a sensor (by index): ");

			} while (!int.TryParse(Console.ReadLine(), out index) || index < 0 || index > Config.SensorConfigurations.Count);


			SensorClusterConfiguration isolatedConfig = new SensorClusterConfiguration();
			isolatedConfig.SensorConfigurations.Add(Config.SensorConfigurations[index]); // Only capture relevant data
			MainTest = new LabTest(
				isolatedConfig
				);

			StandardQualityProtocol.GenerateStagesWithProcedures(
				"", MainTest, Config.SensorConfigurations[index],
				StandardQualityProtocol.GenerationType.FromSignalStream
				);

			Console.WriteLine("Note: All positional instructions are relative to the signal currently be captured.\n");

			while (MainTest.AdvanceCurrentStage())
			{
				Console.WriteLine(StandardQualityProtocol.StageDescriptions[MainTest.CurrentStageIndex]);

				Console.WriteLine("Press Enter to begin.");
				Console.ReadLine();

				MainTest.CurrentStage.BeginCapture();
				Console.WriteLine("Stage started...");

				while (!MainTest.CurrentStage.IsDone)
					System.Threading.Thread.Sleep(10);

				while (Console.KeyAvailable)
					Console.ReadKey(false);

				Console.Write("Stage complete. Would you like to recapture? ");
				if (Console.ReadLine().ToLower() == "y")
					MainTest.RegressCurrentStage();
				Console.WriteLine("-------------------------------");
			}

			Console.WriteLine("LabTest complete. Writing data to disk...");
			MainTest.SaveStageDataToDisk();
		}
	}
}
