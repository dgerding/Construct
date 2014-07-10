namespace Telerik.Expressions
{
	internal class ConstantExpressionNode : ExpressionNode
	{
		private readonly object value;

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public override ExpressionNodeType NodeType
		{
			get
			{
				return ExpressionNodeType.Constant;
			}
		}

		internal ConstantExpressionNode(object value)
		{
			this.value = value;
		}

		protected internal override ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitConstant(this);
		}

		protected internal override bool EqualsOverride(ExpressionNode node)
		{
			var other = (ConstantExpressionNode)node;
			return object.Equals(this.value, other.value);
		}

		protected internal override int GetHashCodeOverride(int hash)
		{
			var valueHash = this.value == null ? 0 : this.value.GetHashCode();
			return (23 * hash) + valueHash;
		}
	}
}