using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Telerik.Expressions
{
	internal class FunctionExpressionNode : ExpressionNode, IMemberInvocationExpressionNode
	{
		private readonly ExpressionNode instanceField;
		private readonly string name;
		private readonly ReadOnlyCollection<ExpressionNode> argumentsField;

		public ReadOnlyCollection<ExpressionNode> Arguments
		{
			get
			{
				return this.argumentsField;
			}
		}

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

		public override ExpressionNodeType NodeType
		{
			get
			{
				return ExpressionNodeType.Function;
			}
		}

		internal FunctionExpressionNode(ExpressionNode instance, string name, IEnumerable<ExpressionNode> arguments)
		{
			this.instanceField = instance;
			this.name = name;
			this.argumentsField = new ReadOnlyCollection<ExpressionNode>(arguments.ToArray());
		}

		internal FunctionExpressionNode(ExpressionNode instance, string name, params ExpressionNode[] arguments)
			: this(instance, name, (IEnumerable<ExpressionNode>)arguments)
		{
		}

		internal FunctionExpressionNode(string name, IEnumerable<ExpressionNode> arguments)
			: this(null, name, arguments)
		{
		}

		internal FunctionExpressionNode(string name, params ExpressionNode[] arguments)
			: this(name, (IEnumerable<ExpressionNode>)arguments)
		{
		}

		public virtual FunctionExpressionNode Update(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			var args = arguments.ToArray();

			if (instance == this.Instance && this.AreArgumentsSame(args))
			{
				return this;
			}

			return new FunctionExpressionNode(instance, this.Name, arguments);
		}

		ExpressionNode IMemberInvocationExpressionNode.Update(ExpressionNode instance, IEnumerable<ExpressionNode> arguments)
		{
			return this.Update(instance, arguments);
		}

		protected internal override ExpressionNode Accept(ExpressionNodeVisitor visitor)
		{
			return visitor.VisitFunction(this);
		}

		private bool AreArgumentsSame(IList<ExpressionNode> args)
		{
			return ExpressionHelper.AreEqual(this.Arguments, args);
		}

		protected internal override bool EqualsOverride(ExpressionNode node)
		{
			var other = (FunctionExpressionNode)node;
			return ExpressionHelper.AreMemberInvocationsEqual(this, other);
		}

		protected internal override int GetHashCodeOverride(int hash)
		{
			return ExpressionHelper.GetMemberInvocationHashCode(this, hash);
		}
	}
}