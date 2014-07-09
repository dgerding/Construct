namespace CalibrationViewer
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addSensorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeSensorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.manageGUIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeSensorNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lbSensors = new System.Windows.Forms.ListBox();
			this.pnlSensorDataView = new System.Windows.Forms.Panel();
			this.pgSensorView = new System.Windows.Forms.PropertyGrid();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.pnlSensorDataView.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(507, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(146, 22);
			this.toolStripMenuItem1.Text = "New";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSensorToolStripMenuItem,
            this.removeSensorToolStripMenuItem,
            this.manageGUIDToolStripMenuItem,
            this.changeSensorNameToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// addSensorToolStripMenuItem
			// 
			this.addSensorToolStripMenuItem.Name = "addSensorToolStripMenuItem";
			this.addSensorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.addSensorToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.addSensorToolStripMenuItem.Text = "Add Sensor";
			this.addSensorToolStripMenuItem.Click += new System.EventHandler(this.addSensorToolStripMenuItem_Click);
			// 
			// removeSensorToolStripMenuItem
			// 
			this.removeSensorToolStripMenuItem.Name = "removeSensorToolStripMenuItem";
			this.removeSensorToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.removeSensorToolStripMenuItem.Text = "Remove Sensor";
			this.removeSensorToolStripMenuItem.Click += new System.EventHandler(this.removeSensorToolStripMenuItem_Click);
			// 
			// manageGUIDToolStripMenuItem
			// 
			this.manageGUIDToolStripMenuItem.Name = "manageGUIDToolStripMenuItem";
			this.manageGUIDToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.manageGUIDToolStripMenuItem.Text = "Manage GUID";
			this.manageGUIDToolStripMenuItem.Click += new System.EventHandler(this.manageGUIDToolStripMenuItem_Click);
			// 
			// changeSensorNameToolStripMenuItem
			// 
			this.changeSensorNameToolStripMenuItem.Name = "changeSensorNameToolStripMenuItem";
			this.changeSensorNameToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.changeSensorNameToolStripMenuItem.Text = "Change Sensor Name";
			this.changeSensorNameToolStripMenuItem.Click += new System.EventHandler(this.changeSensorNameToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lbSensors);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.pnlSensorDataView);
			this.splitContainer1.Size = new System.Drawing.Size(507, 234);
			this.splitContainer1.SplitterDistance = 231;
			this.splitContainer1.TabIndex = 1;
			// 
			// lbSensors
			// 
			this.lbSensors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbSensors.FormattingEnabled = true;
			this.lbSensors.Location = new System.Drawing.Point(0, 0);
			this.lbSensors.Name = "lbSensors";
			this.lbSensors.Size = new System.Drawing.Size(231, 234);
			this.lbSensors.TabIndex = 0;
			this.lbSensors.SelectedIndexChanged += new System.EventHandler(this.lbSensors_SelectedIndexChanged);
			// 
			// pnlSensorDataView
			// 
			this.pnlSensorDataView.Controls.Add(this.pgSensorView);
			this.pnlSensorDataView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSensorDataView.Location = new System.Drawing.Point(0, 0);
			this.pnlSensorDataView.Name = "pnlSensorDataView";
			this.pnlSensorDataView.Size = new System.Drawing.Size(272, 234);
			this.pnlSensorDataView.TabIndex = 0;
			// 
			// pgSensorView
			// 
			this.pgSensorView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgSensorView.HelpVisible = false;
			this.pgSensorView.Location = new System.Drawing.Point(0, 0);
			this.pgSensorView.Name = "pgSensorView";
			this.pgSensorView.Size = new System.Drawing.Size(272, 234);
			this.pgSensorView.TabIndex = 0;
			this.pgSensorView.ToolbarVisible = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(507, 258);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Sensor Cluster Configuration Viewer";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.pnlSensorDataView.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addSensorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeSensorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manageGUIDToolStripMenuItem;
		private System.Windows.Forms.Panel pnlSensorDataView;
		private System.Windows.Forms.ToolStripMenuItem changeSensorNameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ListBox lbSensors;
		private System.Windows.Forms.PropertyGrid pgSensorView;
	}
}

