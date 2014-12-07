using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	public class PropertyDataStore
	{
		public DateTime DataStartTime { get; private set; }
		public DateTime DataEndTime { get; private set; }

		//	Should try to use something other than ConcurrentBag to allow PropertyDataStores
		//	of different time intervals to share the same data
		public ConcurrentBag<SimplifiedPropertyValue> Data { get; private set; }

		public event Action<SimplifiedPropertyValue> OnNewData;

		private DataRoute SourceDataRoute;

		//	Used to find unreferenced data stores, these data stores may be removed/collected
		//	if memory usage becomes too high
		internal int NumReferences;

		public PropertyDataStore(DataRoute sourceDataRoute, DateTime startTime, DateTime endTime)
		{
			Data = new ConcurrentBag<SimplifiedPropertyValue>();
			DataStartTime = startTime;
			DataEndTime = endTime;

			SourceDataRoute = sourceDataRoute;
			SourceDataRoute.OnData += SourceDataRoute_OnData;
		}

		void SourceDataRoute_OnData(SimplifiedPropertyValue obj)
		{
			if (obj.TimeStamp.Ticks >= DataStartTime.Ticks &&
			    obj.TimeStamp.Ticks <= DataEndTime.Ticks)
			{
				Data.Add(obj);
				if (OnNewData != null)
					OnNewData(obj);
			}
		}

		internal void Release()
		{
			SourceDataRoute.OnData -= SourceDataRoute_OnData;
		}
	}
}
