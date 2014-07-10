using System.Linq;

namespace Telerik.Expressions
{
	internal abstract class ExpressionNodeVisitor
	{
		public virtual ExpressionNode Visit(ExpressionNode node)
		{
			if (node != null)
			{
				return node.Accept(this);
			}
			return null;
		}

		protected internal virtual ExpressionNode VisitExtension(ExpressionNode node)
		{
			return node.VisitChildren(this);
		}

		protected internal virtual ExpressionNode VisitUnary(UnaryExpressionNode node)
		{
			var visitedOperand = this.Visit(node.Operand);

			return node.Update(visitedOperand);
		}

		protected internal virtual ExpressionNode VisitBinary(BinaryExpressionNode node)
		{
			var visitedLeft = this.Visit(node.Left);
			var visitedRight = this.Visit(node.Right);

			return node.Update(visitedLeft, visitedRight);
		}

		protected internal virtual ExpressionNode VisitConstant(ConstantExpressionNode node)
		{
			return node;
		}

		protected internal virtual ExpressionNode VisitMember(MemberExpressionNode node)
		{
			return this.VisitMemberInvocation(node);
		}

		protected internal virtual ExpressionNode VisitIndex(IndexExpressionNode node)
		{
			return this.VisitMemberInvocation(node);
		}

		protected internal virtual ExpressionNode VisitFunction(FunctionExpressionNode node)
		{
			return this.VisitMemberInvocation(node);
		}

		private ExpressionNode VisitMemberInvocation(IMemberInvocationExpressionNode node)												
		{
			var visitedInstance = this.Visit(node.Instance);
			var visitedArguments = node.Arguments.Select(this.Visit).ToArray();

			return node.Update(visitedInstance, visitedArguments);
		}
	}
}
