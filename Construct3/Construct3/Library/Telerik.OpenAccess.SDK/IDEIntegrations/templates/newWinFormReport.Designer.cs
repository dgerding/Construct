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

			this.buttonNavFirst = new System.Windows.Forms.Button();
			this.buttonNavPrev = new System.Windows.Forms.Button();
			this.labelNavLocation = new System.Windows.Forms.Label();
			this.buttonNavNext = new System.Windows.Forms.Button();
			this.buttonLast = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSave = new System.Windows.Forms.Button();

			this.SuspendLayout();
			// 
			// 
			// buttonNavFirst
			// 
			this.buttonNavFirst.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonNavFirst.Location = new System.Drawing.Point(224, 16);
			this.buttonNavFirst.Name = "buttonNavFirst";
			this.buttonNavFirst.Size = new System.Drawing.Size(40, 23);
			this.buttonNavFirst.TabIndex = 1;
			this.buttonNavFirst.Text = "<<";
			this.buttonNavFirst.Click += new System.EventHandler(this.buttonNavFirst_Click);
			// 
			// buttonNavPrev
			// 
			this.buttonNavPrev.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonNavPrev.Location = new System.Drawing.Point(264, 16);
			this.buttonNavPrev.Name = "buttonNavPrev";
			this.buttonNavPrev.Size = new System.Drawing.Size(35, 23);
			this.buttonNavPrev.TabIndex = 2;
			this.buttonNavPrev.Text = "<";
			this.buttonNavPrev.Click += new System.EventHandler(this.buttonNavPrev_Click);
			// 
			// labelNavLocation
			// 
			this.labelNavLocation.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelNavLocation.BackColor = System.Drawing.Color.White;
			this.labelNavLocation.Location = new System.Drawing.Point(296, 16);
			this.labelNavLocation.Name = "labelNavLocation";
			this.labelNavLocation.Size = new System.Drawing.Size(95, 23);
			this.labelNavLocation.TabIndex = 0;
			this.labelNavLocation.Text = "No Objects";
			this.labelNavLocation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonNavNext
			// 
			this.buttonNavNext.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonNavNext.Location = new System.Drawing.Point(392, 16);
			this.buttonNavNext.Name = "buttonNavNext";
			this.buttonNavNext.Size = new System.Drawing.Size(35, 23);
			this.buttonNavNext.TabIndex = 0;
			this.buttonNavNext.Text = ">";
			this.buttonNavNext.Click += new System.EventHandler(this.buttonNavNext_Click);
			// 
			// buttonLast
			// 
			this.buttonLast.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonLast.Location = new System.Drawing.Point(424, 16);
			this.buttonLast.Name = "buttonLast";
			this.buttonLast.Size = new System.Drawing.Size(40, 23);
			this.buttonLast.TabIndex = 4;
			this.buttonLast.Text = ">>";
			this.buttonLast.Click += new System.EventHandler(this.buttonLast_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(16, 16);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 5;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Location = new System.Drawing.Point(104, 16);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.TabIndex = 6;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(512, 16);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSave.Location = new System.Drawing.Point(600, 16);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.TabIndex = 8;
			this.buttonSave.Text = "&Save";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);

			// 
			// %ClassName%
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(696, 333);
			this.Controls.Add(this.buttonNavFirst);
			this.Controls.Add(this.buttonNavPrev);
			this.Controls.Add(this.labelNavLocation);
			this.Controls.Add(this.buttonNavNext);
			this.Controls.Add(this.buttonLast);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSave);

			this.Name = "%ClassName%";
			this.Text = "%ProductName% Data Form";
			this.ResumeLayout(false);
		}
		#endregion


		private System.Windows.Forms.Button buttonNavFirst;
		private System.Windows.Forms.Button buttonNavPrev;
		private System.Windows.Forms.Label labelNavLocation;
		private System.Windows.Forms.Button buttonNavNext;
		private System.Windows.Forms.Button buttonLast;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonSave;
	}
}
