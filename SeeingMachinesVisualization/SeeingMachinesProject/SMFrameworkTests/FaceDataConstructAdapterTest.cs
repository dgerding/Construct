using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using SMFramework;
using System;

namespace SMFrameworkTests
{
	[TestClass]
	public class FaceDataConstructAdapterTest
	{
		Random m_Random = new Random((int)DateTime.Now.Ticks);

		Vector2 Random2()
		{
			return new Vector2(m_Random.Next() * 100.0F, m_Random.Next() * 100.0F);
		}

		Vector3 Random3()
		{
			return new Vector3(m_Random.Next() * 100.0F, m_Random.Next() * 100.0F, m_Random.Next() * 100.0F);
		}

		[TestMethod]
		public void ConstructAdapterBidirectionalTest()
		{
			FaceData sourceData = new FaceData();
			sourceData.HeadPosition = Random3();
			sourceData.CoordinateSystem = FaceData.CoordinateSystemType.Global;
			sourceData.FaceLabFrameIndex = 500000;
			sourceData.HeadPositionConfidence = 0.5F;
			sourceData.HeadRotation = Random3();
			sourceData.LeftEyeClosure = 0.2F;
			sourceData.LeftEyeClosureConfidence = 0.7F;
			sourceData.LeftEyeGazeQualityLevel = 60.0F;
			sourceData.LeftEyeGazeRotation = Random2();
			sourceData.LeftEyePosition = Random3();
			sourceData.LeftPupilDiameter = 2.66F;
			sourceData.LeftPupilPosition = Random3();
			sourceData.RightEyeClosure = 0.1114F;
			sourceData.RightEyeClosureConfidence = 5.0122F;
			sourceData.RightEyeGazeQualityLevel = 7.204F;
			sourceData.RightEyeGazeRotation = Random2();
			sourceData.RightEyePosition = Random3();
			sourceData.RightPupilDiameter = 193.0F;
			sourceData.RightPupilPosition = Random3();
			sourceData.SignalLabel = "ASDASDAS";
			sourceData.SnapshotTimestamp = new DateTime(200, 10, 20);
			sourceData.VergencePosition = Random3();

			FaceDataConstructAdapter adapter = new FaceDataConstructAdapter(sourceData);
			FaceData constructedData = adapter.ToFaceData();

			Assert.AreEqual(sourceData, constructedData);
		}
	}
}
