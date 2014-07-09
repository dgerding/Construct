using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMFramework.Testing;
using Microsoft.Xna.Framework;
using System.IO;

namespace SMFramework.Analytics
{
	static class Utility
	{
		public static void AddCommonProcedures(TestStage target, FaceLabSignalConfiguration signal)
		{
			AddGenericOrientationProcedure(target, signal);
			AddOrientationReliabilityProcedure(target, signal);
			AddPositionalReliabilityProcedure(target, signal);
			AddVergenceReliabilityProcedure(target, signal);
			AddVergenceAnalyticsProcedures(target, signal);
		}

		public static void AddGenericOrientationProcedure(TestStage target, FaceLabSignalConfiguration signal)
		{
			GenericVectorProcedure genericProcedure = new GenericVectorProcedure();
			genericProcedure.Direction = new HeadOrientationDirection(signal.Label);
			genericProcedure.PositionalNullIdentifier = new Vector3ValidityIdentifier(SeeingModule.SensorOrigin(signal));
			genericProcedure.Category = signal.Label;
			target.AddProcedure(genericProcedure);
		}

		public static void AddOrientationReliabilityProcedure(TestStage target, FaceLabSignalConfiguration signal)
		{
			DataReliabilityProcedure directionReliabilityProcedure = new DataReliabilityProcedure();
			directionReliabilityProcedure.Category = signal.Label;
			directionReliabilityProcedure.Signal = signal;
			directionReliabilityProcedure.DataLabel = "HeadRotation";
			directionReliabilityProcedure.NullIdentifier = new Vector3ValidityIdentifier(Vector3.Zero);
			target.AddProcedure(directionReliabilityProcedure);
		}

		public static void AddPositionalReliabilityProcedure(TestStage target, FaceLabSignalConfiguration signal)
		{
			DataReliabilityProcedure positionalReliabilityProcedure = new DataReliabilityProcedure();
			positionalReliabilityProcedure.Category = signal.Label;
			positionalReliabilityProcedure.Signal = signal;
			positionalReliabilityProcedure.DataLabel = "Head";
			positionalReliabilityProcedure.NullIdentifier = new Vector3ValidityIdentifier(Vector3.Zero);
			target.AddProcedure(positionalReliabilityProcedure);
		}

		public static void AddVergenceAnalyticsProcedures(TestStage target, FaceLabSignalConfiguration signal)
		{
			GenericVectorProcedure genericProcedure = new GenericVectorProcedure();
			genericProcedure.Category = signal.Label;
			genericProcedure.Direction = new VergenceDirection(signal.Label);
			genericProcedure.PositionalNullIdentifier = new Vector3ValidityIdentifier(SeeingModule.SensorOrigin(signal));
			target.AddProcedure(genericProcedure);
		}

		public static void AddVergenceReliabilityProcedure(TestStage target, FaceLabSignalConfiguration signal)
		{
			DataReliabilityProcedure vergenceReliability = new DataReliabilityProcedure();
			vergenceReliability.Signal = signal;
			vergenceReliability.DataLabel = "Vergence";
			vergenceReliability.Category = signal.Label;
			//	When there is no vergence data, the signal's origin is reported as the vergence position.
			vergenceReliability.NullIdentifier = new Vector3ValidityIdentifier(SeeingModule.SensorOrigin(signal));
			target.AddProcedure(vergenceReliability);
		}

		public static void AddVergenceTargetAnalyticsProcedure(TestStage target, FaceLabSignalConfiguration signal, Vector3 targetPoint)
		{
			DirectionToPointProcedure directionProcedure = new DirectionToPointProcedure();
			directionProcedure.Category = signal.Label;
			directionProcedure.Direction = new VergenceDirection(signal.Label);
			directionProcedure.Point = new FixedPoint(targetPoint);
			directionProcedure.PositionalNullIdentifier = new Vector3ValidityIdentifier(SeeingModule.SensorOrigin(signal));
			target.AddProcedure(directionProcedure);
		}

