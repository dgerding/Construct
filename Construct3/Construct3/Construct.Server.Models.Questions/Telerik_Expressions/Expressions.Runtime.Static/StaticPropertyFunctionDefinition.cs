using System.Reflection;

namespace Telerik.Expressions
{
	internal class StaticPropertyFunctionDefinition : FunctionDefinition
	{
		private readonly PropertyInfo property;

		public PropertyInfo Property
		{
			get
			{
				return this.property;
			}
		}

		public StaticPropertyFunctionDefinition(PropertyInfo property, DefinitionMetadata metadata)
			: base(property.Name, metadata)
		{
			this.property = property;
		}
	}
}
