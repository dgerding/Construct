using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	public class AggregateVisualization : UserControl, IVisualizer
	{
		protected event Action<DataSubscription> OnSubscriptionAdded;
		protected event Action<DataSubscription> OnSubscriptionRemoved;

		public event Action OnClosed;

		protected ClientDataStore DataStore { get; private set; }

		public virtual IEnumerable<Guid> VisualizableDataTypes { get { return Enumerable.Empty<Guid>(); } }

		public AggregateVisualization(ClientDataStore sourceDataStore)
		{
			DataStore = sourceDataStore;
		}

		public virtual void ChangeRealTimeRangeEnd(DateTime endTime)
		{
			
		}

		public virtual void ChangeVisualizationArea(SessionInfo sessionInfo)
		{
			
		}

		public virtual void ChangeVisualizedDataRange(SessionInfo sessionInfo)
		{
			
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

		public void Close()
		{
			if (OnClosed != null)
				OnClosed();
		}
	}
}
