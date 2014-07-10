namespace Telerik.Expressions
{
	internal class DecimalToken : NumberToken<decimal>
	{
		public DecimalToken(decimal value)
			: base(value)
		{
		}

		public override decimal NegatedValue
		{
			get
			{
				return -this.Value;
			}
		}
	}
}