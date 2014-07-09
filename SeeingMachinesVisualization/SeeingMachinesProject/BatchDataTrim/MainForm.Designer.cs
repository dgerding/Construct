namespace BatchDataTrim
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
			this.cbSignals = new System.Windows.Forms.CheckedListBox();
			this.btnTrim = new System.Windows.Forms.Button();
			this.btnSelectCluster = new System.Windows.Forms.Button();
			this.btnSelectFiles = new System.Windows.Forms.Button();
			this.clusterFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.sourceFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.lblOutput = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cbSignals
			// 
			this.cbSignals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSignals.CheckOnClick = true;
			this.cbSignals.FormattingEnabled = true;
			this.cbSignals.Location = new System.Drawing.Point(13, 41);
			this.cbSignals.Name = "cbSignals";
			this.cbSignals.Size = new System.Drawing.Size(407, 154);
			this.cbSignals.TabIndex = 0;
			this.cbSignals.SelectedIndexChanged += new System.EventHandler(this.cbSignals_SelectedIndexChanged);
			// 
			// btnTrim
			// 
			this.btnTrim.Location = new System.Drawing.Point(148, 201);
			this.btnTrim.Name = "btnTrim";
			this.btnTrim.Size = new System.Drawing.Size(130, 55);
			this.btnTrim.TabIndex = 1;
			this.btnTrim.Text = "Trim";
			this.btnTrim.UseVisualStyleBackColor = true;
			this.btnTrim.Click += new System.EventHandler(this.btnTrim_Click);
			// 
			// btnSelectCluster
			// 
			this.btnSelectCluster.Location = new System.Drawing.Point(13, 12);
			this.btnSelectCluster.Name = "btnSelectCluster";
			this.btnSelectCluster.Size = new System.Drawing.Size(84, 23);
			this.btnSelectCluster.TabIndex = 2;
			this.btnSelectCluster.Text = "Select Cluster";
			this.btnSelectCluster.UseVisualStyleBackColor = true;
			this.btnSelectCluster.Click += new System.EventHandler(this.btnSelectCluster_Click);
			// 
			// btnSelectFiles
			// 
			this.btnSelectFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectFiles.Location = new System.Drawing.Point(345, 12);
			this.btnSelectFiles.Name = "btnSelectFiles";
			this.btnSelectFiles.Size = new System.Drawing.Size(75, 23);
			this.btnSelectFiles.TabIndex = 3;
			this.btnSelectFiles.Text = "Select Files";
			this.btnSelectFiles.UseVisualStyleBackColor = true;
			this.btnSelectFiles.Click += new System.EventHandler(this.btnSelectFiles_Click);
			// 
			// clusterFileDialog
			// 
			this.clusterFileDialog.Filter = "Sensor Cluster Configurations|*.cfg";
			// 
			// sourceFileDialog
			// 
			this.sourceFileDialog.Filter = "Signal Recordings|*.csv";
			this.sourceFileDialog.Multiselect = true;
			// 
			// lblOutput
			// 
			this.lblOutput.AutoSize = true;
			this.lblOutput.Enabled = false;
			this.lblOutput.Location = new System.Drawing.Point(10, 272);
			this.lblOutput.Name = "lblOutput";
			this.lblOutput.Size = new System.Drawing.Size(124, 13);
			this.lblOutput.TabIndex = 4;
			this.lblOutput.Text = "Nothing\'s happening yet.";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(432, 294);
			this.Controls.Add(this.lblOutput);
			this.Controls.Add(this.btnSelectFiles);
			this.Controls.Add(this.btnSelectCluster);
			this.Controls.Add(this.btnTrim);
			this.Controls.Add(this.cbSignals);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "MainForm";
			this.Text = "Batch Data Trimming";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox cbSignals;
		private System.Windows.Forms.Button btnTrim;
		private System.Windows.Forms.Button btnSelectCluster;
		private System.Windows.Forms.Button btnSelectFiles;
		private System.Windows.Forms.OpenFileDialog clusterFileDialog;
		private System.Windows.Forms.OpenFileDialog sourceFileDialog;
		private System.Windows.Forms.Label lblOutput;
	}
}

