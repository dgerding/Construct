namespace Telerik.Expressions
{
	internal class IdentifierToken : NamedToken
	{
		public override TokenType TokenType
		{
			get
			{
				return Expressions.TokenType.Identifier;
			}
		}

		public IdentifierToken(string name)
			: base(name)
		{
		}
	}
}