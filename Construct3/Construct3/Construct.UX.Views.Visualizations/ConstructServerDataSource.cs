using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Models.Data.PropertyValues.Services;
using Construct.Utilities.Shared;

namespace Construct.UX.Views.Visualizations
{
	class ConstructServerDataSource : IQueryableDataSource
	{
		private Uri constructServerUri;
		private SubscriptionTranslator translator;
		private Dictionary<Type, Type> propertyServiceTypeTable = new Dictionary<Type, Type>()
		{
			{ typeof(byte[]), typeof(IByteArrayPropertyValueService) },
			{ typeof(double), typeof(IDoublePropertyValueService) },
			{ typeof(float), typeof(ISinglePropertyValueService) },
			{ typeof(int), typeof(IIntPropertyValueService) },
			{ typeof(string), typeof(IStringPropertyValueService) },
			{ typeof(bool), typeof(IBooleanPropertyValueService) },
			{ typeof(Guid), typeof(IGuidPropertyValueService) },
			{ typeof(long), typeof(IIntPropertyValueService) },
			{ typeof(DateTime), typeof(IDateTimePropertyValueService) }
		};

		public bool EmitUTC { get; set; }

		public ConstructServerDataSource(Uri constructServerUri, SubscriptionTranslator translator)
		{
			this.constructServerUri = constructServerUri;
			this.translator = translator;

			EmitUTC = false;
		}

		public IEnumerable<SimplifiedPropertyValue> GetData(DateTime startTime, DateTime endTime, DataSubscription dataToGet)
		{
			var utcStart = startTime.Kind != DateTimeKind.Utc ? startTime.ToUniversalTime() : startTime;
			var utcEnd = endTime.Kind != DateTimeKind.Utc ? endTime.ToUniversalTime() : endTime;

			dynamic propertyService = GetServiceHostForProperty(dataToGet);
			dynamic queryResult = propertyService.GetBetween(utcStart, utcEnd);
			//	How to close service connection?

			List<SimplifiedPropertyValue> result = new List<SimplifiedPropertyValue>(queryResult.Length);
			foreach (var value in queryResult)
			{
				var newValue = new SimplifiedPropertyValue()
				{
					PropertyId = value.PropertyID,
					SensorId = value.SourceID,
					//	TimeStamps are received as UTC but Kind may not be specified, which can screw with time-based calculations
					TimeStamp = DateTime.SpecifyKind(value.StartTime, DateTimeKind.Utc),
					Value = value.Value
				};

				if (newValue.Value is DateTime)
					newValue.Value = DateTime.SpecifyKind((DateTime) newValue.Value, DateTimeKind.Utc);

				if (!EmitUTC)
				{
					newValue.TimeStamp = newValue.TimeStamp.ToLocalTime();
					if (newValue.Value is DateTime)
						newValue.Value = ((DateTime) newValue.Value).ToLocalTime();
				}

				result.Add(newValue);
			}

			return result;
		}

		private dynamic GetServiceHostForProperty(DataSubscription property)
		{
			var translation = translator.GetTranslation(property);
			Uri serviceUri = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(constructServerUri,
				translation.DataTypeName, translation.PropertyName);

			EndpointAddress serviceEndpoint = new EndpointAddress(serviceUri);
			BasicHttpBinding binding = new BasicHttpBinding();
			binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
			binding.MaxReceivedMessageSize = int.MaxValue;

			Type endpointServiceType = propertyServiceTypeTable[property.PropertyType];
			var channelFactoryType = typeof (ChannelFactory<>);
			channelFactoryType = channelFactoryType.MakeGenericType(endpointServiceType);

			dynamic channelFactory = Activator.CreateInstance(channelFactoryType, binding, serviceEndpoint);

			return channelFactory.CreateChannel();
		}

		public void Connect()
		{
			//	Nothing to do
		}

		public void Disconnect()
		{
			//	Nothing to do
		}
	}
}
