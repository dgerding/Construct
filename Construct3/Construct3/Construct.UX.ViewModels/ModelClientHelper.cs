using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Construct.UX.ViewModels
{
	public static class ModelClientHelper
	{
		public static void EnhanceModelClientBandwidth<T>(T modelClient)
		{
			Type clientType = typeof(T);
			var endpointPropertyInfo = clientType.GetProperty("Endpoint");
			ServiceEndpoint clientEndpoint = endpointPropertyInfo.GetValue(modelClient, null) as ServiceEndpoint;

			if (clientEndpoint == null)
				return;

			WSDualHttpBinding clientBinding = clientEndpoint.Binding as WSDualHttpBinding;
			if (clientBinding == null)
				return;

			clientBinding.MaxBufferPoolSize = 2147483647;
			clientBinding.MaxReceivedMessageSize = 2147483647;
			clientBinding.ReaderQuotas.MaxArrayLength = 2147483647;
			clientBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
			clientBinding.ReaderQuotas.MaxDepth = 2147483647;
			clientBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
			clientBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
		}
	}
}
