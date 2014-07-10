using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.Windows.Data;

namespace Telerik.Expressions.Runtime
{
	internal class ClrMethodResolver : IMethodResolver
	{
		private readonly LinqExpressionTranslator translator;

		protected LinqExpressionTranslator Translator
		{
			get
			{
				return this.translator;
			}
		}

		public ClrMethodResolver(LinqExpressionTranslator translator)
		{
			this.translator = translator;
		}

		public virtual Expression ResolveMethodCall(FunctionExpressionNode functionNode)
		{
			Expression methodCall;

			if (!this.TryResolveInstanceMethodCall(functionNode, out methodCall))
			{
				if (!this.TryResolveScopeMethodCall(functionNode, out methodCall))
				{
					return null;
				}
			}

			return methodCall;
		}

		private bool TryResolveInstanceMethodCall(FunctionExpressionNode functionNode, out Expression methodCall)
		{
			methodCall = this.ResolveInstanceMethodCall(functionNode);

			return methodCall != null;
		}

		private Expression ResolveInstanceMethodCall(FunctionExpressionNode functionNode)
		{
			if (functionNode.Instance == null)
			{
				return null;
			}

			return this.ResolveInstanceMethodCallOverride(functionNode);
		}

		protected virtual Expression ResolveInstanceMethodCallOverride(FunctionExpressionNode functionNode)
		{
			var instance = this.Translate(functionNode.Instance);

			var methods = FindInstanceMethods(instance.Type, functionNode.Name);

			var methodCall = this.ResolveBestMethodCall(methods, instance, functionNode.Arguments);
			return methodCall;
		}

		private bool TryResolveScopeMethodCall(FunctionExpressionNode functionNode, out Expression methodCall)
		{
			methodCall = this.ResolveScopeMethodCall(functionNode);

			return methodCall != null;
		}

		private Expression ResolveScopeMethodCall(FunctionExpressionNode functionNode)
		{
			var methodDefinitions = this.translator.Scope.GetFunctions(functionNode.Name);

			// Extension methods trick
			var effectiveArgs = functionNode.Arguments.ToList();
			if (functionNode.Instance != null)
			{
				effectiveArgs.Insert(0, functionNode.Instance);
			}

			return this.ResolveBestScopeMethodCall(methodDefinitions, effectiveArgs);
		}

		private Expression ResolveBestScopeMethodCall(IEnumerable<FunctionDefinition> methodDefinitions, IList<ExpressionNode> arguments)
		{
			Expression expression;
			if (this.TryResolveSpecialFunction(methodDefinitions, arguments, out expression))
			{
				return expression;
			}

			var methods = methodDefinitions.OfType<MethodInfoFunctionDefinition>().Select(m => m.Method);
			return this.ResolveBestMethodCall(methods, null, arguments);
		}

		private bool TryResolveSpecialFunction(
			IEnumerable<FunctionDefinition> functionDefinitions,
			IList<ExpressionNode> arguments,
			out Expression expression)
		{
			if (!TryResolveStaticProperty(functionDefinitions, arguments, out expression))
			{
				if (!this.TryResolveExpressionFactoryMethod(functionDefinitions, arguments, out expression))
				{
					return false;
				}
			}

			return true;
		}

		private static bool TryResolveStaticProperty(
			IEnumerable<FunctionDefinition> functionDefinitions,
			IList<ExpressionNode> arguments,
			out Expression expression)
		{
			var staticPropertyDefinitions =
				functionDefinitions
				.OfType<StaticPropertyFunctionDefinition>()
				.FirstOrDefault();

			if (staticPropertyDefinitions != null && arguments.Count == 0)
			{
				expression = Expression.Property(null, staticPropertyDefinitions.Property);
				return true;
			}

			expression = null;
			return false;
		}

		private bool TryResolveExpressionFactoryMethod(
			IEnumerable<FunctionDefinition> methodDefinitions, 
			IList<ExpressionNode> arguments, 
			out Expression expression)
		{
			var expressionFactoryMethod = 
				methodDefinitions
				.OfType<ExpressionFactoryMethodFunctionDefinition>()
				.Select(md => md.Method)
				.Where(m => m.GetParameters().Length == arguments.Count)
				.FirstOrDefault();

			if (expressionFactoryMethod != null)
			{
				var args = arguments.Select(this.Translate).ToArray();
				expression = (Expression)expressionFactoryMethod.Invoke(null, args);

				return true;
			}

			expression = null;
			return false;
		}

