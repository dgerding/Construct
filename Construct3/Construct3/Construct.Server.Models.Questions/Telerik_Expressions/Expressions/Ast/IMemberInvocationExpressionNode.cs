using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal interface IMemberInvocationExpressionNode : IMetadataProvider
	{
		ReadOnlyCollection<ExpressionNode> Arguments { get; }
		string Name { get; }
		ExpressionNode Instance { get; }

		ExpressionNode Update(ExpressionNode instance, IEnumerable<ExpressionNode> arguments);
	}
}