using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.Expressions
{
	internal class MemberExpressionNode : ExpressionNode, IMemberInvocationExpressionNode
	{
		private readonly string name;
		private readonly ExpressionNode instanceField;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ExpressionNode Instance
		{
			get
			{
				return this.instanceField;
			}
		}

		ReadOnlyCollection<ExpressionNode> IMemberInvocationExpressionNode.Arguments
		{
			get
			{
				return new List<ExpressionNode>().AsReadOnly();
			}
		}

		public override ExpressionNodeType NodeType
		{
			get
			{
				return ExpressionNodeType.Member;
			}
		}

		internal MemberExpressionNode(string name, ExpressionNode instance)
		{
			this.name = name;
			this.instanceField = instance;
		}

		internal MemberExpressionNode(string name)
			: this(name, null)
		{
		}

		public virtual MemberExpressionNode Update(ExpressionNode instance)
		{
			if (this.Instance == instance)
			{
				return this;
			}

			return new MemberExpressionNode(this.Name, instance);
		}

		ExpressionNode IMemberInvocationExpressionNode.Update(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			return this.Update(instance);
		}

		protected internal override ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitMember(this);
		}

		protected internal override bool EqualsOverride(ExpressionNode node)
		{
			var other = (MemberExpressionNode)node;
			return ExpressionHelper.AreMemberInvocationsEqual(this, other);
		}

		protected internal override int GetHashCodeOverride(int hash)
		{
			return ExpressionHelper.GetMemberInvocationHashCode(this, hash);
		}
	}
}
