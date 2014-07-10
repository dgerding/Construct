namespace Telerik.Expressions
{
	internal class LiteralToken<T> : Token, ILiteralToken
	{
		private readonly T value;

		public T Value
		{
			get
			{
				return this.value;
			}
		}

		object ILiteralToken.Value
		{
			get
			{
				return this.Value;
			}
		}

		public override TokenType TokenType
		{
			get
			{
				return Expressions.TokenType.Literal;
			}
		}

		public LiteralToken(T value)
		{
			this.value = value;
		}
	}

	internal interface ILiteralToken
	{
		object Value { get; }
	}
}