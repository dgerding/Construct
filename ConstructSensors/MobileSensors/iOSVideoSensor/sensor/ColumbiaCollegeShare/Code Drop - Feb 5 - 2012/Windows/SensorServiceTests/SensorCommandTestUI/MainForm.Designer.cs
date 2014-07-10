namespace SensorCommandTestUI
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
            this.components = new System.ComponentModel.Container();
            this.buttonStart = new System.Windows.Forms.Button();
            this.listViewSensors = new System.Windows.Forms.ListView();
            this.columnHeaderSensor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIsConnected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPendingCommand = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonShowCaptures = new System.Windows.Forms.Button();
            this.listViewMessages = new System.Windows.Forms.ListView();
            this.columnHeaderMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.comboBoxResolution = new System.Windows.Forms.ComboBox();
            this.labelResolution = new System.Windows.Forms.Label();
            this.buttonShowAllSensors = new System.Windows.Forms.Button();
            this.buttonLogs = new System.Windows.Forms.Button();
            this.buttonTools = new System.Windows.Forms.Button();
            this.buttonShowStreamsInProgress = new System.Windows.Forms.Button();
            this.groupBoxCurrentSensor = new System.Windows.Forms.GroupBox();
            this.groupBoxGlobalActions = new System.Windows.Forms.GroupBox();
            this.groupBoxCurrentSensor.SuspendLayout();
            this.groupBoxGlobalActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(14, 19);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(149, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // listViewSensors
            // 
            this.listViewSensors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSensors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderSensor,
            this.columnHeaderIsConnected,
            this.columnHeaderPendingCommand});
            this.listViewSensors.FullRowSelect = true;
            this.listViewSensors.HideSelection = false;
            this.listViewSensors.Location = new System.Drawing.Point(12, 13);
            this.listViewSensors.MultiSelect = false;
            this.listViewSensors.Name = "listViewSensors";
            this.listViewSensors.Size = new System.Drawing.Size(664, 244);
            this.listViewSensors.TabIndex = 1;
            this.listViewSensors.UseCompatibleStateImageBehavior = false;
            this.listViewSensors.View = System.Windows.Forms.View.Details;
            this.listViewSensors.SelectedIndexChanged += new System.EventHandler(this.listViewSensors_SelectedIndexChanged);
            // 
            // columnHeaderSensor
            // 
            this.columnHeaderSensor.Text = "Sensor";
            this.columnHeaderSensor.Width = 536;
            // 
            // columnHeaderIsConnected
            // 
            this.columnHeaderIsConnected.Text = "Is Connected";
            // 
            // columnHeaderPendingCommand
            // 
            this.columnHeaderPendingCommand.Text = "Pending Command";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Location = new System.Drawing.Point(14, 48);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(149, 23);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonShowCaptures
            // 
            this.buttonShowCaptures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowCaptures.Location = new System.Drawing.Point(14, 132);
            this.buttonShowCaptures.Name = "buttonShowCaptures";
            this.buttonShowCaptures.Size = new System.Drawing.Size(152, 23);
            this.buttonShowCaptures.TabIndex = 3;
            this.buttonShowCaptures.Text = "Show Recorded Streams";
            this.buttonShowCaptures.UseVisualStyleBackColor = true;
            this.buttonShowCaptures.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // listViewMessages
            // 
            this.listViewMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderMessage});
            this.listViewMessages.Location = new System.Drawing.Point(12, 263);
            this.listViewMessages.Name = "listViewMessages";
            this.listViewMessages.Size = new System.Drawing.Size(664, 196);
            this.listViewMessages.TabIndex = 4;
            this.listViewMessages.UseCompatibleStateImageBehavior = false;
            this.listViewMessages.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderMessage
            // 
            this.columnHeaderMessage.Text = "Messages";
            // 
            // timer
            // 
            this.timer.Interval = 5000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // comboBoxResolution
            // 
            this.comboBoxResolution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxResolution.FormattingEnabled = true;
            this.comboBoxResolution.Location = new System.Drawing.Point(14, 95);
            this.comboBoxResolution.Name = "comboBoxResolution";
            this.comboBoxResolution.Size = new System.Drawing.Size(152, 21);
            this.comboBoxResolution.TabIndex = 5;
            // 
            // labelResolution
            // 
            this.labelResolution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResolution.AutoSize = true;
            this.labelResolution.Location = new System.Drawing.Point(14, 79);
            this.labelResolution.Name = "labelResolution";
            this.labelResolution.Size = new System.Drawing.Size(93, 13);
            this.labelResolution.TabIndex = 6;
            this.labelResolution.Text = "Stream Resolution";
            // 
            // buttonShowAllSensors
            // 
            this.buttonShowAllSensors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowAllSensors.Location = new System.Drawing.Point(13, 19);
            this.buttonShowAllSensors.Name = "buttonShowAllSensors";
            this.buttonShowAllSensors.Size = new System.Drawing.Size(155, 23);
            this.buttonShowAllSensors.TabIndex = 7;
            this.buttonShowAllSensors.Text = "All Sensor Data";
            this.buttonShowAllSensors.UseVisualStyleBackColor = true;
            this.buttonShowAllSensors.Click += new System.EventHandler(this.buttonShowAllSensors_Click);
            // 
            // buttonLogs
            // 
            this.buttonLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLogs.Location = new System.Drawing.Point(13, 48);
            this.buttonLogs.Name = "buttonLogs";
            this.buttonLogs.Size = new System.Drawing.Size(155, 23);
            this.buttonLogs.TabIndex = 8;
            this.buttonLogs.Text = "Logs";
            this.buttonLogs.UseVisualStyleBackColor = true;
            this.buttonLogs.Click += new System.EventHandler(this.buttonLogs_Click);
            // 
            // buttonTools
            // 
            this.buttonTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTools.Location = new System.Drawing.Point(13, 77);
            this.buttonTools.Name = "buttonTools";
            this.buttonTools.Size = new System.Drawing.Size(155, 23);
            this.buttonTools.TabIndex = 9;
            this.buttonTools.Text = "Media Tools";
            this.buttonTools.UseVisualStyleBackColor = true;
            this.buttonTools.Click += new System.EventHandler(this.buttonTools_Click);
            // 
            // buttonShowStreamsInProgress
            // 
            this.buttonShowStreamsInProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowStreamsInProgress.Location = new System.Drawing.Point(14, 161);
            this.buttonShowStreamsInProgress.Name = "buttonShowStreamsInProgress";
            this.buttonShowStreamsInProgress.Size = new System.Drawing.Size(152, 23);
            this.buttonShowStreamsInProgress.TabIndex = 10;
            this.buttonShowStreamsInProgress.Text = "Show Streams In Progress";
            this.buttonShowStreamsInProgress.UseVisualStyleBackColor = true;
            this.buttonShowStreamsInProgress.Click += new System.EventHandler(this.buttonShowStreamsInProgress_Click);
            // 
            // groupBoxCurrentSensor
            // 
            this.groupBoxCurrentSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCurrentSensor.Controls.Add(this.buttonStart);
            this.groupBoxCurrentSensor.Controls.Add(this.buttonShowStreamsInProgress);
            this.groupBoxCurrentSensor.Controls.Add(this.buttonStop);
            this.groupBoxCurrentSensor.Controls.Add(this.buttonShowCaptures);
            this.groupBoxCurrentSensor.Controls.Add(this.comboBoxResolution);
            this.groupBoxCurrentSensor.Controls.Add(this.labelResolution);
            this.groupBoxCurrentSensor.Location = new System.Drawing.Point(682, 13);
            this.groupBoxCurrentSensor.Name = "groupBoxCurrentSensor";
            this.groupBoxCurrentSensor.Size = new System.Drawing.Size(176, 202);
            this.groupBoxCurrentSensor.TabIndex = 11;
            this.groupBoxCurrentSensor.TabStop = false;
            this.groupBoxCurrentSensor.Text = "Selected Sensor";
            // 
            // groupBoxGlobalActions
            // 
            this.groupBoxGlobalActions.Controls.Add(this.buttonShowAllSensors);
            this.groupBoxGlobalActions.Controls.Add(this.buttonLogs);
            this.groupBoxGlobalActions.Controls.Add(this.buttonTools);
            this.groupBoxGlobalActions.Location = new System.Drawing.Point(683, 263);
            this.groupBoxGlobalActions.Name = "groupBoxGlobalActions";
            this.groupBoxGlobalActions.Size = new System.Drawing.Size(175, 110);
            this.groupBoxGlobalActions.TabIndex = 12;
            this.groupBoxGlobalActions.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 471);
            this.Controls.Add(this.groupBoxGlobalActions);
            this.Controls.Add(this.groupBoxCurrentSensor);
            this.Controls.Add(this.listViewMessages);
            this.Controls.Add(this.listViewSensors);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "MainForm";
            this.Text = "Sensor Monitor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.groupBoxCurrentSensor.ResumeLayout(false);
            this.groupBoxCurrentSensor.PerformLayout();
            this.groupBoxGlobalActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ListView listViewSensors;
        private System.Windows.Forms.ColumnHeader columnHeaderSensor;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonShowCaptures;
        private System.Windows.Forms.ListView listViewMessages;
        private System.Windows.Forms.ColumnHeader columnHeaderMessage;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ColumnHeader columnHeaderIsConnected;
        private System.Windows.Forms.ColumnHeader columnHeaderPendingCommand;
        private System.Windows.Forms.ComboBox comboBoxResolution;
        private System.Windows.Forms.Label labelResolution;
        private System.Windows.Forms.Button buttonShowAllSensors;
        private System.Windows.Forms.Button buttonLogs;
        private System.Windows.Forms.Button buttonTools;
        private System.Windows.Forms.Button buttonShowStreamsInProgress;
        private System.Windows.Forms.GroupBox groupBoxCurrentSensor;
        private System.Windows.Forms.GroupBox groupBoxGlobalActions;
    }
}

