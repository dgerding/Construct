using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	public class ClientDataStore
	{
		public IDataSource DataSource { get; private set; }
		private DataRouteCollection DataRoutes = new DataRouteCollection();

		ConcurrentDictionary<DataStoreDescriptor, PropertyDataStore> ExistingDataStores = new ConcurrentDictionary<DataStoreDescriptor, PropertyDataStore>(); 

		public ClientDataStore(IDataSource dataSource)
		{
			this.DataSource = dataSource;
			this.DataSource.OnData += dataSource_OnData;

			DataRoutes.OnRouteOpened += OnRouteOpened;
			DataRoutes.OnRouteClosed += OnRouteClosed;
		}

		void dataSource_OnData(SimplifiedPropertyValue propertyValue)
		{
			var route = DataRoutes.GetRoute(new DataSubscription()
			{
				PropertyId = propertyValue.PropertyId,
				SourceId = propertyValue.SensorId
			});

			route.NotifyData(propertyValue);
		}

		public PropertyDataStore GenerateDataStore(DataSubscription dataType)
		{
			//	Data store covering all time
			return GenerateDataStore(dataType, DateTime.MinValue, DateTime.MaxValue);
		}

		public PropertyDataStore GenerateDataStore(DataSubscription dataType, DateTime startTime)
		{
			//	Data store starting from the specified time
			return GenerateDataStore(dataType, startTime, DateTime.MaxValue);
		}

		public PropertyDataStore GenerateDataStore(DataSubscription dataType, DateTime startTime, DateTime endTime)
		{
			//	Data store for a specific time period
			var descriptor = new DataStoreDescriptor()
			{
				StartTime = startTime,
				EndTime = endTime,
				Subscription = dataType
			};

			PropertyDataStore dataStore;
			//	Try to get a store that perfectly matches this descriptor
			if (!ExistingDataStores.TryGetValue(descriptor, out dataStore))
			{
				var dataRouteForStore = DataRoutes.GetRoute(dataType);
				dataStore = new PropertyDataStore(dataRouteForStore, startTime, endTime);

				//	Try to copy over data from an existing store that contains data for this descriptor
				var possibleSourceStores =
					ExistingDataStores.Where(s => s.Key.AggregationInterval == descriptor.AggregationInterval &&
					                              s.Key.StartTime.Ticks <= descriptor.StartTime.Ticks &&
					                              s.Key.EndTime.Ticks >= descriptor.EndTime.Ticks &&
					                              s.Key.Subscription == descriptor.Subscription);
				if (possibleSourceStores.Any())
				{
					//	Could offload this loading to a worker thread and invoke the OnData event
					var sourceStore = possibleSourceStores.First();
					foreach (var data in sourceStore.Value.Data)
					{
						//	Check that the data is within this data store's range
						if (data.TimeStamp.Ticks >= descriptor.StartTime.Ticks &&
							data.TimeStamp.Ticks <= descriptor.EndTime.Ticks)
						{
							dataStore.Data.Add(data);
						}
					}
				}
				else
				{
					//	Should pull for data in this range from data source
				}

				ExistingDataStores.TryAdd(descriptor, dataStore);
			}

			Interlocked.Increment(ref dataStore.NumReferences);

			return dataStore;
		}

		public void ReleaseDataStore(PropertyDataStore dataStore)
		{
			Interlocked.Decrement(ref dataStore.NumReferences);
		}

		private void OnRouteClosed(DataRoute dataRoute)
		{
			DataSource.RemoveSubscription(dataRoute.RouteSubscription.SourceId, dataRoute.RouteSubscription.PropertyId);
		}

		private void OnRouteOpened(DataRoute dataRoute)
		{
			DataSource.AddSubscription(dataRoute.RouteSubscription.SourceId, dataRoute.RouteSubscription.PropertyId);
		}
	}
}
