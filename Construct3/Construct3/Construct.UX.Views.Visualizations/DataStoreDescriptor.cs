using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	internal class DataStoreDescriptor
	{
		public int AggregationInterval;

		public DateTime StartTime;
		public DateTime EndTime;

		public DataSubscription Subscription;

		public override int GetHashCode()
		{
			return StartTime.GetHashCode() ^ EndTime.GetHashCode() ^ Subscription.GetHashCode() ^
			       AggregationInterval.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DataStoreDescriptor))
				return base.Equals(obj);

			var other = obj as DataStoreDescriptor;
			return other.StartTime == StartTime &&
			       other.EndTime == EndTime &&
			       other.Subscription == Subscription &&
			       other.AggregationInterval == AggregationInterval;
		}

		public static bool operator== (DataStoreDescriptor a, DataStoreDescriptor b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(DataStoreDescriptor a, DataStoreDescriptor b)
		{
			return !a.Equals(b);
		}
	}
}
