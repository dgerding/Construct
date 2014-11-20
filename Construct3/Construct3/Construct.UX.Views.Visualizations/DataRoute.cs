﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	class DataRoute
	{
		public DataRoute(DataSubscription subscription)
		{
			RouteSubscription = subscription;
		}

		public DataSubscription RouteSubscription { get; private set; }

		private event Action<SimplifiedPropertyValue> onData;

		//	Invoked when an OnData listener was added, causing at least 1 listener to exist for the route
		public event Action<DataRoute> OnRouteOpened;
		//	Invoked when all OnDate listeners have been removed
		public event Action<DataRoute> OnRouteClosed;

		public void SendData(SimplifiedPropertyValue propertyValue)
		{
			if (onData != null)
				onData(propertyValue);
		}

		public event Action<SimplifiedPropertyValue> OnData
		{
			add
			{
				lock (onData)
				{
					onData += value;
					if (onData.GetInvocationList().Length == 1 && OnRouteOpened != null)
						OnRouteOpened(this);
				}
			}
			remove
			{
				lock (onData)
				{
					onData -= value;
					if (onData.GetInvocationList().Length == 0 && OnRouteClosed != null)
						OnRouteClosed(this);
				}
			}
		}
	}
}