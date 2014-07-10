namespace Telerik.Expressions
{
	internal abstract class RuntimeErrorListener
	{
		// TODO: Add Severity: Error, Warning
		public abstract void ReportError(ExpressionNode node, string message);
	}
}