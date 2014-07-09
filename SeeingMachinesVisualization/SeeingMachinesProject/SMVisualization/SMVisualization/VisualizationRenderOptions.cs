using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMVisualization
{
	public class VisualizationRenderOptions
	{
		public SubjectRenderOptions[] SubjectOptions;

		public VisualizationRenderOptions(int numSubjects)
		{
			SubjectOptions = new SubjectRenderOptions[numSubjects];
			for (int i = 0; i < numSubjects; i++)
				SubjectOptions[i] = new SubjectRenderOptions();
		}

		public bool AllSubjectsHaveOptionEnabled(FieldInfo option)
		{
			bool allSubjectHaveEnabled = true;
			foreach (SubjectRenderOptions subjectOptions in SubjectOptions)
			{
				bool isEnabled = (bool)option.GetValue(subjectOptions);
				if (!isEnabled)
				{
					allSubjectHaveEnabled = false;
					break;
				}
			}

			return allSubjectHaveEnabled;
		}

		public bool AllSubjectsHaveOptionDisabled(FieldInfo option)
		{
			bool allSubjectHaveDisabled = true;
			foreach (SubjectRenderOptions subjectOptions in SubjectOptions)
			{
				bool isEnabled = (bool)option.GetValue(subjectOptions);
				if (isEnabled)
				{
					allSubjectHaveDisabled = false;
					break;
				}
			}

			return allSubjectHaveDisabled;
		}

		public void SetAllSubjectsOptionToValue(FieldInfo option, bool value)
		{
			foreach (var subject in SubjectOptions)
				option.SetValue(subject, value);
		}
	}
}
