using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal partial class ExpressionNode
	{
		public static ConstantExpressionNode Constant(object value)
		{
			return new ConstantExpressionNode(value);
		}

		public static UnaryExpressionNode Unary(ExpressionNode operand, ExpressionNodeOperator @operator)
		{
			return new UnaryExpressionNode(operand, @operator);
		}

		public static UnaryExpressionNode Negate(ExpressionNode operand)
		{
			return Unary(operand, ExpressionNodeOperator.Negate);
		}

		public static UnaryExpressionNode Not(ExpressionNode operand)
		{
			return Unary(operand, ExpressionNodeOperator.Not);
		}

		public static BinaryExpressionNode Binary(ExpressionNode left, ExpressionNode right, ExpressionNodeOperator @operator)
		{
			return new BinaryExpressionNode(left, right, @operator);
		}

		public static BinaryExpressionNode Add(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Add);
		}

		public static BinaryExpressionNode And(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.And);
		}

		public static BinaryExpressionNode Equal(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Equal);
		}

		public static BinaryExpressionNode GreaterThan(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.GreaterThan);
		}

		public static BinaryExpressionNode GreaterThanOrEqual(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.GreaterThanOrEqual);
		}

		public static BinaryExpressionNode LessThan(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.LessThan);
		}

		public static BinaryExpressionNode LessThanOrEqual(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.LessThanOrEqual);
		}

		public static BinaryExpressionNode NotEqual(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.NotEqual);
		}

		public static BinaryExpressionNode Or(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Or);
		}

		public static BinaryExpressionNode Subtract(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Subtract);
		}

		public static BinaryExpressionNode Modulo(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Modulo);
		}

		public static BinaryExpressionNode Multiply(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Multiply);
		}

		public static BinaryExpressionNode Divide(ExpressionNode left, ExpressionNode right)
		{
			return Binary(left, right, ExpressionNodeOperator.Divide);
		}

		public static MemberExpressionNode Member(string name)
		{
			return new MemberExpressionNode(name);
		}

		public static MemberExpressionNode Member(string name, ExpressionNode instance)
		{
			return new MemberExpressionNode(name, instance);
		}

		public static IndexExpressionNode Index(IEnumerable<ExpressionNode> arguments)
		{
			return new IndexExpressionNode(arguments);
		}

		public static IndexExpressionNode Index(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			return new IndexExpressionNode(instance, arguments);
		}

		public static FunctionExpressionNode Function(string name, IEnumerable<ExpressionNode> arguments)
		{
			return new FunctionExpressionNode(name, arguments);
		}

		public static FunctionExpressionNode Function(string name, params ExpressionNode[] arguments)
		{
			return new FunctionExpressionNode(name, arguments);
		}

		public static FunctionExpressionNode Function(ExpressionNode instance, string name, IEnumerable<ExpressionNode> arguments)
		{
			return new FunctionExpressionNode(instance, name, arguments);
		}

		public static FunctionExpressionNode Function(ExpressionNode instance, string name, params ExpressionNode[] arguments)
		{
			return new FunctionExpressionNode(instance, name, arguments);
		}

		public static ConstantExpressionNode True()
		{
			return CreateKeywordConstant(TokenType.True);	
		}

		public static ConstantExpressionNode False()
		{
			return CreateKeywordConstant(TokenType.False);	
		}

		public static ConstantExpressionNode Null()
		{
			return CreateKeywordConstant(TokenType.Null);	
		}

		private static ConstantExpressionNode CreateKeywordConstant(TokenType type)
		{
			return Constant(type.GetKeywordConstantValue());
		}
	}
}
