namespace Telerik.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Telerik.Windows.Data;

    internal static class ExpressionHelper
    {
        private static readonly bool IsIdentifierCaseSensitive = false;

        private static readonly ILookup<TokenType, ExpressionNodeOperator> OperatorToTokenLookup =
            new Dictionary<ExpressionNodeOperator, TokenType>
			{
				{ ExpressionNodeOperator.Add, TokenType.Plus },
				{ ExpressionNodeOperator.And, TokenType.And },
				{ ExpressionNodeOperator.Divide, TokenType.Divide },
				{ ExpressionNodeOperator.Equal, TokenType.Equal },
				{ ExpressionNodeOperator.GreaterThan, TokenType.GreaterThan },
				{ ExpressionNodeOperator.GreaterThanOrEqual, TokenType.GreaterThanOrEqual },
				{ ExpressionNodeOperator.LessThan, TokenType.LessThan },
				{ ExpressionNodeOperator.LessThanOrEqual, TokenType.LessThanOrEqual },
				{ ExpressionNodeOperator.Modulo, TokenType.Percent },
				{ ExpressionNodeOperator.Multiply, TokenType.Multiply },
				{ ExpressionNodeOperator.Not, TokenType.Not },
				{ ExpressionNodeOperator.Negate, TokenType.Minus },
				{ ExpressionNodeOperator.NotEqual, TokenType.NotEqual },
				{ ExpressionNodeOperator.Or, TokenType.Or },
				{ ExpressionNodeOperator.Subtract, TokenType.Minus },
			}
            .ToLookup(pair => pair.Value, pair => pair.Key);

        private static readonly IDictionary<TokenType, object> KeywordConstantsMap =
            new Dictionary<TokenType, object>
			{
				{ TokenType.True, true },
				{ TokenType.False, false },
				{ TokenType.Null, null },
			};

        internal static IEnumerable<ExpressionNodeOperator> GetExpressionNodeOperators(this TokenType tokenType)
        {
            Debug.Assert(OperatorToTokenLookup.Contains(tokenType), "OperatorToTokenLookup.Contains(tokenType)");

            return OperatorToTokenLookup[tokenType];
        }

        internal static string ToTokenString(this ExpressionNodeOperator @operator)
        {
            return OperatorToTokenLookup.Single(g => g.Contains(@operator)).Key.ToTokenString();
        }

        internal static bool IsKeywordOperator(this KeywordToken token)
        {
            return OperatorToTokenLookup.Contains(token.TokenType);
        }

        internal static bool IsKeywordBinaryOperator(this KeywordToken token)
        {
            if (token.IsKeywordOperator())
            {
                return OperatorToTokenLookup[token.TokenType].All(op => op.IsBinaryOperator());
            }
            return false;
        }

        internal static ExpressionNodeOperator ToExpressionNodeOperator(this KeywordToken token)
        {
            Debug.Assert(token.IsKeywordOperator(), "token.IsKeywordOperator()");
            Debug.Assert(OperatorToTokenLookup[token.TokenType].Count() == 1, "OperatorToTokenLookup[token.TokenType].Count() == 1");

            return OperatorToTokenLookup[token.TokenType].First();
        }

        internal static bool IsKeywordConstant(this TokenType tokenType)
        {
            return KeywordConstantsMap.ContainsKey(tokenType);
        }

        internal static object GetKeywordConstantValue(this TokenType tokenType)
        {
            Debug.Assert(tokenType.IsKeywordConstant(), "tokenType.IsKeywordConstant()");

            return KeywordConstantsMap[tokenType];
        }

        internal static bool AreEqual(IList<ExpressionNode> left, IList<ExpressionNode> right)
        {
            return
                left.Count == right.Count
                && left
                    .Zip(right, (l, r) => object.Equals(l, r))
                    .All(areEqual => areEqual);
        }

        internal static bool AreMemberInvocationsEqual(IMemberInvocationExpressionNode left, IMemberInvocationExpressionNode right)
        {
            return
                IdentifierEquals(left.Name, right.Name)
                && object.Equals(left.Instance, right.Instance)
                && AreEqual(left.Arguments, right.Arguments);
        }

        private static bool IdentifierEquals(string name1, string name2)
        {
            return IdentifierComparer.Equals(name1, name2);
        }

        internal static int GetMemberInvocationHashCode(IMemberInvocationExpressionNode node, int hash)
        {
            hash = (23 * hash) + ExpressionHelper.IdentifierGetHashCode(node.Name);
            hash = (23 * hash) + (null == node.Instance ? 0 : node.Instance.GetHashCode());
            hash = (23 * hash) + node.Arguments.Count;
            foreach (var arg in node.Arguments)
            {
                hash = (23 * hash) + arg.GetHashCode();
            }

            return hash;
        }

        internal static StringComparer IdentifierComparer
        {
            get
            {
                if (IsIdentifierCaseSensitive)
                {
                    return StringComparer.InvariantCulture;
                }

                return StringComparer.InvariantCultureIgnoreCase;
            }
        }

        private static int IdentifierGetHashCode(string name)
        {
            if (!IsIdentifierCaseSensitive)
            {
                return name.ToUpper(CultureInfo.CurrentCulture).GetHashCode();
            }
            return name.GetHashCode();
        }
    }
}
