using System;

namespace Telerik.Expressions
{
	internal class DateTimeToken : LiteralToken<DateTime>
	{
		public DateTimeToken(DateTime value)
			: base(value)
		{
		}
	}
}