using System;
using System.Linq.Expressions;
namespace Telerik.Expressions
{
	internal static class ExpressionExtensions
	{
		public static Type EffectiveType(this Expression expression)
		{
			if (expression.NodeType == ExpressionType.Lambda)
			{
				return expression.GetType();
			}

			return expression.Type;
		}
	}
}
