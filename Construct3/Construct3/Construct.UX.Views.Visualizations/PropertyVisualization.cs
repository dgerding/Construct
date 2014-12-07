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
	public class PropertyVisualization : UserControl
	{
		public virtual Type GetVisualizedType()
		{
			return null; 
		}

		public event Action<object, ChartVisualizationInfo> OnVisualizationRangeChanged;

		protected event Action<DataSubscription> OnSubscriptionAdded;
		protected event Action<DataSubscription> OnSubscriptionRemoved;

		public event Action OnClosed;

		protected ClientDataStore DataStore { get; private set; }

		public virtual IEnumerable<Type> VisualizableTypes { get { return null; } } 

		public String VisualizationName { get; protected set; }
		public virtual int MaxProperties { get { return int.MaxValue; } }

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

		public virtual void ChangeVisualizationArea(SessionInfo newInfo)
		{
			
		}

		protected void NotifyUserChangedVisualizationRange(ChartVisualizationInfo newChartInfo)
		{
			if (OnVisualizationRangeChanged != null)
				OnVisualizationRangeChanged(this, newChartInfo);
		}
	}
}
