using System;
using System.Linq;
using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal partial class OperatorToken : ReservedToken
	{
		private static readonly IDictionary<string, TokenType> StringTokenTypeMap;

		static OperatorToken()
		{
			var operatorTokenTypes = new[]
			{
				TokenType.Dot,
				TokenType.Plus,
				TokenType.Minus,
				TokenType.Percent,
				TokenType.Multiply,
				TokenType.Divide,
				TokenType.Equal,
				TokenType.NotEqual,
				TokenType.GreaterThan,
				TokenType.GreaterThanOrEqual,
				TokenType.LessThan,
				TokenType.LessThanOrEqual,
			};

			StringTokenTypeMap = operatorTokenTypes.ToDictionary(t => t.ToTokenString());
		}

		public static OperatorToken FromName(string name)
		{
			return new OperatorToken(name, StringTokenTypeMap[name]);
		}

		public static bool IsOperatorName(string name)
		{
			return StringTokenTypeMap.ContainsKey(name);
		}

		internal static bool IsOperatorStart(string value)
		{
			return StringTokenTypeMap.Keys.Any(name => name.StartsWith(value, StringComparison.Ordinal));
		}

		internal OperatorToken(string name, TokenType tokenType)
			: base(name, tokenType)
		{
		}
	}
}