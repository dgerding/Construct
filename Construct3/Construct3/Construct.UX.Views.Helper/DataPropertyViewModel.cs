using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Helper
{
	class DataPropertyViewModel : INotifyPropertyChanged
	{
		private DataPropertyModel model;

		public DataPropertyViewModel(DataPropertyModel model)
		{
			this.model = model;
		}

		public String PropertyName { get { return this.model.PropertyName; } }
		public String DataTypeName { get { return this.model.DataTypeName; } }
		public String SensorName { get { return this.model.SensorHostName + "/" + this.model.SensorTypeName; } }

		public DataPropertyModel Model { get { return this.model; } }

		private bool isSelected = false;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
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
