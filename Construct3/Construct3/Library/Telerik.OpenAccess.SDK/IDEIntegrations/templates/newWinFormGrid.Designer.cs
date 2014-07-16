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
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSave.Location = new System.Drawing.Point(613, 10);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.TabIndex = 1;
			this.buttonSave.Text = "&SaveAll";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.Location = new System.Drawing.Point(8, 10);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "&CancelAll";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// %ClassName%
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(696, 485);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonCancel);
			this.Name = "%ClassName%";
			this.Text = "%ProductName% Data Form";
			this.ResumeLayout(false);
		}
		#endregion

		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonCancel;
	}
}
