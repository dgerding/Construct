namespace Telerik.Expressions
{
	internal abstract class NumberToken<TNumber> : LiteralToken<TNumber>, INumberToken where TNumber : struct
	{
		protected NumberToken(TNumber value)
			: base(value)
		{
		}

		public abstract TNumber NegatedValue { get; }

		object INumberToken.NegatedValue
		{
			get
			{
				return this.NegatedValue;
			}
		}
	}

	internal interface INumberToken : ILiteralToken
	{
		object NegatedValue { get; }
	}
}
