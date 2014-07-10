using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.UX.ViewModels.Data
{
	public struct SpecificTimeSpan
	{
		public SpecificTimeSpan(DateTime start, DateTime end)
		{
			if (start > end)
				throw new ArgumentException("Start time cannot be later than the end time.");

			Start = start;
			End = end;
		}

		public SpecificTimeSpan(DateTime start, TimeSpan timeSpan)
		{
			//	For negative time-spans
			if (timeSpan.Ticks < 0)
			{
				Start = start + timeSpan;
				End = start;
			}
			else
			{
				Start = start;
				End = start + timeSpan;
			}
		}

		public TimeSpan GetOverlap(SpecificTimeSpan other)
		{
			long ticks = Math.Min(this.End.Ticks, other.End.Ticks) - Math.Max(this.Start.Ticks, other.Start.Ticks);
			return TimeSpan.FromTicks(Math.Max(0, ticks));
		}

		public static SpecificTimeSpan operator +(SpecificTimeSpan specificTimeSpan, TimeSpan generalTimeSpan)
		{
			return new SpecificTimeSpan(specificTimeSpan.Start + generalTimeSpan, specificTimeSpan.End + generalTimeSpan);
		}

		public static bool operator ==(SpecificTimeSpan lhs, SpecificTimeSpan rhs)
		{
			return lhs.Start == rhs.Start && lhs.End == rhs.End;
		}

		public static bool operator !=(SpecificTimeSpan lhs, SpecificTimeSpan rhs)
		{
			return lhs.Start != rhs.Start || lhs.End != rhs.End;
		}

		public DateTime Start;
		public DateTime End;
		public TimeSpan TimeSpan { get { return End - Start; } }
	}
}
