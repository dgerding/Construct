using System.Collections.Generic;
using System.Linq;

namespace Telerik.Expressions
{
	internal partial class SyntaxToken
	{
		private static readonly IDictionary<char, TokenType> CharTokenTypeMap;

		static SyntaxToken()
		{
			var syntaxTokenTypes = new[]
			{
				TokenType.Bracket,
				TokenType.CloseBracket,
				TokenType.Paren,
				TokenType.CloseParen,
				TokenType.Comma,
				TokenType.Eof,	
			};

			CharTokenTypeMap = syntaxTokenTypes.ToDictionary(t => t.ToTokenString()[0]);
		}

		public static SyntaxToken FromChar(char ch)
		{
			return new SyntaxToken(ch, CharTokenTypeMap[ch]);
		}

		public static bool IsSyntaxToken(char ch)
		{
			return CharTokenTypeMap.ContainsKey(ch);
		}
	}
}