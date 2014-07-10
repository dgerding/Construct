using System;

namespace Telerik.Expressions
{
	internal class DateTimeLexer : StartEndLiteralLexer
	{
		private const char DateSeparator = '#';

		private DateTimeLexer(ParserContext context)
			: base(context, DateSeparator)
		{
		}

		public static DateTimeToken ReadDateTime(ParserContext context)
		{
			return (DateTimeToken)new DateTimeLexer(context).ReadToken();
		}

		public static bool IsCharDateTimeStart(char ch)
		{
			return ch == DateSeparator;
		}

		protected internal override bool TryCreateTokenFromLiteral(string literal, out Token token)
		{
			token = null;
			DateTime value;

			if (DateTime.TryParse(literal, out value))
			{
				token = new DateTimeToken(value);
				return true;
			}

			return false;
		}
	}
}