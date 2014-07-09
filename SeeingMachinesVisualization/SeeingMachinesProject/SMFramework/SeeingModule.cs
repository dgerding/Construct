using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMFramework
{
	public struct PersonOrientation
	{
		public FaceData DataSource
		{
			get;
			internal set;
		}

		/// <summary>
		/// Position of the center of the head in world space.
		/// </summary>
		public Vector3 WorldSpacePosition;

		/// <summary>
		/// Position of the forehead; directly between the eyes. (Data as reported from the Seeing Machines sensors)
		/// </summary>
		public Vector3 WorldSpaceForeheadPosition
		{
			get
			{
				return WorldSpacePosition + Vector3.Transform(new Vector3(0.0F, 0.0f, -Size.Z / 2.0F), Matrix.CreateRotationY(RotationEuler.Y));
			}

			set
			{
				WorldSpacePosition = value + Vector3.Transform(new Vector3(0.0F, 0.0F, Size.Z / 2.0F), Matrix.CreateRotationY(RotationEuler.Y));
			}
		}

		/// <summary>
		/// Size of the head
		/// </summary>
		public Vector3 Size;

		/// <summary>
		/// Rotation of the head about its center. (Behind the forehead position)
		/// </summary>
		public Vector3 RotationEuler;

		public Vector3 LeftEyePosition;
		public Vector3 RightEyePosition;
		public Vector3 LeftPupilPosition;
		public Vector3 RightPupilPosition;
		public Vector2 RightEyeGaze;
		public Vector2 LeftEyeGaze;
		public Vector3 VergencePoint;
	}

	/// <summary>
	/// Uses information from a FaceLabSensorConfiguration to transform and visualize data from
	/// a FaceData object to real-world values.
	/// </summary>
	public class SeeingModule
	{
		PersonOrientation m_Person = new PersonOrientation();
		public PersonOrientation Person
		{
			get { return m_Person; }
		}

		FaceData m_LastFaceData = new FaceData();
		public FaceData LastFaceData
		{
			get { return m_LastFaceData; }
		}

		public String SignalLabel
		{
			get;
			private set;
		}

		public FaceLabSignalConfiguration SensorConfiguration
		{
			get;
			private set;
		}

		public SeeingModule(FaceLabSignalConfiguration configuration)
		{
			m_Person.Size = new Vector3(0.16F, 0.23F, 0.20f) * Units.Meter;
			SensorConfiguration = configuration;

			//	Start off with default data
			UseDirectCameraData(m_LastFaceData);
		}

		public static Vector3 SensorOrigin(FaceLabSignalConfiguration configuration)
		{
			Vector3 globalRotationRadians = new Vector3(
					MathHelper.ToRadians(configuration.GlobalRotationDegrees.X),
					MathHelper.ToRadians(configuration.GlobalRotationDegrees.Y),
					MathHelper.ToRadians(configuration.GlobalRotationDegrees.Z)
					);

			Vector3 localRotationRadians = new Vector3(
				MathHelper.ToRadians(configuration.LocalRotationDegrees.X),
				MathHelper.ToRadians(configuration.LocalRotationDegrees.Y),
				MathHelper.ToRadians(configuration.LocalRotationDegrees.Z)
				);

			Matrix cameraMatrixAffine =
				Matrix.CreateRotationY(localRotationRadians.Y) *
				Matrix.CreateTranslation(configuration.LocalTranslation) *
				Matrix.CreateRotationY(globalRotationRadians.Y) *
				Matrix.CreateTranslation(configuration.GlobalTranslation);

			return Vector3.Transform(Vector3.Zero, cameraMatrixAffine);
		}

		public static FaceData EvaluateCameraData(FaceLabSignalConfiguration configuration, FaceData data, bool useInverseMatrix)
		{
			FaceData result = new FaceData();

			result.SignalLabel = configuration.Label;

			result.LeftEyeGazeQualityLevel	= data.LeftEyeGazeQualityLevel;
			result.RightEyeGazeQualityLevel = data.RightEyeGazeQualityLevel;
			result.LeftEyeClosure			= data.LeftEyeClosure;
			result.RightEyeClosure			= data.RightEyeClosure;
			result.LeftEyeClosureConfidence = data.LeftEyeClosureConfidence;
			result.RightEyeClosureConfidence= data.RightEyeClosureConfidence;
			result.SnapshotTimestamp		= data.SnapshotTimestamp;
			result.HeadPositionConfidence	= data.HeadPositionConfidence;
			result.LeftPupilDiameter		= data.LeftPupilDiameter;
			result.RightPupilDiameter		= data.RightPupilDiameter;
			result.FaceLabFrameIndex		= data.FaceLabFrameIndex;

			result.FaceBoundsUVs				= data.FaceBoundsUVs;
			result.LeftEyebrowVertices			= data.LeftEyebrowVertices;
			result.RightEyebrowVertices			= data.RightEyebrowVertices;
			result.MouthOuterUpperLipVertices	= data.MouthOuterUpperLipVertices;
			result.MouthInnerUpperLipVertices	= data.MouthInnerUpperLipVertices;
			result.MouthInnerLowerLipVertices	= data.MouthInnerLowerLipVertices;
			result.MouthOuterLowerLipVertices	= data.MouthOuterLowerLipVertices;

			result.FaceTextureData = data.FaceTextureData;

			if (data.CoordinateSystem == FaceData.CoordinateSystemType.Local)
			{
				Vector3 globalRotationRadians = new Vector3(
					MathHelper.ToRadians(configuration.GlobalRotationDegrees.X),
					MathHelper.ToRadians(configuration.GlobalRotationDegrees.Y),
					MathHelper.ToRadians(configuration.GlobalRotationDegrees.Z)
					);

				Vector3 localRotationRadians = new Vector3(
					MathHelper.ToRadians(configuration.LocalRotationDegrees.X),
					MathHelper.ToRadians(configuration.LocalRotationDegrees.Y),
					MathHelper.ToRadians(configuration.LocalRotationDegrees.Z)
					);

				Matrix cameraMatrixAffine =
					Matrix.CreateRotationX(localRotationRadians.X) *
					Matrix.CreateRotationY(localRotationRadians.Y) *
					Matrix.CreateTranslation(configuration.LocalTranslation) *
					Matrix.CreateRotationY(globalRotationRadians.Y) *
					Matrix.CreateTranslation(configuration.GlobalTranslation);

				if (useInverseMatrix)
					cameraMatrixAffine = Matrix.Invert(cameraMatrixAffine);

				result.HeadPosition			= Vector3.Transform(data.HeadPosition,			cameraMatrixAffine);
				result.LeftPupilPosition	= Vector3.Transform(data.LeftPupilPosition,		cameraMatrixAffine);
				result.RightPupilPosition	= Vector3.Transform(data.RightPupilPosition,	cameraMatrixAffine);
				result.VergencePosition		= Vector3.Transform(data.VergencePosition,		cameraMatrixAffine);
				result.LeftEyePosition		= Vector3.Transform(data.LeftEyePosition,		cameraMatrixAffine);
				result.RightEyePosition		= Vector3.Transform(data.RightEyePosition,		cameraMatrixAffine);

				/* What transformations do we need? */
				Vector2 gazeOffset = new Vector2(localRotationRadians.X + globalRotationRadians.X, localRotationRadians.Y + globalRotationRadians.Y);
				result.LeftEyeGazeRotation = data.LeftEyeGazeRotation + gazeOffset;
				result.RightEyeGazeRotation = data.RightEyeGazeRotation + gazeOffset;

				result.HeadRotation = data.HeadRotation;
				if (useInverseMatrix)
					result.HeadRotation -= globalRotationRadians + localRotationRadians;
				else
					result.HeadRotation += globalRotationRadians + localRotationRadians;
			}
			else
			{
				result.HeadPosition			= data.HeadPosition;
				result.HeadRotation			= data.HeadRotation;
				result.LeftPupilPosition	= data.LeftPupilPosition;
				result.RightPupilPosition	= data.RightPupilPosition;
				result.VergencePosition		= data.VergencePosition;
				result.LeftEyeGazeRotation	= data.LeftEyeGazeRotation;
				result.RightEyeGazeRotation	= data.RightEyeGazeRotation;
				result.LeftEyePosition		= data.LeftEyePosition;
				result.RightEyePosition		= data.RightEyePosition;
			}

			result.CoordinateSystem = FaceData.CoordinateSystemType.Global;

			return result;
		}

		public void UseDirectCameraData(FaceData data)
		{
			SignalLabel = data.SignalLabel;

			m_Person.DataSource					= data;
			m_Person.WorldSpaceForeheadPosition	= data.HeadPosition;
			m_Person.LeftEyePosition			= data.LeftEyePosition;
			m_Person.RightEyePosition			= data.RightEyePosition;
			m_Person.LeftPupilPosition			= data.LeftPupilPosition;
			m_Person.RightPupilPosition			= data.RightPupilPosition;
			m_Person.RotationEuler				= data.HeadRotation;
			m_Person.VergencePoint				= data.VergencePosition;
			m_Person.LeftEyeGaze				= data.LeftEyeGazeRotation;
			m_Person.RightEyeGaze				= data.RightEyeGazeRotation;
		}
	}
}
