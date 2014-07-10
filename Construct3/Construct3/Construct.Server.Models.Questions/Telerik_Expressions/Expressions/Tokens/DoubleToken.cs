namespace Telerik.Expressions
{
	internal class DoubleToken : NumberToken<double>
	{
		public DoubleToken(double value)
			: base(value)
		{
		}

		public override double NegatedValue
		{
			get
			{
				return -this.Value;
			}
		}
	}
}