using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	class TextPropertyVisualization : SplitVisualizationContainer
	{
		public TextPropertyVisualization(SessionInfo sessionInfo, ClientDataStore dataStore, SubscriptionTranslator translator) : base(translator)
		{
			PreviewContainer.Children.Add(new TextPropertyPreview(dataStore, translator, sessionInfo));
			DetailsContainer.Children.Add(new TextPropertyDetails(dataStore, translator));

			VisualizationName = "Text Visualization";
		}
	}
}
