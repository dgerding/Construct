using System.Collections.Generic;
using System.Linq;

namespace Telerik.Expressions
{
	internal partial class KeywordToken : ReservedToken
	{
		private static IDictionary<string, TokenType> keywordsLookup;

		static KeywordToken()
		{
			var keywordTokenTypes = new[] 
            {
				TokenType.Null,
				TokenType.True,
				TokenType.False,
				TokenType.And,
				TokenType.Or,
				TokenType.Not
			};

			keywordsLookup = keywordTokenTypes.ToDictionary(t => t.ToTokenString(), ExpressionHelper.IdentifierComparer);
		}

		public static KeywordToken FromName(string name)
		{
			return new KeywordToken(name, keywordsLookup[name]);
		}

		public static bool IsKeywordName(string name)
		{
			return keywordsLookup.ContainsKey(name);
		}

		private KeywordToken(string name, TokenType tokenType)
			: base(name, tokenType)
		{
		}
	}
}