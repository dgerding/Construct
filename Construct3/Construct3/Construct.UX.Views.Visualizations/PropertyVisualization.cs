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

		protected event Action<DataSubscription> OnSubscriptionAdded;
		protected event Action<DataSubscription> OnSubscriptionRemoved;

		public event Action OnClosed;

		protected ClientDataStore DataStore { get; private set; }

		public virtual IEnumerable<Type> VisualizableTypes { get { return null; } } 

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

		/// <summary>
		/// Usable when the current session has no end time specified (visualizing real-time data ad infinitum), specifies the new end time
		/// to use for rendering purposes.
		/// </summary>
		/// <param name="endTime">The new end time to use for display purposes.</param>
		public virtual void ChangeRealTimeRangeEnd(DateTime endTime)
		{
			//	Required since one new piece of data means that the time ranges of *all* visualizations need to be updated to that latest time.
			//		Would be easiest to let the visualizations update their own time ranges based on their own data, but i.e. text data probably
			//		won't update nearly as often as numeric data.

			//	This is a piss-poor attempt at an implementation of real-time data visualization. The data backend is fine but the UI bits
			//		are lacking in architecture. Should probably use composition with some implementation class that would define the
			//		visualized data range.
		}

		/// <summary>
		/// Changes the area of loaded data to visualize, generally used for pan/zoom operations.
		/// </summary>
		/// <param name="sessionInfo">The new period of time to visualize. (ViewStartTime, ViewEndTime)</param>
		public virtual void ChangeVisualizationArea(SessionInfo sessionInfo)
		{
			
		}

		/// <summary>
		/// Changes the data that the visualization is currently using as a data source.
		/// </summary>
		/// <param name="sessionInfo">The new period of time to pull data from. (StartTime, EndTime)</param>
		public virtual void ChangeVisualizedDataRange(SessionInfo sessionInfo)
		{
			
		}
	}
}
