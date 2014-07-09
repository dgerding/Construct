using System;
using System.Windows.Forms;

namespace BayesDesigner
{
	public partial class RequestStringBox : Form
	{
        public bool AllowEmpty = false;

		public RequestStringBox()
		{
			InitializeComponent();
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
            if (!AllowEmpty && tbInput.Text.Trim().Length == 0)
            {
                MessageBox.Show("An empty string is not a valid value.");
                return;
            }

			this.DialogResult = DialogResult.OK;
            Text = tbInput.Text;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
	}
}
