using System;

namespace SMFramework
{
	public class FaceDataSerialization
	{
		public static FaceData ReadFaceDataFromSnapshot(DataSnapshot source, String sensorLabel)
		{
			FaceData result = new FaceData();

			result.SignalLabel					= sensorLabel;
			result.RightEyePosition				= source.ComposeVector3(sensorLabel + " RightEye");
			result.LeftEyePosition				= source.ComposeVector3(sensorLabel + " LeftEye");

			result.RightPupilPosition			= source.ComposeVector3(sensorLabel + " RightPupil");
			result.LeftPupilPosition			= source.ComposeVector3(sensorLabel + " LeftPupil");

			result.LeftPupilDiameter			= source.ComposeFloat(sensorLabel + " LeftPupilDiameter");
			result.RightPupilDiameter			= source.ComposeFloat(sensorLabel + " RightPupilDiameter");

			result.RightEyeClosure				= source.ComposeFloat(sensorLabel + " RightEyeClosure");
			result.RightEyeClosureConfidence	= source.ComposeFloat(sensorLabel + " RightEyeClosureConfidence");
			result.LeftEyeClosure				= source.ComposeFloat(sensorLabel + " LeftEyeClosure");
			result.LeftEyeClosureConfidence		= source.ComposeFloat(sensorLabel + " LeftEyeClosureConfidence");

			result.LeftEyeGazeQualityLevel		= source.ComposeFloat(sensorLabel + " LeftEyeGazeQualityLevel");
			result.RightEyeGazeQualityLevel		= source.ComposeFloat(sensorLabel + " RightEyeGazeQualityLevel");

			result.LeftEyeGazeRotation			= source.ComposeVector2(sensorLabel + " LeftEyeGazeRotation");
			result.RightEyeGazeRotation			= source.ComposeVector2(sensorLabel + " RightEyeGazeRotation");

			result.VergencePosition				= source.ComposeVector3(sensorLabel + " Vergence");

			result.FaceLabFrameIndex			= (uint)source.ComposeFloat(sensorLabel + " FaceLabFrameIndex");

			result.HeadPosition					= source.ComposeVector3(sensorLabel + " Head");
			result.HeadPositionConfidence		= source.ComposeFloat(sensorLabel + " HeadPositionConfidence");
			result.HeadRotation					= source.ComposeVector3(sensorLabel + " HeadRotation");

			result.FaceBoundsUVs				= source.ComposeVector2List(sensorLabel + " FaceBoundsUVs");
			result.FaceBoundsVertices			= source.ComposeVector3List(sensorLabel + " FaceBoundsVertex");
			result.LeftEyebrowVertices			= source.ComposeVector3List(sensorLabel + " LeftEyebrowVertex");
			result.RightEyebrowVertices			= source.ComposeVector3List(sensorLabel + " RightEyebrowVertex");

			result.MouthInnerLowerLipVertices	= source.ComposeVector3List(sensorLabel + " MouthInnerLowerLipVertex");
			result.MouthOuterLowerLipVertices	= source.ComposeVector3List(sensorLabel + " MouthOuterLowerLipVertices");
			result.MouthInnerUpperLipVertices	= source.ComposeVector3List(sensorLabel + " MouthInnerUpperLipVertices");
			result.MouthOuterUpperLipVertices	= source.ComposeVector3List(sensorLabel + " MouthOuterUpperLipVertices");

			return result;
		}

		public static void WriteFaceDataToSnapshot(FaceData data, DataSnapshot target)
		{
			target.Write(data.SignalLabel + " RightEye", data.RightEyePosition);
			target.Write(data.SignalLabel + " LeftEye", data.LeftEyePosition);

			target.Write(data.SignalLabel + " RightPupil", data.RightPupilPosition);
			target.Write(data.SignalLabel + " LeftPupil", data.LeftPupilPosition);

			target.Write(data.SignalLabel + " LeftPupilDiameter", data.RightPupilDiameter);
			target.Write(data.SignalLabel + " RightPupilDiameter", data.LeftPupilDiameter);

			target.Write(data.SignalLabel + " RightEyeClosure", data.RightEyeClosure);
			target.Write(data.SignalLabel + " RightEyeClosureConfidence", data.RightEyeClosureConfidence);
			target.Write(data.SignalLabel + " LeftEyeClosure", data.LeftEyeClosure);
			target.Write(data.SignalLabel + " LeftEyeClosureConfidence", data.LeftEyeClosureConfidence);

			target.Write(data.SignalLabel + " LeftEyeGazeQualityLevel", data.LeftEyeGazeQualityLevel);
			target.Write(data.SignalLabel + " RightEyeGazeQualityLevel", data.RightEyeGazeQualityLevel);

			target.Write(data.SignalLabel + " LeftEyeGazeRotation", data.LeftEyeGazeRotation);
			target.Write(data.SignalLabel + " RightEyeGazeRotation", data.RightEyeGazeRotation);

			target.Write(data.SignalLabel + " Vergence", data.VergencePosition);

			target.Write(data.SignalLabel + " FaceLabFrameIndex", data.FaceLabFrameIndex);

			target.Write(data.SignalLabel + " Head", data.HeadPosition);
			target.Write(data.SignalLabel + " HeadPositionConfidence", data.HeadPositionConfidence);
			target.Write(data.SignalLabel + " HeadRotation", data.HeadRotation);

			if (data.LeftEyebrowVertices != null)
				target.Write(data.SignalLabel + " LeftEyebrowVertex", data.LeftEyebrowVertices);
			if (data.RightEyebrowVertices != null)
				target.Write(data.SignalLabel + " RightEyebrowVertex", data.RightEyebrowVertices);

			if (data.MouthOuterUpperLipVertices != null)
			{
				target.Write(data.SignalLabel + " MouthOuterUpperLipVertex", data.MouthOuterUpperLipVertices);
				target.Write(data.SignalLabel + " MouthInnerUpperLipVertex", data.MouthInnerUpperLipVertices);
				target.Write(data.SignalLabel + " MouthInnerLowerLipVertex", data.MouthInnerLowerLipVertices);
				target.Write(data.SignalLabel + " MouthOuterLowerLipVertex", data.MouthOuterLowerLipVertices);
			}

			if (data.FaceBoundsVertices != null)
				target.Write(data.SignalLabel + " FaceBoundsVertex", data.FaceBoundsVertices);

			if (data.FaceBoundsUVs != null)
				target.Write(data.SignalLabel + " FaceBoundsUVs", data.FaceBoundsUVs);
		}

		public static void WriteFaceDataToDatabase(FaceData data, FaceLabDataStream target)
		{
			DataSnapshot targetSnapshot = target.CurrentSnapshot;
			WriteFaceDataToSnapshot(data, targetSnapshot);
		}
	}
}
