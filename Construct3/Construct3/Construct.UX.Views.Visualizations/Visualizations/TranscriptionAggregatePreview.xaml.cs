using System;
using System.Collections.Concurrent;
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
using Telerik.Windows.Controls.TimeBar;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	/// <summary>
	/// Interaction logic for TranscriptionAggregatePreview.xaml
	/// </summary>
	public partial class TranscriptionAggregatePreview : AggregateVisualization
	{
		//	Guids pulled from Transcription sensor's construct.xml
		private static Guid TranscribedTextProperty = Guid.Parse("659C11DC-D8C4-4D1D-8ECC-379004191758");
		private static Guid TranscriptionEndTimeProperty = Guid.Parse("96A631F9-6711-4260-8180-3262349DC692");
		private static Guid ConfidenceProperty = Guid.Parse("84852A1B-2179-4267-AAF0-F096BF5A07A3");

		private static Dictionary<Guid, Type> RequiredDataProperties = new Dictionary<Guid, Type>()
		{
			{ TranscribedTextProperty, typeof(String) },
			{ TranscriptionEndTimeProperty, typeof(DateTime) },
			{ ConfidenceProperty, typeof(double) }
		};

		public override IEnumerable<Guid> VisualizableDataTypes
		{
			//	Transcription datatype Guid (pulled from Transcription sensor's construct.xml definition)
			get { return new List<Guid>() { Guid.Parse("26265F71-DB2A-4194-ADE3-DE983600D28D") }; }
		}

		public TranscriptionAggregatePreview(ClientDataStore dataStore, SubscriptionTranslator translator) : base(dataStore)
		{
			InitializeComponent();

			this.translator = translator;

			OnSubscriptionAdded += AddVisualization;
			OnSubscriptionRemoved += RemoveVisualization;

			Timeline.ItemsSource = visualizedData;
		}

		private SubscriptionTranslator translator;

		//	Not all TimelineViewModels are fully constructed upon creation, they may need to wait
		//		for additional property data before they can be added to the timeline
		private ConcurrentBag<TimelineViewModel> collectedViewModels = new ConcurrentBag<TimelineViewModel>(); 

		private ObservableCollection<TimelineViewModel> visualizedData = new ObservableCollection<TimelineViewModel>();
		private Dictionary<DataSubscription, List<PropertyDataStore>> propertyDataStores = new Dictionary<DataSubscription, List<PropertyDataStore>>();

		private SessionInfo visualizedSession;


		//	Maintains all SimplifiedPropertyValues that are required to make a single datatype
		class TimelineViewModel
		{
			public static TimelineViewModel FromSimplifiedPropertyValue(SubscriptionTranslator translator, SimplifiedPropertyValue value)
			{
				var subscriptionForData = new DataSubscription() {SourceId = value.SensorId, PropertyId = value.PropertyId};
				var propertyTypeTranslation = translator.GetTranslation(subscriptionForData);
				TimelineViewModel result = new TimelineViewModel()
				{
					TimeStamp = value.TimeStamp,
					VisualizedSubscription = new DataSubscription() {SourceId = value.SensorId},
					Category = String.Format("{0} {1}", propertyTypeTranslation.SourceName, propertyTypeTranslation.SourceTypeName)
				};
				result.AddNewData(value);

				return result;
			}

			private ConcurrentBag<SimplifiedPropertyValue> collectedData = new ConcurrentBag<SimplifiedPropertyValue>();

			private bool IsDatatypeComplete
			{
				get
				{
					List<Guid> requiredProperties = new List<Guid>()
					{
						TranscribedTextProperty, // TranscribedText
						TranscriptionEndTimeProperty, // TranscriptionEndTime
						ConfidenceProperty, // Confidence
					};
					return collectedData.Select(spv => spv.PropertyId).Intersect(requiredProperties).Count() == requiredProperties.Count;
				}
			}

			public void AddNewData(SimplifiedPropertyValue value)
			{
				if (IsDatatypeComplete)
					return;

				collectedData.Add(value);

				if (IsDatatypeComplete && AggregationComplete != null)
				{
					var transcribedText = collectedData.First(spv => spv.PropertyId == TranscribedTextProperty);
					var confidence = collectedData.First(spv => spv.PropertyId == ConfidenceProperty);
					var transcriptionEndTime = collectedData.First(spv => spv.PropertyId == TranscriptionEndTimeProperty);

					Text = transcribedText.Value as String;
					Confidence = Convert.ToDouble(confidence.Value);
					TimeSpan = ((DateTime) transcriptionEndTime.Value) - TimeStamp;

					AggregationComplete(this);
				}
			}

			public event Action<TimelineViewModel> AggregationComplete;

			public double Confidence { get; set; }
			public String Text { get; set; }
			public String Category { get; set; }
			public DateTime TimeStamp { get; set; }
			public TimeSpan TimeSpan { get; set; }
			public DataSubscription VisualizedSubscription { get; set; }
		}

		public override void ChangeVisualizedDataRange(SessionInfo newInfo)
		{
			base.ChangeVisualizationArea(newInfo);

			Timeline.PeriodStart = newInfo.StartTime.Value;
			Timeline.PeriodEnd = newInfo.EndTime ?? newInfo.StartTime.Value;

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

		public override void ChangeVisualizationArea(SessionInfo sessionInfo)
		{
			base.ChangeVisualizationArea(sessionInfo);

			Timeline.VisiblePeriodStart = sessionInfo.ViewStartTime.Value;
			Timeline.VisiblePeriodEnd = sessionInfo.ViewEndTime.Value;
		}

		public override void ChangeRealTimeRangeEnd(DateTime endTime)
		{
			base.ChangeRealTimeRangeEnd(endTime);

			Timeline.PeriodEnd = endTime;
		}

		private void RemoveVisualization(DataSubscription dataSubscription)
		{
			if (propertyDataStores.ContainsKey(dataSubscription))
			{
				var dataStores = propertyDataStores[dataSubscription];
				if (dataStores != null)
				{
					foreach (var store in dataStores)
					{
						if (store != null)
						{
							store.OnNewData -= newDataStore_OnNewData;
							DataStore.ReleaseDataStore(store);
						}
					}
				}

				propertyDataStores.Remove(dataSubscription);
			}

			for (int i = 0; i < visualizedData.Count; i++)
			{
				if (visualizedData[i].VisualizedSubscription.SourceId == dataSubscription.SourceId)
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

			List<PropertyDataStore> requiredDataStores = new List<PropertyDataStore>();
			foreach (var property in RequiredDataProperties)
			{
				var propertySubscription = new DataSubscription()
				{
					PropertyId = property.Key,
					PropertyType = property.Value,
					SourceId = dataSubscription.SourceId
				};

				var newDataStore = DataStore.GenerateDataStore(propertySubscription, visualizedSession.StartTime.Value,
				visualizedSession.EndTime ?? DateTime.MaxValue);
				newDataStore.OnNewData += newDataStore_OnNewData;

				foreach (var existingData in newDataStore.Data)
					ProcessNewData(existingData);

				requiredDataStores.Add(newDataStore);
			}

			propertyDataStores.Add(dataSubscription, requiredDataStores);
		}

		private void ProcessNewData(SimplifiedPropertyValue value)
		{
			var viewModelForValue = collectedViewModels.SingleOrDefault(vm => vm.TimeStamp == value.TimeStamp);

			if (viewModelForValue == null)
			{
				var newViewModel = TimelineViewModel.FromSimplifiedPropertyValue(translator, value);
				newViewModel.AggregationComplete += OnModelAggregationComplete;
				collectedViewModels.Add(newViewModel);
			}
			else
			{
				viewModelForValue.AddNewData(value);
			}
		}

		private void OnModelAggregationComplete(TimelineViewModel model)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				visualizedData.Add(model);
			}));
		}

		void newDataStore_OnNewData(SimplifiedPropertyValue obj)
		{
			ProcessNewData(obj);
		}
	}
}
