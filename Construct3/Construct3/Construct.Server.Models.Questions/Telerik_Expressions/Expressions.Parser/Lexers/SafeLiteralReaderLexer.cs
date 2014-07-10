namespace Telerik.Expressions
{
	internal abstract class SafeLiteralReaderLexer : LiteralReaderLexer
	{
		protected SafeLiteralReaderLexer(ParserContext context)
			: base(context)
		{
		}

		protected internal override bool TryCreateTokenFromLiteral(string literal, out Token token)
		{
			token = this.CreateTokenFromLiteral(literal);
			
			return true;
		}

		protected abstract Token CreateTokenFromLiteral(string literal);
	}
}