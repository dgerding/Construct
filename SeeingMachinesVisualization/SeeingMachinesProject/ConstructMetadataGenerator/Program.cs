using System;
using System.Windows.Forms;

namespace ConstructMetadataGenerator
{
	//	TODO: Need to be able to open sensor XML files as well

	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
