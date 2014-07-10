namespace Telerik.Expressions
{
	internal sealed class ParserContext
	{
		private readonly ISourceReader sourceReader;
		private readonly ParserErrorListener errorListener;

		public ISourceReader SourceReader
		{
			get
			{
				return this.sourceReader;
			}
		}

		public ParserErrorListener ErrorListener
		{
			get
			{
				return this.errorListener;
			}
		}

		public ParserContext(ISourceReader sourceReader)
			: this(sourceReader, new ThrowingParserErrorListener())
		{
		}

		public ParserContext(ISourceReader sourceReader, ParserErrorListener errorListener)
		{
			this.sourceReader = sourceReader;
			this.errorListener = errorListener;
		}
	}
}