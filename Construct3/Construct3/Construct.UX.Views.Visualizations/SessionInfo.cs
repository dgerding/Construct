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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
