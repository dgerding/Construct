namespace Telerik.Expressions
{
	internal class ThrowingRuntimeErrorListener : RuntimeErrorListener
	{
		public override void ReportError(ExpressionNode node, string message)
		{
			throw new RuntimeExecutionException(message, node);
		}
	}
}