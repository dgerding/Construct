using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMFramework;
using System;

namespace SMFrameworkTests
{
	[TestClass]
	public class FaceDataTest
	{
		[TestMethod]
		public void ParseCoordinateSystem()
		{
			String localToString = FaceData.CoordinateSystemType.Local.ToString();
			String globalToString = FaceData.CoordinateSystemType.Global.ToString();

			FaceData.CoordinateSystemType expectedLocalDirect = FaceData.ParseCoordinateSystem(localToString);
			FaceData.CoordinateSystemType expectedGlobalDirect = FaceData.ParseCoordinateSystem(globalToString);

			FaceData.CoordinateSystemType expectedLocalManual = FaceData.ParseCoordinateSystem("Local");
			FaceData.CoordinateSystemType expectedGlobalManual = FaceData.ParseCoordinateSystem("Global");

			Assert.AreEqual(expectedLocalDirect, FaceData.CoordinateSystemType.Local);
			Assert.AreEqual(expectedGlobalDirect, FaceData.CoordinateSystemType.Global);

			Assert.AreEqual(expectedLocalManual, FaceData.CoordinateSystemType.Local);
			Assert.AreEqual(expectedGlobalManual, FaceData.CoordinateSystemType.Global);
		}

		[TestMethod, ExpectedException(typeof(Exception))]
		public void TryFailCoordinateSystem()
		{
			FaceData.ParseCoordinateSystem("nope");
		}

		[TestMethod]
		public void FaceDataEqualsOverride()
		{
			FaceData first, second;
			first = new FaceData();
			second = new FaceData();

			Assert.AreEqual(first, second);

			first.FaceLabFrameIndex = 1903992;
			first.LeftEyeClosure = 0.2945F;
			Assert.AreNotEqual(first, second);
		}
	}
}
