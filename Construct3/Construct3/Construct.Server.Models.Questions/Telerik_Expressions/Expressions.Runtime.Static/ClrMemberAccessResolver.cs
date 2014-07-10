using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.Windows.Data;

namespace Telerik.Expressions.Runtime
{
	internal class ClrMemberAccessResolver : IMemberAccessResolver
	{
#if WPF
		// ICustomTypeDescriptor reflection.
		private static MethodInfo customTypeDescriptorGetPropertiesMethod = typeof(ICustomTypeDescriptor).GetMethod("GetProperties", new Type[] { });
		private static MethodInfo propertyDescriptorCollectionFindMethod = typeof(PropertyDescriptorCollection).GetMethod("Find", new Type[] { typeof(string), typeof(bool) });
		private static MethodInfo propertyDescriptorGetValueMethod = typeof(PropertyDescriptor).GetMethod("GetValue");
#endif
#if WPF45 || SILVERLIGHT
		private static MethodInfo customTypeProviderGetCustomTypeMethod = typeof(ICustomTypeProvider).GetMethod("GetCustomType", new Type[] { });
		//// private static MethodInfo getTypeMethod = typeof(object).GetMethod("GetType", new Type[] { });
		private static MethodInfo typeGetPropertyMethod = typeof(Type).GetMethod("GetProperty", new Type[] { typeof(string) });
		private static MethodInfo propertyInfoGetValueMethod = typeof(PropertyInfo).GetMethod("GetValue", new Type[] { typeof(object) }, null);
#endif

		private readonly LinqExpressionTranslator translator;
		private readonly IDictionary<string, ParameterExpression> resolvedParametersByName;

		public IEnumerable<ParameterExpression> ResolvedParameters
		{
			get
			{
				return this.resolvedParametersByName.Values;
			}
		}

		protected LinqExpressionTranslator Translator
		{
			get
			{
				return this.translator;
			}
		}

		public ClrMemberAccessResolver(LinqExpressionTranslator translator)
		{
			this.translator = translator;
			this.resolvedParametersByName = new Dictionary<string, ParameterExpression>();
		}

		public Expression ResolveMemberAccess(MemberExpressionNode node)
		{
			Expression instance;
			if (node.Instance == null)
			{
				return this.ResolveVariableExpression(node.Name);
			}
			else
			{
				instance = this.translator.Translate(node.Instance);
			}

			return this.CreateMemberAccessExpression(instance, node.Name);
		}

		protected virtual Expression CreateMemberAccessExpression(Expression instance, string memberName)
		{
#if WPF
			if (instance.Type != null && instance.Type.IsCompatibleWith(typeof(ICustomTypeDescriptor)))
			{
				return this.CreateCustomTypeDescriptorMemberAccessExpression(instance, memberName);
			}
#endif

#if WPF45 || SILVERLIGHT
			if (instance.Type != null && instance.Type.IsCompatibleWith(typeof(ICustomTypeProvider)))
			{
				return this.CreateCustomTypeProviderMemberAccessExpression(instance, memberName);
			}
#endif

			return Expression.PropertyOrField(instance, memberName);
		}

#if WPF45 || SILVERLIGHT
		private Expression CreateCustomTypeProviderMemberAccessExpression(Expression instance, string memberName)
		{
			Type memberType = this.TryResolveCustomTypeProviderMemberType(instance.Type.AssemblyQualifiedName, memberName);
			var customTypeExpression = Expression.Call(instance, customTypeProviderGetCustomTypeMethod);
			var propertyInfo = Expression.Call(customTypeExpression, typeGetPropertyMethod, Expression.Constant(memberName, typeof(string)));
			var propertyValue = Expression.Call(propertyInfo, propertyInfoGetValueMethod, instance);

			if (memberType != null)
			{
				return Expression.Convert(propertyValue, memberType);
			}

			return propertyValue;
		}

		private Type TryResolveCustomTypeProviderMemberType(string customTypeProviderAssemblyQualifiedName, string memberName)
		{
			Type memberType = null;

			VariableDefinition ctpInstanceVariable = null;

			this.translator.Scope.TryGetVariable(customTypeProviderAssemblyQualifiedName, out ctpInstanceVariable);

			Tuple<ICustomTypeProvider, PropertyInfo> parentInstanceAndPropertyInfo;

			if (ClrMemberAccessResolver.TryGetParentInstanceAndPropertyInfo(ctpInstanceVariable, memberName, out parentInstanceAndPropertyInfo))
			{
				PropertyInfo memberPropertyInfo = parentInstanceAndPropertyInfo.Item2;
				ICustomTypeProvider parentInstance = parentInstanceAndPropertyInfo.Item1;
				memberType = memberPropertyInfo.PropertyType;

				if (memberType.IsCompatibleWith(typeof(ICustomTypeProvider)))
				{
					var memberInstance = memberPropertyInfo.GetValue(parentInstance, null);
					if (memberInstance != null)
					{
						this.translator.Scope.AddVariable(new VariableDefinition(memberInstance.GetType().AssemblyQualifiedName, memberInstance));
					}
				}
				else if (memberType.IsCompatibleWith(typeof(IEnumerable)))
				{
					// If the member is IEnumerable<ICustomTypeProvider> we will use the first element if present.
					IEnumerable enumerable = memberPropertyInfo.GetValue(parentInstance, null) as IEnumerable;
					if (enumerable != null)
					{
						object firstElement = null;
						enumerable.TryGetFirstElement(out firstElement);

						var ctp = firstElement as ICustomTypeProvider;
						if (ctp != null)
						{
							this.translator.Scope.AddVariable(new VariableDefinition(ctp.GetType().AssemblyQualifiedName, ctp));
						}
					}
				}
			}

			return memberType;
		}

