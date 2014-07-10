namespace Telerik.Expressions
{
	using System;

	internal abstract partial class ExpressionNode : IEquatable<ExpressionNode>
	{
		public abstract ExpressionNodeType NodeType { get; }

		protected internal virtual ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitExtension(this);
		}

		protected internal virtual ExpressionNode VisitChildren(ExpressionNodeVisitor visitor)
		{
			return this;
		}

		public override string ToString()
		{
			return ExpressionNodeStringBuilder.ExpressionNodeToString(this);
		}

		public override int GetHashCode()
		{
			return this.GetHashCodeOverride(17 + (23 * this.NodeType.GetHashCode()));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExpressionNode);
		}

		public bool Equals(ExpressionNode other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}

			return other != null
					&& this.NodeType == other.NodeType
					&& this.EqualsOverride(other);
		}

		protected internal abstract bool EqualsOverride(ExpressionNode node);
		protected internal abstract int GetHashCodeOverride(int hash);

		public static bool operator ==(ExpressionNode left, ExpressionNode right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(ExpressionNode left, ExpressionNode right)
		{
			return !(left == right);
		}
	}
}