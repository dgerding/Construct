using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	public class DataRouteCollection
	{
		public event Action<DataRoute> OnRouteOpened;
		public event Action<DataRoute> OnRouteClosed;

		public ConcurrentDictionary<DataSubscription, DataRoute> Routes { get; private set; }

		public DataRouteCollection()
		{
			Routes = new ConcurrentDictionary<DataSubscription, DataRoute>();
		}

		public DataRoute GetRoute(DataSubscription routeSubscription)
		{
			if (!Routes.ContainsKey(routeSubscription))
			{
				var newRoute = new DataRoute(routeSubscription);
				if (Routes.TryAdd(routeSubscription, newRoute))
				{
					newRoute.OnRouteOpened += route_OnRouteOpened;
					newRoute.OnRouteClosed += route_OnRouteClosed;
				}
			}

			return Routes[routeSubscription];
		}

		void route_OnRouteClosed(DataRoute obj)
		{
			if (OnRouteClosed != null)
				OnRouteClosed(obj);
		}

		void route_OnRouteOpened(DataRoute obj)
		{
			if (OnRouteOpened != null)
				OnRouteOpened(obj);
		}
	}
}
