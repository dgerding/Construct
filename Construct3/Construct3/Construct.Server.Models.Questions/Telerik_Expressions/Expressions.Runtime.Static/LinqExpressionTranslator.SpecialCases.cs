using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Telerik.Windows.Data;

namespace Telerik.Expressions.Runtime
{
	internal partial class LinqExpressionTranslator
	{
		private static readonly List<BinarySpecialCase> BinarySpecialCases = new List<BinarySpecialCase>
		{
			CreateStringConcatStringStringCase(),
			CreateStringConcatObjectStringCase(),
			CreateStringConcatStringObjectCase(),
			CreateConvertCase(),
		};

		private static BinarySpecialCase CreateStringConcatStringStringCase()
		{
			return new BinarySpecialCase(
				(type, left, right) => 
				{
					return 
						type == ExpressionType.Add && 
						left.Type == typeof(string) &&
						right.Type == typeof(string);
				},
				(type, left, right) => Expression.Add(left, right, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) })));
		}
		
		private static BinarySpecialCase CreateStringConcatObjectStringCase()
		{
			return new BinarySpecialCase(
				(type, left, right) => 
				{
					return 
						type == ExpressionType.Add && 
						left.Type != typeof(string) &&
						right.Type == typeof(string);
				},
				(type, left, right) => 
				{
					if (left.Type.IsValueType)
					{
						left = Expression.Convert(left, typeof(object));
					}

					return Expression.Add(left, right, typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }));
				});
		}

		private static BinarySpecialCase CreateStringConcatStringObjectCase()
		{
            Func<ExpressionType, Expression, Expression, bool> specialCasePredicate = (type, left, right) =>
			{
				return type == ExpressionType.Add && left.Type == typeof(string) && right.Type != typeof(string);
			};

            Func<ExpressionType, Expression, Expression, Expression> binaryCreator = (type, left, right) =>
			{
				if (right.Type.IsValueType)
				{
					right = Expression.Convert(right, typeof(object));
				}

				return Expression.Add(left, right, typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }));
			};

            return new BinarySpecialCase(specialCasePredicate, binaryCreator);
		}

		private static BinarySpecialCase CreateConvertCase()
		{
            Func<ExpressionType, Expression, Expression, bool> specialCasePredicate = (type, left, right) =>
			{
				return
					left.Type != right.Type &&
					(AreTypesConvertible(left.Type, right.Type) || AreTypesConvertible(right.Type, left.Type));
			};

            Func<ExpressionType, Expression, Expression, Expression> binaryCreator = (type, left, right) =>
			{
				if (AreTypesConvertible(left.Type, right.Type))
				{
					left = Expression.Convert(left, right.Type);
				}
				else
				{
					right = Expression.Convert(right, left.Type);
				}

				return Expression.MakeBinary(type, left, right);
			};

            return new BinarySpecialCase(specialCasePredicate, binaryCreator);
		}

		private static bool AreTypesConvertible(Type source, Type target)
		{
			return
				source.IsCompatibleWith(target) || AreTypesNumericConvertible(source, target);
		}

		private static bool AreTypesNumericConvertible(Type source, Type target)
		{
			return
				source.IsNumericType() &&
				target.IsNumericType() &&
				Type.GetTypeCode(source.GetNonNullableType()) < Type.GetTypeCode(target.GetNonNullableType());
		}

		private static bool TryCreateBinarySpecialCase(ExpressionType type, Expression left, Expression right, out Expression result)
		{
			var specialCase = BinarySpecialCases.FirstOrDefault(c => c.IsSpecialCase(type, left, right));
			if (specialCase != null)
			{
				result = specialCase.MakeBinary(type, left, right);
				return true;
			}

			result = null;
			return false;
		}

		private class BinarySpecialCase
		{
			private readonly Func<ExpressionType, Expression, Expression, bool> specialCasePredicate;
			private readonly Func<ExpressionType, Expression, Expression, Expression> binaryCreator;

			public BinarySpecialCase(
				Func<ExpressionType, Expression, Expression, bool> specialCasePredicate,
				Func<ExpressionType, Expression, Expression, Expression> binaryCreator)
			{
				this.specialCasePredicate = specialCasePredicate;
				this.binaryCreator = binaryCreator;
			}

			public bool IsSpecialCase(ExpressionType type, Expression left, Expression right)
			{
				return this.specialCasePredicate(type, left, right);
			}

			public Expression MakeBinary(ExpressionType type, Expression left, Expression right)
			{
                Debug.Assert(this.specialCasePredicate(type, left, right), "this.specialCasePredicate(type, left, right)");

				return this.binaryCreator(type, left, right);
			}
		}
	}
}
