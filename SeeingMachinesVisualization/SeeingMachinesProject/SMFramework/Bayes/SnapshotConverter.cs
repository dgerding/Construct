using Microsoft.Xna.Framework;
using SMFramework.Testing;
using System;
using System.Collections.Generic;

/*
 * Hosts various LabProcedures to generate Bayes/Accord-compatible metrics from a DataSnapshot.
 */

namespace SMFramework.Bayes
{
	public interface SnapshotConverter
	{
		double[] GenerateFromSnapshot(DataSnapshot source);

		int ClassCount
		{
			get;
		}
	}

	public class SingleDirectConverter : SnapshotConverter
	{
		public int ClassCount
		{
			get { return 34; }
		}

		public String SourceSignalLabel;

        public SingleDirectConverter()
        {

        }

        public SingleDirectConverter(String sourceSignalLabel)
        {
            SourceSignalLabel = sourceSignalLabel;
        }

		public double[] GenerateFromSnapshot(DataSnapshot source)
		{
			double[] result = new double[ClassCount];

			if (!source.ContainsKeyContaining(SourceSignalLabel))
				return result;

			FaceData faceData = FaceDataSerialization.ReadFaceDataFromSnapshot(source, SourceSignalLabel);
			result[ 0] = faceData.HeadPosition.X;
			result[ 1] = faceData.HeadPosition.Y;
			result[ 2] = faceData.HeadPosition.Z;
			result[ 3] = faceData.HeadPositionConfidence;
			result[ 4] = faceData.HeadRotation.X;
			result[ 5] = faceData.HeadRotation.Y;
			result[ 6] = faceData.HeadRotation.Z;
			result[ 7] = faceData.LeftEyeClosure;
			result[ 8] = faceData.LeftEyeClosureConfidence;
			result[ 9] = faceData.LeftEyeGazeQualityLevel;
			result[10] = faceData.LeftEyeGazeRotation.X;
			result[11] = faceData.LeftEyeGazeRotation.Y;
			result[12] = faceData.LeftEyePosition.X;
			result[13] = faceData.LeftEyePosition.Y;
			result[14] = faceData.LeftEyePosition.Z;
			result[15] = faceData.LeftPupilDiameter;
			result[16] = faceData.LeftPupilPosition.X;
			result[17] = faceData.LeftPupilPosition.Y;
			result[18] = faceData.LeftPupilPosition.Z;
			result[19] = faceData.RightEyeClosure;
			result[20] = faceData.RightEyeClosureConfidence;
			result[21] = faceData.RightEyeGazeQualityLevel;
			result[22] = faceData.RightEyeGazeRotation.X;
			result[23] = faceData.RightEyeGazeRotation.Y;
			result[24] = faceData.RightEyePosition.X;
			result[25] = faceData.RightEyePosition.Y;
			result[26] = faceData.RightEyePosition.Z;
			result[27] = faceData.RightPupilDiameter;
			result[28] = faceData.RightPupilPosition.X;
			result[29] = faceData.RightPupilPosition.Y;
			result[30] = faceData.RightPupilPosition.Z;
			result[31] = faceData.VergencePosition.X;
			result[32] = faceData.VergencePosition.Y;
			result[33] = faceData.VergencePosition.Z;

			return result;
		}
	}

    public struct AngularComparator
    {
        public LabDirection FirstDirection, SecondDirection;

		public bool IsValid
		{
			get
			{
				return FirstDirection.IsValid && SecondDirection.IsValid;
			}
		}

        public double Angle
        {
            get
            {
                float dot = Vector3.Dot(FirstDirection.DirectionNormal, SecondDirection.DirectionNormal);
                dot = MathHelper.Clamp(dot, -1.0F, 1.0F);
				double result = Math.Acos(dot);

				if (double.IsNaN(result))
					return -1.0;

				return result;
            }
        }
    }

    public class AngularConverter : SnapshotConverter
    {
        public List<AngularComparator> AngularDataSources
        {
            get;
            private set;
        }

        public AngularConverter()
        {
            AngularDataSources = new List<AngularComparator>();
        }

		public int ClassCount
		{
			get { return AngularDataSources.Count; }
		}

        public double[] GenerateFromSnapshot(DataSnapshot source)
        {
			double[] results = new double[ClassCount];

            for (int i = 0; i < AngularDataSources.Count; i++)
            {
				AngularComparator comparator = AngularDataSources[i];
                comparator.FirstDirection.InterpretData(source);
                comparator.SecondDirection.InterpretData(source);

				if (comparator.IsValid)
					results[i] = comparator.Angle;
            }

			return results;
        }
    }
}
