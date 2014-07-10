using System;

namespace Telerik.Expressions
{
	internal partial struct SourceLocation
	{
		private readonly int line;
		private readonly int column;

		public int Line
		{
			get
			{
				return this.line;
			}
		}

		public int Column
		{
			get
			{
				return this.column;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.line != 0 && this.column != 0;
			}
		}

		public SourceLocation(int line, int column)
			: this(line, column, true)
		{
		}

		private SourceLocation(int line, int column, bool shouldValidate)
		{
			if (shouldValidate)
			{
				Validate(line, column);
			}

			this.line = line;
			this.column = column;
		}

		public static bool operator ==(SourceLocation left, SourceLocation right)
		{
			return AreEqual(left, right);
		}

		public static bool operator !=(SourceLocation left, SourceLocation right)
		{
			return !AreEqual(left, right);
		}

		public static readonly SourceLocation Invalid = new SourceLocation(0, 0, false);
		
		public static readonly SourceLocation MinValue = new SourceLocation(1, 1);

		public override bool Equals(object obj)
		{
			if (!(obj is SourceLocation))
			{
				return false;
			}

			var other = (SourceLocation)obj;
			return AreEqual(this, other);
		}

		public override int GetHashCode()
		{
			return (this.line << 16) ^ this.column;
		}

		public override string ToString()
		{
			return "(" + this.line + "," + this.column + ")";
		}

		private static bool AreEqual(SourceLocation left, SourceLocation right)
		{
			return
				left.line == right.line &&
				left.column == right.column;
		}

		private static void Validate(int line, int column)
		{
			var message = "Must be greater than or equal to 1";

			if (line < 1)
			{
				throw new ArgumentOutOfRangeException("line", message);
			}

			if (column < 1)
			{
				throw new ArgumentOutOfRangeException("column", message);
			}
		}
	}
}