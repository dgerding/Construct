using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.MessageBrokering.Serialization
{
	//	Used for real-time data streaming of unique property values
	public struct SimplifiedPropertyValue
	{
		public object Value { get; set; }

		public Guid SensorId { get; set; }

		public Guid PropertyId { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
