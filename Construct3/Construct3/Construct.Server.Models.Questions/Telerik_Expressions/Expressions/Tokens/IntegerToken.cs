namespace Telerik.Expressions
{
	internal class IntegerToken : NumberToken<int>
	{
		public IntegerToken(int value) : base(value)
		{
		}

		public override int NegatedValue
		{
			get
			{
				return -this.Value;
			}
		}
	}
}