namespace Telerik.Expressions
{
	internal static class SourceReaderExtensions
	{
		public static char ReadChar(this ISourceReader reader)
		{
			return unchecked((char)reader.Read());
		}

		public static char PeekChar(this ISourceReader reader)
		{
			return unchecked((char)reader.Peek());
		}
	}
}