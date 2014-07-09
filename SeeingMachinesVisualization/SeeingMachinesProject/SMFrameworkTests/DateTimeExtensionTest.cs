using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMFramework;
using System;

namespace SMFrameworkTests
{
	[TestClass]
	public class DateTimeExtensionTest
	{
		[TestMethod]
		public void TestToStringUTC()
		{
			//	Year 5, Month 10, Day 15, Hour 20, Minute 30, Second 50, Millisecond 500
			DateTime baseTime = new DateTime(5, 10, 15, 20, 30, 50, 500, DateTimeKind.Utc);
			String utcBaseTimeString = baseTime.ToStringUTC();
			String expectedString = "0005-10-15T20:30:50.500Z";

			Assert.AreEqual(
				utcBaseTimeString,
				expectedString
				);

			DateTime interpretedTime = DateTime.Parse(utcBaseTimeString).ToUniversalTime();
			Assert.AreEqual(interpretedTime, baseTime);
		}
	}
}
