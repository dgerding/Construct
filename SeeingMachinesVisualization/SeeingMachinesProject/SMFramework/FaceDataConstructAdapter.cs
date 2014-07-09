using Microsoft.Xna.Framework;
using System;
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

		public int LandmarkCount { get; set; }
		public bool WearingGlasses { get; set; }
		public bool IsBlinking { get; set; }

		public String CoordinateSystem { get; set; }
	}
}
