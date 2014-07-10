using System.Diagnostics;

namespace Telerik.Expressions
{
	internal static class NameLexer
	{
		public static bool IsCharNameStart(char ch)
		{
			return
				OperatorToken.IsOperatorStart(ch.ToString()) ||
				IdentifierLexer.IsCharIdentifierStart(ch);
		}

		public static NamedToken ReadName(ParserContext context)
		{
			var ch = context.SourceReader.PeekChar();

			if (OperatorToken.IsOperatorStart(ch.ToString()))
			{
				return OperatorLexer.ReadOperator(context);
			}

            Debug.Assert(IdentifierLexer.IsCharIdentifierStart(ch), "IdentifierLexer.IsCharIdentifierStart(ch)");

			return ReadIdentifierOrKeyword(context);
		}

		private static NamedToken ReadIdentifierOrKeyword(ParserContext context)
		{
			var token = IdentifierLexer.ReadIdentifier(context);

			if (KeywordToken.IsKeywordName(token.Name))
			{
				var keyword = KeywordToken.FromName(token.Name);
				keyword.SetSpan(token.Span());

				return keyword;
			}

			return token;
		}
	}
}
