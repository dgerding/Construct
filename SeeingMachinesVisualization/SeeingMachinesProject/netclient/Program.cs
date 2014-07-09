using System;
using System.Net;
using System.Windows.Forms;

namespace netclient
{
	static class Program
	{
		public static AppConfig Config = new AppConfig();
		public static bool SpinMain = false;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(String[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: netclient sport:<source-port> dport:<dest-port> host:<target-host> [silent]\n");
				Console.WriteLine("Parameters:");

				Console.WriteLine("- sport:<source-port number>");
				Console.WriteLine("      Specifies the local port to pull UDP data from.");

				Console.WriteLine("- dport:<destination-port number>");
				Console.WriteLine("      Specifies the port on the target machine that the data will be forwarded to.");

				Console.WriteLine("- host:<target hostname>");
				Console.WriteLine("      Specifies the target machine that UDP data will be forwarded to.");

				Console.WriteLine("- silent");
				Console.WriteLine("      Runs the forwarder without a GUI. If a parameter is incorrect, the GUI will");
				Console.WriteLine("      open for the user to correct it. The GUI can be closed after corrections have");
				Console.WriteLine("      been made.");

				Console.WriteLine("\nExamples:");
				Console.WriteLine("      netclient sport:2001 dport:13001 host:r2d2");
				Console.WriteLine("      netclient sport:500 dport:202 host:www.google.com silent");
			}

			foreach (String param in args)
			{
				Config.Parse(param);
			}

			ForwardingThread.OnError += ForwardingThread_OnError;

			if (Config.ConfigComplete)
			{
				ForwardingThread.AddForwardTarget(Config.DestinationPort.Value, Config.TargetHost);
				ForwardingThread.Start(Config.SourcePort.Value);
			}

			SpinMain = Config.RunSilent;

			if (!Config.RunSilent || !Config.ConfigComplete)
			{
				if (!Config.ConfigComplete && Config.RunSilent)
				{
					MessageBox.Show("Not all parameters were valid for silent operation, please fill in the remaining options. Close the window when the proper changes have been made and forwarding has been started.", "Invalid netclient parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				MainForm newForm = new MainForm();
				newForm.UpdateUIToConfig(Config);
				newForm.SetRunning(Config.ConfigComplete);
				Application.Run(newForm);
			}

			while (SpinMain && ForwardingThread.ContinueThread)
			{
				System.Threading.Thread.Sleep(100);
			}

			ForwardingThread.Stop();
		}

		static void ForwardingThread_OnError(string errorMessage)
		{
			MessageBox.Show("Forwarding operation failed: " + errorMessage);
		}
	}
}
