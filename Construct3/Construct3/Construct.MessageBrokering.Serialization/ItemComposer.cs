using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.MessageBrokering.Serialization
{
	public class ItemComposer
	{
		public String ConstructUri { get; private set; }

		public ItemComposer(String constructUri)
		{
			ConstructUri = constructUri;
		}

		public ComposedItemCollection SynthesizeData(Guid dataTypeId, Guid[] sources, DateTime startTimeUtc, DateTime endTimeUtc)
		{
			ComposedItemCollection itemCollection = new ComposedItemCollection();

			return itemCollection;
		}











		Guid getDataTypeGuid();
		Guid[] getSourceIds();
		DateTime getStartTimeUTC();
		DateTime getEndTimeUTC();

		String constructServerUri;
		public void example()
		{
			ItemComposer itemComposer = new ItemComposer(constructServerUri);

			Guid myDataSourceId = getDataTypeGuid();
			Guid[] mySourceIds = getSourceIds(); // Can be null (specified all sources that emit that datatype)
			DateTime startTime = getStartTimeUTC();
			DateTime endTime = getEndTimeUTC();

			ComposedItemCollection itemCollection = itemComposer.SynthesizeData(myDataSourceId, mySourceIds, startTime, endTime);

			foreach (Guid propertyTypeId in itemCollection.Doubles.Keys)
			{
				Dictionary<Guid, ValuePayload<double>> itemsWithProperty = itemCollection.Doubles[propertyTypeId];

				foreach (Guid itemId in itemsWithProperty.Keys)
				{
					ValuePayload<double> currentValue = itemsWithProperty[itemId];
					// currentValue.Value
					// currentValue.Latitude
					// currentValue.Longitude
					// currentValue.StartTime
					// currentValue.TimeSpan
				}
			}
		}
	}
}