		private static bool TryGetParentInstanceAndPropertyInfo(VariableDefinition ctpInstanceVariable, string memberName, out Tuple<ICustomTypeProvider, PropertyInfo> result)
		{
			result = null;
			if (ctpInstanceVariable == null)
			{
				return false;
			}

			ICustomTypeProvider parentInstance = ctpInstanceVariable.Value as ICustomTypeProvider;
			if (parentInstance == null)
			{
				return false;
			}

			Type typeProviderCustomType = parentInstance.GetCustomType();
			if (typeProviderCustomType == null)
			{
				return false;
			}

			PropertyInfo memberPropertyInfo = typeProviderCustomType.GetProperty(memberName);
			if (memberPropertyInfo == null)
			{
				return false;
			}

			result = Tuple.Create<ICustomTypeProvider, PropertyInfo>(parentInstance, memberPropertyInfo);
			return true;
		}
#endif

#if WPF
		private Expression CreateCustomTypeDescriptorMemberAccessExpression(Expression instance, string memberName)
		{
			Type memberType = this.TryResolveCustomTypeDescriptorMemberType(instance.Type.AssemblyQualifiedName, memberName);

			var propertyDescriptorCollection = Expression.Call(instance,
				customTypeDescriptorGetPropertiesMethod);

			var propertyDescriptor = Expression.Call(propertyDescriptorCollection,
				propertyDescriptorCollectionFindMethod,
				Expression.Constant(memberName, typeof(string)),
				Expression.Constant(false, typeof(bool)));

			var propertyValue = Expression.Call(propertyDescriptor,
				propertyDescriptorGetValueMethod,
				instance);

			if (memberType != null)
			{
				return Expression.Convert(propertyValue, memberType);
			}

			return propertyValue;
		}

		private Type TryResolveCustomTypeDescriptorMemberType(
			string customTypeDescriptorAssemblyQualifiedName, string memberName)
		{
			Type memberType = null;
			VariableDefinition ctdInstanceVariable = null;

			// We store an instance variable for each ICustomTypeDescriptor that we encounter
			// We need a one concrete instance in order to discover its member types.
			this.translator.Scope.TryGetVariable(customTypeDescriptorAssemblyQualifiedName, out ctdInstanceVariable);

			if (ctdInstanceVariable == null)
			{
				return null;
			}

			ICustomTypeDescriptor parentInstance = ctdInstanceVariable.Value as ICustomTypeDescriptor;

			if (parentInstance == null)
			{
				return null;
			}

			PropertyDescriptorCollection parentInstanceProperties = parentInstance.GetProperties();
			PropertyDescriptor memberPropertyDescriptor = parentInstanceProperties[memberName];

			if (memberPropertyDescriptor == null)
			{
				return null;
			}

			memberType = memberPropertyDescriptor.PropertyType;

			// We have found the memberType. Now let's see whether the member is an ICustomTypeDescriptor as well
			// and add its instance to the scope while we can.
			if (memberType.IsCompatibleWith(typeof(ICustomTypeDescriptor)))
			{
				var memberInstance = memberPropertyDescriptor.GetValue(parentInstance);
				if (memberInstance != null)
				{
					this.translator.Scope.AddVariable(new VariableDefinition(memberInstance.GetType().AssemblyQualifiedName, memberInstance));
				}
			}
			else if (memberType.IsCompatibleWith(typeof(IEnumerable)))
			{
				// If the member is IEnumerable<ICustomTypeDescriptor> we will use the first element if present.
				IEnumerable enumerable = memberPropertyDescriptor.GetValue(parentInstance) as IEnumerable;
				if (enumerable != null)
				{
					object firstElement = null;
					enumerable.TryGetFirstElement(out firstElement);

					var ctd = firstElement as ICustomTypeDescriptor;
					if (ctd != null)
					{
						this.translator.Scope.AddVariable(new VariableDefinition(ctd.GetType().AssemblyQualifiedName, ctd));
					}
				}
			}

			return memberType;
		}
#endif

		private bool TryGetFromResolvedParameters(string name, out Expression expression)
		{
			expression = null;
			ParameterExpression parameter;

			if (this.resolvedParametersByName.TryGetValue(name, out parameter))
			{
				expression = parameter;
				return true;
			}

			return false;
		}

		private Expression ResolveVariableExpression(string name)
		{
			Expression instance;
			if (this.TryGetFromResolvedParameters(name, out instance))
			{
				return instance;
			}

			return this.ResolveExpressionFromScope(name);
		}

		private Expression ResolveExpressionFromScope(string name)
		{
			VariableDefinition variable;
			var hasNamelessVariable = false;

			if (!this.translator.Scope.TryGetVariable(name, out variable))
			{
				hasNamelessVariable = this.translator.Scope.TryGetVariable(string.Empty, out variable);
			}

			var expression = this.CreateExpressionFromVariable(variable);

			if (hasNamelessVariable)
			{
				return this.CreateMemberAccessExpression(expression, name);
			}

			return expression;
		}

		private Expression CreateExpressionFromVariable(VariableDefinition variable)
		{
			var parameter = variable as ParameterDefinition;
			if (parameter != null)
			{
				return this.GetOrCreateExpressionFromParameter(parameter);
			}

			Debug.Assert(variable.HasValue, "variable.HasValue");
			return Expression.Constant(variable.Value);
		}

		private ParameterExpression GetOrCreateExpressionFromParameter(ParameterDefinition parameter)
		{
			ParameterExpression expression;
			if (!this.resolvedParametersByName.TryGetValue(parameter.Name, out expression))
			{
				expression = Expression.Parameter(parameter.Type, parameter.Name);
				this.resolvedParametersByName.Add(parameter.Name, expression);
			}

			return expression;
		}
	}
}
