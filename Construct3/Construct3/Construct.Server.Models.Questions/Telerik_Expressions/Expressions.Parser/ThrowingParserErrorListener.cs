namespace Telerik.Expressions
{
	internal class ThrowingParserErrorListener : ParserErrorListener
	{
		public override void ReportError(SourceSpan span, string message)
		{
			throw new SyntaxErrorException(message, span);
		}
	}
}