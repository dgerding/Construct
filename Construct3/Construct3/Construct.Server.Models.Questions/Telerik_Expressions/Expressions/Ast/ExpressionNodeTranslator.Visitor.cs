namespace Telerik.Expressions
{
	internal partial class ExpressionNodeTranslator<T>
	{
		private class Visitor : ExpressionNodeVisitor
		{
			private readonly ExpressionNodeTranslator<T> translator;

			public T Result
			{
				get;
				private set;
			}

			public Visitor(ExpressionNodeTranslator<T> translator)
			{
				this.translator = translator;
				this.Result = default(T);
			}

			protected internal override ExpressionNode VisitBinary(BinaryExpressionNode node)
			{
				this.Result = this.translator.TranslateBinary(node);

				return node;
			}

			protected internal override ExpressionNode VisitConstant(ConstantExpressionNode node)
			{
				this.Result = this.translator.TranslateConstant(node);

				return node;
			}

			protected internal override ExpressionNode VisitFunction(FunctionExpressionNode node)
			{
				this.Result = this.translator.TranslateFunction(node);

				return node;
			}

			protected internal override ExpressionNode VisitIndex(IndexExpressionNode node)
			{
				this.Result = this.translator.TranslateIndex(node);

				return node;
			}

			protected internal override ExpressionNode VisitMember(MemberExpressionNode node)
			{
				this.Result = this.translator.TranslateMember(node);

				return node;
			}

			protected internal override ExpressionNode VisitUnary(UnaryExpressionNode node)
			{
				this.Result = this.translator.TranslateUnary(node);

				return node;
			}
		}
	}
}