		private MethodCallExpression ResolveBestMethodCall(
			IEnumerable<MethodInfo> methods, Expression instance, IList<ExpressionNode> arguments)
		{
			var compatibleMethods = methods.Where(m => IsMethodCompatible(m, arguments));

			foreach (var method in compatibleMethods)
			{
				MethodCallExpression methodCall;
				if (this.TryCreateMethodCall(method, instance, arguments, out methodCall))
				{
					return methodCall;
				}
			}

			return null;
		}

		private bool TryCreateMethodCall(MethodInfo method, Expression instance, IList<ExpressionNode> args, out MethodCallExpression methodCall)
		{
			if (method.IsGenericMethodDefinition)
			{
				return this.TryCreateGenericMethodCall(method, instance, args, out methodCall);
			}

			return this.TryCreateNonGenericMethodCall(method, instance, args, out methodCall);
		}

		private bool TryCreateGenericMethodCall(MethodInfo method, Expression instance, IList<ExpressionNode> args, out MethodCallExpression methodCall)
		{
			try
			{
				return this.TryCreateGenericMethodCallCore(method, instance, args, out methodCall);
			}
			catch
			{
				methodCall = null;
				return false;
			}
		}

		private bool TryCreateGenericMethodCallCore(MethodInfo method, Expression instance, IList<ExpressionNode> args, out MethodCallExpression methodCall)
		{
			var translatedArgs = new List<Expression>();

			var parameters = method.GetParameters();
			var genericMethod = new GenericMethodInfo(method);
			while (genericMethod.IsPartialDefinition)
			{
				var position = translatedArgs.Count;
				var parameter = parameters[position];
				var argument = args[position];

				Expression expression = null;
				if (parameter.ParameterType.IsGenericType)
				{
					expression = this.TranslateGenericArgument(argument, parameter.ParameterType, genericMethod.ResolvedGenericArguments);
				}
				else
				{
					expression = this.Translate(argument);	
				}

				translatedArgs.Add(expression);

				genericMethod.ResolveGenericArgumentsByMethodArgumentType(position, expression.EffectiveType());
			}

			var effectiveMethod = genericMethod.MakeGenericMethod();
			var effectiveParameters = effectiveMethod.GetParameters();

			var otherArgs = 
				args.Skip(translatedArgs.Count)
				.Zip(effectiveParameters.Skip(translatedArgs.Count), this.TranslateArgument);
			translatedArgs.AddRange(otherArgs);

			return TryCreateMethodCallForTranslatedArgs(effectiveMethod, instance, translatedArgs, out methodCall);
		}

		private bool TryCreateNonGenericMethodCall(MethodInfo method, Expression instance, IList<ExpressionNode> args, out MethodCallExpression methodCall)
		{
			var translatedArgs = args.Select(this.Translate).ToArray();

			return TryCreateMethodCallForTranslatedArgs(method, instance, translatedArgs, out methodCall);
		}

		private static bool TryCreateMethodCallForTranslatedArgs(
			MethodInfo method, Expression instance, IEnumerable<Expression> arguments, out MethodCallExpression methodCall)
		{
			methodCall = null;

			if (IsMethodCompatibleWithArgumentTypes(method, arguments.Select(e => e.EffectiveType())))
			{
				arguments = EnsureCorrectArgumentTypes(arguments, method.GetParameters().Select(p => p.ParameterType));
				methodCall = Expression.Call(instance, method, arguments);
				return true;
			}

			return false;
		}

		private static IEnumerable<Expression> EnsureCorrectArgumentTypes(IEnumerable<Expression> arguments, IEnumerable<Type> perameterTypes)
		{
            Func<Expression, Type, Expression> resultSelector = (arg, type) => 
					 {
						 if (arg.Type != type && 
							 arg.Type.IsValueType &&
							 arg.Type.IsCompatibleWith(type))
						 {
							 return Expression.Convert(arg, type);
						 }
						 return arg;
					 };

			return arguments.Zip(perameterTypes, resultSelector);
		}

		protected Expression Translate(ExpressionNode argument)
		{
			return this.translator.Translate(argument);
		}

