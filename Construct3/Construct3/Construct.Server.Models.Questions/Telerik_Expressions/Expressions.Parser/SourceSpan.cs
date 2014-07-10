using System;
namespace Telerik.Expressions
{
	internal struct SourceSpan
	{
		private readonly SourceLocation start;
		private readonly SourceLocation end;

		public SourceLocation Start
		{
			get
			{
				return this.start;
			}
		}

		public SourceLocation End
		{
			get
			{
				return this.end;
			}
		}

		public bool IsValid
		{
			get
			{
				return
					this.start.IsValid &&
					this.end.IsValid;
			}
		}

		public SourceSpan(SourceLocation start, SourceLocation end)
		{
			ValidateLocations(start, end);

			this.start = start;
			this.end = end;
		}

		public static readonly SourceSpan Invalid = new SourceSpan(SourceLocation.Invalid, SourceLocation.Invalid);

		public static bool operator ==(SourceSpan left, SourceSpan right)
		{
			return AreEqual(left, right);
		}

		public static bool operator !=(SourceSpan left, SourceSpan right)
		{
			return !AreEqual(left, right);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SourceSpan))
			{
				return false;
			}

			var other = (SourceSpan)obj;
			return AreEqual(this, other);
		}

		public override string ToString()
		{
			return this.start + " - " + this.end;
		}

		public override int GetHashCode()
		{
			// 7 bits for each column (0-128), 9 bits for each row (0-512), xor helps if
			// we have a bigger file.
			return 
				this.start.Column ^ 
				(this.end.Column << 7) ^ 
				(this.start.Line << 14) ^ 
				(this.end.Line << 23);
		}

		private static void ValidateLocations(SourceLocation start, SourceLocation end)
		{
			if (start.IsValid && end.IsValid)
			{
				if (start > end)
				{
					throw new ArgumentException("Start must be less than or equal to End");
				}
			}
			else if (start.IsValid || end.IsValid)
			{
				throw new ArgumentException("Start and End must both be valid or both invalid");
			}
		}

		private static bool AreEqual(SourceSpan left, SourceSpan right)
		{
			return
				left.start == right.start &&
				left.end == right.end;
		}
	}
}