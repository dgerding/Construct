using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	class TranscriptionAggregateVisualization : SplitVisualizationContainer
	{
		public TranscriptionAggregateVisualization(SessionInfo sessionInfo, ClientDataStore dataStore, SubscriptionTranslator translator)
			: base(translator)
		{
			PreviewContainer.Children.Add(new TranscriptionAggregatePreview(dataStore, translator));

			VisualizationName = "Transcription Visualization";
		}
	}
}
