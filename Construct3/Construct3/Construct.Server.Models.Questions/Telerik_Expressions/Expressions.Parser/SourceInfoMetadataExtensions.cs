namespace Telerik.Expressions
{
	internal static class SourceInfoMetadataExtensions
	{
		private const string SpanKey = "Span";

		public static void SetSpan(this IMetadataProvider source, SourceLocation start, SourceLocation end)
		{
			source.SetSpan(new SourceSpan(start, end));
		}

		public static void SetSpan(this IMetadataProvider source, SourceSpan span)
		{
			source.SetMetadataValue(SpanKey, span);
		}

		public static SourceSpan Span(this IMetadataProvider source)
		{
			return source.GetMetadataValueOrDefault(SpanKey, SourceSpan.Invalid);
		}

		private static T GetMetadataValueOrDefault<T>(this IMetadataProvider source, string key, T defaultValue)
		{
			object valueObj;

			if (source.Metadata.TryGetValue(key, out valueObj))
			{
				return (T)valueObj;
			}

			return defaultValue;
		}

		private static void SetMetadataValue(this IMetadataProvider source, string key, object value)
		{
			source.Metadata[key] = value;
		}
	}
}
