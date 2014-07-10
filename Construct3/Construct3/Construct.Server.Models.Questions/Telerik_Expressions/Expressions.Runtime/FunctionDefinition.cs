using System;

namespace Telerik.Expressions
{
	internal class FunctionDefinition : DefinitionBase
	{
		public FunctionDefinition(string name)
			: this(name, DefinitionMetadata.Empty())
		{
		}

		public FunctionDefinition(string name, DefinitionMetadata metadata)
			: base(name, metadata)
		{
			if (name.Length == 0)
			{
				throw new ArgumentOutOfRangeException("name");
			}
		}
	}
}
