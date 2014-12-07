using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations
{
	/// <summary>
	/// Converts between the arbitrary pan and zoom values of a RadChartView and the absolute time values of a SessionInfo
	/// </summary>
	class ChartToSessionConverter
	{
		/*
		 * Normalize pan/zoom values.
		 * 
		 * Normalization: x / zoom / totalArea
		 * 
		 */

		public bool UpdateSessionToChart(SessionInfo targetSession, ChartVisualizationInfo sourceChart)
		{
			if (!targetSession.StartTime.HasValue || !targetSession.EndTime.HasValue)
				return false;

			//	Normalized values from 0-1 of the overall Connect/End time
			double normalizedViewStart, normalizedViewEnd, normalizedSelectionStart, normalizedSelectionEnd;
			normalizedViewStart = -sourceChart.PanOffset.X/sourceChart.Zoom.Width/sourceChart.VisSize.Width;
			normalizedViewEnd = -sourceChart.PanOffset.X/sourceChart.Zoom.Width/sourceChart.VisSize.Width +
			                    1.0/sourceChart.Zoom.Width;

			if (normalizedViewEnd < 0.0 || normalizedViewStart > 1.0)
				return false;
			if (normalizedViewStart < 0.0 || normalizedViewStart > 1.0)
				return false;

			targetSession.ViewStartTime = targetSession.StartTime.Value.AddSeconds((targetSession.EndTime - targetSession.StartTime).Value.TotalSeconds*normalizedViewStart);
			targetSession.ViewEndTime = targetSession.StartTime.Value.AddSeconds((targetSession.EndTime - targetSession.StartTime).Value.TotalSeconds*normalizedViewEnd);

			return true;
		}

		public bool UpdateChartToSession(ChartVisualizationInfo targetChart, SessionInfo sourceSession)
		{
			if (!sourceSession.StartTime.HasValue || !sourceSession.EndTime.HasValue ||
				!sourceSession.ViewStartTime.HasValue || !sourceSession.ViewEndTime.HasValue)
				return false;

			//	Can't define a zoom on a single point
			if (sourceSession.ViewStartTime == sourceSession.ViewEndTime)
				return false;

			//	Normalized values from 0-1 of the overall Connect/End time
			double normalizedViewStart, normalizedViewEnd, normalizedSelectionStart, normalizedSelectionEnd;
			normalizedViewStart = (sourceSession.ViewStartTime - sourceSession.StartTime).Value.TotalSeconds/
			                      (sourceSession.EndTime - sourceSession.StartTime).Value.TotalSeconds;
			normalizedViewEnd = (sourceSession.ViewEndTime - sourceSession.StartTime).Value.TotalSeconds/
			                    (sourceSession.EndTime - sourceSession.StartTime).Value.TotalSeconds;

			if (sourceSession.StartTime.Value.Ticks > sourceSession.EndTime.Value.Ticks)
				return false;

			if (normalizedViewStart > normalizedViewEnd)
				return false;

			if (targetChart.VisSize.IsEmpty)
				throw new Exception("ChartInfo VisSize cannot be blank.");

			targetChart.Zoom = new Size(Math.Max(1.0, 1.0 / (normalizedViewEnd - normalizedViewStart)), 1.0);
			targetChart.PanOffset = new Point(-normalizedViewStart * targetChart.Zoom.Width * targetChart.VisSize.Width, 0.0);

			return true;
		}
	}
}
