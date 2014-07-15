using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.MessageBrokering.Serialization
{
	public struct ValuePayload<T>
	{
		public DateTime StartTime;
		public TimeSpan TimeSpan;
		public String Latitude, Longitude;
		public T Value;
	}
}
