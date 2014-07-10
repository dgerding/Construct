using System.Text;

namespace Telerik.Expressions
{
	internal abstract class LiteralLexerBase : LexerBase
	{
		private readonly StringBuilder chars;

		protected LiteralLexerBase(ParserContext context)
			: base(context)
		{
			this.chars = new StringBuilder();
		}

		protected void ReadAndAddChar()
		{
			this.AddChar(this.ReadChar());
		}

		protected void AddChar(char ch)
		{
			this.chars.Append(ch);
		}

		public virtual Token ReadToken()
		{
			var start = this.Reader.Location;
			
			this.ReadToEnd();

			var end = this.Reader.Location;

			var literal = this.CreateLiteralFromReadChars();
			
			return this.CreateToken(literal, new SourceSpan(start, end));
		}

		private Token CreateToken(string literal, SourceSpan tokenSpan)
		{
			Token token;

			if (this.TryCreateTokenFromLiteral(literal, out token))
			{
				token.SetSpan(tokenSpan);
			}
			else
			{
				this.ReportInvalidTokenError(tokenSpan);
			}

			return token;
		}

		protected internal abstract bool TryCreateTokenFromLiteral(string literal, out Token token);

		protected virtual void ReadToEnd()
		{
		}

		protected string CreateLiteralFromReadChars()
		{
			return this.chars.ToString();
		}

		private void ReportInvalidTokenError(SourceSpan errorSpan)
		{
			this.ReportError(errorSpan, "Invalid value.");
		}
	}
}
