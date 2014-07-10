namespace Telerik.Expressions
{
	internal abstract partial class ExpressionNodeTranslator<T>
	{
		public virtual T Translate(ExpressionNode node)
		{
			var visitor = new Visitor(this);

			visitor.Visit(node);

			return visitor.Result;
		}

		protected internal virtual T TranslateBinary(BinaryExpressionNode node)
		{
			return default(T);
		}

		protected internal virtual T TranslateConstant(ConstantExpressionNode node)
		{
			return default(T);
		}

		protected internal virtual T TranslateFunction(FunctionExpressionNode node)
		{
			return default(T);
		}

		protected internal virtual T TranslateIndex(IndexExpressionNode node)
		{
			return default(T);
		}

		protected internal virtual T TranslateMember(MemberExpressionNode node)
		{
			return default(T);
		}

		protected internal virtual T TranslateUnary(UnaryExpressionNode node)
		{
			return default(T);
		}
	}
}
