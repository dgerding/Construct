namespace Telerik.Expressions
{
	internal abstract class LexerBase
	{
		private static readonly char Eof = TokenType.Eof.ToTokenString()[0];

		private readonly ParserContext context;

		public ParserContext Context
		{
			get
			{
				return this.context;
			}
		}

		protected ISourceReader Reader
		{
			get
			{
				return this.context.SourceReader;
			}
		}

		protected LexerBase(ParserContext context)
		{
			this.context = context;
		}

		public static bool IsCharEof(char ch)
		{
			return ch == Eof;
		}

		protected char ReadChar()
		{
			return this.Reader.ReadChar();
		}

		protected char PeekChar()
		{
			return this.Reader.PeekChar();
		}

		protected void ReportError(SourceSpan span, string message)
		{
			this.context.ErrorListener.ReportError(span, message);
		}
	}
}
