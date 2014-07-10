using System;

namespace Telerik.Expressions
{
#if WPF
	[Serializable]
#endif
    internal class RuntimeExecutionException : Exception
	{
		private readonly ExpressionNode expression;

		public ExpressionNode Expression
		{
			get
			{
				return this.expression;
			}
		}

		public RuntimeExecutionException()
		{
		}

		public RuntimeExecutionException(string message) 
			: base(message)
		{ 
		}

		public RuntimeExecutionException(string message, ExpressionNode expression) 
			: this(message)
		{
			this.expression = expression;
		}
	}
}