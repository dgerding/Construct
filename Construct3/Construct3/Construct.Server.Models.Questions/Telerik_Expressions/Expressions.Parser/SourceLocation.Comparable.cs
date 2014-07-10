using System;

namespace Telerik.Expressions
{
    internal partial struct SourceLocation : IComparable<SourceLocation>
	{
		public static int Compare(SourceLocation left, SourceLocation right)
		{
			var lineComparisonResult = left.line.CompareTo(right.line);

			if (lineComparisonResult == 0)
			{
				return left.column.CompareTo(right.column);
			}

			return lineComparisonResult;
		}

		public static bool operator <(SourceLocation left, SourceLocation right)
		{
			return Compare(left, right) < 0;
		}

		public static bool operator <=(SourceLocation left, SourceLocation right)
		{
			return Compare(left, right) <= 0;
		}

		public static bool operator >(SourceLocation left, SourceLocation right)
		{
			return Compare(left, right) > 0;
		}

		public static bool operator >=(SourceLocation left, SourceLocation right)
		{
			return Compare(left, right) >= 0;
		}

		public int CompareTo(SourceLocation other)
		{
			return Compare(this, other);
		}
	}
}
