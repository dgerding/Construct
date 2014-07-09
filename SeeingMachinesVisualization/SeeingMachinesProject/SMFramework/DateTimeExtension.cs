using System;

namespace SMFramework
{
	public static class DateTimeExtension
	{
		public static String ToStringUTC(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
		}
	}
}