		private Expression TranslateArgument(ExpressionNode argument, ParameterInfo respectiveParameter)
		{
			var parameterType = respectiveParameter.ParameterType;

			if (parameterType.IsCompatibleWith<LambdaExpression>())
			{
				var funcType = parameterType.GetGenericArguments()[0];
				var funcGenericArguments = funcType.GetGenericArguments();
				var allArgsExceptLast = funcGenericArguments.Take(funcGenericArguments.Length - 1);

				return this.TranslateLambdaArgument(argument, allArgsExceptLast);
			}

			return this.Translate(argument);
		}

		private Expression TranslateLambdaArgument(ExpressionNode argument, IEnumerable<Type> parameterTypes)
		{
			var paramsArray = parameterTypes.ToArray();
			var parameterDefinitions = new List<ParameterDefinition>(paramsArray.Length);

			if (paramsArray.Length == 1)
			{
				parameterDefinitions.Add(new ParameterDefinition(string.Empty, paramsArray[0]));
			}
			else
			{
				foreach (var type in parameterTypes)
				{
					// TODO: think better naming of parameters
					parameterDefinitions.Add(new ParameterDefinition(type.Name.ToLowerInvariant(), type));
				}
			}

			return this.Translator.Runtime.EvaluateToLambdaExpression(argument, this.translator.Scope, parameterDefinitions);
		}

		private Expression TranslateGenericArgument(
			ExpressionNode argument, Type genericType, IDictionary<Type, Type> resolvedGenericArguments)
		{
			if (genericType.IsCompatibleWith<LambdaExpression>())
			{
				var funcType = genericType.GetGenericArguments()[0];
				var funcGenericArguments = funcType.GetGenericArguments();
				var allArgsExceptLast = funcGenericArguments.Take(funcGenericArguments.Length - 1);
				var resolvedParameterTypes = allArgsExceptLast.Select(arg => resolvedGenericArguments[arg]);

				return this.TranslateLambdaArgument(argument, resolvedParameterTypes);
			}

			var expression = this.Translate(argument);
			expression = PrometeExpression(expression, genericType);

			return expression;
		}

		private static Expression PrometeExpression(Expression expression, Type targetType)
		{
			if (targetType.IsCompatibleWith<IQueryable>() &&
				(expression.Type.IsCompatibleWith<IEnumerable>() && !expression.Type.IsCompatibleWith<IQueryable>()))
			{
				var elementType = expression.Type.FindGenericType(typeof(IEnumerable<>)).GetGenericArguments().First();
				return Expression.Call(typeof(Queryable), "AsQueryable", new[] { elementType }, expression);
			}

			return expression;
		}

		private static bool IsMethodCompatible(MethodInfo method, IList<ExpressionNode> args)
		{
			var parameters = method.GetParameters();
				
			if (parameters.Length != args.Count)
			{
				return false;
			}

			return
				args
				.Zip(parameters, AreArgumentAndParameterCompatible)
				.All(areCompatible => areCompatible);
		}

		private static bool IsMethodCompatibleWithArgumentTypes(MethodInfo method, IEnumerable<Type> argumentTypes)
		{
			var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
				
			if (parameterTypes.Length != argumentTypes.Count())
			{
				return false;
			}

			if (parameterTypes.Length == 0)
			{
				return true;
			}

			return
				argumentTypes
				.Zip(parameterTypes, TypeExtensions.IsCompatibleWith)
				.All(areCompatible => areCompatible);
		}

		private static bool AreArgumentAndParameterCompatible(ExpressionNode argument, ParameterInfo parameter)
		{
			if (argument.NodeType == ExpressionNodeType.Constant)
			{
				var constant = (ConstantExpressionNode)argument;
				return AreContantArgumentAndParameterCompatible(constant, parameter);
			}

			return true;
		}

		private static bool AreContantArgumentAndParameterCompatible(ConstantExpressionNode constant, ParameterInfo parameter)
		{
			if (constant.Value != null)
			{
				return constant.Value.GetType().IsCompatibleWith(parameter.ParameterType);
			}

			return parameter.ParameterType.IsNullableType() || !parameter.ParameterType.IsValueType;
		}

		private static IEnumerable<MethodInfo> FindInstanceMethods(Type type, string methodName)
		{
			var instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

			return
				type
				.FindMembers(MemberTypes.Method, instanceFlags, Type.FilterNameIgnoreCase, methodName)
				.Cast<MethodInfo>();
		}
	}
}
