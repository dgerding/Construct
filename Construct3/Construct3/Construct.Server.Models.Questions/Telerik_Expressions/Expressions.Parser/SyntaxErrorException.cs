using System;

namespace Telerik.Expressions
{
#if WPF
	[Serializable]
#endif
    internal class SyntaxErrorException : Exception
	{
		private readonly SourceSpan span;

		public SourceSpan Span
		{
			get
			{
				return this.span;
			}
		}

		public SyntaxErrorException()
		{
		}

		public SyntaxErrorException(string message) 
			: base(message)
		{ 
		}

		public SyntaxErrorException(string message, SourceSpan span) 
			: this(message)
		{
			this.span = span;
		}
	}
}