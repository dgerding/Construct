using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMFramework;

namespace SMVisualization.Visualization
{
	public interface PersonRenderer
	{
		void Draw(SeeingModule source, SeeingModule[] moduleBatch, SubjectRenderOptions renderOptions);
	}
}
