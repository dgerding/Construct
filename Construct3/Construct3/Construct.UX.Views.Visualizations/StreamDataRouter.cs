using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	public class StreamDataRouter
	{
		public IStreamDataSource DataSource { get; private set; }
		private ConcurrentDictionary<DataSubscription, DataRoute> subscriptionRouteMap = new ConcurrentDictionary<DataSubscription, DataRoute>();
		public StreamDataRouter(IStreamDataSource dataSource)
		{
			this.DataSource = dataSource;

			this.DataSource.OnData += dataSource_OnData;
		}

		void dataSource_OnData(SimplifiedPropertyValue propertyValue)
		{
			var routeMap = subscriptionRouteMap;
			var subscriptionForProperty = new DataSubscription()
			{
				SourceId = propertyValue.SensorId,
				PropertyId = propertyValue.PropertyId
			};

			DataRoute dataRoute;
			if (routeMap.TryGetValue(subscriptionForProperty, out dataRoute))
			{
				dataRoute.SendData(propertyValue);
			}
		}

		/// <summary>
		/// Returns the data route for the given source and property ID
		/// </summary>
		public DataRoute Route(Guid sourceId, Guid propertyId)
		{
			var dataSubscription = new DataSubscription() {SourceId = sourceId, PropertyId = propertyId};
			if (!subscriptionRouteMap.ContainsKey(dataSubscription))
			{
				var newRoute = new DataRoute(dataSubscription);
				if (subscriptionRouteMap.TryAdd(dataSubscription, newRoute))
				{
					newRoute.OnRouteOpened += newRoute_OnRouteOpened;
					newRoute.OnRouteClosed += newRoute_OnRouteClosed;
				}
			}

			return subscriptionRouteMap[dataSubscription];
		}

		private void newRoute_OnRouteClosed(DataRoute dataRoute)
		{
			DataSource.RemoveSubscription(dataRoute.RouteSubscription.SourceId, dataRoute.RouteSubscription.PropertyId);
		}

		private void newRoute_OnRouteOpened(DataRoute dataRoute)
		{
			DataSource.AddSubscription(dataRoute.RouteSubscription.SourceId, dataRoute.RouteSubscription.PropertyId);
		}
	}
}
