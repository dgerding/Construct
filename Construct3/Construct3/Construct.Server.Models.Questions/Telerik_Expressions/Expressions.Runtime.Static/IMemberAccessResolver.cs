using System.Linq.Expressions;
using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal interface IMemberAccessResolver
	{
		Expression ResolveMemberAccess(MemberExpressionNode node);

		IEnumerable<ParameterExpression> ResolvedParameters 
		{ 
			get;
		}
	}
}
