using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Helper
{
	class DataPropertyViewModel
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

		public bool IsSelected { get; set; }
	}
}
