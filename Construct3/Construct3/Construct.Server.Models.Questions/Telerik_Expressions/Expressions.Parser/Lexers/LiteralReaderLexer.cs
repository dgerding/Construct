namespace Telerik.Expressions
{
	internal abstract class LiteralReaderLexer : LiteralLexerBase
	{
		protected LiteralReaderLexer(ParserContext context)
			: base(context)
		{
		}

		protected abstract bool Read();

		protected override void ReadToEnd() 
		{ 
			while (this.Read())
			{
			}
		}
	}
}
