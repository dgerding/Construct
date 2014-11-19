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
	class SignalRStreamDataSource : IStreamDataSource, IDisposable
	{
		public event Action<String, String, object> OnData;

		public String SourceHostName { get; private set; }
		public String DataUri { get; private set; }

		private HubConnection hubConnection;
		private IHubProxy dataProxy;

		public SignalRStreamDataSource(String dataHostname)
		{
			SourceHostName = dataHostname;
			DataUri = "http://" + SourceHostName + ":15999/00000000-0000-0000-0000-000000000000/Data";

			hubConnection = new HubConnection(DataUri);
			dataProxy = hubConnection.CreateHubProxy("ItemStreamHub");
			dataProxy.On<SimplifiedPropertyValue>("newData", DispatchData);
		}

		public void Dispose()
		{
			this.Stop();
		}
		public void Start()
		{
			hubConnection.Start().Wait();
			dataProxy.Invoke("RequestSubscription", Guid.NewGuid(), Guid.NewGuid());
		}

		public void Stop()
		{
			hubConnection.Stop();
		}

		private void DispatchData(SimplifiedPropertyValue propertyValue)
		{
			
		}
	}
}
