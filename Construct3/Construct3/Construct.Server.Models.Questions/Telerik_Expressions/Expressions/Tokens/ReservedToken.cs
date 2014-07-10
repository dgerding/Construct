namespace Telerik.Expressions
{
	internal abstract class ReservedToken : NamedToken
	{
		private readonly TokenType tokenType;
		
		public override TokenType TokenType
		{
			get
			{
				return this.tokenType;
			}
		}

		protected ReservedToken(string name, TokenType tokenType)
			: base(name)
		{
			this.tokenType = tokenType;
		}
	}
}