using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Owin;

namespace Construct.Server.Models.Data
{
	class PropertySubscriptionsSet
	{
		private readonly Dictionary<long, List<object>> subscriptionsMap = new Dictionary<long, List<object>>();
		private static long GetPropertySourceHash(Guid sensorId, Guid propertyId)
		{
			return ((long)sensorId.GetHashCode() << 32) | propertyId.GetHashCode();
		}

		public PropertySubscriptionsSet()
		{
			
		}

		public PropertySubscriptionsSet(PropertySubscriptionsSet copySource)
		{
			//	Duplicate the source subscriptions with new lists to prevent indirect mutations
			foreach (var propertySubscriptions in copySource.subscriptionsMap)
				subscriptionsMap.Add(propertySubscriptions.Key, new List<object>(propertySubscriptions.Value));
		}

		public IEnumerable<object> GetSubscriptions(Guid sourceId, Guid propertyId)
		{
			var hash = GetPropertySourceHash(sourceId, propertyId);
			if (this.subscriptionsMap.ContainsKey(hash))
				return this.subscriptionsMap[hash];
			else
				return Enumerable.Empty<object>();
		}

		public void AddSubscription(Guid sourceId, Guid propertyId, object subscriber)
		{
			var hash = GetPropertySourceHash(sourceId, propertyId);
			if (!subscriptionsMap.ContainsKey(hash))
				subscriptionsMap.Add(hash, new List<object>());
			subscriptionsMap[hash].Add(subscriber);
		}

		public void RemoveSubscription(Guid sourceId, Guid propertyId, object subscriber)
		{
			var hash = GetPropertySourceHash(sourceId, propertyId);
			if (!subscriptionsMap.ContainsKey(hash))
			{
				//	This shouldn't happen; if we're debugging when this happens, we should know
				if (Debugger.IsAttached)
					Debugger.Break();
				return;
			}

			subscriptionsMap[hash].Remove(subscriber);
		}
	}
	
	public class ItemRealtimeStreamer : IDisposable
	{
		private Thread dataStreamThread;
		private readonly ConcurrentQueue<String> pushDataQueue = new ConcurrentQueue<string>(); 
		private bool isRunning = false;

		private PropertySubscriptionsSet subscriptions = new PropertySubscriptionsSet();
		private readonly object subscriptionsMutex = new object();

		private ConstructSerializationAssistant assistant;
		private IHubContext hubContext;

		public ItemRealtimeStreamer(String streamUri, ConstructSerializationAssistant assistant)
		{
			//	Whenever a hub is instantiated, start listening to it for subscription events

			ItemRealtimeStreamerHub.OnConnectionStarted += delegate(ItemRealtimeStreamerHub sender)
			{
				sender.OnNewSubscriptionRequest += AddDataSubscription;
				sender.OnRemoveSubscriptionRequest += AddDataSubscription;
			};

			ItemRealtimeStreamerHub.OnConnectionEnded += delegate(ItemRealtimeStreamerHub sender)
			{
				sender.OnNewSubscriptionRequest -= AddDataSubscription;
				sender.OnRemoveSubscriptionRequest -= AddDataSubscription;
			};



			WebApp.Start(streamUri);

			this.assistant = assistant;

			hubContext = GlobalHost.ConnectionManager.GetHubContext<ItemRealtimeStreamerHub>();

			isRunning = true;
			dataStreamThread = new Thread(DataStreamThread);
			dataStreamThread.Start();
		}

		public void Dispose()
		{
			isRunning = false;
			dataStreamThread.Join();
			dataStreamThread = null;
		}

		public void ProcessItemPayload(String jsonPayload)
		{
			pushDataQueue.Enqueue(jsonPayload);
		}
		public void AddDataSubscription(object client, Guid sourceId, Guid propertyId)
		{
			lock (subscriptionsMutex)
			{
				var newSubscriptionsList = new PropertySubscriptionsSet(this.subscriptions);
				newSubscriptionsList.AddSubscription(sourceId, propertyId, client);
				this.subscriptions = newSubscriptionsList;
			}
		}
		public void RemoveDataSubscription(object client, Guid sourceId, Guid propertyId)
		{
			lock (subscriptionsMutex)
			{
				var newSubscriptionsList = new PropertySubscriptionsSet(this.subscriptions);
				newSubscriptionsList.RemoveSubscription(sourceId, propertyId, client);
				this.subscriptions = newSubscriptionsList;
			}
		}
		private void DataStreamThread()
		{
			var currentSubscriptions = subscriptions;

			while (isRunning)
			{
				String newData;
				pushDataQueue.TryDequeue(out newData);
				if (newData == null)
				{
					Thread.Sleep(1);
					continue;
				}

				//	Must be reassigned each iteration to get latest subscriptions dictionary
				currentSubscriptions = this.subscriptions;

				DataItem deserializedDataItem = assistant.DeserializeDataItemFromString(newData);
				var sourceId = deserializedDataItem.SourceId;
				var simplifiedPropertyValue = new SimplifiedPropertyValue();
				//	Dispatch individual property values to any clients subscribed to those values from this sensor
				foreach (var propertyPair in deserializedDataItem.PropertyValues)
				{
					var propertyId = assistant.PropertyIDTables[deserializedDataItem.Name][propertyPair.Key];

					var propertySubscriptions = currentSubscriptions.GetSubscriptions(sourceId, propertyId).Cast<String>().ToList();
					if (propertySubscriptions.Count == 0)
						continue;

					simplifiedPropertyValue.PropertyId = propertyId;
					simplifiedPropertyValue.SensorId = deserializedDataItem.SourceId;
					simplifiedPropertyValue.TimeStamp = deserializedDataItem.Timestamp;
					simplifiedPropertyValue.Value = propertyPair.Value;
					hubContext.Clients.Clients(propertySubscriptions).newData(simplifiedPropertyValue);
				}
			}
		}
	}
}
