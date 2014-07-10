using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.Windows.Controls;

namespace Telerik.Expressions
{
	internal partial class ClrExpressionRuntime
	{
		private void InitializeDefaultFunctions()
		{
			this.AddGlobalFunctions(CreateQueryableFunctions());
			this.AddGlobalFunctions(CreateLogicalFunctions());
			this.AddGlobalFunctions(CreateDateTimeFunctions());
			this.AddGlobalFunctions(CreateMathFunctions());
		}

		private void AddGlobalFunctions(IEnumerable<FunctionDefinition> functions)
		{
			foreach (var function in functions)
			{
				this.GlobalScope.AddFunction(function);
			}
		}

		private static IEnumerable<FunctionDefinition> CreateLogicalFunctions()
		{
			var logical = LocalizationManager.GetString("ExpressionEditor_Logical");
			var metadata = new DefinitionMetadata("Checks whether a condition is met, and returns one value if True, and other value if False", logical);

			var method = typeof(Expression).GetMethod("Condition", new[] { typeof(Expression), typeof(Expression), typeof(Expression) });
			yield return new ExpressionFactoryMethodFunctionDefinition(method, "IF", metadata);
		}

		private static IEnumerable<FunctionDefinition> CreateDateTimeFunctions()
		{
			var date = LocalizationManager.GetString("ExpressionEditor_DateTime");

			yield return new StaticPropertyFunctionDefinition(
				typeof(DateTime).GetProperty("Now"), new DefinitionMetadata("Return the current date and time on this computer, expressed as the local time.", date));
			yield return new StaticPropertyFunctionDefinition(
				typeof(DateTime).GetProperty("Today"), new DefinitionMetadata("Return the current date.", date));
		}

		private static IEnumerable<FunctionDefinition> CreateMathFunctions()
		{
			var math = LocalizationManager.GetString("ExpressionEditor_Math");

			var metadataLookup = new Dictionary<string, DefinitionMetadata> 
            {
				{ "Abs", new DefinitionMetadata("Returns the absolute value of a number.", math) },
				{ "Acos", new DefinitionMetadata("Returns the angle whose cosine is the specified number.", math) },
				{ "Asin", new DefinitionMetadata("Returns the angle whose sine is the specified number.", math) },
				{ "Atan", new DefinitionMetadata("Returns the angle whose tangent is the specified number.", math) },
				{ "Atan2", new DefinitionMetadata("Returns the angle whose tangent is the quotient of two specified numbers.", math) },
				{ "Ceiling", new DefinitionMetadata("Returns the smallest integer greater than or equal to the specified number.", math) },
				{ "Cos", new DefinitionMetadata("Returns the cosine of the specified angle.", math) },
				{ "Cosh", new DefinitionMetadata("Returns the hyperbolic cosine of the specified angle.", math) },
				{ "Exp", new DefinitionMetadata("Returns e raised to the specified power.", math) },
				{ "Floor", new DefinitionMetadata("Returns the largest integer less than or equal to the specified number.", math) },
				{ "Log", new DefinitionMetadata("Returns the natural (base e) logarithm of a specified number.", math) },
				{ "Log10", new DefinitionMetadata("Returns the base 10 logarithm of a specified number.", math) },
				{ "Max", new DefinitionMetadata("Returns the larger of two numbers.", math) },
				{ "Min", new DefinitionMetadata("Returns the smaller of two numbers.", math) },
				{ "Pow", new DefinitionMetadata("Returns a specified number raised to the specified power.", math) },
				{ "Round", new DefinitionMetadata("Rounds a decimal number to the nearest integer number.", math) },
				{ "Sign", new DefinitionMetadata("Returns a value indicating the sign of a number.", math) },
				{ "Sin", new DefinitionMetadata("Returns the sine of the specified angle.", math) },
				{ "Sinh", new DefinitionMetadata("Returns the hyperbolic sine of the specified angle.", math) },
				{ "Sqrt", new DefinitionMetadata("Returns the square root of a specified number.", math) },
				{ "Tan", new DefinitionMetadata("Returns the tangent of the specified angle.", math) },
				{ "Tanh", new DefinitionMetadata("Returns the hyperbolic tangent of the specified angle.", math) },
				{ "Truncate", new DefinitionMetadata("Calculates the integral part of a specified number.", math) },
			};

			return CreateFunctions(typeof(Math), metadataLookup);
		}

		private static IEnumerable<FunctionDefinition> CreateQueryableFunctions()
		{
			var aggregate = LocalizationManager.GetString("ExpressionEditor_Aggregate");
			var other = LocalizationManager.GetString("ExpressionEditor_Other");

			var metadataLookup = new Dictionary<string, DefinitionMetadata> 
            {
				{ "Average", new DefinitionMetadata("Computes the average of a sequence of values.", aggregate) },
				{ "Count", new DefinitionMetadata("Returns the number of elements in a sequence.", aggregate) },
				{ "First", new DefinitionMetadata("Returns the first element of a sequence.", aggregate) },
				{ "Last", new DefinitionMetadata("Returns the last element of a sequence.", aggregate) },
				{ "Max", new DefinitionMetadata("Returns the maximum value in a sequence.", aggregate) },
				{ "Min", new DefinitionMetadata("Returns the minimum value in a sequence.", aggregate) },
				{ "Sum", new DefinitionMetadata("Computes the sum of a sequence of values.", aggregate) },
				{ "All", new DefinitionMetadata("Determines whether all the elements of a sequence satisfy a condition.", other) },
				{ "Any", new DefinitionMetadata("Determines whether any element of a sequence satisfies a condition.", other) },
				{ "Contains", new DefinitionMetadata("Determines whether a sequence contains a specified element.", other) },
				{ "Distinct", new DefinitionMetadata("Returns distinct elements from a sequence.", other) },
				{ "Where", new DefinitionMetadata("Filters a sequence of values based on a condition.", other) },
			};

			return CreateFunctions(typeof(Queryable), metadataLookup);
		}

		private static IEnumerable<FunctionDefinition> CreateFunctions(
			Type functionProvider, IDictionary<string, DefinitionMetadata> metadataLookup)
		{
			var methods = GetStaticMethodsFromType(functionProvider);

			foreach (var method in methods)
			{
				DefinitionMetadata metadata;
				if (!metadataLookup.TryGetValue(method.Name, out metadata))
				{
					metadata = DefinitionMetadata.Empty();
				}

				yield return new MethodInfoFunctionDefinition(method, metadata);
			}
		}

		private static IEnumerable<MethodInfo> GetStaticMethodsFromType(Type type)
		{
			var staticFlags = BindingFlags.Static | BindingFlags.Public;

			return type.GetMethods(staticFlags);
		}
	}
}
