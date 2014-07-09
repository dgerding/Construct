using System;
using System.Windows.Forms;

namespace ConstructMetadataGenerator
{
	public partial class ListBoxSelectDialog : Form
	{
		public object SelectedItem = null;

		public ListBox.ObjectCollection ItemsSource
		{
			get { return lbSelectBox.Items; }
		}

		public ListBoxSelectDialog()
		{
			InitializeComponent();
			DialogResult = DialogResult.Ignore;

			lbSelectBox.MouseDoubleClick += lbSelectBox_MouseDoubleClick;
		}

		void HandleAccept(int itemIndex)
		{
			if (itemIndex != ListBox.NoMatches && itemIndex > 0)
			{
				SelectedItem = lbSelectBox.Items[itemIndex];
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		void lbSelectBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int itemIndex = lbSelectBox.IndexFromPoint(e.Location);
			HandleAccept(itemIndex);
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			HandleAccept(lbSelectBox.SelectedIndex);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
