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
	public class ChartToSessionConverter
	{
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
			normalizedViewStart = (double)(sourceSession.ViewStartTime - sourceSession.StartTime).Value.Ticks/
			                      (double)(sourceSession.EndTime - sourceSession.StartTime).Value.Ticks;
			normalizedViewEnd = (double)(sourceSession.ViewEndTime - sourceSession.StartTime).Value.Ticks/
			                    (double)(sourceSession.EndTime - sourceSession.StartTime).Value.Ticks;

			if (sourceSession.StartTime.Value.Ticks > sourceSession.EndTime.Value.Ticks)
				return false;

			if (normalizedViewStart > normalizedViewEnd)
				return false;

			if (targetChart.VisSize.IsEmpty)
				throw new Exception("ChartInfo VisSize cannot be blank.");

			var newZoom = new Size(Math.Max(1.0, 1.0/(normalizedViewEnd - normalizedViewStart)), 1.0);
			targetChart.Zoom = newZoom;
			var newPan = new Point(-normalizedViewStart*targetChart.Zoom.Width*targetChart.VisSize.Width, 0.0);
			targetChart.PanOffset = newPan;

			return true;
		}
	}
}
