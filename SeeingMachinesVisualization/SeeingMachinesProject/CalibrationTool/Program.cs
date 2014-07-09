using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMFramework;
using Microsoft.Xna.Framework;

namespace CalibrationTool
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());

		//	OpenFileDialog ofd = new OpenFileDialog();
		//	ofd.Filter = "Cluster Configuration File|*.cfg";
		//	DialogResult result = ofd.ShowDialog();
		//	if (result != DialogResult.OK)
		//		return;

		//	m_Config = new SensorClusterConfiguration(ofd.FileName, SensorClusterConfiguration.FileLoadResponse.FailIfMissing);

		//	FaceLabSignalConfiguration selectedSignal = GetSelectedSignal();
		//	if (selectedSignal == null)
		//		return;
		//	List<CaptureChunk> totalData = new List<CaptureChunk>();

		//	bool running = true;
		//	while (running)
		//	{
		//		CaptureChunk currentChunk = new CaptureChunk(selectedSignal);

		//		Vector3 targetPosition;
		//		while (true)
		//		{
		//			Console.WriteLine("Enter the target vergence position (x, y, z): ");

		//			String [] components = Console.ReadLine().Replace(" ", "").Split(',');
		//			if (components.Length != 3)
		//				continue;

		//			float x, y, z;
		//			if (!float.TryParse(components[0], out x))
		//				continue;
		//			if (!float.TryParse(components[1], out y))
		//				continue;
		//			if (!float.TryParse(components[2], out z))
		//				continue;

		//			targetPosition.X = x;
		//			targetPosition.Y = y;
		//			targetPosition.Z = z;

		//			break;
		//		}

		//		currentChunk.TargetPosition = targetPosition;
		//		Console.Write("Press Enter to start collection.");
		//		Console.ReadLine();
		//		currentChunk.StartCollection();
		//		Console.WriteLine("- Collection started");

		//		Console.Write("Press Enter to stop collection.");
		//		Console.ReadLine();
		//		currentChunk.StopCollection();
		//		Console.WriteLine("- Collection stopped, {0} snapshots", currentChunk.RecordingDatabase.PairingSnapshots.Count);

		//		Console.WriteLine("- Collection contains {0} valid vergence captures", currentChunk.ValidSnapshots);

		//		String input;
		//		do
		//		{
		//			Console.WriteLine("Would you like to keep this recording? (y/n)");
		//			input = Console.ReadLine().ToLower();

		//		} while (input != "y" && input != "n");

		//		switch (input)
		//		{
		//			case ("y"):
		//				Console.WriteLine("Storing results.");
		//				totalData.Add(currentChunk);
		//				break;

		//			case ("n"):
		//				Console.WriteLine("Discarding results");
		//				currentChunk = null;
		//				break;
		//		}

		//		do 
		//		{
		//			Console.WriteLine("Would you like to run another recording? (y/n)");
		//			input = Console.ReadLine().ToLower();
		//		} while (input != "y" && input != "n");

		//		switch (input)
		//		{
		//			case ("y"):
		//				break;

		//			case ("n"):
		//				running = false;
		//				break;
		//		}
		//	}

		//	Console.WriteLine("Generating new config...");
		//	Console.ReadLine();
		}
	}
}
