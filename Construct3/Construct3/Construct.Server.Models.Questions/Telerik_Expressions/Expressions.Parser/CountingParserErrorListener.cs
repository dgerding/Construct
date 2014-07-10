namespace Telerik.Expressions
{
	internal class CountingParserErrorListener : ParserErrorListener
	{
		public int ErrorsCount { get; private set; }

		public bool HasErrors
		{
			get
			{
				return this.ErrorsCount > 0;
			}
		}

		public override void ReportError(SourceSpan span, string message)
		{
			this.ErrorsCount++;
		}
	}
}