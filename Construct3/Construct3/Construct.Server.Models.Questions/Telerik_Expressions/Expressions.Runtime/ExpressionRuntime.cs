namespace Telerik.Expressions
{
	internal abstract class ExpressionRuntime
	{
		private ExpressionExecutionScope globalScope;

		public ExpressionExecutionScope GlobalScope
		{
			get
			{
				if (this.globalScope == null)
				{
					this.globalScope = this.CreateGlobalScope();
				}

				return this.globalScope;
			}
		}

		protected internal virtual ExpressionExecutionScope CreateScope(ExpressionExecutionScope parent)
		{
			return new ExpressionExecutionScope(parent);
		}

		protected internal ExpressionExecutionScope CreateScope()
		{
			return this.CreateScope(this.GlobalScope);
		}

		public abstract object Evaluate(ExpressionNode node, ExpressionExecutionScope scope);

		public virtual object Evaluate(ExpressionNode node)
		{
			var scope = this.CreateScope();
			
			return this.Evaluate(node, scope);
		}

		private ExpressionExecutionScope CreateGlobalScope()
		{
			return this.CreateScope(null);
		}
	}
}