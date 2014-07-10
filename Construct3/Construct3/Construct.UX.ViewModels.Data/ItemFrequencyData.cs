using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.UX.ViewModels.Data
{
	struct ItemFrequencyData
	{
		public ItemFrequencyData(uint itemFrequency, SpecificTimeSpan timeSpan)
		{
			this.ItemFrequency = itemFrequency;
			this.SpecificTimeSpan = timeSpan;
		}

		public uint ItemFrequency;
		public SpecificTimeSpan SpecificTimeSpan;

		public static ItemFrequencyData GetWeightedMerge(SpecificTimeSpan targetTimeSpan, params object[] frequencyData)
		{
			throw new NotImplementedException();
		}
	}
}
