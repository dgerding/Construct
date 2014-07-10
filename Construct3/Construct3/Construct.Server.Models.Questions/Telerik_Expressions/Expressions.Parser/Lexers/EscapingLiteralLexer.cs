using System.Collections.Generic;
using System.Diagnostics;

namespace Telerik.Expressions
{
	internal abstract class EscapingLiteralLexer : StartEndLiteralLexer
	{
		private readonly HashSet<char> escapingChars = new HashSet<char>();
		private readonly HashSet<char> charsToEscape = new HashSet<char>();

		public EscapingLiteralLexer(ParserContext context, char startChar, char endChar)
			: base(context, startChar, endChar)
		{
		}

		public EscapingLiteralLexer(ParserContext context, char uniformStartEndChar)
			: base(context, uniformStartEndChar)
		{
		}

		public void AddEscapingChar(char ch)
		{
			this.escapingChars.Add(ch);
		}

		public void AddCharToEscape(char ch)
		{
			this.charsToEscape.Add(ch);
		}

		protected override bool Read()
		{
			var ch = this.PeekChar();

			if (this.IsEscapingChar(ch))
			{
				return this.ReadEscapingSequence();
			}
			else
			{
				return base.Read();
			}
		}

		private bool ReadEscapingSequence()
		{
			var escapingChar = this.ReadChar();
            Debug.Assert(this.IsEscapingChar(escapingChar), "this.IsEscapingChar(escapingChar)");

			var escapedChar = this.PeekChar();
			if (this.ShouldEscapeChar(escapedChar))
			{
				this.ReadAndAddChar();
				return true;
			}
			else
			{
				return this.TryAddChar(escapingChar);
			}
		}

		private bool IsEscapingChar(char ch)
		{
			return this.escapingChars.Contains(ch);
		}

		private bool ShouldEscapeChar(char ch)
		{
			return this.charsToEscape.Contains(ch);
		}
	}
}