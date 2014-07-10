using System;
using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal partial class Lexer : LexerBase
	{
		private Stack<Token> storedTokens;

		private bool HasStoredTokens
		{
			get
			{
				return this.storedTokens.Count > 0;
			}
		}

		public Lexer(ParserContext context)
			: base(context)
		{
			this.storedTokens = new Stack<Token>();
		}

		public void AddToken(Token token)
		{
			this.storedTokens.Push(token);
		}

		public Token ReadToken()
		{
			if (this.HasStoredTokens)
			{
				return this.PopStoredToken();
			}

			this.SkipWhitespace();

			var ch = this.PeekChar();

			if (SyntaxToken.IsSyntaxToken(ch))
			{
				return this.ReadSyntaxOrIdentifier();
			}
			else if (NumberLexer.IsCharNumberStart(ch))
			{
				return NumberLexer.ReadNumber(this.Context);
			}
			else if (DateTimeLexer.IsCharDateTimeStart(ch))
			{
				return DateTimeLexer.ReadDateTime(this.Context);
			}
			else if (StringLexer.IsCharStringStart(ch))
			{
				return StringLexer.ReadString(this.Context);
			}
			else if (NameLexer.IsCharNameStart(ch))
			{
				return NameLexer.ReadName(this.Context);
			}

			this.ReportUnexpectedCharacter();

			return this.ReadToken();
		}

		private Token ReadSyntaxOrIdentifier()
		{
			var syntax = this.ReadSyntax();
			
			// Special case for indexer expressions
			if (syntax.TokenType == TokenType.Bracket)
			{
				var nextChar = this.PeekChar();
				if (IdentifierLexer.IsCharIdentifierStart(nextChar))
				{
					return this.ReadEscapedIdentifier(syntax);
				}
			}

			return syntax;
		}

		private SyntaxToken ReadSyntax()
		{
			return SyntaxLexer.ReadSyntax(this.Context);
		}

		private IdentifierToken ReadEscapedIdentifier(SyntaxToken readSyntax)
		{
			var token = IdentifierLexer.EscapingIdentifierLexer.ReadIdentifier(this.Context, readSyntax.TokenChar);

			// fix up start location, because the escape char was already read
			token.SetSpan(readSyntax.Span().Start, token.Span().End);

			return token;
		}

		public static IEnumerable<Token> Tokenize(ParserContext context)
		{
			var lexer = new Lexer(context);

			return lexer.ReadAllTokens();
		}

		private IEnumerable<Token> ReadAllTokens()
		{
			var token = this.ReadToken();

			while (token.TokenType != TokenType.Eof)
			{
				yield return token;

				token = this.ReadToken();
			}
		}

		private Token PopStoredToken()
		{
			if (!this.HasStoredTokens)
			{
				throw new InvalidOperationException("Store token first before popping it.");
			}

			return this.storedTokens.Pop();
		}

		private void SkipWhitespace()
		{
			var ch = this.PeekChar();
			while (char.IsWhiteSpace(ch))
			{
				this.ReadChar();
				ch = this.PeekChar();
			}
		}

		private void ReportUnexpectedCharacter()
		{
			var start = this.Reader.Location;
			var unexpectedChar = this.ReadChar();
			var end = this.Reader.Location;

			var errorSpan = new SourceSpan(start, end);
			this.ReportError(errorSpan, string.Format("Unexpected character: '{0}'", unexpectedChar));
		}
	}
}
