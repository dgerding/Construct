using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Telerik.Windows.Data;

namespace Telerik.Expressions
{
	internal class GenericMethodInfo
	{
		private readonly MethodInfo genericMethodDefinition;
		private readonly Type[] genericArguments;
		private readonly Type[] resolvedGenericArguments;

		public GenericMethodInfo(MethodInfo genericMethodDefinition)
		{
			if (!genericMethodDefinition.IsGenericMethodDefinition)
			{
				throw new ArgumentOutOfRangeException("genericMethodDefinition");
			}

			this.genericMethodDefinition = genericMethodDefinition;

			this.genericArguments = this.genericMethodDefinition.GetGenericArguments();
            this.resolvedGenericArguments = new Type[this.genericArguments.Length];
		}

		public bool IsPartialDefinition
		{
			get
			{
				return this.resolvedGenericArguments.Any(a => a == null);
			}
		}

		public IDictionary<Type, Type> ResolvedGenericArguments
		{
			get
			{
				return
					this.genericArguments
					.Zip(this.resolvedGenericArguments, Tuple.Create)
					.Where(pair => pair.Item2 != null)
					.ToDictionary(pair => pair.Item1, pair => pair.Item2);
			}
		}

		public void ResolveGenericArgumentsByMethodArgumentType(int parameterIndex, Type argumentType)
		{
			// TODO: check for out of range
			var parameter = this.genericMethodDefinition.GetParameters()[parameterIndex];
			var parameterType = parameter.ParameterType;

			if (parameterType.ContainsGenericParameters)
			{
				var genericType = argumentType.FindGenericType(parameterType.GetGenericTypeDefinition());
				if (genericType != null)
				{
					this.ResolveGenericArgumentsByGenericDefinition(parameterType, genericType);
				}
			}
		}

		private void ResolveGenericArgumentsByGenericDefinition(Type genericTypeDefinition, Type actualArgumentType)
		{
			var genericArgumentDefinitions = genericTypeDefinition.GetGenericArguments();
			var actualGenericArguments = actualArgumentType.GetGenericArguments();

            Debug.Assert(genericArgumentDefinitions.Length == actualGenericArguments.Length, "genericArgumentDefinitions.Length == actualGenericArguments.Length");

			for (int i = 0; i < genericArgumentDefinitions.Length; i++)
			{
				this.ResolveGenericArguments(genericArgumentDefinitions[i], actualGenericArguments[i]);
			}
		}

		private void ResolveGenericArguments(Type genericArgument, Type actualArgumentType)
		{
			if (genericArgument.IsGenericParameter)
			{
				var genericArgumentIndex = Array.IndexOf(this.genericArguments, genericArgument);

                Debug.Assert(genericArgumentIndex >= 0, "genericArgumentIndex >= 0");

                Debug.Assert((this.resolvedGenericArguments[genericArgumentIndex] ?? actualArgumentType) == actualArgumentType, "(this.resolvedGenericArguments[genericArgumentIndex] ?? actualArgumentType) == actualArgumentType");

				this.resolvedGenericArguments[genericArgumentIndex] = actualArgumentType;
			}
			else
			{
                // step down recursively over all generic arguments
				var genericArgs = genericArgument.GetGenericArguments();
				var actualArgs = actualArgumentType.GetGenericArguments();

                Debug.Assert(genericArgs.Length == actualArgs.Length, "genericArgs.Length == actualArgs.Length");

				for (int i = 0; i < genericArgs.Length; i++)
				{
					this.ResolveGenericArguments(genericArgs[i], actualArgs[i]);
				}
			}
		}

		public MethodInfo MakeGenericMethod()
		{
			if (this.IsPartialDefinition)
			{
				throw new InvalidOperationException();
			}

			return this.genericMethodDefinition.MakeGenericMethod(this.resolvedGenericArguments);
		}
	}
}
