using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Telerik.Expressions
{
	internal class IndexExpressionNode : ExpressionNode, IMemberInvocationExpressionNode
	{
		private readonly ReadOnlyCollection<ExpressionNode> argumentsField;
		private readonly ExpressionNode instanceField;

		public ReadOnlyCollection<ExpressionNode> Arguments
		{
			get
			{
				return this.argumentsField;
			}
		}

		public ExpressionNode Instance
		{
			get
			{
				return this.instanceField;
			}
		}

		public override ExpressionNodeType NodeType
		{
			get
			{
				return ExpressionNodeType.Index;
			}
		}

		string IMemberInvocationExpressionNode.Name
		{
			get
			{
				return TokenType.Bracket.ToTokenString() + TokenType.CloseBracket.ToTokenString();
			}
		}

		internal IndexExpressionNode(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			this.instanceField = instance;
			this.argumentsField = new ReadOnlyCollection<ExpressionNode>(arguments.ToArray());
		}

		internal IndexExpressionNode(IEnumerable<ExpressionNode> arguments)
			: this(null, arguments)
		{
		}

		public virtual IndexExpressionNode Update(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			var args = arguments.ToArray();

			if (instance == this.Instance && ExpressionHelper.AreEqual(this.Arguments, args))
			{
				return this;
			}

			return new IndexExpressionNode(instance, args);
		}

		ExpressionNode IMemberInvocationExpressionNode.Update(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			return this.Update(instance, arguments);
		}

		protected internal override ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitIndex(this);
		}

		protected internal override bool EqualsOverride(ExpressionNode node)
		{
			var other = (IndexExpressionNode)node;
			return ExpressionHelper.AreMemberInvocationsEqual(this, other);
		}

		protected internal override int GetHashCodeOverride(int hash)
		{
			return ExpressionHelper.GetMemberInvocationHashCode(this, hash);
		}
	}
}