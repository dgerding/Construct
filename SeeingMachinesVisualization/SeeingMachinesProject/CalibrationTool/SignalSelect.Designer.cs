namespace CalibrationTool
{
	partial class SignalSelect
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lbSignals = new System.Windows.Forms.ListBox();
			this.btnDone = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(164, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select the signal for configuration";
			// 
			// lbSignals
			// 
			this.lbSignals.FormattingEnabled = true;
			this.lbSignals.Location = new System.Drawing.Point(12, 25);
			this.lbSignals.Name = "lbSignals";
			this.lbSignals.Size = new System.Drawing.Size(311, 225);
			this.lbSignals.TabIndex = 1;
			// 
			// btnDone
			// 
			this.btnDone.Location = new System.Drawing.Point(126, 256);
			this.btnDone.Name = "btnDone";
			this.btnDone.Size = new System.Drawing.Size(75, 23);
			this.btnDone.TabIndex = 2;
			this.btnDone.Text = "Done";
			this.btnDone.UseVisualStyleBackColor = true;
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// SignalSelect
			// 
			this.AcceptButton = this.btnDone;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(335, 286);
			this.ControlBox = false;
			this.Controls.Add(this.btnDone);
			this.Controls.Add(this.lbSignals);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "SignalSelect";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Signal Selection";
			this.Load += new System.EventHandler(this.SignalSelect_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lbSignals;
		private System.Windows.Forms.Button btnDone;
	}
}