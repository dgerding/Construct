using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	class NumericPropertyVisualization : SplitVisualizationContainer
	{
		public NumericPropertyVisualization(SessionInfo sessionInfo, ClientDataStore dataStore, SubscriptionTranslator translator) : base(translator)
		{
			PreviewContainer.Children.Add(new NumericPropertyPreview(dataStore, translator));
			DetailsContainer.Children.Add(new NumericPropertyDetails(dataStore, translator));

			VisualizationName = "Numeric Visualization";
		}
	}
}
