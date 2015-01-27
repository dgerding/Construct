using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Construct.UX.ViewModels.Visualizations.VisualizationsServiceReference;
using Construct.UX.Views.Visualizations.Visualizations;
using Telerik.Windows.Controls;

namespace Construct.UX.Views.Visualizations
{
	class VisualizationSerializationHelper
	{
		public IEnumerable<Visualization> GetVisualizationsForContainer(IEnumerable<Visualization> referenceVisualizations, SplitVisualizationContainer visualizationContainer)
		{
			List<Visualization> result = new List<Visualization>();

			var visualizedProperties = visualizationContainer.SelectedProperties;
			foreach (var property in visualizedProperties)
			{
				Visualization newVis = new Visualization()
				{
					PropertyID = ((DataSubscription) property.Reference).PropertyId,
					SourceID = ((DataSubscription) property.Reference).SourceId,
					PaneID = visualizationContainer.ID
				};

				//	Aggregate visualizations don't visualize specific properties, they visualize sources
				if (visualizationContainer.PreviewVisualization is AggregateVisualization)
					newVis.PropertyID = ((DataSubscription) property.Reference).SourceId;

				var matchedVis =
					referenceVisualizations.SingleOrDefault(v => v.PropertyID == newVis.PropertyID && v.PaneID == newVis.PaneID);

				if (matchedVis == null)
					newVis.ID = Guid.NewGuid();
				else
				{
					newVis.ID = matchedVis.ID;
					newVis.VisualizerID = matchedVis.VisualizerID;
				}

				result.Add(newVis);
			}

			return result;
		}

		public String GenerateSerializationTagForVisualization(SplitVisualizationContainer visContainer)
		{
			return visContainer.ID + "|" + visContainer.GetType().Name;
		}

		public Type GetVisualizationTypeFromSerializationTag(String tag)
		{
			var typeString = tag.Split('|')[1];
			//	BAD programmer! Hard-coded strings...
			switch (typeString)
			{
				case "TranscriptionAggregateVisualization":
					return typeof (TranscriptionAggregateVisualization);

				case "NumericPropertyVisualization":
					return typeof (NumericPropertyVisualization);

				default:
					throw new Exception(String.Format("No known visualization type with name '{0}'", typeString));
			}
		}

		public Guid GetVisualizationIdFromSerializationTag(String tag)
		{
			return Guid.Parse(tag.Split('|')[0]);
		}
	}
}
