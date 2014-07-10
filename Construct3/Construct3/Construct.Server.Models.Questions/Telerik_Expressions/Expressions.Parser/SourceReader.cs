using System;
using System.IO;

namespace Telerik.Expressions
{
	internal interface ISourceReader : IDisposable
	{
		int Peek();
		int Read();

		SourceLocation Location { get; }
	}

	internal abstract class SourceReaderBase : ISourceReader
	{
		private int line;
		private int column;

		public SourceLocation Location
		{
			get
			{
				return new SourceLocation(this.line, this.column);
			}
		}

		protected SourceReaderBase()
		{
			var minLocation = SourceLocation.MinValue;

			this.line = minLocation.Line;
			this.column = minLocation.Column;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	
		public virtual int Peek()
		{
			return this.PeekCore();
		}

		public virtual int Read()
		{
			this.column++;

            // TODO: increment line as well
			return this.ReadCore();
		}

		protected internal virtual void Dispose(bool disposing)
		{
		}

		protected internal abstract int PeekCore();

		protected internal abstract int ReadCore();
	}

	internal class StringSourceReader : TextReaderSourceReader
	{
		public StringSourceReader(string text)
			: base(new StringReader(text))
		{
		}
	}

	internal class TextReaderSourceReader : SourceReaderBase
	{
		private readonly TextReader reader;

		public TextReaderSourceReader(TextReader reader)
		{
			this.reader = reader;
		}

		protected internal override int PeekCore()
		{
			return this.reader.Peek();
		}

		protected internal override int ReadCore()
		{
			return this.reader.Read();
		}

		protected internal override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.reader.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}