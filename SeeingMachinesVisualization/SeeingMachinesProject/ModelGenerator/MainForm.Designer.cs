namespace BayesDesigner
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
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnRemoveClass = new System.Windows.Forms.Button();
			this.btnAddClass = new System.Windows.Forms.Button();
			this.lbClassList = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnAddSamples = new System.Windows.Forms.Button();
			this.btnRemoveSamples = new System.Windows.Forms.Button();
			this.lbSampleList = new System.Windows.Forms.ListBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnGenerate = new System.Windows.Forms.Button();
			this.lblGenerate = new System.Windows.Forms.Label();
			this.clbCluster = new System.Windows.Forms.CheckedListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnSetCluster = new System.Windows.Forms.Button();
			this.clusterFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.samplesFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.modelSaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.cbxSaveReport = new System.Windows.Forms.CheckBox();
			this.saveReportDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Location = new System.Drawing.Point(212, 12);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.groupBox2);
			this.splitContainer.Panel1.Controls.Add(this.lbClassList);
			this.splitContainer.Panel1MinSize = 180;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer.Panel2.Controls.Add(this.lbSampleList);
			this.splitContainer.Panel2MinSize = 180;
			this.splitContainer.Size = new System.Drawing.Size(666, 431);
			this.splitContainer.SplitterDistance = 260;
			this.splitContainer.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.btnClear);
			this.groupBox2.Controls.Add(this.btnRemoveClass);
			this.groupBox2.Controls.Add(this.btnAddClass);
			this.groupBox2.Location = new System.Drawing.Point(4, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(253, 52);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Class Controls";
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(168, 19);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(75, 23);
			this.btnClear.TabIndex = 2;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnRemoveClass
			// 
			this.btnRemoveClass.Location = new System.Drawing.Point(87, 19);
			this.btnRemoveClass.Name = "btnRemoveClass";
			this.btnRemoveClass.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveClass.TabIndex = 1;
			this.btnRemoveClass.Text = "Remove";
			this.btnRemoveClass.UseVisualStyleBackColor = true;
			this.btnRemoveClass.Click += new System.EventHandler(this.btnRemoveClass_Click);
			// 
			// btnAddClass
			// 
			this.btnAddClass.Location = new System.Drawing.Point(6, 19);
			this.btnAddClass.Name = "btnAddClass";
			this.btnAddClass.Size = new System.Drawing.Size(75, 23);
			this.btnAddClass.TabIndex = 0;
			this.btnAddClass.Text = "Add";
			this.btnAddClass.UseVisualStyleBackColor = true;
			this.btnAddClass.Click += new System.EventHandler(this.btnAddClass_Click);
			// 
			// lbClassList
			// 
			this.lbClassList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbClassList.FormattingEnabled = true;
			this.lbClassList.IntegralHeight = false;
			this.lbClassList.Location = new System.Drawing.Point(4, 58);
			this.lbClassList.Name = "lbClassList";
			this.lbClassList.Size = new System.Drawing.Size(253, 367);
			this.lbClassList.TabIndex = 0;
			this.lbClassList.SelectedIndexChanged += new System.EventHandler(this.lbClassList_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnAddSamples);
			this.groupBox1.Controls.Add(this.btnRemoveSamples);
			this.groupBox1.Location = new System.Drawing.Point(3, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(396, 52);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data Sample Controls";
			// 
			// btnAddSamples
			// 
			this.btnAddSamples.Location = new System.Drawing.Point(6, 19);
			this.btnAddSamples.Name = "btnAddSamples";
			this.btnAddSamples.Size = new System.Drawing.Size(75, 23);
			this.btnAddSamples.TabIndex = 1;
			this.btnAddSamples.Text = "Add";
			this.btnAddSamples.UseVisualStyleBackColor = true;
			this.btnAddSamples.Click += new System.EventHandler(this.btnAddSamples_Click);
			// 
			// btnRemoveSamples
			// 
			this.btnRemoveSamples.Location = new System.Drawing.Point(87, 19);
			this.btnRemoveSamples.Name = "btnRemoveSamples";
			this.btnRemoveSamples.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveSamples.TabIndex = 2;
			this.btnRemoveSamples.Text = "Remove";
			this.btnRemoveSamples.UseVisualStyleBackColor = true;
			this.btnRemoveSamples.Click += new System.EventHandler(this.btnRemoveSamples_Click);
			// 
			// lbSampleList
			// 
			this.lbSampleList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbSampleList.FormattingEnabled = true;
			this.lbSampleList.IntegralHeight = false;
			this.lbSampleList.Location = new System.Drawing.Point(3, 58);
			this.lbSampleList.Name = "lbSampleList";
			this.lbSampleList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbSampleList.Size = new System.Drawing.Size(396, 367);
			this.lbSampleList.TabIndex = 0;
			this.lbSampleList.SelectedIndexChanged += new System.EventHandler(this.lbSampleList_SelectedIndexChanged);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(746, 447);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(132, 32);
			this.btnSave.TabIndex = 2;
			this.btnSave.Text = "Save Classifier";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnGenerate.Location = new System.Drawing.Point(18, 448);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(182, 42);
			this.btnGenerate.TabIndex = 3;
			this.btnGenerate.Text = "Generate Bayes Classifier";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// lblGenerate
			// 
			this.lblGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblGenerate.Enabled = false;
			this.lblGenerate.Location = new System.Drawing.Point(12, 493);
			this.lblGenerate.Name = "lblGenerate";
			this.lblGenerate.Size = new System.Drawing.Size(194, 13);
			this.lblGenerate.TabIndex = 4;
			this.lblGenerate.Text = "No Output";
			this.lblGenerate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// clbCluster
			// 
			this.clbCluster.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clbCluster.CheckOnClick = true;
			this.clbCluster.FormattingEnabled = true;
			this.clbCluster.IntegralHeight = false;
			this.clbCluster.Location = new System.Drawing.Point(6, 58);
			this.clbCluster.Name = "clbCluster";
			this.clbCluster.Size = new System.Drawing.Size(182, 367);
			this.clbCluster.TabIndex = 5;
			this.clbCluster.SelectedIndexChanged += new System.EventHandler(this.clbCluster_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox3.Controls.Add(this.btnSetCluster);
			this.groupBox3.Controls.Add(this.clbCluster);
			this.groupBox3.Location = new System.Drawing.Point(12, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(194, 431);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Source Sensor Information";
			// 
			// btnSetCluster
			// 
			this.btnSetCluster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSetCluster.Location = new System.Drawing.Point(6, 19);
			this.btnSetCluster.Name = "btnSetCluster";
			this.btnSetCluster.Size = new System.Drawing.Size(176, 23);
			this.btnSetCluster.TabIndex = 6;
			this.btnSetCluster.Text = "Set Cluster";
			this.btnSetCluster.UseVisualStyleBackColor = true;
			this.btnSetCluster.Click += new System.EventHandler(this.btnSetCluster_Click);
			// 
			// clusterFileDialog
			// 
			this.clusterFileDialog.Filter = "Cluster Config Files|*.cfg|All Files|*.*";
			// 
			// samplesFileDialog
			// 
			this.samplesFileDialog.Filter = "CSV Recordings|*.csv|All Files|*.*";
			this.samplesFileDialog.Multiselect = true;
			// 
			// modelSaveDialog
			// 
			this.modelSaveDialog.Filter = "Machine Learning Files|*.ml|All Files|*.*";
			// 
			// cbxSaveReport
			// 
			this.cbxSaveReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbxSaveReport.AutoSize = true;
			this.cbxSaveReport.Location = new System.Drawing.Point(746, 485);
			this.cbxSaveReport.Name = "cbxSaveReport";
			this.cbxSaveReport.Size = new System.Drawing.Size(132, 17);
			this.cbxSaveReport.TabIndex = 7;
			this.cbxSaveReport.Text = "Save Report CSV Too";
			this.cbxSaveReport.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(889, 514);
			this.Controls.Add(this.cbxSaveReport);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.lblGenerate);
			this.Controls.Add(this.btnGenerate);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.splitContainer);
			this.Name = "MainForm";
			this.Text = "Bayes Designer";
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lbClassList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAddSamples;
        private System.Windows.Forms.Button btnRemoveSamples;
        private System.Windows.Forms.ListBox lbSampleList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAddClass;
        private System.Windows.Forms.Button btnRemoveClass;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblGenerate;
        private System.Windows.Forms.CheckedListBox clbCluster;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSetCluster;
        private System.Windows.Forms.OpenFileDialog clusterFileDialog;
        private System.Windows.Forms.OpenFileDialog samplesFileDialog;
        private System.Windows.Forms.SaveFileDialog modelSaveDialog;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.CheckBox cbxSaveReport;
		private System.Windows.Forms.SaveFileDialog saveReportDialog;
    }
}

