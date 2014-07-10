namespace Telerik.Expressions
{
	internal class UnaryExpressionNode : ExpressionNode
	{
		private readonly ExpressionNode operandField;
		private readonly ExpressionNodeOperator @operator;

		public ExpressionNode Operand
		{
			get
			{
				return this.operandField;
			}
		}

		public ExpressionNodeOperator Operator
		{
			get
			{
				return this.@operator;
			}
		}

		public override ExpressionNodeType NodeType
		{
			get
			{
				return ExpressionNodeType.Unary;
			}
		}

		internal UnaryExpressionNode(ExpressionNode operand, ExpressionNodeOperator @operator)
		{
			this.operandField = operand;
			this.@operator = @operator;
		}

		public virtual UnaryExpressionNode Update(ExpressionNode operand)
		{
			if (operand == this.Operand)
			{
				return this;
			}

			return new UnaryExpressionNode(operand, this.Operator);
		}

		protected internal override ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitUnary(this);
		}

		protected internal override bool EqualsOverride(ExpressionNode node)
		{
			var other = (UnaryExpressionNode)node;
			return this.@operator == other.@operator
					&& this.operandField == other.operandField;
		}

		protected internal override int GetHashCodeOverride(int hash)
		{
			hash = (23 * hash) + this.operandField.GetHashCode();
			hash = (23 * hash) + this.@operator.GetHashCode();
			return hash;
		}
	}
}
