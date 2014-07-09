using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMFramework;
using System;

namespace SMFrameworkTests
{
	[TestClass]
	public class SetupDebugOutputStream
	{
		[AssemblyInitialize]
		public static void Initialize(TestContext test)
		{
			RandomString.g_Random = new Random((int)DateTime.Now.Ticks);
			DebugOutputStream.SlowInstance = new ExceptionOutputStream();
		}
	}
}