		public static TestStage CreateStage(LabTest target, FaceLabSignalConfiguration signal, String fileRoot, String stageLabel, String fileSuffix, StandardQualityProtocol.GenerationType mode)
		{
			switch (mode)
			{
				case StandardQualityProtocol.GenerationType.FromFile:
					return target.CreateStage(
						stageLabel,
						new FileStreamCollectionSource(Path.Combine(fileRoot, signal.Label + fileSuffix + ".csv"), FaceData.CoordinateSystemType.Global)
						);

				case StandardQualityProtocol.GenerationType.FromSignalStream:
					return target.CreateStage(
						stageLabel,
						new TimedCollectionSource(30.0F, new PairedDatabase(), Path.Combine(fileRoot, signal.Label + fileSuffix))
						);
			}

			throw new Exception();
		}
	}

	public static class StandardQualityProtocol
	{
		public static String[] StageDescriptions
		{
			get
			{
				List<String> description = new List<String>();

				description.Add("The subject will have their head oriented straight (no yaw, pitch, or roll) while sitting in front of the sensor. Vergence data is not collected, only head data. This will be maintained for 30 seconds.");

				description.Add("Keeping their head oriented straight ahead, the subject will stare at the far right table corner for 30 seconds.");
				description.Add("Still keeping their head straight, the subject will stare at the far left table corner for 30 seconds.");

				description.Add("Place Spike on the opposite side of the table, pressed up against the table and centered. The subject will stare at Spike's head for 30 seconds. The subject's head should be oriented towards spike.");
				description.Add("Keeping Spike in the same position, have the subject rotate their head slightly (approx. 20 degrees) to the right and have them stare at Spike's head for another 30 seconds.");
				description.Add("Keeping Spike in the same position, the subject will instead rotate their head slightly (approx. 20 degrees) to the left and continue to stare at Spike's head for 30 seconds.");

				description.Add("Spike will be placed on the right side of the table, being pressed up against the table's edge and centered. Have the subject keep their head oriented straight but stare at Spike's head for 30 seconds.");
				description.Add("Keeping Spike on the right, the subject will orient their head towards Spike and continue to stare for 30 seconds.");

				description.Add("Place Spike on the far left side of the table, pressed against the table's edge and centered. Have the subject orient their head straight and stare at Spike's head for 30 seconds.");
				description.Add("Keeping Spike on the left, have the subject orient their head towards Spike and stare for 30 seconds.");

				return description.ToArray();
			}
		}

		public enum GenerationType
		{
			FromFile,
			FromSignalStream
		}

		public static void GenerateStagesWithProcedures(
								String referenceFolder,
								Testing.LabTest target,
								FaceLabSignalConfiguration signal,
								GenerationType mode)
		{
			TestStage currentStage;


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Head Oriented Straight", "-headstraight", mode);
			Utility.AddGenericOrientationProcedure(currentStage, signal);
			Utility.AddOrientationReliabilityProcedure(currentStage, signal);
			Utility.AddPositionalReliabilityProcedure(currentStage, signal);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Head Straight, Looking at the Far Right Table Corner", "-headstraightlookright", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(.608F, .73F, -.608F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Head Straight, Looking at the Far Left Table Corner", "-headstraightlookleft", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(-.608F, .73F, -.608F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Opposite Side, Head Straight", "-spikestraightheadstraight", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(0.0F, 1.155F, -.608F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Opposite Side, Head Slightly Turned Right", "-spikestraightheadright", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(0.0F, 1.155F, -.608F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Opposite Side, Head Slightly Turned Left", "-spikestraightheadleft", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(0.0F, 1.155F, -.608F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Right, Head Straight", "-spikerightheadstraight", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(.608F, 1.155F, 0.0F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Right, Head Oriented Toward Spike", "-spikerightheadright", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(.608F, 1.155F, 0.0F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Left, Head Straight", "-spikeleftheadstraight", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(-.608F, 1.155F, 0.0F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);


			currentStage = Utility.CreateStage(target, signal, referenceFolder,
				"Spike on the Left, Head Oriented Toward Spike", "-spikeleftheadleft", mode);
			Utility.AddCommonProcedures(currentStage, signal);
			Utility.AddVergenceTargetAnalyticsProcedure(currentStage, signal,
				Vector3.Transform(
					new Vector3(-.608F, 1.155F, 0.0F),
					Matrix.CreateRotationY(MathHelper.ToRadians(signal.GlobalRotationDegrees.Y)))
				);
		}
	}
}
