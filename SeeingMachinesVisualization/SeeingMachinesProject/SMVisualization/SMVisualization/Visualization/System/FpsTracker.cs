using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMVisualization.Visualization
{
	public class FpsTracker
	{
		private int m_CurrentFrames = 0;
		private int m_LastFPS = 60; // Guess that we're starting at 60FPS
		private long m_StartTime = 0;

		public int Value
		{
			get
			{
				return m_LastFPS;
			}
		}

		public FpsTracker()
		{
			m_StartTime = DateTime.Now.Ticks;
		}

		public void MarkNewFrame()
		{
			if ((DateTime.Now.Ticks - m_StartTime) / 10000000.0F >= 1.0F)
			{
				m_LastFPS = m_CurrentFrames;
				m_CurrentFrames = 0;
				m_StartTime = DateTime.Now.Ticks;
			}

			DebugView.AddText("FPS: " + Value);
			DebugView.AddText("Time Delta: " + SMVisualization.TimeDelta);

			m_CurrentFrames++;
		}
	}
}
