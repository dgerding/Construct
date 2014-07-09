using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sm.eod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SMFramework
{
	public class FaceData
	{
		//	Should use SMFramework.ImageOutputDataConverter to convert to Texture2D
		public ImageOutputData FaceTextureData = null;

		public String SignalLabel = "UNLABELED";

		public Vector3 HeadPosition = Vector3.Zero;
		public Vector3 HeadRotation = Vector3.Zero;

		public Vector3 LeftEyePosition = Vector3.Zero;
		public Vector3 RightEyePosition = Vector3.Zero;

		public Vector3 LeftPupilPosition = Vector3.Zero;
		public Vector3 RightPupilPosition = Vector3.Zero;

		public Vector3 VergencePosition = Vector3.Zero;
		public Vector2 LeftEyeGazeRotation = Vector2.Zero;
		public Vector2 RightEyeGazeRotation = Vector2.Zero;
		public float LeftEyeGazeQualityLevel = 0.0F;
		public float RightEyeGazeQualityLevel = 0.0F;

		//	TODO: FaceAPI reports bounds for eyes?
		public List<Vector3> MouthInnerLowerLipVertices = null;
		public List<Vector3> MouthOuterLowerLipVertices = null;
		public List<Vector3> MouthInnerUpperLipVertices = null;
		public List<Vector3> MouthOuterUpperLipVertices = null;
		public List<Vector3> LeftEyebrowVertices = null;
		public List<Vector3> RightEyebrowVertices = null;
		public List<Vector3> FaceBoundsVertices = null;
		public List<Vector2> FaceBoundsUVs = null;

		public DateTime SnapshotTimestamp = DateTime.Now;

		public float HeadPositionConfidence = 0.0F;

		public float LeftPupilDiameter = 0.0F;
		public float RightPupilDiameter = 0.0F;

		public uint FaceLabFrameIndex = 0;

		public float LeftEyeClosure = 0.0F;
		public float LeftEyeClosureConfidence = 0.0F;

		public float RightEyeClosure = 0.0F;
		public float RightEyeClosureConfidence = 0.0F;

		public uint LandmarkCount = 0;
		public bool WearingGlasses = false;
		public bool IsBlinking = false;


		public enum CoordinateSystemType
		{
			Local,
			Global
		}

		public static CoordinateSystemType ParseCoordinateSystem(String coordinateSystem)
		{
			switch (coordinateSystem.ToLower())
			{
				case ("local"): return CoordinateSystemType.Local;
				case ("global"): return CoordinateSystemType.Global;
			}

			throw new Exception("Invalid coordinate system string.");
		}

		// SeeingMachines passes us local data by default
		public CoordinateSystemType CoordinateSystem = CoordinateSystemType.Global;

		public override bool Equals(object obj)
		{
			if (!(obj is FaceData))
				return false;

			Type faceDataType = typeof(FaceData);
			FieldInfo[] properties = faceDataType.GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (FieldInfo property in properties)
			{
				if (!property.GetValue(this).Equals(property.GetValue(obj)))
					return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
