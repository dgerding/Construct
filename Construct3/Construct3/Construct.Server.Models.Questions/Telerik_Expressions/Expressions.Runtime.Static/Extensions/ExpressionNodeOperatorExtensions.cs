using System;
using System.Linq.Expressions;

namespace Telerik.Expressions.Runtime
{
	internal static class ExpressionNodeOperatorExtensions
	{
		public static ExpressionType ToExpressionType(this ExpressionNodeOperator @operator)
		{
			return (ExpressionType)Enum.Parse(typeof(ExpressionType), @operator.ToString(), true);
		}
	}
}
