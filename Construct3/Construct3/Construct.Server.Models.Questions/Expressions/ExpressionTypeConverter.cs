using System;
using System.ComponentModel;
using System.Globalization;
using Telerik.Expressions;

namespace Telerik.Windows.Data
{
	/// <summary>
	/// Converts a string to a LINQ node expression containing the parsed string.
	/// Uses Telerik Expression Parser to parse the string to an ExpressionNode (AST) object.
	/// If there is a parse error returns null.
	/// </summary>
	public class ExpressionTypeConverter : TypeConverter
	{
        /// <inheritdoc />
		/// <remarks>
		/// True if <paramref name="sourceType" /> is a <see cref="string" /> type; otherwise, false.
		/// </remarks>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}

        /// <inheritdoc />
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var expressionString = value as string;
			if (expressionString != null)
			{
				ExpressionNode node;
				if (TryParseExpression(expressionString, out node))
				{
					return new NodeExpression(node);
				}
			}

			return null;
		}

		private static bool TryParseExpression(string input, out ExpressionNode node)
		{
			using (var sourceReader = new StringSourceReader(input))
			{
				var errorListener = new CountingParserErrorListener();
				var parserContext = new ParserContext(sourceReader, errorListener);
				node = Telerik.Expressions.ExpressionParser.Parse(parserContext);

				return !errorListener.HasErrors;
			}
		}
	}
}