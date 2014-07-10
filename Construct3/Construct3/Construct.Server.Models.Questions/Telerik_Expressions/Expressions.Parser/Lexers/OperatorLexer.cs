using System.Diagnostics;

namespace Telerik.Expressions
{
	internal class OperatorLexer : SafeLiteralReaderLexer
	{
		private OperatorLexer(ParserContext context)
			: base(context)
		{
		}

		public static OperatorToken ReadOperator(ParserContext context)
		{
            Debug.Assert(OperatorToken.IsOperatorStart(context.SourceReader.PeekChar().ToString()), "OperatorToken.IsOperatorStart(context.SourceReader.PeekChar().ToString())");

			return (OperatorToken)new OperatorLexer(context).ReadToken();
		}

		protected override bool Read()
		{
			var ch = this.PeekChar();

            if (LexerBase.IsCharEof(ch))
			{
				return false;
			}

			return this.TryAddChar(ch);
		}

		private bool TryAddChar(char ch)
		{
			var name = this.CreateLiteralFromReadChars() + ch;
			if (OperatorToken.IsOperatorStart(name))
			{
				this.ReadAndAddChar();
				return true;
			}

			return false;
		}

		protected override Token CreateTokenFromLiteral(string literal)
		{
            Debug.Assert(OperatorToken.IsOperatorName(literal), "OperatorToken.IsOperatorName(literal)");

			return OperatorToken.FromName(literal);
		}
	}
}
