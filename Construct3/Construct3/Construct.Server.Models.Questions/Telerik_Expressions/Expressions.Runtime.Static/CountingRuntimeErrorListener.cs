namespace Telerik.Expressions
{
	internal class CountingRuntimeErrorListener : RuntimeErrorListener
	{
		public int ErrorsCount { get; private set; }

		public bool HasErrors
		{
			get
			{
				return this.ErrorsCount > 0;
			}
		}

		public override void ReportError(ExpressionNode node, string message)
		{
			this.ErrorsCount++;
		}
	}
}