namespace Telerik.Expressions
{
	internal abstract class ParserErrorListener
	{
		public abstract void ReportError(SourceSpan span, string message);
	}
}