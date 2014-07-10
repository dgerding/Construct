using System.Diagnostics;

namespace Telerik.Expressions
{
	internal partial class IdentifierLexer : SafeLiteralReaderLexer
	{
		private IdentifierLexer(ParserContext context)
			: base(context)
		{
		}

		public static bool IsCharIdentifierStart(char ch)
		{
			return IsLetterOrUnderscore(ch) || EscapingIdentifierLexer.IsCharEscapedIdentifierStart(ch);
		}

		private static bool IsValidIdentifierChar(char ch)
		{
			return IsLetterOrUnderscore(ch) || char.IsDigit(ch);
		}

		private static bool IsLetterOrUnderscore(char ch)
		{
			return char.IsLetter(ch) || ch == '_';
		}

		public static IdentifierToken ReadIdentifier(ParserContext context)
		{
			var ch = context.SourceReader.PeekChar();

			if (EscapingIdentifierLexer.IsCharEscapedIdentifierStart(ch))
			{
				return EscapingIdentifierLexer.ReadIdentifier(context);
			}

			var lexer = new IdentifierLexer(context);
			return (IdentifierToken)lexer.ReadToken();
		}

		protected override bool Read()
		{
			var ch = this.PeekChar();

            if (LexerBase.IsCharEof(ch))
			{
				return false;
			}

			if (IsValidIdentifierChar(ch))
			{
				this.ReadAndAddChar();
				return true;
			}

			return false;
		}

		public override Token ReadToken()
		{
            Debug.Assert(IsCharIdentifierStart(this.PeekChar()), "IsCharIdentifierStart(this.PeekChar())");

			return base.ReadToken();
		}

		protected override Token CreateTokenFromLiteral(string literal)
		{
			return new IdentifierToken(literal);
		}
	}
}