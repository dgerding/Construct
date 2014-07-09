using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalibrationViewer
{
	class MessageBoxOutputStream : SMFramework.DebugOutputStream
	{
		public override void WriteLine(string text)
		{
			MessageBox.Show(text);
		}
	}
}
