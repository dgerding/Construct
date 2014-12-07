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
	class SignalRDataSource : IDataSource, IDisposable
	{
		public event Action<SimplifiedPropertyValue> OnData;
		public String SourceHostName { get; private set; }
		public String DataUri { get; private set; }

		private HubConnection hubConnection;
		private IHubProxy dataProxy;

		public bool IsQueryable { get { return false; } }

		public SignalRDataSource(String dataHostname)
		{
			SourceHostName = dataHostname;
			DataUri = "http://" + SourceHostName + ":15999/00000000-0000-0000-0000-000000000000/Data";

			hubConnection = new HubConnection(DataUri);
			dataProxy = hubConnection.CreateHubProxy("ItemStreamHub");
			dataProxy.On<SimplifiedPropertyValue>("newData", DispatchData);
		}

		public IEnumerable<SimplifiedPropertyValue> GetData(DateTime startTime, DateTime endTime, DataSubscription dataToGet)
		{
			return Enumerable.Empty<SimplifiedPropertyValue>();
		}

		public void AddSubscription(Guid sourceId, Guid propertyId)
		{
			dataProxy.Invoke("RequestSubscription", sourceId, propertyId);
		}

		public void RemoveSubscription(Guid sourceId, Guid propertyId)
		{
			dataProxy.Invoke("RemoveSubscription", sourceId, propertyId);
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

		private void DispatchData(SimplifiedPropertyValue propertyValue)
		{
			if (OnData != null)
				OnData(propertyValue);
		}
	}
}
