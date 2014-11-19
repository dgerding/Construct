using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	struct DataSubscription
	{
		public Guid PropertyId { get; set; }
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
	}
}
