//Copyright (c) %CompanyName%. All rights reserved.
//
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace %DefaultNamespace%
{
	/// <summary>
	/// Summary description for %ClassName%.
	/// </summary>
	public partial class %ClassName% : System.Windows.Forms.Form
	{
		public %ClassName%()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// Add any constructor code after InitializeComponent call
			//
			// Add the following code if you want to dispose on close
			this.Closed += new System.EventHandler(this.FormClosedEvent);
		}

		private void buttonSave_Click(object sender, System.EventArgs e)
		{
			this.objectProvider1.SaveAll();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.objectProvider1.CancelAll();
		}

		private void FormClosedEvent(object sender, System.EventArgs e)
		{
			this.Dispose();
		}
	}
}
