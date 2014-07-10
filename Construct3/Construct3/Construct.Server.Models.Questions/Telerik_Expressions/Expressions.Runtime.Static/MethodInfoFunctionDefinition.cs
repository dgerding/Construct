using System.Reflection;

namespace Telerik.Expressions
{
	internal class MethodInfoFunctionDefinition : FunctionDefinition
	{
		private readonly MethodInfo method;

		public MethodInfo Method
		{
			get
			{
				return this.method;
			}
		}

		public MethodInfoFunctionDefinition(MethodInfo method)
			: this(method, DefinitionMetadata.Empty())
		{
		}

		public MethodInfoFunctionDefinition(MethodInfo method, DefinitionMetadata metadata)
			: this(method, method.Name, metadata)
		{
		}

		public MethodInfoFunctionDefinition(MethodInfo method, string name)
			: this(method, name, DefinitionMetadata.Empty())
		{
		}

		public MethodInfoFunctionDefinition(MethodInfo method, string name, DefinitionMetadata metadata)
			: base(name, metadata)
		{
			this.method = method;
		}
	}

	internal class ExpressionFactoryMethodFunctionDefinition : MethodInfoFunctionDefinition
	{
		public ExpressionFactoryMethodFunctionDefinition(MethodInfo method, string name, DefinitionMetadata metadata)
			: base(method, name, metadata)
		{
		}
	}
}
