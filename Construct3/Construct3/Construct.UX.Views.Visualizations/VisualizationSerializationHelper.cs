using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	class VisualizationSerializationHelper
	{
		public IEnumerable<Visualization> GetVisualizationsForContainer(RadPane visualizationContainer)
		{
			List<Visualization> result = new List<Visualization>();
			SplitVisualizationContainer uiVis = visualizationContainer.Content as SplitVisualizationContainer;
			if (uiVis == null)
				throw new Exception("Couldn't get visualization for serialization");

			var visualizedProperties = uiVis.SelectedProperties;
			foreach (var property in visualizedProperties)
			{
				result.Add(new Visualization()
				{
					ID = (Guid)visualizationContainer.Tag,
					PropertyID = ((DataSubscription)property.Reference).PropertyId,
				});
			}


			return result;
		}
	}
}
