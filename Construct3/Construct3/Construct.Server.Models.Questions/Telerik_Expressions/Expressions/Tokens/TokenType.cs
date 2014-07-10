using System;
using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal enum TokenType
	{
		// Syntax

        /// <summary>
        /// Paren.
        /// </summary>
		Paren,

        /// <summary>
        /// CloseParen.
        /// </summary>
		CloseParen,

        /// <summary>
        /// Bracket.
        /// </summary>
		Bracket,

        /// <summary>
        /// CloseBracket.
        /// </summary>
		CloseBracket,

        /// <summary>
        /// Comma.
        /// </summary>
		Comma,

        /// <summary>
        /// Eof.
        /// </summary>
		Eof,

		// Keywords

        /// <summary>
        /// Null.
        /// </summary>
		Null,

        /// <summary>
        /// True.
        /// </summary>
		True,

        /// <summary>
        /// False.
        /// </summary>
		False,

        /// <summary>
        /// And.
        /// </summary>
		And,

        /// <summary>
        /// Or.
        /// </summary>
		Or,

        /// <summary>
        /// Not.
        /// </summary>
		Not,

		// Operators

        /// <summary>
        /// Dot.
        /// </summary>
		Dot,

        /// <summary>
        /// Plus.
        /// </summary>
		Plus,

        /// <summary>
        /// Minus.
        /// </summary>
		Minus,

        /// <summary>
        /// Percent.
        /// </summary>
		Percent,

        /// <summary>
        /// Multiply.
        /// </summary>
		Multiply,

        /// <summary>
        /// Divide.
        /// </summary>
		Divide,

        /// <summary>
        /// Equal.
        /// </summary>
		Equal,

        /// <summary>
        /// NotEqual.
        /// </summary>
		NotEqual,

        /// <summary>
        /// GreaterThan.
        /// </summary>
		GreaterThan,

        /// <summary>
        /// GreaterThanOrEqual.
        /// </summary>
		GreaterThanOrEqual,

        /// <summary>
        /// LessThan.
        /// </summary>
		LessThan,

        /// <summary>
        /// LessThanOrEqual.
        /// </summary>
		LessThanOrEqual,

        /// <summary>
        /// Literal.
        /// </summary>
		Literal,

        /// <summary>
        /// Identifier.
        /// </summary>
		Identifier,
	}

	internal static class TokenTypeExtensions
	{
		private static readonly Dictionary<TokenType, string> TokenStrings = new Dictionary<TokenType, string> 
        {
			{ TokenType.Paren, "(" },
			{ TokenType.CloseParen, ")" },
			{ TokenType.Bracket, "[" },
			{ TokenType.CloseBracket, "]" },
			{ TokenType.Comma, "," },
			{ TokenType.Eof, new string(unchecked((char)(-1)), 1) },
			{ TokenType.Null, "Null" },
			{ TokenType.True, "True" },
			{ TokenType.False, "False" },
			{ TokenType.And, "And" },
			{ TokenType.Or, "Or" },
			{ TokenType.Not, "Not" },
			{ TokenType.Dot, "." },
			{ TokenType.Plus, "+" },
			{ TokenType.Minus, "-" },
			{ TokenType.Percent, "%" },
			{ TokenType.Multiply, "*" },
			{ TokenType.Divide, "/" },
			{ TokenType.Equal, "=" },
			{ TokenType.NotEqual, "<>" },
			{ TokenType.GreaterThan, ">" },
			{ TokenType.GreaterThanOrEqual, ">=" },
			{ TokenType.LessThan, "<" },
			{ TokenType.LessThanOrEqual, "<=" },
		};

		public static string ToTokenString(this TokenType tokenType)
		{
			string value;
			if (TokenStrings.TryGetValue(tokenType, out value))
			{
				return value;
			}

			throw new ArgumentOutOfRangeException("tokenType");
		}
	}
}
