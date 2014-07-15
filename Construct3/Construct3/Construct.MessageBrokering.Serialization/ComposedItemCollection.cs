using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.MessageBrokering.Serialization
{
	public class ComposedItemCollection
	{
		/// <summary>
		///	Format: Doubles[PropertyId][ItemId] = PropertyValueForAnItem
		/// </summary>
		public Dictionary<Guid, Dictionary<Guid, ValuePayload<double>>> Doubles = new Dictionary<Guid,Dictionary<Guid,ValuePayload<double>>>();

		/// <summary>
		///	Format: Floats[PropertyId][ItemId] = PropertyValueForAnItem
		/// </summary>
		public Dictionary<Guid, Dictionary<Guid, ValuePayload<float>>> Floats = new Dictionary<Guid,Dictionary<Guid,ValuePayload<float>>>();

		/// <summary>
		///	Format: Bools[PropertyId][ItemId] = PropertyValueForAnItem
		/// </summary>
		public Dictionary<Guid, Dictionary<Guid, ValuePayload<bool>>> Bools = new Dictionary<Guid,Dictionary<Guid,ValuePayload<bool>>>();

		/// <summary>
		///	Format: Ints[PropertyId][ItemId] = PropertyValueForAnItem
		/// </summary>
		public Dictionary<Guid, Dictionary<Guid, ValuePayload<int>>> Ints = new Dictionary<Guid,Dictionary<Guid,ValuePayload<int>>>();

		/// <summary>
		///	Format: Longs[PropertyId][ItemId] = PropertyValueForAnItem
		/// </summary>
		public Dictionary<Guid, Dictionary<Guid, ValuePayload<long>>> Longs = new Dictionary<Guid,Dictionary<Guid,ValuePayload<long>>>();



		public DateTime StartTime;
		public TimeSpan TimeSpan;
	}
}
