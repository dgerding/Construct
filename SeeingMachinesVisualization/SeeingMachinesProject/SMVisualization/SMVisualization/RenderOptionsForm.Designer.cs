namespace SMVisualization
{
	partial class RenderOptionsForm
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
			this.ddlSelectedSubject = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cblRenderOptions = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// ddlSelectedSubject
			// 
			this.ddlSelectedSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ddlSelectedSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlSelectedSubject.FormattingEnabled = true;
			this.ddlSelectedSubject.Items.AddRange(new object[] {
            "All",
            "Subject 1",
            "Subject 2",
            "Subject 3",
            "Subject 4"});
			this.ddlSelectedSubject.Location = new System.Drawing.Point(61, 12);
			this.ddlSelectedSubject.Name = "ddlSelectedSubject";
			this.ddlSelectedSubject.Size = new System.Drawing.Size(287, 21);
			this.ddlSelectedSubject.TabIndex = 0;
			this.ddlSelectedSubject.SelectedIndexChanged += new System.EventHandler(this.ddlSelectedSubject_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Subject";
			// 
			// cblRenderOptions
			// 
			this.cblRenderOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cblRenderOptions.FormattingEnabled = true;
			this.cblRenderOptions.Location = new System.Drawing.Point(15, 39);
			this.cblRenderOptions.Name = "cblRenderOptions";
			this.cblRenderOptions.Size = new System.Drawing.Size(333, 349);
			this.cblRenderOptions.TabIndex = 2;
			this.cblRenderOptions.ThreeDCheckBoxes = true;
			this.cblRenderOptions.SelectedIndexChanged += new System.EventHandler(this.cblRenderOptions_SelectedIndexChanged);
			// 
			// RenderOptionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(360, 400);
			this.Controls.Add(this.cblRenderOptions);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ddlSelectedSubject);
			this.Name = "RenderOptionsForm";
			this.Text = "Render Options";
			this.Load += new System.EventHandler(this.RenderOptionsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox ddlSelectedSubject;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckedListBox cblRenderOptions;
	}
}