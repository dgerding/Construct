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
			this.objectView1_PositionChanged();
		}


		private void FormClosedEvent(object sender, System.EventArgs e)
		{
			this.Dispose();
		}

		private void objectView1_PositionChanged()
		{
			this.labelNavLocation.Text = ((((this.BindingContext[objectView1].Position + 1)).ToString() + " of  ") 
				+ this.BindingContext[objectView1].Count.ToString());
		}

		private void buttonNavFirst_Click(object sender, System.EventArgs e)
		{
			this.BindingContext[objectView1].Position = 0;
			this.objectView1_PositionChanged();
		}

		private void buttonNavNext_Click(object sender, System.EventArgs e)
		{
			this.BindingContext[objectView1].Position = (this.BindingContext[objectView1].Position + 1);
			this.objectView1_PositionChanged();
		}

		private void buttonNavPrev_Click(object sender, System.EventArgs e)
		{
			this.BindingContext[objectView1].Position = (this.BindingContext[objectView1].Position - 1);
			this.objectView1_PositionChanged();
		}

		private void buttonLast_Click(object sender, System.EventArgs e)
		{
			this.BindingContext[objectView1].Position = (this.objectView1.Count - 1);
			this.objectView1_PositionChanged();
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			try 
			{
				// Clear out the current edits
				this.BindingContext[objectView1].EndCurrentEdit();
				this.BindingContext[objectView1].AddNew();
			}
			catch (System.Exception eEndEdit) 
			{
				System.Windows.Forms.MessageBox.Show(eEndEdit.Message);
			}
			this.objectView1_PositionChanged();
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if ((this.BindingContext[objectView1].Count > 0)) 
			{
				DialogResult r = MessageBox.Show("Do you really want to delete it ?","confirm", MessageBoxButtons.YesNo );
				if (r == DialogResult.Yes)
				{
					this.BindingContext[objectView1].RemoveAt(this.BindingContext[objectView1].Position);
					this.objectView1_PositionChanged();
				}
			}
		}

		private void buttonSave_Click(object sender, System.EventArgs e)
		{
			this.BindingContext[objectView1].EndCurrentEdit();
			this.objectProvider1.SaveAll();
			this.objectView1_PositionChanged();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult r = MessageBox.Show("Do you really want to cancel ?","confirm", MessageBoxButtons.YesNo);
			if (r == DialogResult.Yes)
			{
				this.BindingContext[objectView1].CancelCurrentEdit();
				this.objectProvider1.CancelAll();
				this.objectView1_PositionChanged();
			}
		}

	}
}
