using System.Diagnostics;

namespace Telerik.Expressions
{
	internal class StringLexer : EscapingLiteralLexer
	{
		private const string StringDelimiters = "\'\"";

		private StringLexer(ParserContext context)
			: base(context, context.SourceReader.PeekChar())
		{
			this.AddEscapingChar(this.StartChar);
			this.AddCharToEscape(this.StartChar);
		}

		public static bool IsCharStringStart(char ch)
		{
			return StringDelimiters.IndexOf(ch) >= 0;
		}

		public static StringToken ReadString(ParserContext context)
		{
			var lexer = new StringLexer(context);
			return (StringToken)lexer.ReadToken();
		}

		public override Token ReadToken()
		{
            Debug.Assert(IsCharStringStart(this.PeekChar()), "IsCharStringStart(this.PeekChar())");

			return base.ReadToken();
		}

		protected internal override bool TryCreateTokenFromLiteral(string literal, out Token token)
		{
			token = new StringToken(literal);
			return true;
		}
	}
}
