using System.Diagnostics;

namespace Telerik.Expressions
{
	internal class SyntaxLexer : SafeLiteralReaderLexer
	{
		private SyntaxLexer(ParserContext context)
			: base(context)
		{
		}

		public static SyntaxToken ReadSyntax(ParserContext context)
		{
            Debug.Assert(SyntaxToken.IsSyntaxToken(context.SourceReader.PeekChar()), "SyntaxToken.IsSyntaxToken(context.SourceReader.PeekChar())");

			return (SyntaxToken)new SyntaxLexer(context).ReadToken();
		}

		protected override bool Read()
		{
			this.ReadAndAddChar();

			return false;
		}

		protected override Token CreateTokenFromLiteral(string literal)
		{
            Debug.Assert(literal.Length == 1, "literal.Length == 1");

			return SyntaxToken.FromChar(literal[0]);
		}
	}
}
