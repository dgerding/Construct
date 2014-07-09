using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ServiceStack;
using ServiceStack.Text;
using System.Runtime.Serialization;

namespace SMFramework.Testing
{
	public interface TestProcedure
	{
		ProcedureOutput GetOutput(PairedDatabase dataSource);
	}

	public interface ProcedureOutput
	{
		String ToFormat(LabTest.ReportFormat format);

		/// <summary>
		/// ProcedureOutput objects are generally grouped by some category, i.e. the sensor that data was received
		/// from.
		/// </summary>
		String Category { get; set; }

		/// <summary>
		/// A label describing this specific output.
		/// </summary>
		String Label { get; set; }

		/// <summary>
		/// A label describing the type of output this object is. Generally used for identification of data types in
		/// data visualizations.
		/// </summary>
		String TypeLabel { get; set; }
	}

	/// <summary>
	/// Metrics regarding the angle specified by Direction and the angle between the specified Point and
	/// the Direction's RootPosition. i.e. angle between a subject's head and a point, in comparison to
	/// the angle of the subject's head orientation.
	/// </summary>
	public class DirectionToPointProcedure : TestProcedure
	{
		public class Output : ProcedureOutput
		{
			public double	AverageAngle { get; set; }
			public double	MinAngle { get; set; }
			public double	MaxAngle { get; set; }

			public double	StandardDeviation { get; set; }

			public double	FirstQuartile { get; set; }
			public double	SecondQuartile { get; set; }
			public double	ThirdQuartile { get; set; }



			public double MinAngleDegreesXZ { set; get; }
			public double FirstAngularDegreesXZ { get; set; }
			public double SecondAngularDegreesXZ { get; set; }
			public double ThirdAngularDegreesXZ { get; set; }
			public double MaxAngleDegreesXZ { get; set; }
			public double MeanAngleDegreesXZ { get; set; }
			public double StandardAngularDeviationDegreesXZ { get; set; }

			public double MinAngleDegreesYZ { set; get; }
			public double FirstAngularDegreesYZ { get; set; }
			public double SecondAngularDegreesYZ { get; set; }
			public double ThirdAngularDegreesYZ { get; set; }
			public double MaxAngleDegreesYZ { get; set; }
			public double MeanAngleDegreesYZ { get; set; }
			public double StandardAngularDeviationDegreesYZ { get; set; }

			public double MinAngleDegreesXY { set; get; }
			public double FirstAngularDegreesXY { get; set; }
			public double SecondAngularDegreesXY { get; set; }
			public double ThirdAngularDegreesXY { get; set; }
			public double MaxAngleDegreesXY { get; set; }
			public double MeanAngleDegreesXY { get; set; }
			public double StandardAngularDeviationDegreesXY { get; set; }

			public int GeneralQuality { get; set; }



			public String	Label { get; set; }
			public String	Category { get; set; }
			public String	TypeLabel { get; set; }

			public String ToFormat(LabTest.ReportFormat format)
			{
				switch (format)
				{
					case LabTest.ReportFormat.JSON:
						return JsonSerializer.SerializeToString(this);

					default:
						return "Invalid Format";
				}
			}
		}

		public LabDirection Direction;
		public LabPoint Point;

		public String Category = "Unknown";

		/// <summary>
		/// If assigned to an object, the procedure will ignore data that is determined to be "null". Comparisons
		/// are done against positional data
		/// </summary>
		public Vector3ValidityIdentifier PositionalNullIdentifier = null;

		/* Same as above, but for directions */
		public Vector3ValidityIdentifier DirectionalNullIdentifier = null;

