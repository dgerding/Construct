namespace SMVisualization
{
	partial class ScrubberForm
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
			this.barPlaybackLocation = new System.Windows.Forms.TrackBar();
			this.btnPlayPause = new System.Windows.Forms.Button();
			this.lblPlayTime = new System.Windows.Forms.Label();
			this.btnSetSource = new System.Windows.Forms.Button();
			this.lblDataSourcePath = new System.Windows.Forms.Label();
			this.chbxUseRecording = new System.Windows.Forms.CheckBox();
			this.btnRestart = new System.Windows.Forms.Button();
			this.btnDataLocation = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnClearRecording = new System.Windows.Forms.Button();
			this.btnSignalRStartStop = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxServer = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblSignalRState = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbGlobalSource = new System.Windows.Forms.CheckBox();
			this.bayesOpenDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.clbBayesSources = new System.Windows.Forms.CheckedListBox();
			this.btnRemoveBayes = new System.Windows.Forms.Button();
			this.btnAddBayes = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.saveRecordingDialog = new System.Windows.Forms.SaveFileDialog();
			this.btnShowRenderOptions = new System.Windows.Forms.Button();
			this.btnReloadCluster = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.barPlaybackLocation)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// barPlaybackLocation
			// 
			this.barPlaybackLocation.Location = new System.Drawing.Point(13, 59);
			this.barPlaybackLocation.Maximum = 0;
			this.barPlaybackLocation.Name = "barPlaybackLocation";
			this.barPlaybackLocation.Size = new System.Drawing.Size(1240, 45);
			this.barPlaybackLocation.TabIndex = 0;
			this.barPlaybackLocation.TabStop = false;
			this.barPlaybackLocation.TickFrequency = 10000;
			this.barPlaybackLocation.TickStyle = System.Windows.Forms.TickStyle.None;
			// 
			// btnPlayPause
			// 
			this.btnPlayPause.Location = new System.Drawing.Point(13, 12);
			this.btnPlayPause.Name = "btnPlayPause";
			this.btnPlayPause.Size = new System.Drawing.Size(75, 23);
			this.btnPlayPause.TabIndex = 1;
			this.btnPlayPause.Text = "Play";
			this.btnPlayPause.UseVisualStyleBackColor = true;
			this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
			// 
			// lblPlayTime
			// 
			this.lblPlayTime.AutoSize = true;
			this.lblPlayTime.Location = new System.Drawing.Point(20, 43);
			this.lblPlayTime.Name = "lblPlayTime";
			this.lblPlayTime.Size = new System.Drawing.Size(72, 13);
			this.lblPlayTime.TabIndex = 5;
			this.lblPlayTime.Text = "0:000 / 0:000";
			// 
			// btnSetSource
			// 
			this.btnSetSource.Location = new System.Drawing.Point(116, 15);
			this.btnSetSource.Name = "btnSetSource";
			this.btnSetSource.Size = new System.Drawing.Size(75, 23);
			this.btnSetSource.TabIndex = 8;
			this.btnSetSource.Text = "Set Source";
			this.btnSetSource.UseVisualStyleBackColor = true;
			this.btnSetSource.Click += new System.EventHandler(this.btnSetSource_Click);
			// 
			// lblDataSourcePath
			// 
			this.lblDataSourcePath.AutoSize = true;
			this.lblDataSourcePath.Enabled = false;
			this.lblDataSourcePath.Location = new System.Drawing.Point(197, 20);
			this.lblDataSourcePath.Name = "lblDataSourcePath";
			this.lblDataSourcePath.Size = new System.Drawing.Size(103, 13);
			this.lblDataSourcePath.TabIndex = 9;
			this.lblDataSourcePath.Text = "No Source Selected";
			// 
			// chbxUseRecording
			// 
			this.chbxUseRecording.AutoSize = true;
			this.chbxUseRecording.Location = new System.Drawing.Point(211, 16);
			this.chbxUseRecording.Name = "chbxUseRecording";
			this.chbxUseRecording.Size = new System.Drawing.Size(97, 17);
			this.chbxUseRecording.TabIndex = 11;
			this.chbxUseRecording.Text = "Use Recording";
			this.chbxUseRecording.UseVisualStyleBackColor = true;
			this.chbxUseRecording.CheckedChanged += new System.EventHandler(this.chbxEnabled_CheckedChanged);
			// 
			// btnRestart
			// 
			this.btnRestart.Location = new System.Drawing.Point(97, 12);
			this.btnRestart.Name = "btnRestart";
			this.btnRestart.Size = new System.Drawing.Size(108, 23);
			this.btnRestart.TabIndex = 12;
			this.btnRestart.Text = "Restart Playback";
			this.btnRestart.UseVisualStyleBackColor = true;
			this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
			// 
			// btnDataLocation
			// 
			this.btnDataLocation.Location = new System.Drawing.Point(228, 19);
			this.btnDataLocation.Name = "btnDataLocation";
			this.btnDataLocation.Size = new System.Drawing.Size(158, 23);
			this.btnDataLocation.TabIndex = 13;
			this.btnDataLocation.Text = "Open Recordings Location";
			this.btnDataLocation.UseVisualStyleBackColor = true;
			this.btnDataLocation.Click += new System.EventHandler(this.btnDataLocation_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(120, 19);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(102, 23);
			this.btnSave.TabIndex = 14;
			this.btnSave.Text = "Save Recording";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnClearRecording
			// 
			this.btnClearRecording.Location = new System.Drawing.Point(6, 19);
			this.btnClearRecording.Name = "btnClearRecording";
			this.btnClearRecording.Size = new System.Drawing.Size(108, 23);
			this.btnClearRecording.TabIndex = 15;
			this.btnClearRecording.Text = "Erase Recording";
			this.btnClearRecording.UseVisualStyleBackColor = true;
			this.btnClearRecording.Click += new System.EventHandler(this.btnClearRecording_Click);
			// 
			// btnSignalRStartStop
			// 
			this.btnSignalRStartStop.Location = new System.Drawing.Point(355, 19);
			this.btnSignalRStartStop.Name = "btnSignalRStartStop";
			this.btnSignalRStartStop.Size = new System.Drawing.Size(75, 23);
			this.btnSignalRStartStop.TabIndex = 16;
			this.btnSignalRStartStop.Text = "Start";
			this.btnSignalRStartStop.UseVisualStyleBackColor = true;
			this.btnSignalRStartStop.Click += new System.EventHandler(this.btnSignalRStartStop_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbxServer);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.lblSignalRState);
			this.groupBox1.Controls.Add(this.btnSignalRStartStop);
			this.groupBox1.Location = new System.Drawing.Point(344, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(523, 50);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "SignalR Synchronization";
			// 
			// cbxServer
			// 
			this.cbxServer.FormattingEnabled = true;
			this.cbxServer.Location = new System.Drawing.Point(50, 21);
			this.cbxServer.Name = "cbxServer";
			this.cbxServer.Size = new System.Drawing.Size(299, 21);
			this.cbxServer.TabIndex = 21;
			this.cbxServer.Text = "http://daisy.colum.edu/ScrubbeR";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 20;
			this.label1.Text = "Server";
			// 
			// lblSignalRState
			// 
			this.lblSignalRState.AutoSize = true;
			this.lblSignalRState.Enabled = false;
			this.lblSignalRState.Location = new System.Drawing.Point(436, 24);
			this.lblSignalRState.Name = "lblSignalRState";
			this.lblSignalRState.Size = new System.Drawing.Size(69, 13);
			this.lblSignalRState.TabIndex = 18;
			this.lblSignalRState.Text = "Not initialized";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnClearRecording);
			this.groupBox2.Controls.Add(this.btnSave);
			this.groupBox2.Controls.Add(this.btnDataLocation);
			this.groupBox2.Location = new System.Drawing.Point(13, 110);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(395, 50);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Recording Management";
			// 
			// cbGlobalSource
			// 
			this.cbGlobalSource.AutoSize = true;
			this.cbGlobalSource.Checked = true;
			this.cbGlobalSource.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbGlobalSource.Location = new System.Drawing.Point(6, 19);
			this.cbGlobalSource.Name = "cbGlobalSource";
			this.cbGlobalSource.Size = new System.Drawing.Size(104, 17);
			this.cbGlobalSource.TabIndex = 20;
			this.cbGlobalSource.Text = "Source Is Global";
			this.cbGlobalSource.UseVisualStyleBackColor = true;
			// 
			// bayesOpenDialog
			// 
			this.bayesOpenDialog.Filter = "Machine Learning Files|*.ml|All Files|*.*";
			this.bayesOpenDialog.Multiselect = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.clbBayesSources);
			this.groupBox3.Controls.Add(this.btnRemoveBayes);
			this.groupBox3.Controls.Add(this.btnAddBayes);
			this.groupBox3.Location = new System.Drawing.Point(740, 110);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(528, 81);
			this.groupBox3.TabIndex = 22;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Bayes Management";
			// 
			// clbBayesSources
			// 
			this.clbBayesSources.CheckOnClick = true;
			this.clbBayesSources.FormattingEnabled = true;
			this.clbBayesSources.Location = new System.Drawing.Point(87, 19);
			this.clbBayesSources.Name = "clbBayesSources";
			this.clbBayesSources.Size = new System.Drawing.Size(435, 49);
			this.clbBayesSources.TabIndex = 24;
			this.clbBayesSources.SelectedIndexChanged += new System.EventHandler(this.clbBayesSources_SelectedIndexChanged);
			// 
			// btnRemoveBayes
			// 
			this.btnRemoveBayes.Enabled = false;
			this.btnRemoveBayes.Location = new System.Drawing.Point(6, 48);
			this.btnRemoveBayes.Name = "btnRemoveBayes";
			this.btnRemoveBayes.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveBayes.TabIndex = 23;
			this.btnRemoveBayes.Text = "Remove";
			this.btnRemoveBayes.UseVisualStyleBackColor = true;
			this.btnRemoveBayes.Click += new System.EventHandler(this.btnRemoveBayes_Click);
			// 
			// btnAddBayes
			// 
			this.btnAddBayes.Location = new System.Drawing.Point(6, 19);
			this.btnAddBayes.Name = "btnAddBayes";
			this.btnAddBayes.Size = new System.Drawing.Size(75, 23);
			this.btnAddBayes.TabIndex = 22;
			this.btnAddBayes.Text = "Add New";
			this.btnAddBayes.UseVisualStyleBackColor = true;
			this.btnAddBayes.Click += new System.EventHandler(this.btnAddBayes_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbGlobalSource);
			this.groupBox4.Controls.Add(this.btnSetSource);
			this.groupBox4.Controls.Add(this.lblDataSourcePath);
			this.groupBox4.Location = new System.Drawing.Point(873, 4);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(395, 49);
			this.groupBox4.TabIndex = 23;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Playback Source";
			// 
			// saveRecordingDialog
			// 
			this.saveRecordingDialog.Filter = "Recording File|*.csv|All Files|*.*";
			// 
			// btnShowRenderOptions
			// 
			this.btnShowRenderOptions.Location = new System.Drawing.Point(452, 110);
			this.btnShowRenderOptions.Name = "btnShowRenderOptions";
			this.btnShowRenderOptions.Size = new System.Drawing.Size(241, 50);
			this.btnShowRenderOptions.TabIndex = 24;
			this.btnShowRenderOptions.Text = "Render Options";
			this.btnShowRenderOptions.UseVisualStyleBackColor = true;
			this.btnShowRenderOptions.Click += new System.EventHandler(this.btnShowRenderOptions_Click);
			// 
			// btnReloadCluster
			// 
			this.btnReloadCluster.Location = new System.Drawing.Point(12, 166);
			this.btnReloadCluster.Name = "btnReloadCluster";
			this.btnReloadCluster.Size = new System.Drawing.Size(96, 23);
			this.btnReloadCluster.TabIndex = 25;
			this.btnReloadCluster.Text = "Reload Cluster";
			this.btnReloadCluster.UseVisualStyleBackColor = true;
			this.btnReloadCluster.Click += new System.EventHandler(this.btnReloadCluster_Click);
			// 
			// ScrubberForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1280, 199);
			this.Controls.Add(this.btnReloadCluster);
			this.Controls.Add(this.btnShowRenderOptions);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnRestart);
			this.Controls.Add(this.chbxUseRecording);
			this.Controls.Add(this.lblPlayTime);
			this.Controls.Add(this.btnPlayPause);
			this.Controls.Add(this.barPlaybackLocation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ScrubberForm";
			this.Text = "Seeing Machines Visualization Playback Manager";
			((System.ComponentModel.ISupportInitialize)(this.barPlaybackLocation)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar barPlaybackLocation;
		private System.Windows.Forms.Button btnPlayPause;
		private System.Windows.Forms.Label lblPlayTime;
		private System.Windows.Forms.Button btnSetSource;
		private System.Windows.Forms.Label lblDataSourcePath;
		private System.Windows.Forms.CheckBox chbxUseRecording;
		private System.Windows.Forms.Button btnRestart;
		private System.Windows.Forms.Button btnDataLocation;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnClearRecording;
		private System.Windows.Forms.Button btnSignalRStartStop;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblSignalRState;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbxServer;
        public System.Windows.Forms.CheckBox cbGlobalSource;
        private System.Windows.Forms.OpenFileDialog bayesOpenDialog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnRemoveBayes;
        private System.Windows.Forms.Button btnAddBayes;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckedListBox clbBayesSources;
		private System.Windows.Forms.SaveFileDialog saveRecordingDialog;
		private System.Windows.Forms.Button btnShowRenderOptions;
		private System.Windows.Forms.Button btnReloadCluster;
	}
}

