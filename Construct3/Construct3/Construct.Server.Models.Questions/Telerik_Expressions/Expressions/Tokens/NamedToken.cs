namespace Telerik.Expressions
{
	internal abstract class NamedToken : Token
	{
		private readonly string name;

		protected NamedToken(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string ToString()
		{
			return string.Format("<{0} {1}>", this.GetType().Name, this.Name);
		}
	}
}