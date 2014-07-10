using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Telerik.Expressions
{
	internal class ExpressionNodeStringBuilder : ExpressionNodeVisitor
	{
		private readonly StringBuilder output;

		internal ExpressionNodeStringBuilder()
		{
			this.output = new StringBuilder();
		}

		internal static string ExpressionNodeToString(ExpressionNode node)
		{
            Debug.Assert(node != null, "node != null");

			var builder = new ExpressionNodeStringBuilder();
			builder.Visit(node);

			return builder.ToString();
		}

		public override string ToString()
		{
			return this.output.ToString();
		}

		protected internal override ExpressionNode VisitConstant(ConstantExpressionNode node)
		{
			if (node.Value != null)
			{
				var valueToString = node.Value.ToString();
				if (node.Value is string)
				{
					this.AppendFormat("{0}{1}{0}", '"', valueToString);
				}
				else if (valueToString == node.Value.GetType().ToString())
				{
					this.AppendFormat("value({0})", valueToString);
				}
				else
				{
					this.Append(valueToString);
				}
			}
			else
			{
				this.Append(TokenType.Null.ToTokenString().ToLower(CultureInfo.InvariantCulture));
			}

			return node;
		}

		protected internal override ExpressionNode VisitUnary(UnaryExpressionNode node)
		{
			this.Append(node.Operator.ToTokenString());

			this.BeginBlock();
			this.Visit(node.Operand);
			this.EndBlock();

			return node;
		}

		protected internal override ExpressionNode VisitBinary(BinaryExpressionNode node)
		{
			this.BeginBlock();

			this.Visit(node.Left);
			this.AppendFormat(" {0} ", node.Operator.ToTokenString());
			this.Visit(node.Right);

			this.EndBlock();

			return node;
		}

		protected internal override ExpressionNode VisitMember(MemberExpressionNode node)
		{
			if (node.Instance != null)
			{
				this.Visit(node.Instance);

				this.AppendMemberAccess();
			}

			this.Append(node.Name);

			return node;
		}

		protected internal override ExpressionNode VisitIndex(IndexExpressionNode node)
		{
			if (node.Instance != null)
			{
				this.Visit(node.Instance);
			}

			this.BeginIndexBlock();
			this.VisitArguments(node.Arguments);
			this.EndIndexBlock();

			return node;
		}

		protected internal override ExpressionNode VisitFunction(FunctionExpressionNode node)
		{
			if (node.Instance != null)
			{
				this.Visit(node.Instance);

				this.AppendMemberAccess();
			}

			this.Append(node.Name);

			this.BeginBlock();
			this.VisitArguments(node.Arguments);
			this.EndBlock();

			return node;
		}

		private void VisitArguments(IEnumerable<ExpressionNode> args)
		{
			foreach (var arg in args)
			{
				this.Visit(arg);

				if (arg != args.Last())
				{
					this.Append(TokenType.Comma.ToTokenString() + " ");
				}
			}
		}

		private void BeginBlock()
		{
			this.Append(TokenType.Paren.ToTokenString());
		}

		private void EndBlock()
		{
			this.Append(TokenType.CloseParen.ToTokenString());
		}

		private void BeginIndexBlock()
		{
			this.Append(TokenType.Bracket.ToTokenString());
		}

		private void EndIndexBlock()
		{
			this.Append(TokenType.CloseBracket.ToTokenString());
		}

		private void AppendMemberAccess()
		{
			this.output.Append(TokenType.Dot.ToTokenString());
		}

		private void Append(string s)
		{
			this.output.Append(s);
		}

		private void AppendFormat(string format, params object[] args)
		{
			this.output.AppendFormat(CultureInfo.InvariantCulture, format, args);
		}
	}
}