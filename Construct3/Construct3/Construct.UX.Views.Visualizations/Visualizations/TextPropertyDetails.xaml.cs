using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Construct.MessageBrokering.Serialization;
using Telerik.Windows.Controls.Timeline;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	/// <summary>
	/// Interaction logic for TextPropertyDetails.xaml
	/// </summary>
	public partial class TextPropertyDetails : PropertyVisualization
	{
		private SubscriptionTranslator translator;

		private ObservableCollection<TimelineViewModel> visualizedData = new ObservableCollection<TimelineViewModel>();
		private Dictionary<DataSubscription, PropertyDataStore> propertyDataStores = new Dictionary<DataSubscription, PropertyDataStore>();

		private SessionInfo visualizedSession;

		struct TimelineViewModel
		{
			public static TimelineViewModel FromSimplifiedPropertyValue(SubscriptionTranslator translator, SimplifiedPropertyValue value)
			{
				if (!(value.Value is string))
				{
					if (Debugger.IsAttached)
						Debugger.Break();
				}

				var subscriptionForData = new DataSubscription() {PropertyId = value.PropertyId, SourceId = value.SensorId};
				var propertyTypeTranslation = translator.GetTranslation(subscriptionForData);
				TimelineViewModel result = new TimelineViewModel()
				{
					Text = value.Value as String,
					TimeStamp = value.TimeStamp,
					VisualizedSubscription = subscriptionForData,
					Category = String.Format("{0} {1} - {2}", propertyTypeTranslation.SourceName, propertyTypeTranslation.SourceTypeName, propertyTypeTranslation.PropertyName)
				};

				return result;
			}

			public String Text { get; set; }
			public String Category { get; set; }
			public DateTime TimeStamp { get; set; }
			public DataSubscription VisualizedSubscription { get; set; }
		}

		public override void ChangeVisualizedDataRange(SessionInfo newInfo)
		{
			base.ChangeVisualizationArea(newInfo);

			Timeline.PeriodStart = newInfo.SelectedStartTime.Value;
			Timeline.PeriodEnd = newInfo.SelectedEndTime.Value;

			visualizedSession = newInfo;

			var visualizedTypes = propertyDataStores.Select(kvp => kvp.Key).ToList();
			visualizedData.Clear();

			foreach (var subscription in visualizedTypes)
			{
				//	Remove, re-add to reinitialize datastores and change visualized data
				RemoveVisualization(subscription);
				AddVisualization(subscription);
			}
		}



		public TextPropertyDetails(ClientDataStore dataStore, SubscriptionTranslator translator) :
			base(dataStore)
		{
			InitializeComponent();

			this.translator = translator;

			OnSubscriptionAdded += AddVisualization;
			OnSubscriptionRemoved += RemoveVisualization;

			Timeline.ItemsSource = visualizedData;
		}

		private void RemoveVisualization(DataSubscription dataSubscription)
		{
			if (propertyDataStores.ContainsKey(dataSubscription))
			{
				var propertyDataStore = propertyDataStores[dataSubscription];
				if (propertyDataStore != null)
				{
					propertyDataStore.OnNewData -= newDataStore_OnNewData;
					DataStore.ReleaseDataStore(propertyDataStore);
				}

				propertyDataStores.Remove(dataSubscription);
			}

			for (int i = 0; i < visualizedData.Count; i++)
			{
				if (visualizedData[i].VisualizedSubscription == dataSubscription)
					visualizedData.RemoveAt(i--);
			}
		}

		private void AddVisualization(DataSubscription dataSubscription)
		{
			if (visualizedSession == null)
			{
				propertyDataStores.Add(dataSubscription, null);
				return;
			}

			var newDataStore = DataStore.GenerateDataStore(dataSubscription, visualizedSession.SelectedStartTime.Value,
				visualizedSession.SelectedEndTime.Value);
			newDataStore.OnNewData += newDataStore_OnNewData;

			foreach (var existingData in newDataStore.Data)
				visualizedData.Add(TimelineViewModel.FromSimplifiedPropertyValue(translator, existingData));

			propertyDataStores.Add(dataSubscription, newDataStore);
		}

		void newDataStore_OnNewData(SimplifiedPropertyValue obj)
		{
			visualizedData.Add(TimelineViewModel.FromSimplifiedPropertyValue(translator, obj));
		}
	}
}
