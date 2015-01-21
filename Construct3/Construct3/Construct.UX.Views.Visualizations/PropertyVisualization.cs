using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Windows;
using System.Windows.Controls;
using Construct.MessageBrokering.Serialization;
using Construct.UX.Views.Helper;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations
{
	public class PropertyVisualization : UserControl, IVisualizer
	{
		protected event Action<DataSubscription> OnSubscriptionAdded;
		protected event Action<DataSubscription> OnSubscriptionRemoved;

		public event Action OnClosed;

		protected ClientDataStore DataStore { get; private set; }

		public virtual IEnumerable<Type> VisualizableTypes { get { return Enumerable.Empty<Type>(); } } 

		public PropertyVisualization(ClientDataStore sourceDataStore)
		{
			DataStore = sourceDataStore;
		}

		public void Close()
		{
			if (OnClosed != null)
				OnClosed();
		}

		public void RequestAddVisualization(DataSubscription subscription)
		{
			if (OnSubscriptionAdded != null)
				OnSubscriptionAdded(subscription);
		}

		public void RequestRemoveVisualization(DataSubscription subscription)
		{
			if (OnSubscriptionRemoved != null)
				OnSubscriptionRemoved(subscription);
		}

		public virtual void ChangeRealTimeRangeEnd(DateTime endTime)
		{
			//	Required since one new piece of data means that the time ranges of *all* visualizations need to be updated to that latest time.
			//		Would be easiest to let the visualizations update their own time ranges based on their own data, but i.e. text data probably
			//		won't update nearly as often as numeric data.

			//	This is a piss-poor attempt at an implementation of real-time data visualization. The data backend is fine but the UI bits
			//		are lacking in architecture. Should probably use composition with some implementation class that would define the
			//		visualized data range.
		}

		public virtual void ChangeVisualizationArea(SessionInfo sessionInfo)
		{
			
		}

		public virtual void ChangeVisualizedDataRange(SessionInfo sessionInfo)
		{
			
		}
	}
}
