using System.Collections.Generic;
using System.Diagnostics;

namespace Telerik.Expressions
{
	internal partial class ExpressionParser
	{
		// TODO: handle unary nodes as well
		private static BinaryExpressionNode RewriteBinaryOperatorPrecedence(BinaryExpressionNode binary)
		{
			if (binary.Right.NodeType == ExpressionNodeType.Binary)
			{
				var rightBinary = (BinaryExpressionNode)binary.Right;

				if (OperatorPrecedenceComparer.Instance.Compare(binary.Operator, rightBinary.Operator) <= 0)
				{
					var newLeft = binary.Update(binary.Left, rightBinary.Left);

					// Make sure the new left binary operand is following the precendence rules as well.
					// This handles expressions like: 20 / 2 / 2 / 2
					newLeft = RewriteBinaryOperatorPrecedence(newLeft);

					return rightBinary.Update(newLeft, rightBinary.Right);
				}
			}

			return binary;
		}

		private class OperatorPrecedenceComparer : Comparer<ExpressionNodeOperator>
		{
			public static readonly OperatorPrecedenceComparer Instance = new OperatorPrecedenceComparer();

			public override int Compare(ExpressionNodeOperator x, ExpressionNodeOperator y)
			{
				var leftRank = GetOperatorPrecedenceRank(x);
				var rightRank = GetOperatorPrecedenceRank(y);

				return leftRank.CompareTo(rightRank);
			}

			private static int GetOperatorPrecedenceRank(ExpressionNodeOperator @operator)
			{
				switch (@operator.Category())
				{
					case ExpressionNodeOperatorCategory.Unary:
						return 1;
					case ExpressionNodeOperatorCategory.Multiplicative:
						return 2;
					case ExpressionNodeOperatorCategory.Additive:
						return 3;
					case ExpressionNodeOperatorCategory.Relational:
						return 4;
					case ExpressionNodeOperatorCategory.Equality:
						return 5;
					case ExpressionNodeOperatorCategory.Logical:
						return @operator == ExpressionNodeOperator.And ? 6 : 7; // AND binds stronger than OR
						
					default:
						Debug.Assert(false, "false");
						return default(int);
				}
			}
		}
	}
}