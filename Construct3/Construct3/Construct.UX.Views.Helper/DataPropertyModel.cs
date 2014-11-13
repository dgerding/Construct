using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Helper
{
	public class DataPropertyModel
	{
		public String PropertyName { get; set; }

		public String DataTypeName { get; set; }

		public String SensorHostName { get; set; }

		public String SensorTypeName { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as DataPropertyModel;
			if (other == null)
				return false;

			return other.PropertyName == this.PropertyName &&
			       other.DataTypeName == this.DataTypeName &&
			       other.SensorHostName == this.SensorHostName &&
			       other.SensorTypeName == this.SensorTypeName;
		}

		//	User-supplied identifier
		public object Reference;
	}
}
