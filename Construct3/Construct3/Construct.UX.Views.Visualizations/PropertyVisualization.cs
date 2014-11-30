using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Windows;
using System.Windows.Controls;
using Construct.MessageBrokering.Serialization;
using Construct.UX.Views.Helper;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	public class PropertyVisualization : UserControl
	{
		public virtual Type GetVisualizedType()
		{
			return null; 
		}

		//	Needs to be subscribed to by the derived class
		protected event Action<SimplifiedPropertyValue> OnRoutedData;

		protected event Action<DataSubscription> OnSubscriptionAdded;
		protected event Action<DataSubscription> OnSubscriptionRemoved;

		protected event Action OnClosed;

		protected virtual IEnumerable<Type> VisualizableTypes { get { return null; } } 

		private List<DataPropertyModel> selectedProperties = new List<DataPropertyModel>();
		private StreamDataRouter sourceDataRouter;
		private SubscriptionTranslator subscriptionTranslator;

		public bool EnablePropertySelectionDialog { get; protected set; }
		public RadTimeBar TimeBar { get; protected set; }
		public String VisualizationName { get; protected set; }
		public virtual int MaxProperties { get { return int.MaxValue; } }

		public PropertyVisualization(StreamDataRouter dataRouter, SubscriptionTranslator subscriptionTranslator)
		{
			this.subscriptionTranslator = subscriptionTranslator;
			this.sourceDataRouter = dataRouter;

			this.EnablePropertySelectionDialog = true;

			this.ContextMenu = new ContextMenu();
			var showPropertyMenuItem = new MenuItem();
			showPropertyMenuItem.Header = "_Show Property Selection";
			showPropertyMenuItem.Click += showPropertyMenuItem_Click;
			ContextMenu.Items.Add(showPropertyMenuItem);
		}

		public void Close()
		{
			if (OnClosed != null)
				OnClosed();

			foreach (var propertyModel in selectedProperties)
			{
				var subscription = (DataSubscription)propertyModel.Reference;
				sourceDataRouter.Route(subscription.SourceId, subscription.PropertyId).OnData -= OnRoutedData;
			}
		}

		void showPropertyMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!EnablePropertySelectionDialog)
				return;

			var allProperties = new List<DataPropertyModel>();
			foreach (var possibleSubscription in subscriptionTranslator.AllTranslations(VisualizableTypes))
			{
				allProperties.Add(new DataPropertyModel()
				{
					DataTypeName = possibleSubscription.Value.DataTypeName,
					PropertyName = possibleSubscription.Value.PropertyName,
					SensorHostName = possibleSubscription.Value.SourceName,
					SensorTypeName = possibleSubscription.Value.SourceTypeName,
					Reference = possibleSubscription.Key
				});
			}

			var propertyDialog = new PropertySelectionDialog(allProperties, selectedProperties);

			if (propertyDialog.ShowDialog().GetValueOrDefault(false))
			{
				var newSelectedProperties = propertyDialog.SelectedProperties.ToList();
				if (newSelectedProperties.Count >= MaxProperties)
				{
					MessageBox.Show("Cannot display more than " + MaxProperties + " properties (" + newSelectedProperties.Count + " selected)");
					return;
				}

				//	Remove route listeners for removed properties
				var removedProperties = selectedProperties.Where((model) => !newSelectedProperties.Contains(model));
				foreach (var removedProperty in removedProperties)
				{
					var propertyDescriptor = (DataSubscription)removedProperty.Reference;
					var dataRoute = sourceDataRouter.Route(propertyDescriptor.SourceId, propertyDescriptor.PropertyId);
					dataRoute.OnData -= OnRoutedData;
					if (OnSubscriptionRemoved != null)
						OnSubscriptionRemoved(propertyDescriptor);
				}

				//	Add route listeners for new properties
				var uniqueNewProperties = newSelectedProperties.Where((model) => !selectedProperties.Contains(model));
				foreach (var newProperty in uniqueNewProperties)
				{
					var propertyDescriptor = (DataSubscription)newProperty.Reference;
					var dataRoute = sourceDataRouter.Route(propertyDescriptor.SourceId, propertyDescriptor.PropertyId);
					dataRoute.OnData += OnRoutedData;
					if (OnSubscriptionAdded != null)
						OnSubscriptionAdded(propertyDescriptor);
				}

				selectedProperties = newSelectedProperties;
			}
		}
	}
}
