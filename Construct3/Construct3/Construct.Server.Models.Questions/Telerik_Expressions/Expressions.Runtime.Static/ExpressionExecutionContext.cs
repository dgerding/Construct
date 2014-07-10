namespace Telerik.Expressions
{
	internal class ExpressionExecutionContext
	{
		private readonly ExpressionNode expression;
		private readonly RuntimeErrorListener errorListener;
		private readonly ExpressionExecutionScope scope;

		public ExpressionNode Expression
		{
			get
			{
				return this.expression;
			}
		}

		public ExpressionExecutionScope Scope
		{
			get
			{
				return this.scope;
			}
		}

		public RuntimeErrorListener ErrorListener
		{
			get
			{
				return this.errorListener;
			}
		}

		public ExpressionExecutionContext(ExpressionNode expression, ExpressionExecutionScope scope)
			: this(expression, scope, new ThrowingRuntimeErrorListener())
		{
		}

		public ExpressionExecutionContext(ExpressionNode expression, ExpressionExecutionScope scope, RuntimeErrorListener errorListener)
		{
			this.expression = expression;
			this.scope = scope;
			this.errorListener = errorListener;
		}
	}
}