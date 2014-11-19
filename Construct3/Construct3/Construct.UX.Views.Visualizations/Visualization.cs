using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Construct.MessageBrokering.Serialization;
using Construct.UX.Views.Helper;

namespace Construct.UX.Views.Visualizations
{
	abstract class Visualization : UserControl
	{
		public abstract Type GetVisualizedType();

		protected event Action<SimplifiedPropertyValue> OnRoutedData;

		private List<DataPropertyModel> selectedProperties = new List<DataPropertyModel>();
		private StreamDataRouter sourceDataRouter;
		private SubscriptionTranslator subscriptionTranslator;
		public Visualization(StreamDataRouter dataRouter, SubscriptionTranslator subscriptionTranslator)
		{
			this.MouseRightButtonUp += MouseRightButtonUp_OpenPropertySelectionDialog;

			this.subscriptionTranslator = subscriptionTranslator;
			this.sourceDataRouter = dataRouter;
		}

		void MouseRightButtonUp_OpenPropertySelectionDialog(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var propertyDialog = new PropertySelectionDialog(null, selectedProperties);

			var allProperties = new List<DataPropertyModel>();
			foreach (var possibleSubscription in subscriptionTranslator.AllTranslations())
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

			if (propertyDialog.ShowDialog().GetValueOrDefault(false))
			{
				var newSelectedProperties = propertyDialog.SelectedProperties.ToList();
				//	Add route listeners for new properties
				var uniqueNewProperties = newSelectedProperties.Where((model) => !selectedProperties.Contains(model));
				foreach (var newProperty in uniqueNewProperties)
				{
					var propertyDescriptor = (DataSubscription)newProperty.Reference;
					var dataRoute = sourceDataRouter.Route(propertyDescriptor.SourceId, propertyDescriptor.PropertyId);
					dataRoute.OnData += OnRoutedData;
				}

				//	Remove route listeners for removed properties
				var removedProperties = selectedProperties.Where((model) => !newSelectedProperties.Contains(model));
				foreach (var removedProperty in removedProperties)
				{
					var propertyDescriptor = (DataSubscription) removedProperty.Reference;
					var dataRoute = sourceDataRouter.Route(propertyDescriptor.SourceId, propertyDescriptor.PropertyId);
					dataRoute.OnData -= OnRoutedData;
				}

				selectedProperties = newSelectedProperties;
			}
		}
	}
}
