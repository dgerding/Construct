using System;

namespace Telerik.Expressions
{
	internal class ParameterDefinition : VariableDefinition
	{
		private readonly Type type;

		public ParameterDefinition(string name, Type type)
			: base(name)
		{
			this.type = type;
		}

		public override bool HasValue
		{
			get
			{
				return false;
			}
		}

		public override object Value
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override Type Type
		{
			get
			{
				return this.type;
			}
		}
	}
}