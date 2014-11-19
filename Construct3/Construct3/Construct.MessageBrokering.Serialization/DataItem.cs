using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.MessageBrokering.Serialization
{
	public class DataItem
	{
		public String Name { get; internal set; }

		public Dictionary<String, object> PropertyValues { get; internal set; }

		public Guid SourceId { get; internal set; }

		public DateTime Timestamp { get; internal set; }
	}
}
