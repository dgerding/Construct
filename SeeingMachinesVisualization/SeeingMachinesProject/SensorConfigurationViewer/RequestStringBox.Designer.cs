namespace CalibrationViewer
{
	partial class RequestStringBox
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
			this.tbInput = new System.Windows.Forms.TextBox();
			this.btnDone = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbInput
			// 
			this.tbInput.Location = new System.Drawing.Point(93, 12);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(278, 20);
			this.tbInput.TabIndex = 0;
			// 
			// btnDone
			// 
			this.btnDone.Location = new System.Drawing.Point(12, 10);
			this.btnDone.Name = "btnDone";
			this.btnDone.Size = new System.Drawing.Size(75, 23);
			this.btnDone.TabIndex = 1;
			this.btnDone.Text = "OK";
			this.btnDone.UseVisualStyleBackColor = true;
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(377, 10);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// RequestString
			// 
			this.AcceptButton = this.btnDone;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(466, 48);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnDone);
			this.Controls.Add(this.tbInput);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "RequestString";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RequestString";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.Button btnDone;
		private System.Windows.Forms.Button btnCancel;
	}
}