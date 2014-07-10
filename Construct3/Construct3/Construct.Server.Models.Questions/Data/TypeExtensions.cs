using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace Telerik.Windows.Data
{
    internal static class TypeExtensions
    {
#if SILVERLIGHT || WPF45
        internal static bool IsCustomTypeProvider(this Type type)
        {
            if (type != null && type.IsCompatibleWith(typeof(System.Reflection.ICustomTypeProvider)))
            {
                return true;
            }
            return false;
        }
#endif

#if SILVERLIGHT || WPF40
        internal static bool IsDynamicMetaObjectProvider(this Type type)
        {
            if (type != null && type.IsCompatibleWith(typeof(System.Dynamic.IDynamicMetaObjectProvider)))
            {
                return true;
            }
            return false;
        }
#endif

        internal static readonly Type[] PredefinedTypes = 
        {
            typeof(object),
            typeof(bool),
            typeof(char),
            typeof(string),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };

        internal static bool IsPredefinedType(this Type type)
        {
            foreach (Type t in PredefinedTypes)
            {
                if (t == type)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        internal static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        internal static string GetTypeName(this Type type)
        {
            Type baseType = GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

        internal static bool IsDynamic(this Type type)
		{
#if WPF40 || SILVERLIGHT
			return type.IsCompatibleWith<System.Dynamic.IDynamicMetaObjectProvider>();
#else
            return false;
#endif
		}

        internal static bool IsNumericType(this Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        internal static bool IsSignedIntegralType(this Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        internal static bool IsUnsignedIntegralType(this Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        internal static int GetNumericTypeKind(this Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

		internal static PropertyInfo GetIndexerPropertyInfo(this Type type, params Type[] indexerArguments)
		{
			return GetIndexerPropertyInfo(type, (IEnumerable<Type>)indexerArguments);
		}

		internal static PropertyInfo GetIndexerPropertyInfo(this Type type, IEnumerable<Type> indexerArguments)
        {
			// We will lookup first the generic interface implementations, in order to have 
			// richer type information in scenarios like IList and IList<T>
			var implementedInterfacesProperties =
				type.GetInterfaces().OrderBy(i => !i.IsGenericType).SelectMany(i => i.GetProperties());

            return
				(from p in type.GetProperties().Concat(implementedInterfacesProperties)
                 where AreArgumentsApplicable(indexerArguments, p.GetIndexParameters())
                 select p).FirstOrDefault();
        }

        private static bool AreArgumentsApplicable(IEnumerable<Type> arguments, IEnumerable<ParameterInfo> parameters)
        {
            var argumentList = arguments.ToList();
            var parameterList = parameters.ToList();

            if (argumentList.Count != parameterList.Count)
            {
                return false;
            }

            for (int i = 0; i < argumentList.Count; i++)
            {
                if (parameterList[i].ParameterType != argumentList[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsEnumType(this Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

		internal static bool IsCompatibleWith<TTargetType>(this Type source)
		{
			return source.IsCompatibleWith(typeof(TTargetType));
		}

        internal static bool IsCompatibleWith(this Type source, Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = source.GetNonNullableType();
            Type tt = target.GetNonNullableType();
            if (st != source && tt == target) return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        internal static Type FindGenericType(this Type type, Type genericType)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType) return type;
                if (genericType.IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = intfType.FindGenericType(genericType);
                        if (found != null) return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        internal static object DefaultValue(this Type type)
        {
            if (type != null && type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        internal static MemberInfo FindPropertyOrField(this Type type, string memberName)
        {
            MemberInfo memberInfo = type.FindPropertyOrField(memberName, false);
            
            if (memberInfo == null)
            {
                memberInfo = type.FindPropertyOrField(memberName, true);
            }

            return memberInfo;
        }

        internal static MemberInfo FindPropertyOrField(this Type type, string memberName, bool staticAccess)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in type.SelfAndBaseTypes())
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName);

                if (members.Length != 0)
                {
                    return members[0];
                }
            }
            return null;
        }

        internal static IEnumerable<Type> SelfAndBaseTypes(this Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();

                AddInterface(types, type);

                return types;
            }
            return SelfAndBaseClasses(type);
        }

        internal static IEnumerable<Type> SelfAndBaseClasses(this Type type)
        {
            while (type != null)
            {
                yield return type;

                type = type.BaseType;
            }
        }

        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);

                foreach (Type t in type.GetInterfaces())
                {
                    AddInterface(types, t);
                }
            }
        }

        internal static bool IsIComparable(this Type source)
        {
            if (source != null)
            {
                return typeof(IComparable).IsAssignableFrom(source) || typeof(IComparable<>).MakeGenericType(source).IsAssignableFrom(source);
            }

            return false;
        }

        internal static bool IsIEquatable(this Type source)
        {
            if (source != null)
            {
                return typeof(IEquatable<>).MakeGenericType(source).IsAssignableFrom(source);
            }

            return false;
        }

        internal static bool CanSort(this Type source)
        {
            if (source == null) return false;

            Type typeToCheck = Nullable.GetUnderlyingType(source) ?? source;

            return typeToCheck.IsIComparable();
        }

        internal static bool CanGroup(this Type source)
        {
            if (source == null) return false;

            Type typeToCheck = Nullable.GetUnderlyingType(source) ?? source;

            return typeToCheck.IsIEquatable() || typeToCheck.IsIComparable();
        }

        internal static bool CanFilter(this Type source)
        {
            if (source == null) return false;

            Type typeToCheck = Nullable.GetUnderlyingType(source) ?? source;

            return typeToCheck.IsIEquatable() || typeToCheck.IsIComparable();
        }

		internal static IEnumerable<FilterOperator> ApplicableFilterOperators(this Type sourceType)
		{
			if (sourceType == null)
			{
				return Enumerable.Empty<FilterOperator>();
			}

			var result = new List<FilterOperator>();
			
			result.Add(FilterOperator.IsEqualTo);
			result.Add(FilterOperator.IsNotEqualTo);

			var nonNullableType = sourceType.GetNonNullableType();

			var isString = nonNullableType == typeof(string);
			if (isString)
			{
				result.Add(FilterOperator.StartsWith);
				result.Add(FilterOperator.EndsWith);
				result.Add(FilterOperator.Contains);
                result.Add(FilterOperator.DoesNotContain);
				result.Add(FilterOperator.IsContainedIn);
				result.Add(FilterOperator.IsNotContainedIn);
				result.Add(FilterOperator.IsEmpty);
				result.Add(FilterOperator.IsNotEmpty);
			}

			var isNumericType = nonNullableType.IsNumericType();
			
			var methods = nonNullableType.GetMethods().Where(mi => mi.ReturnType == typeof(bool));

			if (isString || isNumericType || methods.Any(mi => mi.Name == "op_LessThan"))
			{
				result.Add(FilterOperator.IsLessThan);
			}

			if (isString || isNumericType || methods.Any(mi => mi.Name == "op_LessThanOrEqual"))
			{
				result.Add(FilterOperator.IsLessThanOrEqualTo);
			}

			if (isString || isNumericType || methods.Any(mi => mi.Name == "op_GreaterThan"))
			{
				result.Add(FilterOperator.IsGreaterThan);
			}

			if (isString || isNumericType || methods.Any(mi => mi.Name == "op_GreaterThanOrEqual"))
			{
				result.Add(FilterOperator.IsGreaterThanOrEqualTo);
			}

			if (sourceType.CanBeTestedForNull())
			{
				result.Add(FilterOperator.IsNull);
				result.Add(FilterOperator.IsNotNull);
			}

			return result;
		}

		internal static bool IsPrimitiveOrValueType(this Type type)
		{
			return type.IsPrimitive || type.IsValueType;
		}

		internal static bool CanBeTestedForNull(this Type type)
		{
			return !type.IsValueType || type.IsNullableType();
		}

		internal static TypeConverter GetTypeConverter(this Type source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");				
			}
			
			TypeConverter converter = null;
#if WPF
            converter = TypeDescriptor.GetConverter(source);
#else
			
			var attribute = source.GetCustomAttribute<TypeConverterAttribute>(true);
			if (attribute != null && !string.IsNullOrEmpty(attribute.ConverterTypeName))
			{
				converter = Activator.CreateInstance(Type.GetType(attribute.ConverterTypeName)) as TypeConverter;
			}
#endif
			return converter;
		}

		internal static bool TryConvert(object value, Type type, out object returnValue)
		{
			try
			{
				var sourceType = value.GetType();
				var typeConverter = type.GetTypeConverter();
				if (typeConverter != null && typeConverter.CanConvertFrom(sourceType))
				{
					returnValue = typeConverter.ConvertFrom(value);
					return true;
				}

#if SILVERLIGHT
				if (value is string && typeof(Enum).IsAssignableFrom(type))
				{
					returnValue = Enum.Parse(type, (string)value, true);
					return true;
				}
#endif

				returnValue = System.Convert.ChangeType(value, type.GetNonNullableType(), CultureInfo.CurrentCulture);
				return true;
			}
			catch (Exception)
			{
				returnValue = null;
				return false;
			}
		}
	}
}