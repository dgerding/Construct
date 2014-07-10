namespace Telerik.Expressions
{
	internal class BinaryExpressionNode : ExpressionNode
	{
		private readonly ExpressionNode leftField;
		private readonly ExpressionNode rightField;
		private readonly ExpressionNodeOperator @operator;

		public ExpressionNode Left
		{
			get
			{
				return this.leftField;
			}
		}

		public ExpressionNode Right
		{
			get
			{
				return this.rightField;
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
				return ExpressionNodeType.Binary;
			}
		}

		internal BinaryExpressionNode(ExpressionNode left, ExpressionNode right, ExpressionNodeOperator @operator)
		{
			this.leftField = left;
			this.rightField = right;
			this.@operator = @operator;
		}

		public virtual BinaryExpressionNode Update(ExpressionNode left, ExpressionNode right)
		{
			if (left == this.Left && right == this.Right)
			{
				return this;
			}

			return new BinaryExpressionNode(left, right, this.Operator);
		}

		protected internal override ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitBinary(this);
		}

		protected internal override bool EqualsOverride(ExpressionNode node)
		{
			var other = (BinaryExpressionNode)node;
			return this.@operator == other.@operator
					&& this.leftField == other.leftField
					&& this.rightField == other.rightField;
		}

		protected internal override int GetHashCodeOverride(int hash)
		{
			hash = (23 * hash) + this.leftField.GetHashCode();
			hash = (23 * hash) + this.rightField.GetHashCode();
			hash = (23 * hash) + this.@operator.GetHashCode();
			return hash;
		}
	}
}
