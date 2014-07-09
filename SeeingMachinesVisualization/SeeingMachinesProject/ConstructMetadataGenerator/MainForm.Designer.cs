namespace ConstructMetadataGenerator
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
			this.tbSensorName = new System.Windows.Forms.TextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lbDataTypes = new System.Windows.Forms.ListBox();
			this.btnAddType = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnRemoveType = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.dgTypeMembers = new System.Windows.Forms.DataGridView();
			this.MemberNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MemberType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnRandomTypeGUID = new System.Windows.Forms.Button();
			this.tbSourceType = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cbAutogenerateTypeName = new System.Windows.Forms.CheckBox();
			this.tbSourceAssembly = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbDataTypeName = new System.Windows.Forms.TextBox();
			this.tbDataTypeID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.selectAssemblyDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveSensorDialog = new System.Windows.Forms.SaveFileDialog();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sensorPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgTypeMembers)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbSensorName
			// 
			this.tbSensorName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSensorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbSensorName.Location = new System.Drawing.Point(12, 27);
			this.tbSensorName.Name = "tbSensorName";
			this.tbSensorName.Size = new System.Drawing.Size(656, 29);
			this.tbSensorName.TabIndex = 0;
			this.tbSensorName.Text = "sensorName";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(680, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.newToolStripMenuItem.Text = "New";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// lbDataTypes
			// 
			this.lbDataTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbDataTypes.FormattingEnabled = true;
			this.lbDataTypes.IntegralHeight = false;
			this.lbDataTypes.Location = new System.Drawing.Point(3, 3);
			this.lbDataTypes.Name = "lbDataTypes";
			this.lbDataTypes.Size = new System.Drawing.Size(169, 356);
			this.lbDataTypes.TabIndex = 3;
			this.lbDataTypes.SelectedIndexChanged += new System.EventHandler(this.lbDataTypes_SelectedIndexChanged);
			// 
			// btnAddType
			// 
			this.btnAddType.Location = new System.Drawing.Point(6, 19);
			this.btnAddType.Name = "btnAddType";
			this.btnAddType.Size = new System.Drawing.Size(75, 23);
			this.btnAddType.TabIndex = 4;
			this.btnAddType.Text = "Add";
			this.btnAddType.UseVisualStyleBackColor = true;
			this.btnAddType.Click += new System.EventHandler(this.btnAddType_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnRemoveType);
			this.groupBox1.Controls.Add(this.btnAddType);
			this.groupBox1.Location = new System.Drawing.Point(3, 365);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(169, 50);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data Type Management";
			// 
			// btnRemoveType
			// 
			this.btnRemoveType.Enabled = false;
			this.btnRemoveType.Location = new System.Drawing.Point(87, 19);
			this.btnRemoveType.Name = "btnRemoveType";
			this.btnRemoveType.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveType.TabIndex = 5;
			this.btnRemoveType.Text = "Remove";
			this.btnRemoveType.UseVisualStyleBackColor = true;
			this.btnRemoveType.Click += new System.EventHandler(this.btnRemoveType_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(12, 62);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel1.Controls.Add(this.lbDataTypes);
			this.splitContainer1.Panel1MinSize = 175;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel2MinSize = 175;
			this.splitContainer1.Size = new System.Drawing.Size(656, 418);
			this.splitContainer1.SplitterDistance = 175;
			this.splitContainer1.TabIndex = 6;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.dgTypeMembers);
			this.groupBox3.Location = new System.Drawing.Point(3, 193);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(471, 222);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Type Members";
			// 
			// dgTypeMembers
			// 
			this.dgTypeMembers.AllowUserToAddRows = false;
			this.dgTypeMembers.AllowUserToDeleteRows = false;
			this.dgTypeMembers.AllowUserToResizeRows = false;
			this.dgTypeMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgTypeMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgTypeMembers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberNameColumn,
            this.MemberType});
			this.dgTypeMembers.Location = new System.Drawing.Point(6, 19);
			this.dgTypeMembers.Name = "dgTypeMembers";
			this.dgTypeMembers.ReadOnly = true;
			this.dgTypeMembers.RowHeadersVisible = false;
			this.dgTypeMembers.ShowCellErrors = false;
			this.dgTypeMembers.ShowCellToolTips = false;
			this.dgTypeMembers.ShowEditingIcon = false;
			this.dgTypeMembers.ShowRowErrors = false;
			this.dgTypeMembers.Size = new System.Drawing.Size(459, 197);
			this.dgTypeMembers.TabIndex = 0;
			// 
			// MemberNameColumn
			// 
			this.MemberNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.MemberNameColumn.HeaderText = "Name";
			this.MemberNameColumn.Name = "MemberNameColumn";
			this.MemberNameColumn.ReadOnly = true;
			// 
			// MemberType
			// 
			this.MemberType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.MemberType.HeaderText = "Data Type";
			this.MemberType.Name = "MemberType";
			this.MemberType.ReadOnly = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.btnRandomTypeGUID);
			this.groupBox2.Controls.Add(this.tbSourceType);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.cbAutogenerateTypeName);
			this.groupBox2.Controls.Add(this.tbSourceAssembly);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.tbDataTypeName);
			this.groupBox2.Controls.Add(this.tbDataTypeID);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(471, 184);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Type Information";
			// 
			// btnRandomTypeGUID
			// 
			this.btnRandomTypeGUID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRandomTypeGUID.Location = new System.Drawing.Point(347, 31);
			this.btnRandomTypeGUID.Name = "btnRandomTypeGUID";
			this.btnRandomTypeGUID.Size = new System.Drawing.Size(118, 23);
			this.btnRandomTypeGUID.TabIndex = 10;
			this.btnRandomTypeGUID.Text = "Generate Random";
			this.btnRandomTypeGUID.UseVisualStyleBackColor = true;
			this.btnRandomTypeGUID.Click += new System.EventHandler(this.btnRandomTypeGUID_Click);
			// 
			// tbSourceType
			// 
			this.tbSourceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSourceType.Location = new System.Drawing.Point(6, 157);
			this.tbSourceType.Name = "tbSourceType";
			this.tbSourceType.ReadOnly = true;
			this.tbSourceType.Size = new System.Drawing.Size(459, 20);
			this.tbSourceType.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 141);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Source Type";
			// 
			// cbAutogenerateTypeName
			// 
			this.cbAutogenerateTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbAutogenerateTypeName.AutoSize = true;
			this.cbAutogenerateTypeName.Checked = true;
			this.cbAutogenerateTypeName.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAutogenerateTypeName.Location = new System.Drawing.Point(347, 62);
			this.cbAutogenerateTypeName.Name = "cbAutogenerateTypeName";
			this.cbAutogenerateTypeName.Size = new System.Drawing.Size(90, 17);
			this.cbAutogenerateTypeName.TabIndex = 7;
			this.cbAutogenerateTypeName.Text = "Autogenerate";
			this.cbAutogenerateTypeName.UseVisualStyleBackColor = true;
			this.cbAutogenerateTypeName.CheckedChanged += new System.EventHandler(this.cbAutogenerateTypeName_CheckedChanged);
			// 
			// tbSourceAssembly
			// 
			this.tbSourceAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSourceAssembly.Location = new System.Drawing.Point(6, 118);
			this.tbSourceAssembly.Name = "tbSourceAssembly";
			this.tbSourceAssembly.ReadOnly = true;
			this.tbSourceAssembly.Size = new System.Drawing.Size(459, 20);
			this.tbSourceAssembly.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 102);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Source Assembly";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Type Name";
			// 
			// tbDataTypeName
			// 
			this.tbDataTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDataTypeName.Location = new System.Drawing.Point(74, 59);
			this.tbDataTypeName.Name = "tbDataTypeName";
			this.tbDataTypeName.Size = new System.Drawing.Size(267, 20);
			this.tbDataTypeName.TabIndex = 2;
			// 
			// tbDataTypeID
			// 
			this.tbDataTypeID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDataTypeID.Location = new System.Drawing.Point(74, 33);
			this.tbDataTypeID.Name = "tbDataTypeID";
			this.tbDataTypeID.Size = new System.Drawing.Size(267, 20);
			this.tbDataTypeID.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Type GUID";
			// 
			// selectAssemblyDialog
			// 
			this.selectAssemblyDialog.Filter = "DLL Files|*.dll|Executable Files|*.exe|All Files|*.*";
			// 
			// saveSensorDialog
			// 
			this.saveSensorDialog.Filter = "Construct Sensor Definition|*.xml|All Files|*.*";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sensorPropertiesToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// sensorPropertiesToolStripMenuItem
			// 
			this.sensorPropertiesToolStripMenuItem.Name = "sensorPropertiesToolStripMenuItem";
			this.sensorPropertiesToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.sensorPropertiesToolStripMenuItem.Text = "Sensor Properties";
			this.sensorPropertiesToolStripMenuItem.Click += new System.EventHandler(this.sensorPropertiesToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(680, 492);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.tbSensorName);
			this.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "Construct Sensor Generator";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgTypeMembers)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbSensorName;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ListBox lbDataTypes;
		private System.Windows.Forms.Button btnAddType;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnRemoveType;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox tbSourceAssembly;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbDataTypeName;
		private System.Windows.Forms.TextBox tbDataTypeID;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbAutogenerateTypeName;
		private System.Windows.Forms.OpenFileDialog selectAssemblyDialog;
		private System.Windows.Forms.TextBox tbSourceType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnRandomTypeGUID;
		private System.Windows.Forms.DataGridView dgTypeMembers;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberNameColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberType;
		private System.Windows.Forms.SaveFileDialog saveSensorDialog;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sensorPropertiesToolStripMenuItem;
	}
}