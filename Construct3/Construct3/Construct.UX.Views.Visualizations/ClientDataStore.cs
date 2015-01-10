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
		public ISubscribableDataSource RealtimeDataSource { get; private set; }
		public IQueryableDataSource TemporalDataSource { get; private set; }
		private DataRouteCollection DataRoutes = new DataRouteCollection();

		ConcurrentDictionary<DataStoreDescriptor, PropertyDataStore> ExistingDataStores = new ConcurrentDictionary<DataStoreDescriptor, PropertyDataStore>(); 

		public ClientDataStore(ISubscribableDataSource realtimeDataSource, IQueryableDataSource temporalDataSource)
		{
			this.RealtimeDataSource = realtimeDataSource;
			this.RealtimeDataSource.OnData += realtimeDataSource_OnData;

			this.TemporalDataSource = temporalDataSource;

			DataRoutes.OnRouteOpened += OnRouteOpened;
			DataRoutes.OnRouteClosed += OnRouteClosed;
		}

		void realtimeDataSource_OnData(SimplifiedPropertyValue propertyValue)
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
			//	Try to get a store that perfectly matches this descriptor (aka have we polled this exact data before)
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
					//	No possible data stores to pull data from, query for what already exists
					//		on server-side
					var existingData = TemporalDataSource.GetData(startTime, endTime, dataType);
					foreach (var data in existingData)
						dataStore.Data.Add(data);
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
			RealtimeDataSource.RemoveSubscription(dataRoute.RouteSubscription.SourceId, dataRoute.RouteSubscription.PropertyId);
		}

		private void OnRouteOpened(DataRoute dataRoute)
		{
			RealtimeDataSource.AddSubscription(dataRoute.RouteSubscription.SourceId, dataRoute.RouteSubscription.PropertyId);
		}
	}
}
