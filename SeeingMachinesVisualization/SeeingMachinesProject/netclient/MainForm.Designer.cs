namespace netclient
{
	partial class MainForm
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
			this.tbSourcePort = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbDestPort1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbTargetHost1 = new System.Windows.Forms.TextBox();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblSpeed = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbDestPort2 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbTargetHost2 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbSourcePort
			// 
			this.tbSourcePort.Location = new System.Drawing.Point(84, 12);
			this.tbSourcePort.Name = "tbSourcePort";
			this.tbSourcePort.Size = new System.Drawing.Size(46, 20);
			this.tbSourcePort.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Source Port:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Destination Port 1:";
			// 
			// tbDestPort1
			// 
			this.tbDestPort1.Location = new System.Drawing.Point(112, 60);
			this.tbDestPort1.Name = "tbDestPort1";
			this.tbDestPort1.Size = new System.Drawing.Size(46, 20);
			this.tbDestPort1.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 89);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Target Host 1:";
			// 
			// tbTargetHost1
			// 
			this.tbTargetHost1.Location = new System.Drawing.Point(93, 86);
			this.tbTargetHost1.Name = "tbTargetHost1";
			this.tbTargetHost1.Size = new System.Drawing.Size(120, 20);
			this.tbTargetHost1.TabIndex = 5;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(31, 227);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 6;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(112, 227);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 7;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// lblStatus
			// 
			this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatus.Enabled = false;
			this.lblStatus.Location = new System.Drawing.Point(12, 253);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(201, 109);
			this.lblStatus.TabIndex = 8;
			this.lblStatus.Text = "STATUS";
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblSpeed
			// 
			this.lblSpeed.Enabled = false;
			this.lblSpeed.Location = new System.Drawing.Point(12, 200);
			this.lblSpeed.Name = "lblSpeed";
			this.lblSpeed.Size = new System.Drawing.Size(201, 24);
			this.lblSpeed.TabIndex = 9;
			this.lblSpeed.Text = "0B/s";
			this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 130);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(94, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "Destination Port 2:";
			// 
			// tbDestPort2
			// 
			this.tbDestPort2.Location = new System.Drawing.Point(112, 127);
			this.tbDestPort2.Name = "tbDestPort2";
			this.tbDestPort2.Size = new System.Drawing.Size(46, 20);
			this.tbDestPort2.TabIndex = 3;
			this.tbDestPort2.Text = "6050";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 156);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(75, 13);
			this.label6.TabIndex = 4;
			this.label6.Text = "Target Host 2:";
			// 
			// tbTargetHost2
			// 
			this.tbTargetHost2.Location = new System.Drawing.Point(93, 153);
			this.tbTargetHost2.Name = "tbTargetHost2";
			this.tbTargetHost2.Size = new System.Drawing.Size(120, 20);
			this.tbTargetHost2.TabIndex = 5;
			this.tbTargetHost2.Text = "localhost";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(225, 371);
			this.Controls.Add(this.lblSpeed);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.tbTargetHost2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tbTargetHost1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbDestPort2);
			this.Controls.Add(this.tbDestPort1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbSourcePort);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "UDP Forwarder";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbSourcePort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbDestPort1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbTargetHost1;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label lblSpeed;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbDestPort2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbTargetHost2;
	}
}