		public ProcedureOutput GetOutput(PairedDatabase dataSource)
		{
			Analytics.FloatEvaluator angleEvaluator = new Analytics.FloatEvaluator();
			Analytics.FloatEvaluator xyEvaluator = new Analytics.FloatEvaluator();
			Analytics.FloatEvaluator yzEvaluator = new Analytics.FloatEvaluator();
			Analytics.FloatEvaluator xzEvaluator = new Analytics.FloatEvaluator();

			Output result = new Output();
			result.Label = "\"" + Direction.Label + "\" Direction to \"" + Point.Label + "\"";
			result.Category = Category;
			result.TypeLabel = "DirectionToPointProcedure";

			foreach (DataSnapshot snapshot in dataSource.PairingSnapshots)
			{
				Direction.InterpretData(snapshot);
				Point.InterpretData(snapshot);

				if (DirectionalNullIdentifier != null)
				{
					DirectionalNullIdentifier.Value = Direction.Direction;
					if (DirectionalNullIdentifier.IsNull())
						continue;
				}

				if (PositionalNullIdentifier != null)
				{
					PositionalNullIdentifier.Value = Direction.FirstPoint;
					if (PositionalNullIdentifier.IsNull())
						continue;

					PositionalNullIdentifier.Value = Direction.SecondPoint;
					if (PositionalNullIdentifier.IsNull())
						continue;

					PositionalNullIdentifier.Value = Point.Position;
					if (PositionalNullIdentifier.IsNull())
						continue;
				}

				Vector3 headToPointVector = Point.Position - Direction.FirstPoint;
				headToPointVector /= headToPointVector.Length();
				float dot;

				/* Generic */
				dot = Vector3.Dot(headToPointVector, Direction.DirectionNormal);
                dot = MathHelper.Clamp(dot, -1.0F, 1.0F);
				angleEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));

