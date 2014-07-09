using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ConstructMetadataGenerator
{
	public partial class MainForm : Form
	{
		SensorDeclaration m_SensorDeclaration = new SensorDeclaration();

		ItemWrapper<OutputDatatypeDeclaration> SelectedDataType
		{
			get
			{
				return lbDataTypes.SelectedItem as ItemWrapper<OutputDatatypeDeclaration>;
			}
		}

		void RefreshTypesList()
		{
			/* http://stackoverflow.com/questions/519538/c-sharp-force-listbox-to-update-elements */
			lbDataTypes.DisplayMember = "junk";
			lbDataTypes.DisplayMember = "";
		}

		void UpdateAllFields(OutputDatatypeDeclaration declaration)
		{
			splitContainer1.Panel2.Enabled = (declaration != null);

			UpdateGuidFields(declaration);
			UpdateNameFields(declaration);
			UpdateAssemblyFields(declaration);
			UpdatePropertiesList(declaration);
		}

		void UpdateGuidFields(OutputDatatypeDeclaration declaration)
		{
			if (declaration == null)
			{
				tbDataTypeID.Enabled = false;
				tbDataTypeID.Text = "";
				btnRandomTypeGUID.Enabled = false;
				return;
			}

			if (!tbDataTypeID.Enabled)
			{
				tbDataTypeID.Enabled = true;
				btnRandomTypeGUID.Enabled = true;
			}

			tbDataTypeID.Text = declaration.DataTypeID.ToString();
		}

		void UpdateNameFields(OutputDatatypeDeclaration declaration)
		{
			if (declaration == null)
			{
				cbAutogenerateTypeName.Enabled = false;
				tbDataTypeName.Enabled = false;
				tbDataTypeName.Text = "";
				return;
			}

			if (!cbAutogenerateTypeName.Enabled)
			{
				tbDataTypeName.Enabled = true;
				cbAutogenerateTypeName.Enabled = true;

				cbAutogenerateTypeName.Checked = declaration.UsesAutoTypename;
			}

			if (cbAutogenerateTypeName.Checked)
				tbDataTypeName.ReadOnly = true;
			else
				tbDataTypeName.ReadOnly = false;

			tbDataTypeName.Text = declaration.TypeName;
		}

		void UpdateAssemblyFields(OutputDatatypeDeclaration declaration)
		{
			if (declaration == null)
			{
				tbSourceAssembly.Text = "";
				tbSourceType.Text = "";

				tbSourceAssembly.Enabled = false;
				tbSourceType.Enabled = false;
				return;
			}

			if (!tbSourceAssembly.Enabled || !tbSourceType.Enabled)
			{
				tbSourceAssembly.Enabled = true;
				tbSourceType.Enabled = true;
			}

			tbSourceAssembly.Text = declaration.SourceAssembly;
			tbSourceType.Text = declaration.OutputType.FullName;
		}

		void UpdatePropertiesList(OutputDatatypeDeclaration declaration)
		{
			dgTypeMembers.Rows.Clear();

			if (declaration == null)
			{
				dgTypeMembers.Enabled = false;
				return;
			}
			else
				dgTypeMembers.Enabled = true;

			MemberDeclaration[] typeMembers = SelectedDataType.Value.TypeMembers;
			foreach (MemberDeclaration member in typeMembers)
			{
				dgTypeMembers.Rows.Add(member.MemberName, member.MemberType.Name);
			}
		}

		public MainForm()
		{
			InitializeComponent();

			tbDataTypeID.Validating += tbDataTypeID_Validating;
			tbDataTypeName.Validating += tbDataTypeName_Validating;

			UpdateAllFields(null);
		}

		void tbDataTypeID_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (SelectedDataType == null)
				return;

			Guid result;
			if (!Guid.TryParse(tbDataTypeID.Text, out result))
			{
				DialogResult selection = MessageBox.Show("The datatype ID must be in valid GUID format.", "", MessageBoxButtons.OKCancel);
				if (selection == DialogResult.OK)
					e.Cancel = true;
				else
					tbDataTypeID.Text = SelectedDataType.Value.DataTypeID.ToString();
				return;
			}

			SelectedDataType.Value.DataTypeID = result;
		}

		void tbDataTypeName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (SelectedDataType == null)
				return;

			if (tbDataTypeName.Text.Trim().Length == 0)
			{
				//	What's appropriate behavior for this?
				//MessageBox.Show("The datatype name cannot be blank.");
				//e.Cancel = true;
				return;
			}

			var item = SelectedDataType;
			if (item == null)
				throw new Exception("User should not be allowed to modify field when no valid data type has been selected.");

			item.Value.TypeName = tbDataTypeName.Text;
			item.TextValue = tbDataTypeName.Text;

			RefreshTypesList();
		}

		private void btnAddType_Click(object sender, EventArgs e)
		{
			if (selectAssemblyDialog.ShowDialog() != DialogResult.OK)
				return;

			Assembly assembly;
			Type[] assemblyTypes;
			try
			{
				assembly = Assembly.LoadFile(selectAssemblyDialog.FileName);
				assemblyTypes = assembly.GetTypes();
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("Error loading assembly:\n" + ex.Message);
				return;
			}

			ListBoxSelectDialog selectDialog = new ListBoxSelectDialog();
			selectDialog.Text = "Select Data Type from Assembly";

			foreach (Type type in assemblyTypes)
			{
				selectDialog.ItemsSource.Add(type.FullName);
			}

			if (selectDialog.ShowDialog() != DialogResult.OK)
				return;

			Type selectedType = assemblyTypes.First((type) => type.FullName == (String)selectDialog.SelectedItem);
			OutputDatatypeDeclaration dataDeclaration = new OutputDatatypeDeclaration();
			dataDeclaration.OutputType = selectedType;
			dataDeclaration.DataTypeID = Guid.NewGuid();
			dataDeclaration.SourceAssembly = selectAssemblyDialog.FileName;

			lbDataTypes.Items.Add(new ItemWrapper<OutputDatatypeDeclaration>(dataDeclaration, dataDeclaration.TypeName));
			lbDataTypes.SelectedIndex = lbDataTypes.Items.Count - 1;

			UpdateAllFields(dataDeclaration);
		}

		private void cbAutogenerateTypeName_CheckedChanged(object sender, EventArgs e)
		{
			if (cbAutogenerateTypeName.Checked)
				SelectedDataType.Value.TypeName = null;
			else
				SelectedDataType.Value.TypeName = SelectedDataType.Value.TypeName;

			SelectedDataType.TextValue = SelectedDataType.Value.TypeName;

			UpdateNameFields(SelectedDataType.Value);
			RefreshTypesList();
		}

		private void btnRandomTypeGUID_Click(object sender, EventArgs e)
		{
			SelectedDataType.Value.DataTypeID = Guid.NewGuid();
			UpdateGuidFields(SelectedDataType.Value);
		}

		private void lbDataTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedWrapper = SelectedDataType;
			OutputDatatypeDeclaration selected = null;
			if (selectedWrapper != null)
				selected = selectedWrapper.Value;

			//	SuspendDrawing stops flickering (because apparently double-buffering the form isn't enough)
			this.SuspendDrawing();

			btnRemoveType.Enabled = selected != null;
			UpdateAllFields(null); // Clear out old values

			if (selected != null)
				UpdateAllFields(selected);

			this.ResumeDrawing();
		}

		private void btnRemoveType_Click(object sender, EventArgs e)
		{
			int selectedIndex = lbDataTypes.SelectedIndex;
			lbDataTypes.Items.RemoveAt(selectedIndex);

			if (lbDataTypes.Items.Count != 0)
				lbDataTypes.SelectedIndex = Math.Min(selectedIndex, lbDataTypes.Items.Count - 1);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			m_SensorDeclaration.SensorName = tbSensorName.Text;
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (tbSensorName.Text.Trim().Length == 0)
			{
				var result = MessageBox.Show("The sensor has a blank name, would you like to save anyway?", "", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
					return;
			}

			if (saveSensorDialog.ShowDialog() != DialogResult.OK)
				return;

			m_SensorDeclaration.SensorName = tbSensorName.Text;
			m_SensorDeclaration.DataTypes.Clear();
			foreach (object item in lbDataTypes.Items)
				m_SensorDeclaration.DataTypes.Add((item as ItemWrapper<OutputDatatypeDeclaration>).Value);

			XmlDocument sensorDocument = SensorDeclaration.GenerateDocumentFromSensorDeclaration(m_SensorDeclaration);
			sensorDocument.Save(saveSensorDialog.FileName);
		}

		private void sensorPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SensorPropertiesForm properties = new SensorPropertiesForm(m_SensorDeclaration);
			properties.ShowDialog();
		}
	}
}
