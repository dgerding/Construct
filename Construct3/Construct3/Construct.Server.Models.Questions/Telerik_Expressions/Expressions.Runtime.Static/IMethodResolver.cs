using System.Linq.Expressions;

namespace Telerik.Expressions.Runtime
{
	internal interface IMethodResolver
	{
		Expression ResolveMethodCall(FunctionExpressionNode functionNode);
	}
}