using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	//	Info for the data recording session
	public class SessionInfo : INotifyPropertyChanged
	{
		private DateTime? startTime;
		public DateTime? StartTime
		{
			get { return startTime; }
			set
			{
				startTime = value;
				OnPropertyChanged();
			}
		}

		private DateTime? endTime;
		public DateTime? EndTime
		{
			get { return endTime; }
			set
			{
				endTime = value;
				OnPropertyChanged();
			}
		}

		DateTime? viewStartTime;
		public DateTime? ViewStartTime
		{
			get { return viewStartTime; }
			set
			{
				viewStartTime = value;
				OnPropertyChanged();
			}
		}

		DateTime? viewEndTime;
		public DateTime? ViewEndTime
		{
			get { return viewEndTime; }
			set
			{
				viewEndTime = value;
				OnPropertyChanged();
			}
		}

		DateTime? selectedStartTime;
		public DateTime? SelectedStartTime
		{
			get { return selectedStartTime; }
			set
			{
				selectedStartTime = value;
				OnPropertyChanged();
			}
		}

		DateTime? selectedEndTime;
		public DateTime? SelectedEndTime
		{
			get { return selectedEndTime; }
			set
			{
				selectedEndTime = value;
				OnPropertyChanged();
			}
		}

		public void CopyFrom(SessionInfo source)
		{
			if (selectedStartTime != source.selectedStartTime)
				SelectedStartTime = source.selectedStartTime;

			if (selectedEndTime != source.selectedEndTime)
				SelectedEndTime = source.selectedEndTime;

			if (viewStartTime != source.viewStartTime)
				ViewStartTime = source.viewStartTime;

			if (viewEndTime != source.viewEndTime)
				ViewEndTime = source.viewEndTime;

			if (startTime != source.startTime)
				StartTime = source.startTime;

			if (endTime != source.endTime)
				EndTime = source.endTime;
		}

		public bool IsFullyDefined
		{
			get
			{
				return StartTime.HasValue && EndTime.HasValue &&
				       SelectedStartTime.HasValue && SelectedEndTime.HasValue &&
				       ViewStartTime.HasValue && ViewEndTime.HasValue;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
