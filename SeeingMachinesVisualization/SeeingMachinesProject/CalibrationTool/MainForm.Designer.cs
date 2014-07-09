namespace CalibrationTool
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
			this.scMainContainer = new System.Windows.Forms.SplitContainer();
			this.lbChunks = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.gbRecording = new System.Windows.Forms.GroupBox();
			this.tbFileSource = new System.Windows.Forms.TextBox();
			this.btnSetSource = new System.Windows.Forms.Button();
			this.gbInputMethod = new System.Windows.Forms.GroupBox();
			this.rbRecording = new System.Windows.Forms.RadioButton();
			this.rbStream = new System.Windows.Forms.RadioButton();
			this.pgLiveStatistics = new System.Windows.Forms.PropertyGrid();
			this.gbLiveStream = new System.Windows.Forms.GroupBox();
			this.btnForceStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnNew = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnGenerate = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).BeginInit();
			this.scMainContainer.Panel1.SuspendLayout();
			this.scMainContainer.Panel2.SuspendLayout();
			this.scMainContainer.SuspendLayout();
			this.gbRecording.SuspendLayout();
			this.gbInputMethod.SuspendLayout();
			this.gbLiveStream.SuspendLayout();
			this.SuspendLayout();
			// 
			// scMainContainer
			// 
			this.scMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scMainContainer.Location = new System.Drawing.Point(0, 41);
			this.scMainContainer.Name = "scMainContainer";
			// 
			// scMainContainer.Panel1
			// 
			this.scMainContainer.Panel1.Controls.Add(this.lbChunks);
			// 
			// scMainContainer.Panel2
			// 
			this.scMainContainer.Panel2.Controls.Add(this.label1);
			this.scMainContainer.Panel2.Controls.Add(this.gbRecording);
			this.scMainContainer.Panel2.Controls.Add(this.gbInputMethod);
			this.scMainContainer.Panel2.Controls.Add(this.pgLiveStatistics);
			this.scMainContainer.Panel2.Controls.Add(this.gbLiveStream);
			this.scMainContainer.Size = new System.Drawing.Size(659, 462);
			this.scMainContainer.SplitterDistance = 219;
			this.scMainContainer.TabIndex = 0;
			// 
			// lbChunks
			// 
			this.lbChunks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbChunks.FormattingEnabled = true;
			this.lbChunks.Location = new System.Drawing.Point(0, 0);
			this.lbChunks.Name = "lbChunks";
			this.lbChunks.Size = new System.Drawing.Size(219, 462);
			this.lbChunks.TabIndex = 0;
			this.lbChunks.SelectedIndexChanged += new System.EventHandler(this.lbChunks_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 199);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Data Results";
			// 
			// gbRecording
			// 
			this.gbRecording.Controls.Add(this.tbFileSource);
			this.gbRecording.Controls.Add(this.btnSetSource);
			this.gbRecording.Location = new System.Drawing.Point(3, 126);
			this.gbRecording.Name = "gbRecording";
			this.gbRecording.Size = new System.Drawing.Size(430, 57);
			this.gbRecording.TabIndex = 7;
			this.gbRecording.TabStop = false;
			this.gbRecording.Text = "Recording Options";
			// 
			// tbFileSource
			// 
			this.tbFileSource.Location = new System.Drawing.Point(96, 21);
			this.tbFileSource.Name = "tbFileSource";
			this.tbFileSource.Size = new System.Drawing.Size(325, 20);
			this.tbFileSource.TabIndex = 1;
			// 
			// btnSetSource
			// 
			this.btnSetSource.Location = new System.Drawing.Point(6, 19);
			this.btnSetSource.Name = "btnSetSource";
			this.btnSetSource.Size = new System.Drawing.Size(84, 23);
			this.btnSetSource.TabIndex = 0;
			this.btnSetSource.Text = "Select Source";
			this.btnSetSource.UseVisualStyleBackColor = true;
			// 
			// gbInputMethod
			// 
			this.gbInputMethod.Controls.Add(this.rbRecording);
			this.gbInputMethod.Controls.Add(this.rbStream);
			this.gbInputMethod.Location = new System.Drawing.Point(3, 3);
			this.gbInputMethod.Name = "gbInputMethod";
			this.gbInputMethod.Size = new System.Drawing.Size(430, 49);
			this.gbInputMethod.TabIndex = 6;
			this.gbInputMethod.TabStop = false;
			this.gbInputMethod.Text = "Input Method";
			// 
			// rbRecording
			// 
			this.rbRecording.AutoSize = true;
			this.rbRecording.Location = new System.Drawing.Point(93, 19);
			this.rbRecording.Name = "rbRecording";
			this.rbRecording.Size = new System.Drawing.Size(74, 17);
			this.rbRecording.TabIndex = 5;
			this.rbRecording.Text = "Recording";
			this.rbRecording.UseVisualStyleBackColor = true;
			this.rbRecording.CheckedChanged += new System.EventHandler(this.rbRecording_CheckedChanged);
			// 
			// rbStream
			// 
			this.rbStream.AutoSize = true;
			this.rbStream.Checked = true;
			this.rbStream.Location = new System.Drawing.Point(6, 19);
			this.rbStream.Name = "rbStream";
			this.rbStream.Size = new System.Drawing.Size(81, 17);
			this.rbStream.TabIndex = 4;
			this.rbStream.TabStop = true;
			this.rbStream.Text = "Live Stream";
			this.rbStream.UseVisualStyleBackColor = true;
			this.rbStream.CheckedChanged += new System.EventHandler(this.rbStream_CheckedChanged);
			// 
			// pgLiveStatistics
			// 
			this.pgLiveStatistics.HelpVisible = false;
			this.pgLiveStatistics.Location = new System.Drawing.Point(9, 215);
			this.pgLiveStatistics.Name = "pgLiveStatistics";
			this.pgLiveStatistics.Size = new System.Drawing.Size(415, 235);
			this.pgLiveStatistics.TabIndex = 2;
			this.pgLiveStatistics.ToolbarVisible = false;
			// 
			// gbLiveStream
			// 
			this.gbLiveStream.Controls.Add(this.btnForceStop);
			this.gbLiveStream.Controls.Add(this.btnStart);
			this.gbLiveStream.Location = new System.Drawing.Point(3, 58);
			this.gbLiveStream.Name = "gbLiveStream";
			this.gbLiveStream.Size = new System.Drawing.Size(430, 62);
			this.gbLiveStream.TabIndex = 3;
			this.gbLiveStream.TabStop = false;
			this.gbLiveStream.Text = "Live Stream Options";
			// 
			// btnForceStop
			// 
			this.btnForceStop.Location = new System.Drawing.Point(107, 19);
			this.btnForceStop.Name = "btnForceStop";
			this.btnForceStop.Size = new System.Drawing.Size(75, 23);
			this.btnForceStop.TabIndex = 1;
			this.btnForceStop.Text = "Force Stop";
			this.btnForceStop.UseVisualStyleBackColor = true;
			this.btnForceStop.Click += new System.EventHandler(this.btnForceStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(15, 19);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnNew
			// 
			this.btnNew.Location = new System.Drawing.Point(12, 12);
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(75, 23);
			this.btnNew.TabIndex = 1;
			this.btnNew.Text = "Add New";
			this.btnNew.UseVisualStyleBackColor = true;
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(93, 12);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(75, 23);
			this.btnDelete.TabIndex = 2;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerate.Location = new System.Drawing.Point(572, 12);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(75, 23);
			this.btnGenerate.TabIndex = 3;
			this.btnGenerate.Text = "Generate";
			this.btnGenerate.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(659, 503);
			this.Controls.Add(this.btnGenerate);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.scMainContainer);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.scMainContainer.Panel1.ResumeLayout(false);
			this.scMainContainer.Panel2.ResumeLayout(false);
			this.scMainContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).EndInit();
			this.scMainContainer.ResumeLayout(false);
			this.gbRecording.ResumeLayout(false);
			this.gbRecording.PerformLayout();
			this.gbInputMethod.ResumeLayout(false);
			this.gbInputMethod.PerformLayout();
			this.gbLiveStream.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer scMainContainer;
		private System.Windows.Forms.PropertyGrid pgLiveStatistics;
		private System.Windows.Forms.Button btnForceStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.ListBox lbChunks;
		private System.Windows.Forms.GroupBox gbInputMethod;
		private System.Windows.Forms.RadioButton rbRecording;
		private System.Windows.Forms.RadioButton rbStream;
		private System.Windows.Forms.GroupBox gbLiveStream;
		private System.Windows.Forms.GroupBox gbRecording;
		private System.Windows.Forms.TextBox tbFileSource;
		private System.Windows.Forms.Button btnSetSource;
		private System.Windows.Forms.Label label1;
	}
}