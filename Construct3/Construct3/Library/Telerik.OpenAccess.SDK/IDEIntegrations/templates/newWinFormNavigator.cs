using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace %DefaultNamespace%
{
    public partial class %ClassName% : Form
    {
        /// <summary>
	    /// Summary description for %ClassName%.
	    /// </summary>
        public %ClassName%()
        {
            InitializeComponent();
            // Add the following code if you want to dispose on close
            this.Closed += new System.EventHandler(this.FormClosedEvent);            
            this.bindingNavigator1.BindingSource = objectView1;            
        }

        private void FormClosedEvent(object sender, System.EventArgs e)
		{
			this.Dispose();
		}

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear out the current edits
                this.BindingContext[objectView1].EndCurrentEdit();
                this.BindingContext[objectView1].AddNew();
            }
            catch (System.Exception eEndEdit)
            {
                System.Windows.Forms.MessageBox.Show(eEndEdit.Message);
            }
            
        }

                    

        private void bindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.BindingContext[objectView1].EndCurrentEdit();
            this.objectProvider1.SaveAll();
            
        }

        private void bindingNavigatorCancelItem_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Do you really want to cancel ?", "confirm", MessageBoxButtons.YesNo);
            if (r == DialogResult.Yes)
            {
                this.BindingContext[objectView1].CancelCurrentEdit();
                this.objectProvider1.CancelAll();
                
            }
        }

    }
}