namespace Telerik.Expressions
{
	internal partial class SyntaxToken : Token
	{
		private readonly TokenType tokenType;
		private readonly char tokenChar;

		public char TokenChar
		{
			get
			{
				return this.tokenChar;
			}
		}
		
		public override TokenType TokenType
		{
			get
			{
				return this.tokenType;
			}
		}

		private SyntaxToken(char tokenChar, TokenType tokenType)
		{
			this.tokenChar = tokenChar;
			this.tokenType = tokenType;
		}
	}
}