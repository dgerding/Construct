using System.Collections.Generic;
using System.Linq.Expressions;
using Telerik.Expressions.Runtime;

namespace Telerik.Expressions
{
	internal partial class ClrExpressionRuntime : ExpressionRuntime
	{
		public static ClrExpressionRuntime CreateDefault() 
		{
			var runtime = new ClrExpressionRuntime();

			runtime.InitializeDefaultFunctions();

			return runtime;
		}

		public override object Evaluate(ExpressionNode node, ExpressionExecutionScope scope)
		{
			var lambda = this.EvaluateToLambdaExpression(node, scope);

			// TODO: refactor here to remove the slow dynamic invoke
			var result = lambda.Compile().DynamicInvoke();

			return result;
		}

		public LambdaExpression EvaluateToLambdaExpression(
			ExpressionNode node, ExpressionExecutionScope scope, IEnumerable<ParameterDefinition> parameters)
		{
			var childScope = scope.CreateChildScope();
			foreach (var parameter in parameters)
			{
				childScope.AddVariable(parameter);
			}

			return this.EvaluateToLambdaExpressionCore(node, childScope);
		}

		public LambdaExpression EvaluateToLambdaExpression(
			ExpressionNode node, ExpressionExecutionScope scope, params ParameterDefinition[] parameters)
		{
			return this.EvaluateToLambdaExpression(node, scope, (IEnumerable<ParameterDefinition>)parameters);
		}

		protected virtual LambdaExpression EvaluateToLambdaExpressionCore(ExpressionNode node, ExpressionExecutionScope scope)
		{
			var context = new ExpressionExecutionContext(node, scope);

			return this.EvaluateToLambdaExpression(context);
		}

		internal LambdaExpression EvaluateToLambdaExpression(ExpressionExecutionContext context)
		{
			var translator = this.CreateTranslator(context);

			return translator.CreateLambdaExpression();
		}

		internal virtual LinqExpressionTranslator CreateTranslator(ExpressionExecutionContext context)
		{
			return new LinqExpressionTranslator(this, context);
		}
	}
}
