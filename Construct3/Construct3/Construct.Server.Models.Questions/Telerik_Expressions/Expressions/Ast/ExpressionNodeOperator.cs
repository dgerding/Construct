using System;

namespace Telerik.Expressions
{
	internal enum ExpressionNodeOperator
	{
        /// <summary>
        /// Add.
        /// </summary>
		Add,

        /// <summary>
        /// And.
        /// </summary>
		And,

        /// <summary>
        /// Divide.
        /// </summary>
		Divide,

        /// <summary>
        /// Equal.
        /// </summary>
		Equal,

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
        /// Modulo.
        /// </summary>
		Modulo,

        /// <summary>
        /// Multiply.
        /// </summary>
		Multiply,

        /// <summary>
        /// Negate.
        /// </summary>
		Negate,

        /// <summary>
        /// Not.
        /// </summary>
		Not,

        /// <summary>
        /// NotEqual.
        /// </summary>
		NotEqual,

        /// <summary>
        /// Or.
        /// </summary>
		Or,

        /// <summary>
        /// Subtract.
        /// </summary>
		Subtract,
	}

	internal static class ExpressionNodeOperatorExtensions
	{
		public static ExpressionNodeOperatorCategory Category(this ExpressionNodeOperator @operator)
		{
			switch (@operator)
			{
				case ExpressionNodeOperator.Add:
				case ExpressionNodeOperator.Subtract:
					return ExpressionNodeOperatorCategory.Additive;
				
				case ExpressionNodeOperator.And:
				case ExpressionNodeOperator.Or:
					return ExpressionNodeOperatorCategory.Logical;
				
				case ExpressionNodeOperator.Divide:
				case ExpressionNodeOperator.Modulo:
				case ExpressionNodeOperator.Multiply:
					return ExpressionNodeOperatorCategory.Multiplicative;

				case ExpressionNodeOperator.Equal:
				case ExpressionNodeOperator.NotEqual:
					return ExpressionNodeOperatorCategory.Equality;
				
				case ExpressionNodeOperator.GreaterThan:
				case ExpressionNodeOperator.GreaterThanOrEqual:
				case ExpressionNodeOperator.LessThan:
				case ExpressionNodeOperator.LessThanOrEqual:
					return ExpressionNodeOperatorCategory.Relational;

				case ExpressionNodeOperator.Negate:
				case ExpressionNodeOperator.Not:
					return ExpressionNodeOperatorCategory.Unary;

				default:
					throw new ArgumentOutOfRangeException("operator");
			}
		}

		public static string ToExpressionString(this ExpressionNodeOperator @operator)
		{
			return ExpressionHelper.ToTokenString(@operator);
		}

		internal static bool IsUnaryOperator(this ExpressionNodeOperator node)
		{
			return node.Category() == ExpressionNodeOperatorCategory.Unary;
		}

		internal static bool IsBinaryOperator(this ExpressionNodeOperator node)
		{
			return !node.IsUnaryOperator();
		}
	}
}