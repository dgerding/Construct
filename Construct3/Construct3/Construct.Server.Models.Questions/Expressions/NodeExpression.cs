using System;
using System.Linq.Expressions;
using Telerik.Expressions;

namespace Telerik.Windows.Data
{
	internal class NodeExpression : Expression
	{
		private readonly ExpressionNode node;

		public ExpressionNode Node
		{
			get
			{
				return this.node;
			}
		}

#if WPF35
		internal NodeExpression(ExpressionNode node)
			: base(default(ExpressionType), typeof(ExpressionNode))
		{
			this.node = node;
		}
#endif

//#if SILVERLIGHT || WPF40
		internal NodeExpression(ExpressionNode node)
		{
			this.node = node;
		}

		public override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Extension;
			}
		}

		public override Type Type
		{
			get
			{
				return typeof(ExpressionNode);
			}
		}
//#endif

		public override string ToString()
		{
			return this.node.ToString();
		}

		internal LambdaExpression ToLambda(Type parameterType)
		{
			LambdaExpression lambda;
			if (TryEvaluateExpressionNodeToLambda(this.Node, parameterType, out lambda))
			{
				return lambda;
			}

			return null;
		}

		private static bool TryEvaluateExpressionNodeToLambda(ExpressionNode node, Type elementType, out LambdaExpression lambda)
		{
			var runtime = ClrExpressionRuntime.CreateDefault();
			var scope = runtime.CreateScope();
			scope.AddVariable(new ParameterDefinition(string.Empty, elementType));
			var errorListener = new CountingRuntimeErrorListener();

			var executionContext = new ExpressionExecutionContext(node, scope, errorListener);

			lambda = runtime.EvaluateToLambdaExpression(executionContext);

			return !errorListener.HasErrors;
		}
	}
}
