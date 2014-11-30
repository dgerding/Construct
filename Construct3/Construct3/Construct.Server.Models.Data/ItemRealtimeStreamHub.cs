using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Construct.Server.Models.Data
{
	[HubName("ItemStreamHub")]
	public class ItemRealtimeStreamerHub : Hub
	{
		public delegate void SubscriptionChangedHandler(object clientHandle, Guid sourceId, Guid propertyId);

		public delegate void ConnectionChangedHandler(ItemRealtimeStreamerHub sender, object connectionContext);

		public static event ConnectionChangedHandler OnConnectionStarted;
		public static event ConnectionChangedHandler OnConnectionEnded;

		public event SubscriptionChangedHandler OnNewSubscriptionRequest;
		public event SubscriptionChangedHandler OnRemoveSubscriptionRequest;

		public ItemRealtimeStreamerHub()
		{
			if (OnConnectionStarted != null)
				OnConnectionStarted(this, Context != null ? Context.ConnectionId : null);
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			if (OnConnectionEnded != null)
				OnConnectionEnded(this, Context.ConnectionId);

			return base.OnDisconnected(stopCalled);
		}

		public void RequestSubscription(Guid sourceId, Guid propertyId)
		{
			if (OnNewSubscriptionRequest != null)
				OnNewSubscriptionRequest(Context.ConnectionId, sourceId, propertyId);
		}

		public void RemoveSubscription(Guid sourceId, Guid propertyId)
		{
			if (OnRemoveSubscriptionRequest != null)
				OnRemoveSubscriptionRequest(Context.ConnectionId, sourceId, propertyId);
		}
	}
}