				/* XY */
				dot = Vector3.Dot(
					new Vector3(headToPointVector.X, headToPointVector.Y, 0.0F),
					new Vector3(Direction.DirectionNormal.X, Direction.DirectionNormal.Y, 0.0F)
					);
                dot = MathHelper.Clamp(dot, -1.0F, 1.0F);
				xyEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));

				/* YZ */
				dot = Vector3.Dot(
					new Vector3(0.0F, headToPointVector.Y, headToPointVector.Z),
					new Vector3(0.0F, Direction.DirectionNormal.Y, Direction.DirectionNormal.Z)
					);
                dot = MathHelper.Clamp(dot, -1.0F, 1.0F);
				yzEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));

				/* XZ */
				dot = Vector3.Dot(
					new Vector3(headToPointVector.X, 0.0F, headToPointVector.Z),
					new Vector3(Direction.DirectionNormal.X, 0.0F, Direction.DirectionNormal.Z)
					);
                MathHelper.Clamp(dot, -1.0F, 1.0F);
				xzEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));
			}

			#region Overall Base Analytics
			//result.GeneralQuality = ;

			result.AverageAngle = angleEvaluator.Mean;
			result.MinAngle = angleEvaluator.Min;
			result.FirstQuartile = angleEvaluator.FirstQuartile;
			result.SecondQuartile = angleEvaluator.SecondQuartile;
			result.ThirdQuartile = angleEvaluator.ThirdQuartile;
			result.MaxAngle = angleEvaluator.Max;
			result.StandardDeviation = angleEvaluator.StandardDeviation;
			#endregion

			#region XZ Analytics
			result.MinAngleDegreesXZ = xzEvaluator.Min;
			result.FirstAngularDegreesXZ = xzEvaluator.FirstQuartile;
			result.SecondAngularDegreesXZ = xzEvaluator.SecondQuartile;
			result.ThirdAngularDegreesXZ = xzEvaluator.ThirdQuartile;
			result.MaxAngleDegreesXZ = xzEvaluator.Max;
			result.MeanAngleDegreesXZ = xzEvaluator.Mean;
			result.StandardAngularDeviationDegreesXZ = xzEvaluator.StandardDeviation;
			#endregion

			#region YZ Analytics
			result.MinAngleDegreesYZ = yzEvaluator.Min;
			result.FirstAngularDegreesYZ = yzEvaluator.FirstQuartile;
			result.SecondAngularDegreesYZ = yzEvaluator.SecondQuartile;
			result.ThirdAngularDegreesYZ = yzEvaluator.ThirdQuartile;
			result.MaxAngleDegreesYZ = yzEvaluator.Max;
			result.MeanAngleDegreesYZ = yzEvaluator.Mean;
			result.StandardAngularDeviationDegreesYZ = yzEvaluator.StandardDeviation;
			#endregion

			#region XY Analytics
			result.MinAngleDegreesXY = xyEvaluator.Min;
			result.FirstAngularDegreesXY = xyEvaluator.FirstQuartile;
			result.SecondAngularDegreesXY = xyEvaluator.SecondQuartile;
			result.ThirdAngularDegreesXY = xyEvaluator.ThirdQuartile;
			result.MaxAngleDegreesXY = xyEvaluator.Max;
			result.MeanAngleDegreesXY = xyEvaluator.Mean;
			result.StandardAngularDeviationDegreesXY = xyEvaluator.StandardDeviation;
			#endregion

			return result;
		}
	}

	/// <summary>
	/// Metrics between the two specified directions. Metrics are only taken from the DirectionNormal of
	/// the LabDirection objects.
	/// </summary>
	public class AngleBetweenAngleProcedure : TestProcedure
	{
		public class Output : ProcedureOutput
		{
			public double	AverageAngle { get; set; }
			public double	MinAngle { get; set; }
			public double	MaxAngle { get; set; }

			public double	StandardDeviation { get; set; }

			public double	FirstQuartile { get; set; }
			public double	SecondQuartile { get; set; }
			public double	ThirdQuartile { get; set; }

			public String Label { get; set; }
			public String Category { get; set; }
			public String TypeLabel { get; set; }

			public String ToFormat(LabTest.ReportFormat format)
			{
				switch (format)
				{
					case LabTest.ReportFormat.JSON:
						return JsonSerializer.SerializeToString(this);

					default:
						return "Invalid Format";
				}
			}
		}

		public LabDirection FirstDirection;
		public LabDirection SecondDirection;
		public String Category = "Unknown";

		public Vector3ValidityIdentifier PositionalNullIdentifier = null;
		public Vector3ValidityIdentifier DirectionalNullIdentifier = null;

		public ProcedureOutput GetOutput(PairedDatabase dataSource)
		{
			Analytics.FloatEvaluator evaluator = new Analytics.FloatEvaluator();
			foreach (DataSnapshot snapshot in dataSource.PairingSnapshots)
			{
				FirstDirection.InterpretData(snapshot);
				SecondDirection.InterpretData(snapshot);

				if (PositionalNullIdentifier != null)
				{
					PositionalNullIdentifier.Value = FirstDirection.FirstPoint;
					if (PositionalNullIdentifier.IsNull())
						continue;

					PositionalNullIdentifier.Value = SecondDirection.FirstPoint;
					if (PositionalNullIdentifier.IsNull())
						continue;
				}

				if (DirectionalNullIdentifier != null)
				{
					DirectionalNullIdentifier.Value = FirstDirection.DirectionNormal;
					if (DirectionalNullIdentifier.IsNull())
						continue;

					DirectionalNullIdentifier.Value = SecondDirection.DirectionNormal;
					if (DirectionalNullIdentifier.IsNull())
						continue;
				}

				evaluator.AddFloat(Vector3.Dot(FirstDirection.DirectionNormal, SecondDirection.DirectionNormal));
			}

			Output output = new Output();

			output.FirstQuartile = evaluator.FirstQuartile;
			output.SecondQuartile = evaluator.SecondQuartile;
			output.ThirdQuartile = evaluator.ThirdQuartile;
			output.AverageAngle = evaluator.Mean;
			output.MaxAngle = evaluator.Max;
			output.MinAngle = evaluator.Min;
			output.StandardDeviation = evaluator.StandardDeviation;

			output.Label = "\"" + FirstDirection.Label + "\" Direction to \"" + SecondDirection.Label + "\" Direction";
			output.Category = Category;
			output.TypeLabel = "AngleBetweenAngleProcedure";

			return output;
		}
	}

	/// <summary>
	/// Metrics regarding the distance between two LabPoints.
	/// </summary>
	public class DistanceBetweenPointsProcedure : TestProcedure
	{
		public class Output : ProcedureOutput
		{
			public double	AverageDistance { get; set; }
			public double	MaxDistance { get; set; }
			public double	MinDistance { get; set; }
			public double	StandardDeviation { get; set; }

			public double	FirstQuartile { get; set; }
			public double	SecondQuartile { get; set; }
			public double	ThirdQuartile { get; set; }

			public String Label { get; set; }
			public String Category { get; set; }
			public String TypeLabel { get; set; }

			public String ToFormat(LabTest.ReportFormat format)
			{
				switch (format)
				{
					case LabTest.ReportFormat.JSON:
						return JsonSerializer.SerializeToString(this);

					default:
						return "";
				}
			}
		}

		public LabPoint FirstPoint;
		public LabPoint SecondPoint;
		public String Category = "Unknown";

		public Vector3ValidityIdentifier PositionalNullIdentifier = null;

		public ProcedureOutput GetOutput(PairedDatabase dataSource)
		{
			Analytics.FloatEvaluator evaluator = new Analytics.FloatEvaluator();
			
			foreach (DataSnapshot snapshot in dataSource.PairingSnapshots)
			{
				FirstPoint.InterpretData(snapshot);
				SecondPoint.InterpretData(snapshot);

				if (PositionalNullIdentifier != null)
				{
					PositionalNullIdentifier.Value = FirstPoint.Position;
					if (PositionalNullIdentifier.IsNull())
						continue;

					PositionalNullIdentifier.Value = SecondPoint.Position;
					if (PositionalNullIdentifier.IsNull())
						continue;
				}

				evaluator.AddFloat((FirstPoint.Position - SecondPoint.Position).Length());
			}

			Output output = new Output();
			output.AverageDistance = evaluator.Mean;
			output.MaxDistance = evaluator.Max;
			output.MinDistance = evaluator.Min;
			output.StandardDeviation = evaluator.StandardDeviation;
			output.FirstQuartile = evaluator.FirstQuartile;
			output.SecondQuartile = evaluator.SecondQuartile;
			output.ThirdQuartile = evaluator.ThirdQuartile;

			output.Label = "Distance Between \"" + FirstPoint.Label + "\" and \"" + SecondPoint.Label + "\"";
			output.Category = Category;
			output.TypeLabel = "DistanceBetweenPointsProcedure";

			return output;
		}
	}

	public class GenericVectorProcedure : TestProcedure
	{
		public class Output : ProcedureOutput
		{
			public double AverageMagnitude { get; set; }
			public Vector3 AverageVector { get; set; }
			public double StandardAngularDeviationDegrees { get; set; }

			/* All angular comparisons are done against the mean vector */

			public double MeanAngleDegrees { get; set; }

			public double MinAngleDegrees { set; get; }
			public double FirstAngularQuartileDegrees { get; set; }
			public double SecondAngularQuartileDegrees { get; set; }
			public double ThirdAngularQuartileDegrees { get; set; }
			public double MaxAngleDegrees { get; set; }

			public double MinAngleDegreesXZ { set; get; }
			public double FirstAngularDegreesXZ { get; set; }
			public double SecondAngularDegreesXZ { get; set; }
			public double ThirdAngularDegreesXZ { get; set; }
			public double MaxAngleDegreesXZ { get; set; }
			public double MeanAngleDegreesXZ { get; set; }
			public double StandardAngularDeviationDegreesXZ { get; set; }

			public double MinAngleDegreesYZ { set; get; }
			public double FirstAngularDegreesYZ { get; set; }
			public double SecondAngularDegreesYZ { get; set; }
			public double ThirdAngularDegreesYZ { get; set; }
			public double MaxAngleDegreesYZ { get; set; }
			public double MeanAngleDegreesYZ { get; set; }
			public double StandardAngularDeviationDegreesYZ { get; set; }

			public double MinAngleDegreesXY { set; get; }
			public double FirstAngularDegreesXY { get; set; }
			public double SecondAngularDegreesXY { get; set; }
			public double ThirdAngularDegreesXY { get; set; }
			public double MaxAngleDegreesXY { get; set; }
			public double MeanAngleDegreesXY { get; set; }
			public double StandardAngularDeviationDegreesXY { get; set; }

			public String Label { get; set; }
			public String Category { get; set; }
			public String TypeLabel { get; set; }

			public String ToFormat(LabTest.ReportFormat format)
			{
				switch (format)
				{
					case LabTest.ReportFormat.JSON:
						return JsonSerializer.SerializeToString(this);

					default:
						return "Invalid Format";
				}
			}
		}

		public LabDirection Direction;

		public String Category = "Unknown";

		public Vector3ValidityIdentifier DirectionalNullIdentifier = null;
		public Vector3ValidityIdentifier PositionalNullIdentifier = null;

		public ProcedureOutput GetOutput(PairedDatabase dataSource)
		{
			Analytics.FloatEvaluator angleEvaluator = new Analytics.FloatEvaluator();
			List<Vector3> validVectors = new List<Vector3>();
			Analytics.VectorEvaluator vectorEvaluator = new Analytics.VectorEvaluator();
			Output result = new Output();
			result.Label = "\"" + Direction.Label + "\" Generic Analytics";
			result.Category = Category;
			result.TypeLabel = "GenericVectorProcedure";

			foreach (DataSnapshot snapshot in dataSource.PairingSnapshots)
			{
				Direction.InterpretData(snapshot);

				if (DirectionalNullIdentifier != null)
				{
					DirectionalNullIdentifier.Value = Direction.Direction;
					if (DirectionalNullIdentifier.IsNull())
						continue;
				}

				if (PositionalNullIdentifier != null)
				{
					PositionalNullIdentifier.Value = Direction.FirstPoint;
					if (PositionalNullIdentifier.IsNull())
						continue;

					PositionalNullIdentifier.Value = Direction.SecondPoint;
					if (PositionalNullIdentifier.IsNull())
						continue;
				}

				validVectors.Add(Direction.Direction);
			}

			#region Overall Base Analytics
			vectorEvaluator.Clear();
			angleEvaluator.Clear();
			foreach (Vector3 vector in validVectors)
				vectorEvaluator.AddData(vector);

			Vector3 generalAverage = vectorEvaluator.Mean;
			Vector3 generalAverageNormal = generalAverage / generalAverage.Length();
			foreach (Vector3 vector in validVectors)
			{
				float dotProduct = Vector3.Dot(generalAverageNormal, vector / vector.Length());
                dotProduct = MathHelper.Clamp(dotProduct, -1.0F, 1.0F); // For floating-point inaccuracies

				angleEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dotProduct)));
			}

			result.AverageMagnitude = generalAverage.Length();
			result.AverageVector = generalAverage;
			result.MeanAngleDegrees = angleEvaluator.Mean;
			result.MinAngleDegrees = angleEvaluator.Min;
			result.FirstAngularQuartileDegrees = angleEvaluator.FirstQuartile;
			result.SecondAngularQuartileDegrees = angleEvaluator.SecondQuartile;
			result.ThirdAngularQuartileDegrees = angleEvaluator.ThirdQuartile;
			result.MaxAngleDegrees = angleEvaluator.Max;
			result.StandardAngularDeviationDegrees = angleEvaluator.StandardDeviation;
			#endregion

			#region XZ Analytics
			vectorEvaluator.Clear();
			angleEvaluator.Clear();

			foreach (Vector3 vector in validVectors)
				vectorEvaluator.AddData(new Vector3(vector.X, 0.0f, vector.Z));

			Vector3 xzAverageVector = vectorEvaluator.Mean;
			Vector3 xzAverageNormal = xzAverageVector / xzAverageVector.Length();
			foreach (Vector3 vector in validVectors)
			{
				float dot = Vector3.Dot(xzAverageNormal, vector / vector.Length());
				dot = Math.Max(-1.0F, dot);
				dot = Math.Min(1.0F, dot);
				angleEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));
			}

			result.MinAngleDegreesXZ = angleEvaluator.Min;
			result.FirstAngularDegreesXZ = angleEvaluator.FirstQuartile;
			result.SecondAngularDegreesXZ = angleEvaluator.SecondQuartile;
			result.ThirdAngularDegreesXZ = angleEvaluator.ThirdQuartile;
			result.MaxAngleDegreesXZ = angleEvaluator.Max;
			result.MeanAngleDegreesXZ = angleEvaluator.Mean;
			result.StandardAngularDeviationDegreesXZ = angleEvaluator.StandardDeviation;
			#endregion

			#region YZ Analytics
			vectorEvaluator.Clear();
			angleEvaluator.Clear();

			foreach (Vector3 vector in validVectors)
				vectorEvaluator.AddData(new Vector3(0.0F, vector.Y, vector.Z));

			Vector3 yzAverageVector = vectorEvaluator.Mean;
			Vector3 yzAverageNormal = yzAverageVector / yzAverageVector.Length();
			foreach (Vector3 vector in validVectors)
			{
				float dot = Vector3.Dot(yzAverageNormal, vector / vector.Length());
				dot = Math.Max(-1.0F, dot);
				dot = Math.Min(1.0F, dot);
				angleEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));
			}

			result.MinAngleDegreesYZ = angleEvaluator.Min;
			result.FirstAngularDegreesYZ = angleEvaluator.FirstQuartile;
			result.SecondAngularDegreesYZ = angleEvaluator.SecondQuartile;
			result.ThirdAngularDegreesYZ = angleEvaluator.ThirdQuartile;
			result.MaxAngleDegreesYZ = angleEvaluator.Max;
			result.MeanAngleDegreesYZ = angleEvaluator.Mean;
			result.StandardAngularDeviationDegreesYZ = angleEvaluator.StandardDeviation;
			#endregion

			#region XY Analytics
						vectorEvaluator.Clear();
			angleEvaluator.Clear();

			foreach (Vector3 vector in validVectors)
				vectorEvaluator.AddData(new Vector3(vector.X, vector.Y, 0.0F));

			Vector3 xyAverageVector = vectorEvaluator.Mean;
			Vector3 xyAverageNormal = xyAverageVector / xyAverageVector.Length();
			foreach (Vector3 vector in validVectors)
			{
				float dot = Vector3.Dot(xyAverageNormal, vector / vector.Length());
				dot = Math.Max(-1.0F, dot);
				dot = Math.Min(1.0F, dot);
				angleEvaluator.AddFloat(MathHelper.ToDegrees((float)Math.Acos(dot)));
			}

			result.MinAngleDegreesXY = angleEvaluator.Min;
			result.FirstAngularDegreesXY = angleEvaluator.FirstQuartile;
			result.SecondAngularDegreesXY = angleEvaluator.SecondQuartile;
			result.ThirdAngularDegreesXY = angleEvaluator.ThirdQuartile;
			result.MaxAngleDegreesXY = angleEvaluator.Max;
			result.MeanAngleDegreesXY = angleEvaluator.Mean;
			result.StandardAngularDeviationDegreesXY = angleEvaluator.StandardDeviation;
			#endregion

			return result;
		}
	}

	/// <summary>
	/// Based on how often the data drops off (value becomes nulled), score in the range of [0, 1]
	/// </summary>
	public class DataReliabilityProcedure : TestProcedure
	{
		public class Output : ProcedureOutput
		{
			public float Reliability { get; set; }
			public int NonNullSnapshots { get; set; }
			public int NullSnapshots { get; set; }
			public int TotalSnapshotsCount { get; set; }

			public String Label { get; set; }
			public String Category { get; set; }
			public String TypeLabel { get; set; }

			public String ToFormat(LabTest.ReportFormat format)
			{
				switch (format)
				{
					case LabTest.ReportFormat.JSON:
						return JsonSerializer.SerializeToString(this);

					default:
						return "";
				}
			}
		}

		public ValidityIdentifier NullIdentifier = null;
		public String DataLabel = "Unspecified Type";
		public String Category = "Unknown";

		public FaceLabSignalConfiguration Signal = null;

		public ProcedureOutput GetOutput(PairedDatabase dataSource)
		{
			if (NullIdentifier == null)
				throw new Exception("Cannot get output without a NullIdentifier object.");

			int nonNullCount = 0;

			String interpretedLabel = Signal.Label + " " + DataLabel;

			foreach (DataSnapshot snapshot in dataSource.PairingSnapshots)
			{
				NullIdentifier.SetFromSnapshot(interpretedLabel, snapshot);
				if (!NullIdentifier.IsNull())
					nonNullCount++;
			}

			Output output = new Output();

			output.NonNullSnapshots = nonNullCount;
			output.NullSnapshots = dataSource.NumberOfSnapshots - nonNullCount;
			output.TotalSnapshotsCount = dataSource.NumberOfSnapshots;
			output.Reliability = (float)output.NonNullSnapshots / (float)output.TotalSnapshotsCount;

			output.Label = "Reliability of " + DataLabel;
			output.Category = Category;
			output.TypeLabel = "DataReliabilityProcedure";

			return output;
		}
	}
}