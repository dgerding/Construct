namespace FaceDataGenerator
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
			this.btnSetCluster = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tbDuration = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblCurrentCluster = new System.Windows.Forms.Label();
			this.pgCurrentCluster = new System.Windows.Forms.PropertyGrid();
			this.lbSensors = new System.Windows.Forms.ListBox();
			this.btnGenerate = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tbFps = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSetCluster
			// 
			this.btnSetCluster.Location = new System.Drawing.Point(15, 12);
			this.btnSetCluster.Name = "btnSetCluster";
			this.btnSetCluster.Size = new System.Drawing.Size(75, 23);
			this.btnSetCluster.TabIndex = 0;
			this.btnSetCluster.Text = "Set Cluster";
			this.btnSetCluster.UseVisualStyleBackColor = true;
			this.btnSetCluster.Click += new System.EventHandler(this.btnSetCluster_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(12, 75);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lbSensors);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.pgCurrentCluster);
			this.splitContainer1.Size = new System.Drawing.Size(618, 289);
			this.splitContainer1.SplitterDistance = 206;
			this.splitContainer1.TabIndex = 1;
			// 
			// tbDuration
			// 
			this.tbDuration.Location = new System.Drawing.Point(168, 48);
			this.tbDuration.Name = "tbDuration";
			this.tbDuration.Size = new System.Drawing.Size(100, 20);
			this.tbDuration.TabIndex = 2;
			this.tbDuration.Text = "1.0";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(150, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Recording Duration (Seconds)";
			// 
			// lblCurrentCluster
			// 
			this.lblCurrentCluster.AutoSize = true;
			this.lblCurrentCluster.Enabled = false;
			this.lblCurrentCluster.Location = new System.Drawing.Point(96, 17);
			this.lblCurrentCluster.Name = "lblCurrentCluster";
			this.lblCurrentCluster.Size = new System.Drawing.Size(41, 13);
			this.lblCurrentCluster.TabIndex = 4;
			this.lblCurrentCluster.Text = "Not set";
			// 
			// pgCurrentCluster
			// 
			this.pgCurrentCluster.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgCurrentCluster.Location = new System.Drawing.Point(0, 0);
			this.pgCurrentCluster.Name = "pgCurrentCluster";
			this.pgCurrentCluster.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.pgCurrentCluster.Size = new System.Drawing.Size(408, 289);
			this.pgCurrentCluster.TabIndex = 5;
			this.pgCurrentCluster.ToolbarVisible = false;
			// 
			// lbSensors
			// 
			this.lbSensors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbSensors.FormattingEnabled = true;
			this.lbSensors.Location = new System.Drawing.Point(0, 0);
			this.lbSensors.Name = "lbSensors";
			this.lbSensors.Size = new System.Drawing.Size(206, 289);
			this.lbSensors.TabIndex = 4;
			this.lbSensors.SelectedIndexChanged += new System.EventHandler(this.lbSensors_SelectedIndexChanged);
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerate.Location = new System.Drawing.Point(555, 37);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(75, 32);
			this.btnGenerate.TabIndex = 6;
			this.btnGenerate.Text = "Generate";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(299, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(27, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "FPS";
			// 
			// tbFps
			// 
			this.tbFps.Location = new System.Drawing.Point(332, 48);
			this.tbFps.Name = "tbFps";
			this.tbFps.Size = new System.Drawing.Size(37, 20);
			this.tbFps.TabIndex = 3;
			this.tbFps.Text = "60";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(642, 376);
			this.Controls.Add(this.tbFps);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnGenerate);
			this.Controls.Add(this.lblCurrentCluster);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbDuration);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.btnSetCluster);
			this.Name = "MainForm";
			this.Text = "FaceData Generator";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSetCluster;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox tbDuration;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblCurrentCluster;
		private System.Windows.Forms.PropertyGrid pgCurrentCluster;
		private System.Windows.Forms.ListBox lbSensors;
		private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbFps;
	}
}

