
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	public struct DataSubscription
	{
		public Guid PropertyId { get; set; }
		public Type PropertyType { get; set; }
		public Guid SourceId { get; set; }

		public override int GetHashCode()
		{
			return PropertyId.GetHashCode() ^ SourceId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			DataSubscription other = (DataSubscription) obj;

			return PropertyId == other.PropertyId && SourceId == other.SourceId;
		}

		public static bool operator ==(DataSubscription a, DataSubscription b)
		{
			return a.PropertyId == b.PropertyId && a.SourceId == b.SourceId;
		}

		public static bool operator !=(DataSubscription a, DataSubscription b)
		{
			return a.PropertyId != b.PropertyId || a.SourceId != b.SourceId;
		}
	}
}
