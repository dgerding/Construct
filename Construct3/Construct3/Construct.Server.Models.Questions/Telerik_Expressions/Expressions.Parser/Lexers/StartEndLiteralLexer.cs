using System.Diagnostics;

namespace Telerik.Expressions
{
	internal abstract class StartEndLiteralLexer : LiteralReaderLexer
	{
		private readonly char startChar;
		private readonly char endChar;

		public char StartChar
		{
			get
			{
				return this.startChar;
			}
		}

		public char EndChar
		{
			get
			{
				return this.endChar;
			}
		}

		public StartEndLiteralLexer(ParserContext context, char startChar, char endChar)
			: base(context)
		{
			this.startChar = startChar;
			this.endChar = endChar;
		}

		public StartEndLiteralLexer(ParserContext context, char uniformStartEndChar)
			: this(context, uniformStartEndChar, uniformStartEndChar)
		{
		}

		protected override void ReadToEnd()
		{
            Debug.Assert(this.PeekChar() == this.StartChar, "this.PeekChar() == this.StartChar");

			// Read start char
			this.ReadChar();

			this.ReadToEndCore();
		}

		protected void ReadToEndCore()
		{
			base.ReadToEnd();
		}

		protected override bool Read()
		{
			return this.ReadCore();
		}

		protected bool ReadCore()
		{
			var ch = this.ReadChar();

			return this.TryAddChar(ch);
		}

		protected bool TryAddChar(char ch)
		{
            if (LexerBase.IsCharEof(ch))
			{
				this.ReportMissingEndError();
				return false;
			}

			if (!this.IsCharTokenEnd(ch))
			{
				this.AddChar(ch);
				return true;
			}

			return false;
		}

		private void ReportMissingEndError()
		{
			// TODO: think about previous location property on the reader
			var errorEndLocation = this.Reader.Location;
			var errorStartLocation = new SourceLocation(errorEndLocation.Line, errorEndLocation.Column - 1);
			var errorSpan = new SourceSpan(errorStartLocation, errorEndLocation);

			this.ReportError(errorSpan, string.Format("Missing end character: ' {0} '.", this.EndChar));
		}

		private bool IsCharTokenEnd(char ch)
		{
			return ch == this.EndChar;
		}
	}
}
