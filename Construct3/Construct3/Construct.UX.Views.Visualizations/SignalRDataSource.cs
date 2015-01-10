using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Microsoft.AspNet.SignalR.Client;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	//	Intended to connect only to Construct-hosted SigR hub
	internal class SignalRDataSource : ISubscribableDataSource, IDisposable
	{
		public event Action<SimplifiedPropertyValue> OnData;
		public String SourceHostName { get; private set; }
		public String DataUri { get; private set; }
		public bool EmitUTC { get; set; }

		private HubConnection hubConnection;
		private IHubProxy dataProxy;

		//	Used when connection is lost and data needs to be re-subscribed to with new connection
		private List<KeyValuePair<Guid, Guid>> existingSubscriptions = new List<KeyValuePair<Guid, Guid>>(); 

		public SignalRDataSource(String dataHostname)
		{
			SourceHostName = dataHostname;
			DataUri = "http://" + SourceHostName + ":15999/00000000-0000-0000-0000-000000000000/Data";

			hubConnection = new HubConnection(DataUri);
			dataProxy = hubConnection.CreateHubProxy("ItemStreamHub");
			dataProxy.On<SimplifiedPropertyValue>("newData", DispatchData);

			hubConnection.Reconnected += hubConnection_Reconnected;
			hubConnection.Closed += hubConnection_Closed;

			EmitUTC = false;
		}

		void hubConnection_Closed()
		{
			Reconnect();
			Resubscribe();
		}

		private void hubConnection_Reconnected()
		{
			Resubscribe();
		}

		public IEnumerable<SimplifiedPropertyValue> GetData(DateTime startTime, DateTime endTime, DataSubscription dataToGet)
		{
			return Enumerable.Empty<SimplifiedPropertyValue>();
		}

		public void AddSubscription(Guid sourceId, Guid propertyId)
		{
			if (hubConnection.State != ConnectionState.Connected)
				Reconnect();

			dataProxy.Invoke("RequestSubscription", sourceId, propertyId);
			existingSubscriptions.Add(new KeyValuePair<Guid, Guid>(sourceId, propertyId));
		}

		public void RemoveSubscription(Guid sourceId, Guid propertyId)
		{
			if (hubConnection.State != ConnectionState.Connected)
				Reconnect();

			dataProxy.Invoke("RemoveSubscription", sourceId, propertyId);
			existingSubscriptions.RemoveAll(s => s.Key == sourceId && s.Value == propertyId);
		}

		public void Dispose()
		{
			this.Disconnect();
		}
		public void Connect()
		{
			hubConnection.Start().Wait();
		}
		public void Disconnect()
		{
			hubConnection.Stop();
		}

		void Reconnect()
		{
			if (hubConnection.State == ConnectionState.Connected)
				return;

			Connect();
			Resubscribe();
		}

		void Resubscribe()
		{
			foreach (var subscription in existingSubscriptions)
				dataProxy.Invoke("RequestSubscription", subscription.Key, subscription.Value);
		}

		private void DispatchData(SimplifiedPropertyValue propertyValue)
		{
			//	TimeStamps are received as UTC but Kind may not be specified, which can screw with time-based calculations
			propertyValue.TimeStamp = DateTime.SpecifyKind(propertyValue.TimeStamp, DateTimeKind.Utc);

			if (!EmitUTC)
				propertyValue.TimeStamp = propertyValue.TimeStamp.ToLocalTime();

			if (OnData != null)
				OnData(propertyValue);
		}
	}
}
