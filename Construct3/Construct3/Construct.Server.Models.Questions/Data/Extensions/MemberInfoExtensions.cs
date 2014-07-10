using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

#if !WPF35
using System.ComponentModel.DataAnnotations;
#endif

namespace Telerik.Windows.Data
{
	internal static class MemberInfoExtensions
	{
#if !WPF45
		public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
		{
			return memberInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
		}
#endif

		public static string Description(this MemberInfo memberInfo)
		{
			var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>(false);
			return descriptionAttribute != null ? descriptionAttribute.Description : null;
		}

#if !WPF35
		public static string DisplayShortName(this MemberInfo memberInfo)
		{
			var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>(false);
			return displayAttribute != null ? displayAttribute.GetShortName() : null;
		}

		public static int? DisplayOrder(this MemberInfo memberInfo)
		{
			var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>(false);
			return displayAttribute != null ? displayAttribute.GetOrder() : null;
		}
#endif
	}
}
