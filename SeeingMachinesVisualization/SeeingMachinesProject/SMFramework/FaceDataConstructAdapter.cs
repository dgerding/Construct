using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMFramework
{
	public class FaceDataConstructAdapter
	{
		public FaceDataConstructAdapter()
		{

		}

		public FaceDataConstructAdapter(FaceData source)
		{
			SignalLabel = source.SignalLabel;

			AutoFillFlattenedVector("HeadPosition", source.HeadPosition);
			AutoFillFlattenedVector("HeadRotation", source.HeadRotation);
			AutoFillFlattenedVector("LeftEyePosition", source.LeftEyePosition);
			AutoFillFlattenedVector("RightEyePosition", source.RightEyePosition);
			AutoFillFlattenedVector("LeftPupilPosition", source.LeftPupilPosition);
			AutoFillFlattenedVector("RightPupilPosition", source.RightPupilPosition);
			AutoFillFlattenedVector("VergencePosition", source.VergencePosition);

			AutoFillFlattenedVector("LeftEyeGazeRotation", source.LeftEyeGazeRotation);
			AutoFillFlattenedVector("RightEyeGazeRotation", source.RightEyeGazeRotation);

			HeadPositionConfidence = source.HeadPositionConfidence;

			LeftPupilDiameter = source.LeftPupilDiameter;
			RightPupilDiameter = source.RightPupilDiameter;

			LeftEyeGazeQualityLevel = source.LeftEyeGazeQualityLevel;
			RightEyeGazeQualityLevel = source.RightEyeGazeQualityLevel;

			FaceLabFrameIndex = (int)source.FaceLabFrameIndex;

			LeftEyeClosure = source.LeftEyeClosure;
			RightEyeClosure = source.RightEyeClosure;
			LeftEyeClosureConfidence = source.LeftEyeClosureConfidence;
			RightEyeClosureConfidence = source.RightEyeClosureConfidence;

			IsBlinking = source.IsBlinking;
			WearingGlasses = source.WearingGlasses;
			LandmarkCount = (int)source.LandmarkCount;

			NumMouthInnerLowerLipVertices = source.MouthInnerLowerLipVertices == null ? 0 : source.MouthInnerLowerLipVertices.Count;
			NumMouthOuterLowerLipVertices = source.MouthOuterLowerLipVertices == null ? 0 : source.MouthOuterLowerLipVertices.Count;
			NumMouthInnerUpperLipVertices = source.MouthInnerUpperLipVertices == null ? 0 : source.MouthInnerUpperLipVertices.Count;
			NumMouthOuterUpperLipVertices = source.MouthOuterUpperLipVertices == null ? 0 : source.MouthOuterUpperLipVertices.Count;
			NumLeftEyebrowVertices = source.LeftEyebrowVertices == null ? 0 : source.LeftEyebrowVertices.Count;
			NumRightEyebrowVertices = source.RightEyebrowVertices == null ? 0 : source.RightEyebrowVertices.Count;
			NumFaceBoundsVertices = source.FaceBoundsVertices == null ? 0 : source.FaceBoundsVertices.Count;


			AutoFillFlattenedVectorList("MouthInnerLowerLipVertex", 5, source.MouthInnerLowerLipVertices);
			AutoFillFlattenedVectorList("MouthOuterLowerLipVertex", 5, source.MouthOuterLowerLipVertices);
			AutoFillFlattenedVectorList("MouthInnerUpperLipVertex", 5, source.MouthInnerUpperLipVertices);
			AutoFillFlattenedVectorList("MouthOuterUpperLipVertex", 5, source.MouthOuterUpperLipVertices);
			AutoFillFlattenedVectorList("LeftEyebrowVertex", 3, source.LeftEyebrowVertices);
			AutoFillFlattenedVectorList("RightEyebrowVertex", 3, source.RightEyebrowVertices);
			AutoFillFlattenedVectorList("FaceBoundsVertex", 25, source.FaceBoundsVertices);
			AutoFillFlattenedVectorList("FaceBoundsUV", 25, source.FaceBoundsUVs);



			CoordinateSystem = source.CoordinateSystem.ToString();
		}

		private void AutoFillFlattenedVector(String vectorBaseName, Vector2 source)
		{
			var adapterType = this.GetType();

			PropertyInfo x = adapterType.GetProperty(vectorBaseName + "X");
			PropertyInfo y = adapterType.GetProperty(vectorBaseName + "Y");

			x.SetValue(this, source.X);
			y.SetValue(this, source.Y);
		}

		private void AutoFillFlattenedVector(String vectorBaseName, Vector3 source)
		{
			var adapterType = this.GetType();

			PropertyInfo x = adapterType.GetProperty(vectorBaseName + "X");
			PropertyInfo y = adapterType.GetProperty(vectorBaseName + "Y");
			PropertyInfo z = adapterType.GetProperty(vectorBaseName + "Z");

			x.SetValue(this, source.X);
			y.SetValue(this, source.Y);
			z.SetValue(this, source.Z);
		}

		private void AutoFillFlattenedVectorList(String listBaseName, int maxMembers, IEnumerable<Vector3> source)
		{
			int vectorIndex = 1;
			if (source != null)
			{
				foreach (Vector3 vector in source)
					AutoFillFlattenedVector(listBaseName + vectorIndex++, vector);
			}

			while (vectorIndex <= maxMembers)
				AutoFillFlattenedVector(listBaseName + vectorIndex++, Vector3.Zero);
		}

		private void AutoFillFlattenedVectorList(String listBaseName, int maxMembers, IEnumerable<Vector2> source)
		{
			int vectorIndex = 1;
			if (source != null)
			{
				foreach (Vector2 vector in source)
					AutoFillFlattenedVector(listBaseName + vectorIndex++, vector);
			}

			while (vectorIndex < maxMembers)
				AutoFillFlattenedVector(listBaseName + vectorIndex++, Vector2.Zero);
		}

		public FaceData ToFaceData()
		{
			FaceData result = new FaceData();

			result.SignalLabel = SignalLabel;

			result.HeadPosition = new Vector3((float)HeadPositionX, (float)HeadPositionY, (float)HeadPositionZ);
			result.HeadRotation = new Vector3((float)HeadRotationX, (float)HeadRotationY, (float)HeadRotationZ);

			result.LeftEyePosition = new Vector3((float)LeftEyePositionX, (float)LeftEyePositionY, (float)LeftEyePositionZ);
			result.RightEyePosition = new Vector3((float)RightEyePositionX, (float)RightEyePositionY, (float)RightEyePositionZ);

			result.LeftPupilPosition = new Vector3((float)LeftPupilPositionX, (float)LeftPupilPositionY, (float)LeftPupilPositionZ);
			result.RightPupilPosition = new Vector3((float)RightPupilPositionX, (float)RightPupilPositionY, (float)RightPupilPositionZ);

			result.VergencePosition = new Vector3((float)VergencePositionX, (float)VergencePositionY, (float)VergencePositionZ);
			result.LeftEyeGazeRotation = new Vector2((float)LeftEyeGazeRotationX, (float)LeftEyeGazeRotationY);
			result.RightEyeGazeRotation = new Vector2((float)RightEyeGazeRotationX, (float)RightEyeGazeRotationY);

			result.LeftEyeGazeQualityLevel = (float)LeftEyeGazeQualityLevel;
			result.RightEyeGazeQualityLevel = (float)RightEyeGazeQualityLevel;

			//	WARNING: ConstructAdapter has no timestamp, the timestamp should be stored from the original FaceData (if
			//		we held it here as well we'd end up with redundant data)
			result.SnapshotTimestamp = new DateTime();
			result.HeadPositionConfidence = (float)HeadPositionConfidence;

			result.LeftPupilDiameter = (float)LeftPupilDiameter;
			result.RightPupilDiameter = (float)RightPupilDiameter;

			result.FaceLabFrameIndex = (uint)FaceLabFrameIndex;

			result.LeftEyeClosure = (float)LeftEyeClosure;
			result.RightEyeClosure = (float)RightEyeClosure;

			result.RightEyeClosureConfidence = (float)RightEyeClosureConfidence;
			result.LeftEyeClosureConfidence = (float)LeftEyeClosureConfidence;

			result.CoordinateSystem = FaceData.ParseCoordinateSystem(CoordinateSystem);

			result.LandmarkCount = (uint)LandmarkCount;
			result.WearingGlasses = WearingGlasses;
			result.IsBlinking = IsBlinking;

			return result;
		}

		public String SignalLabel { get; set; }

		public double HeadPositionX { get; set; }
		public double HeadPositionY { get; set; }
		public double HeadPositionZ { get; set; }

		public double HeadRotationX { get; set; }
		public double HeadRotationY { get; set; }
		public double HeadRotationZ { get; set; }

		public double LeftEyePositionX { get; set; }
		public double LeftEyePositionY { get; set; }
		public double LeftEyePositionZ { get; set; }

		public double RightEyePositionX { get; set; }
		public double RightEyePositionY { get; set; }
		public double RightEyePositionZ { get; set; }

		public double LeftPupilPositionX { get; set; }
		public double LeftPupilPositionY { get; set; }
		public double LeftPupilPositionZ { get; set; }

		public double RightPupilPositionX { get; set; }
		public double RightPupilPositionY { get; set; }
		public double RightPupilPositionZ { get; set; }

		public double VergencePositionX { get; set; }
		public double VergencePositionY { get; set; }
		public double VergencePositionZ { get; set; }

		public double LeftEyeGazeRotationX { get; set; }
		public double LeftEyeGazeRotationY { get; set; }

		public double RightEyeGazeRotationX { get; set; }
		public double RightEyeGazeRotationY { get; set; }

		public double LeftEyeGazeQualityLevel { get; set; }
		public double RightEyeGazeQualityLevel { get; set; }

		public double HeadPositionConfidence { get; set; }

		public double LeftPupilDiameter { get; set; }
		public double RightPupilDiameter { get; set; }

		public int FaceLabFrameIndex { get; set; }

		public double LeftEyeClosure { get; set; }
		public double RightEyeClosure { get; set; }

		public double LeftEyeClosureConfidence { get; set; }
		public double RightEyeClosureConfidence { get; set; }

		public int NumMouthInnerLowerLipVertices { get; set; }
		public int NumMouthOuterLowerLipVertices { get; set; }
		public int NumMouthInnerUpperLipVertices { get; set; }
		public int NumMouthOuterUpperLipVertices { get; set; }
		public int NumLeftEyebrowVertices { get; set; }
		public int NumRightEyebrowVertices { get; set; }
		public int NumFaceBoundsVertices { get; set; }

		public double MouthInnerLowerLipVertex1X { get; set; }
		public double MouthInnerLowerLipVertex1Y { get; set; }
		public double MouthInnerLowerLipVertex1Z { get; set; }

		public double MouthInnerLowerLipVertex2X { get; set; }
		public double MouthInnerLowerLipVertex2Y { get; set; }
		public double MouthInnerLowerLipVertex2Z { get; set; }

		public double MouthInnerLowerLipVertex3X { get; set; }
		public double MouthInnerLowerLipVertex3Y { get; set; }
		public double MouthInnerLowerLipVertex3Z { get; set; }

		public double MouthInnerLowerLipVertex4X { get; set; }
		public double MouthInnerLowerLipVertex4Y { get; set; }
		public double MouthInnerLowerLipVertex4Z { get; set; }

		public double MouthInnerLowerLipVertex5X { get; set; }
		public double MouthInnerLowerLipVertex5Y { get; set; }
		public double MouthInnerLowerLipVertex5Z { get; set; }


		public double MouthOuterLowerLipVertex1X { get; set; }
		public double MouthOuterLowerLipVertex1Y { get; set; }
		public double MouthOuterLowerLipVertex1Z { get; set; }

		public double MouthOuterLowerLipVertex2X { get; set; }
		public double MouthOuterLowerLipVertex2Y { get; set; }
		public double MouthOuterLowerLipVertex2Z { get; set; }

		public double MouthOuterLowerLipVertex3X { get; set; }
		public double MouthOuterLowerLipVertex3Y { get; set; }
		public double MouthOuterLowerLipVertex3Z { get; set; }

		public double MouthOuterLowerLipVertex4X { get; set; }
		public double MouthOuterLowerLipVertex4Y { get; set; }
		public double MouthOuterLowerLipVertex4Z { get; set; }

		public double MouthOuterLowerLipVertex5X { get; set; }
		public double MouthOuterLowerLipVertex5Y { get; set; }
		public double MouthOuterLowerLipVertex5Z { get; set; }


		public double MouthInnerUpperLipVertex1X { get; set; }
		public double MouthInnerUpperLipVertex1Y { get; set; }
		public double MouthInnerUpperLipVertex1Z { get; set; }

		public double MouthInnerUpperLipVertex2X { get; set; }
		public double MouthInnerUpperLipVertex2Y { get; set; }
		public double MouthInnerUpperLipVertex2Z { get; set; }

		public double MouthInnerUpperLipVertex3X { get; set; }
		public double MouthInnerUpperLipVertex3Y { get; set; }
		public double MouthInnerUpperLipVertex3Z { get; set; }

		public double MouthInnerUpperLipVertex4X { get; set; }
		public double MouthInnerUpperLipVertex4Y { get; set; }
		public double MouthInnerUpperLipVertex4Z { get; set; }

		public double MouthInnerUpperLipVertex5X { get; set; }
		public double MouthInnerUpperLipVertex5Y { get; set; }
		public double MouthInnerUpperLipVertex5Z { get; set; }


		public double MouthOuterUpperLipVertex1X { get; set; }
		public double MouthOuterUpperLipVertex1Y { get; set; }
		public double MouthOuterUpperLipVertex1Z { get; set; }

		public double MouthOuterUpperLipVertex2X { get; set; }
		public double MouthOuterUpperLipVertex2Y { get; set; }
		public double MouthOuterUpperLipVertex2Z { get; set; }

		public double MouthOuterUpperLipVertex3X { get; set; }
		public double MouthOuterUpperLipVertex3Y { get; set; }
		public double MouthOuterUpperLipVertex3Z { get; set; }

		public double MouthOuterUpperLipVertex4X { get; set; }
		public double MouthOuterUpperLipVertex4Y { get; set; }
		public double MouthOuterUpperLipVertex4Z { get; set; }

		public double MouthOuterUpperLipVertex5X { get; set; }
		public double MouthOuterUpperLipVertex5Y { get; set; }
		public double MouthOuterUpperLipVertex5Z { get; set; }


		public double LeftEyebrowVertex1X { get; set; }
		public double LeftEyebrowVertex1Y { get; set; }
		public double LeftEyebrowVertex1Z { get; set; }

		public double LeftEyebrowVertex2X { get; set; }
		public double LeftEyebrowVertex2Y { get; set; }
		public double LeftEyebrowVertex2Z { get; set; }

		public double LeftEyebrowVertex3X { get; set; }
		public double LeftEyebrowVertex3Y { get; set; }
		public double LeftEyebrowVertex3Z { get; set; }


		public double RightEyebrowVertex1X { get; set; }
		public double RightEyebrowVertex1Y { get; set; }
		public double RightEyebrowVertex1Z { get; set; }

		public double RightEyebrowVertex2X { get; set; }
		public double RightEyebrowVertex2Y { get; set; }
		public double RightEyebrowVertex2Z { get; set; }

		public double RightEyebrowVertex3X { get; set; }
		public double RightEyebrowVertex3Y { get; set; }
		public double RightEyebrowVertex3Z { get; set; }


		public double FaceBoundsVertex1X { get; set; }
		public double FaceBoundsVertex1Y { get; set; }
		public double FaceBoundsVertex1Z { get; set; }

		public double FaceBoundsVertex2X { get; set; }
		public double FaceBoundsVertex2Y { get; set; }
		public double FaceBoundsVertex2Z { get; set; }

		public double FaceBoundsVertex3X { get; set; }
		public double FaceBoundsVertex3Y { get; set; }
		public double FaceBoundsVertex3Z { get; set; }

		public double FaceBoundsVertex4X { get; set; }
		public double FaceBoundsVertex4Y { get; set; }
		public double FaceBoundsVertex4Z { get; set; }

		public double FaceBoundsVertex5X { get; set; }
		public double FaceBoundsVertex5Y { get; set; }
		public double FaceBoundsVertex5Z { get; set; }

		public double FaceBoundsVertex6X { get; set; }
		public double FaceBoundsVertex6Y { get; set; }
		public double FaceBoundsVertex6Z { get; set; }

		public double FaceBoundsVertex7X { get; set; }
		public double FaceBoundsVertex7Y { get; set; }
		public double FaceBoundsVertex7Z { get; set; }

		public double FaceBoundsVertex8X { get; set; }
		public double FaceBoundsVertex8Y { get; set; }
		public double FaceBoundsVertex8Z { get; set; }

		public double FaceBoundsVertex9X { get; set; }
		public double FaceBoundsVertex9Y { get; set; }
		public double FaceBoundsVertex9Z { get; set; }

		public double FaceBoundsVertex10X { get; set; }
		public double FaceBoundsVertex10Y { get; set; }
		public double FaceBoundsVertex10Z { get; set; }

		public double FaceBoundsVertex11X { get; set; }
		public double FaceBoundsVertex11Y { get; set; }
		public double FaceBoundsVertex11Z { get; set; }

		public double FaceBoundsVertex12X { get; set; }
		public double FaceBoundsVertex12Y { get; set; }
		public double FaceBoundsVertex12Z { get; set; }

		public double FaceBoundsVertex13X { get; set; }
		public double FaceBoundsVertex13Y { get; set; }
		public double FaceBoundsVertex13Z { get; set; }

		public double FaceBoundsVertex14X { get; set; }
		public double FaceBoundsVertex14Y { get; set; }
		public double FaceBoundsVertex14Z { get; set; }

		public double FaceBoundsVertex15X { get; set; }
		public double FaceBoundsVertex15Y { get; set; }
		public double FaceBoundsVertex15Z { get; set; }

		public double FaceBoundsVertex16X { get; set; }
		public double FaceBoundsVertex16Y { get; set; }
		public double FaceBoundsVertex16Z { get; set; }

		public double FaceBoundsVertex17X { get; set; }
		public double FaceBoundsVertex17Y { get; set; }
		public double FaceBoundsVertex17Z { get; set; }

		public double FaceBoundsVertex18X { get; set; }
		public double FaceBoundsVertex18Y { get; set; }
		public double FaceBoundsVertex18Z { get; set; }

		public double FaceBoundsVertex19X { get; set; }
		public double FaceBoundsVertex19Y { get; set; }
		public double FaceBoundsVertex19Z { get; set; }

		public double FaceBoundsVertex20X { get; set; }
		public double FaceBoundsVertex20Y { get; set; }
		public double FaceBoundsVertex20Z { get; set; }

		public double FaceBoundsVertex21X { get; set; }
		public double FaceBoundsVertex21Y { get; set; }
		public double FaceBoundsVertex21Z { get; set; }

		public double FaceBoundsVertex22X { get; set; }
		public double FaceBoundsVertex22Y { get; set; }
		public double FaceBoundsVertex22Z { get; set; }

		public double FaceBoundsVertex23X { get; set; }
		public double FaceBoundsVertex23Y { get; set; }
		public double FaceBoundsVertex23Z { get; set; }

		public double FaceBoundsVertex24X { get; set; }
		public double FaceBoundsVertex24Y { get; set; }
		public double FaceBoundsVertex24Z { get; set; }

		public double FaceBoundsVertex25X { get; set; }
		public double FaceBoundsVertex25Y { get; set; }
		public double FaceBoundsVertex25Z { get; set; }


		public double FaceBoundsUV1X { get; set; }
		public double FaceBoundsUV1Y { get; set; }

		public double FaceBoundsUV2X { get; set; }
		public double FaceBoundsUV2Y { get; set; }

		public double FaceBoundsUV3X { get; set; }
		public double FaceBoundsUV3Y { get; set; }

		public double FaceBoundsUV4X { get; set; }
		public double FaceBoundsUV4Y { get; set; }

		public double FaceBoundsUV5X { get; set; }
		public double FaceBoundsUV5Y { get; set; }

		public double FaceBoundsUV6X { get; set; }
		public double FaceBoundsUV6Y { get; set; }

		public double FaceBoundsUV7X { get; set; }
		public double FaceBoundsUV7Y { get; set; }

		public double FaceBoundsUV8X { get; set; }
		public double FaceBoundsUV8Y { get; set; }

		public double FaceBoundsUV9X { get; set; }
		public double FaceBoundsUV9Y { get; set; }

		public double FaceBoundsUV10X { get; set; }
		public double FaceBoundsUV10Y { get; set; }

		public double FaceBoundsUV11X { get; set; }
		public double FaceBoundsUV11Y { get; set; }

		public double FaceBoundsUV12X { get; set; }
		public double FaceBoundsUV12Y { get; set; }

		public double FaceBoundsUV13X { get; set; }
		public double FaceBoundsUV13Y { get; set; }

		public double FaceBoundsUV14X { get; set; }
		public double FaceBoundsUV14Y { get; set; }

		public double FaceBoundsUV15X { get; set; }
		public double FaceBoundsUV15Y { get; set; }

		public double FaceBoundsUV16X { get; set; }
		public double FaceBoundsUV16Y { get; set; }

		public double FaceBoundsUV17X { get; set; }
		public double FaceBoundsUV17Y { get; set; }

		public double FaceBoundsUV18X { get; set; }
		public double FaceBoundsUV18Y { get; set; }

		public double FaceBoundsUV19X { get; set; }
		public double FaceBoundsUV19Y { get; set; }

		public double FaceBoundsUV20X { get; set; }
		public double FaceBoundsUV20Y { get; set; }

		public double FaceBoundsUV21X { get; set; }
		public double FaceBoundsUV21Y { get; set; }

		public double FaceBoundsUV22X { get; set; }
		public double FaceBoundsUV22Y { get; set; }

		public double FaceBoundsUV23X { get; set; }
		public double FaceBoundsUV23Y { get; set; }

		public double FaceBoundsUV24X { get; set; }
		public double FaceBoundsUV24Y { get; set; }

		public double FaceBoundsUV25X { get; set; }
		public double FaceBoundsUV25Y { get; set; }



		public int LandmarkCount { get; set; }
		public bool WearingGlasses { get; set; }
		public bool IsBlinking { get; set; }

		public String CoordinateSystem { get; set; }
	}
}
