using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Helper
{
	/// <summary>
	/// Interaction logic for TimeRangeSelectionDialog.xaml
	/// </summary>
	public partial class TimeRangeSelectionDialog : RadRibbonWindow
	{
		public TimeRangeSelectionDialog(DateTime? startTime = null, DateTime? endTime = null)
		{
			EndTimeEnabled = true;
			this.DataContext = this;



			if (startTime.HasValue)
			{
				StartTime = startTime.Value;
				EndTime = startTime.Value;
			}

			if (endTime.HasValue)
			{
				if (endTime.Value != DateTime.MaxValue)
					EndTime = endTime.Value;
				else
					EndTimeEnabled = false;
			}

			IsUTC = (StartTime.Kind == DateTimeKind.Utc);

			InitializeComponent();

			EndTimePicker.IsEnabled = EndTimeEnabled;
			EndTimeEnabledCheckBox.IsChecked = EndTimeEnabled;

			//	Set DateTimePickers to use the same time formatting as the visualizations
			StartTimePicker.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us");
			StartTimePicker.Culture.DateTimeFormat.ShortTimePattern = "HH:mm";
			EndTimePicker.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us");
			EndTimePicker.Culture.DateTimeFormat.ShortTimePattern = "HH:mm";
		}

		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public bool EndTimeEnabled { get; set; }
		public bool IsUTC { get; set; }

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (EndTimeEnabled)
			{
				if ((EndTime - StartTime).Ticks < 0)
				{
					MessageBox.Show("The End Time must be later than the Start Time.");
					return;
				}

				if ((EndTime - StartTime).Ticks == 0)
				{
					MessageBox.Show("The End Time and Start Time cannot be the same.");
					return;
				}
			}

			if (IsUTC)
			{
				StartTime = DateTime.SpecifyKind(StartTime, DateTimeKind.Utc);
				EndTime = DateTime.SpecifyKind(EndTime, DateTimeKind.Utc);
			}
			else
			{
				StartTime = DateTime.SpecifyKind(StartTime, DateTimeKind.Local);
				EndTime = DateTime.SpecifyKind(EndTime, DateTimeKind.Local);
			}

			DialogResult = true;
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}

		private void EndTimeEnabledCbx_Changed(object sender, RoutedEventArgs e)
		{
			var checkbox = (sender as CheckBox);
			EndTimeEnabled = checkbox.IsChecked.GetValueOrDefault();
			EndTimePicker.IsEnabled = EndTimeEnabled;
		}
	}
}
