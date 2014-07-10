using System.Diagnostics;

namespace Telerik.Expressions
{
	internal partial class IdentifierLexer
	{
		internal class EscapingIdentifierLexer : EscapingLiteralLexer
		{
			private const string IdentifierStartEndChars = "[]`";

			private static char GetMatchingEndCharForStartChar(char startChar)
			{
                Debug.Assert(IdentifierStartEndChars.IndexOf(startChar) >= 0, "IdentifierStartEndChars.IndexOf(startChar) >= 0");

				if (startChar == '[')
				{
					return ']';
				}

				return startChar;
			}

			private EscapingIdentifierLexer(ParserContext context, char startChar)
				: base(context, startChar, GetMatchingEndCharForStartChar(startChar))
			{
				// ` is escaped with `
				// ] is escaped with \, which on its own is escaped with \
				if (this.StartChar == '`')
				{
					this.AddEscapingChar(this.StartChar);
					this.AddCharToEscape(this.StartChar);
				}
				else
				{
                    Debug.Assert(this.StartChar == '[', "this.StartChar == '['");
                    Debug.Assert(this.EndChar == ']', "this.EndChar == ']'");

					this.AddEscapingChar('\\');
					this.AddCharToEscape('\\');
					this.AddCharToEscape(this.EndChar);
				}
			}

			protected override void ReadToEnd()
			{
				// We were called with already read start char,
				// so avoid reading the start char from the base lexer.
				if (this.PeekChar() != this.StartChar)
				{
					this.ReadToEndCore();
				}
				else
				{
					base.ReadToEnd();
				}
			}

			public static bool IsCharEscapedIdentifierStart(char ch)
			{
				return IdentifierStartEndChars.IndexOf(ch) >= 0;
			}

			public static IdentifierToken ReadIdentifier(ParserContext context)
			{
				return ReadIdentifier(context, context.SourceReader.PeekChar());
			}

			public static IdentifierToken ReadIdentifier(ParserContext context, char startChar)
			{
				var lexer = new EscapingIdentifierLexer(context, startChar);
				return (IdentifierToken)lexer.ReadToken();
			}

			protected internal override bool TryCreateTokenFromLiteral(string literal, out Token token)
			{
				token = new IdentifierToken(literal);
				
				return true;
			}
		}
	}
}