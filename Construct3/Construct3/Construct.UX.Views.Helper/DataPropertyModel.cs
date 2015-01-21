using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Helper
{
	public struct DataPropertyModel
	{
		public String PropertyName { get; set; }

		public String DataTypeName { get; set; }

		public String SensorHostName { get; set; }

		public String SensorTypeName { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is DataPropertyModel))
				return false;

			var other = (DataPropertyModel)obj;

			return other.PropertyName == this.PropertyName &&
			       other.DataTypeName == this.DataTypeName &&
			       other.SensorHostName == this.SensorHostName &&
			       other.SensorTypeName == this.SensorTypeName;
		}

		public static bool operator== (DataPropertyModel a, DataPropertyModel b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(DataPropertyModel a, DataPropertyModel b)
		{
			return !a.Equals(b);
		}

		//	User-supplied identifier
		public object Reference;
	}
}
