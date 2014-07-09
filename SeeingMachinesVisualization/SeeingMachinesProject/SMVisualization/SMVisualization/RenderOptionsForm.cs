using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMVisualization
{
	public partial class RenderOptionsForm : Form
	{
		private VisualizationRenderOptions m_RenderOptions;
		private SubjectRenderOptions m_SelectedSubjectRenderOptions;

		//	Changes in the RenderOptionsForm directly change the referenceRenderOptions object
		public RenderOptionsForm(VisualizationRenderOptions referenceRenderOptions)
		{
			InitializeComponent();
			m_RenderOptions = referenceRenderOptions;
		}

		private void RenderOptionsForm_Load(object sender, EventArgs e)
		{
			PopulateOptionsListFromRenderOptionsType();

			cblRenderOptions.ItemCheck += cblRenderOptions_ItemCheck;

			ddlSelectedSubject.SelectedItem = "All";
// 			m_SelectedSubjectRenderOptions = null; // 'null' is global
// 			AssignOptionsListUIFromRenderOptions(m_SelectedSubjectRenderOptions);
		}

		void cblRenderOptions_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			String itemName = cblRenderOptions.Items[e.Index] as String;
			String propertyName = itemName.Replace(" ", ""); // Remove all spaces to get the name of the property

			bool isEnabled = e.NewValue == CheckState.Checked;
			FieldInfo field = typeof(SubjectRenderOptions).GetField(propertyName);

			if (m_SelectedSubjectRenderOptions != null)
			{
				//	Update that field based on the new checked-ness
				field.SetValue(m_SelectedSubjectRenderOptions, isEnabled);
			}
			else
			{
				m_RenderOptions.SetAllSubjectsOptionToValue(field, isEnabled);
			}
		}

		private void ddlSelectedSubject_SelectedIndexChanged(object sender, EventArgs e)
		{
			String selectedItem = ddlSelectedSubject.SelectedItem.ToString();

			if (selectedItem == "All")
				m_SelectedSubjectRenderOptions = null;
			else
			{
				//	Assume the "Subjects" are assigned in the format of "Subject 1", "Subject 2",
				//	etc., extract the index from the end of that name and use it to determine which
				//	subject options to render. (Not pretty, works for now)

				String subjectIndexText = selectedItem.Substring(selectedItem.Length - 1);
				int subjectIndex = int.Parse(subjectIndexText) - 1; // Values in the list are 1-indexed

				m_SelectedSubjectRenderOptions = m_RenderOptions.SubjectOptions[subjectIndex];
			}

			AssignOptionsListUIFromRenderOptions(m_SelectedSubjectRenderOptions);
		}

		private void PopulateOptionsListFromRenderOptionsType()
		{
			//	Fills in the available render options from the values that are in the
			//		SubjectRenderOptions type. This assumes that all types in a SubjectRenderOptions
			//		are bool

			FieldInfo[] fields = typeof(SubjectRenderOptions).GetFields();
			foreach (FieldInfo field in fields)
			{
				cblRenderOptions.Items.Add(FormatUpperCamelCaseStringToSpaces(field.Name));
			}
		}

		//	Turn "SomeThing" to "Some Thing"
		private String FormatUpperCamelCaseStringToSpaces(String upperCamelCaseString)
		{
			List<String> wordsList = new List<string>();
			String currentWord = "";

			foreach (char currentCharacter in upperCamelCaseString)
			{
				if (char.IsUpper(currentCharacter) && currentWord.Length > 0)
				{
					wordsList.Add(currentWord);
					currentWord = "";
				}

				currentWord += currentCharacter;
			}

			if (currentWord.Length > 0)
				wordsList.Add(currentWord);

			return String.Join(" ", wordsList);
		}

		private void AssignOptionsListUIFromRenderOptions(SubjectRenderOptions options)
		{
			//	Prevent ItemChecked events
			cblRenderOptions.ItemCheck -= cblRenderOptions_ItemCheck;

			FieldInfo[] fields = typeof(SubjectRenderOptions).GetFields();
			foreach (FieldInfo field in fields)
			{
				String listName = FormatUpperCamelCaseStringToSpaces(field.Name);
				int itemIndex = cblRenderOptions.Items.IndexOf(listName);

				if (options != null)
				{
					bool value = (bool)field.GetValue(options);
					cblRenderOptions.SetItemChecked(itemIndex, value);
				}
				else
				{
					//	If the options are null then we're updating based on all subjects
					if (m_RenderOptions.AllSubjectsHaveOptionEnabled(field))
						cblRenderOptions.SetItemCheckState(itemIndex, CheckState.Checked);

					else if (m_RenderOptions.AllSubjectsHaveOptionDisabled(field))
						cblRenderOptions.SetItemCheckState(itemIndex, CheckState.Unchecked);

						//	If all items aren't checked and all items aren't unchecked, we need the little square-thingy
					else
						cblRenderOptions.SetItemCheckState(itemIndex, CheckState.Indeterminate);
				}
			}

			cblRenderOptions.ItemCheck += cblRenderOptions_ItemCheck;
		}

		private void cblRenderOptions_SelectedIndexChanged(object sender, EventArgs e)
		{
			//	Toggle the selected index
			cblRenderOptions.SetItemChecked(cblRenderOptions.SelectedIndex, !cblRenderOptions.GetItemChecked(cblRenderOptions.SelectedIndex));
		}
	}
}
