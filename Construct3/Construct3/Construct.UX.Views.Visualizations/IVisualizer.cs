using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	public interface IVisualizer
	{
		/// <summary>
		/// Usable when the current session has no end time specified (visualizing real-time data ad infinitum), specifies the new end time
		/// to use for rendering purposes.
		/// </summary>
		/// <param name="endTime">The new end time to use for display purposes.</param>
		void ChangeRealTimeRangeEnd(DateTime endTime);

		/// <summary>
		/// Changes the area of loaded data to visualize, generally used for pan/zoom operations.
		/// </summary>
		/// <param name="sessionInfo">The new period of time to visualize. (ViewStartTime, ViewEndTime)</param>
		void ChangeVisualizationArea(SessionInfo sessionInfo);

		/// <summary>
		/// Changes the data that the visualization is currently using as a data source.
		/// </summary>
		/// <param name="sessionInfo">The new period of time to pull data from. (StartTime, EndTime)</param>
		void ChangeVisualizedDataRange(SessionInfo sessionInfo);

		void RequestAddVisualization(DataSubscription subscription);

		void RequestRemoveVisualization(DataSubscription subscription);

		void Close();
	}
}
