
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	public struct DataSubscription
	{
		/// <summary>
		/// The ID of the property that is being represented (may be unset if only representing a source)
		/// </summary>
		public Guid PropertyId { get; set; }
		/// <summary>
		/// The C# type of the property (may be unset if only representing a source)
		/// </summary>
		public Type PropertyType { get; set; }
		/// <summary>
		/// The ID of the source that is emitting the data
		/// </summary>
		public Guid SourceId { get; set; }
		/// <summary>
		/// The aggregated datatype that is being represented
		/// </summary>
		public Guid AggregateDataTypeId { get; set; }

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
