using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMFramework;

namespace CalibrationTool
{
	public partial class SignalSelect : Form
	{
		public SignalSelect()
		{
			InitializeComponent();
		}

		private void SignalSelect_Load(object sender, EventArgs e)
		{
			foreach (FaceLabSignalConfiguration signal in SourceCluster.SensorConfigurations)
			{
				lbSignals.Items.Add(signal.Label);
			}

			lbSignals.DoubleClick += lbSignals_DoubleClick;

			this.Activate();
		}

		void lbSignals_DoubleClick(object sender, EventArgs e)
		{
			var ee = e as MouseEventArgs;
			int index = lbSignals.IndexFromPoint(ee.Location);
			if (index != ListBox.NoMatches)
				CheckAndFinalize();
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			CheckAndFinalize();
		}

		private void CheckAndFinalize()
		{
			if (lbSignals.SelectedIndex == -1)
			{
				MessageBox.Show("You need to select a signal for calibration.");
				return;
			}

			SelectedSignal = SourceCluster.SensorConfigurations[lbSignals.SelectedIndex];

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		public SensorClusterConfiguration SourceCluster;
		public FaceLabSignalConfiguration SelectedSignal;
	}
}
