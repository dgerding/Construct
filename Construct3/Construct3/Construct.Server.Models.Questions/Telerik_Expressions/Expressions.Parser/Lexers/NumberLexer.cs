using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Telerik.Expressions
{
	internal class NumberLexer : LiteralLexerBase
	{
		private const char ExponentL = 'e';
		private const char ExponentU = 'E';

		private bool isDecimal;
		private bool isExponential;

		private static NumberFormatInfo NumberFormatInfo
		{
			get
			{
				return NumberFormatInfo.InvariantInfo;
			}
		}

		// TODO: Fix this first or default 'limitation'.
		private static readonly char DecimalSeparator = NumberFormatInfo.NumberDecimalSeparator.FirstOrDefault();

		private NumberLexer(ParserContext context)
			: base(context)
		{
		}

		public static Token ReadNumber(ParserContext context)
		{
			return new NumberLexer(context).ReadToken();
		}

		public static bool IsCharNumberStart(char ch)
		{
			return char.IsDigit(ch);
		}

		protected override void ReadToEnd()
		{
            Debug.Assert(char.IsDigit(this.PeekChar()), "char.IsDigit(this.PeekChar())");

			this.ReadDigits();

			this.ReadDecimalSeparator();

			this.ReadDigits();

			this.ReadExponent();
		}

		protected internal override bool TryCreateTokenFromLiteral(string literal, out Token token)
		{
			if (this.isExponential)
			{
				return TryCreateDoubleToken(literal, out token);
			}

			if (this.isDecimal)
			{
				return TryCreateDecimalToken(literal, out token);
			}

			return TryCreateIntegerToken(literal, out token);
		}

		private static bool TryCreateDoubleToken(string literal, out Token token)
		{
			token = null;
			double value;

			if (double.TryParse(literal, NumberStyles.Float, NumberFormatInfo, out value))
			{
				token = new DoubleToken(value);
				return true;
			}
			return false;
		}

		private static bool TryCreateDecimalToken(string literal, out Token token)
		{
			token = null;
			decimal value;

			if (decimal.TryParse(literal, NumberStyles.Number, NumberFormatInfo, out value))
			{
				token = new DecimalToken(value);
				return true;
			}
			return false;
		}

		private static bool TryCreateIntegerToken(string literal, out Token token)
		{
			token = null;
			int value;

			if (int.TryParse(literal, NumberStyles.Integer, NumberFormatInfo, out value))
			{
				token = new IntegerToken(value);
				return true;
			}
			return false;
		}

		private void ReadDecimalSeparator()
		{
			if (this.PeekChar() == DecimalSeparator)
			{
				this.isDecimal = true;
				this.ReadAndAddChar();
			}
		}

		private void ReadExponent()
		{
			var ch = this.PeekChar();

			if (ch == ExponentL || ch == ExponentU)
			{
				this.isExponential = true;

				this.ReadAndAddChar();

				ch = this.PeekChar();
				if (ch == '-' || ch == '+')
				{
					this.ReadAndAddChar();
				}

				this.ReadDigits();
			}
		}

		// We should eagerly read all digits because our source SourceReader is move forward only.
		private void ReadDigits()
		{
			while (char.IsDigit(this.PeekChar()))
			{
				this.ReadAndAddChar();
			}
		}
	}
}
