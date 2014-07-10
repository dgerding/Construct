using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SMFramework
{
	public class FacialAttendanceDetector
	{
		Vector3 GetHeadCenterPosition(FaceData person, float headRadius)
		{
			//	SeeingMachines reports head position as the position between the eyeballs
			Vector3 headOrientation = GetGazeRayFromEulerRotation(new Vector2(person.HeadRotation.X, person.HeadRotation.Y));
			return person.HeadPosition - headOrientation * headRadius;
		}

		public bool IsHeadOrientationAttending(FaceData person, FaceData target, Vector3 targetHeadSize)
		{
			if (person.CoordinateSystem != FaceData.CoordinateSystemType.Global || target.CoordinateSystem != FaceData.CoordinateSystemType.Global)
				throw new Exception("FaceData for attendance must be in global coordinates.");

			Vector3 targetHeadScale = targetHeadSize / 2.0f;

			//	Assume that heads are spherical
			float headRadius = (targetHeadScale.X + targetHeadScale.Y + targetHeadScale.Z) / 3.0f;

			Vector3 targetHeadCenterPosition = GetHeadCenterPosition(target, headRadius);

			Vector3 personLocalHeadOrientation = GetGazeRayFromEulerRotation(new Vector2(person.HeadRotation.X, person.HeadRotation.Y));
			Vector3 rayToTarget = targetHeadCenterPosition - person.HeadPosition;
			rayToTarget.Normalize();
			personLocalHeadOrientation.Normalize();

			Vector3 pointOnSideOfTargetsHead = targetHeadCenterPosition + new Vector3(-rayToTarget.Z, rayToTarget.Y, rayToTarget.X) * headRadius;
			Vector3 rayToSideOfTargetsHead = pointOnSideOfTargetsHead - person.HeadPosition;
			rayToSideOfTargetsHead.Normalize();

			float headOrientationAngle = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(personLocalHeadOrientation, rayToTarget)));
			float maxAngle = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(rayToSideOfTargetsHead, rayToTarget)));

			return headOrientationAngle <= maxAngle * 1.05f; // * 1.05f to remove false negatives due to rounding errors
		}

// 		public bool IsVergenceRayAttending(FaceData person, FaceData target, Vector3 targetHeadSize)
// 		{
// 			if (person.CoordinateSystem != FaceData.CoordinateSystemType.Global || target.CoordinateSystem != FaceData.CoordinateSystemType.Global)
// 				throw new Exception("FaceData for attendance must be in global coordinates.");
// 
// 			//	Vergence requires relatively high-quality data (at least level 2 quality)
// 			if (person.LeftEyeGazeQualityLevel < 2 || person.RightEyeGazeQualityLevel < 2)
// 				return false;
// 
// 			Matrix headLocalSpace = Matrix.CreateTranslation(target.HeadPosition) * Matrix.CreateScale(targetHeadSize);
// 			Matrix globalToHeadLocalTransform = Matrix.Invert(headLocalSpace);
// 
// 			Vector3 personLocalPosition = Vector3.Transform(person.HeadPosition, globalToHeadLocalTransform);
// 			Vector3 personLocalGaze = Vector3.Transform(person.VergencePosition, globalToHeadLocalTransform);
// 
// 			Vector3 personGaze = person.VergencePosition - person.HeadPosition;
// 			Vector3 rayToTarget = target.HeadPosition - person.HeadPosition;
// 			rayToTarget.Normalize();
// 			personGaze.Normalize();
// 
// 			//	The point on the side of their head is relative to the position of the viewer
// 			Vector3 pointOnSideOfTargetsHead = new Vector3(-rayToTarget.Z, rayToTarget.Y, rayToTarget.X);
// 			Vector3 rayToSideOfTargetsHead = pointOnSideOfTargetsHead - person.HeadPosition;
// 			rayToSideOfTargetsHead.Normalize();
// 
// 			return false;
// 		}

		//	0 is not facially attending, 1 is facially attending
		public float GetFacialAttendance(FaceData person, FaceData target)
		{
			//	Treat a person's head as a uniform sphere, compare the angle between gaze and ray-between-subjects to the angle
			//		between ray-between-subjects and ray-to-side-of-person's-head.

			Vector3 gazeSourcePosition;
			Vector3? possiblePersonGaze = this.GetBestGazeRay(person, out gazeSourcePosition);
			if (!possiblePersonGaze.HasValue)
				return 0.0f;

			Vector3 targetCenterHeadPosition = GetHeadCenterPosition(target, AveragePersonHeadRadiusMeters);

			Vector3 personGaze = possiblePersonGaze.Value;
			Vector3 rayToTarget = targetCenterHeadPosition - gazeSourcePosition;
			rayToTarget.Normalize();
			personGaze.Normalize();

			//	The point on the side of their head is relative to the position of the viewer
			Vector3 pointOnSideOfTargetsHead = targetCenterHeadPosition + new Vector3(-rayToTarget.Z, rayToTarget.Y, rayToTarget.X) * AveragePersonHeadRadiusMeters;
			Vector3 rayToSideOfTargetsHead = pointOnSideOfTargetsHead - gazeSourcePosition;
			rayToSideOfTargetsHead.Normalize();

			float gazeAngle = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(rayToTarget, personGaze)));
			//float maxAngle = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(rayToTarget, rayToSideOfTargetsHead)));
			float maxAngle = 10.0f; // Hard-coded cone angle

			float falloffDistance = maxAngle * 2.5f; // Magic number

			if (gazeAngle < maxAngle)
				return 1.0f;
			if (gazeAngle > maxAngle + falloffDistance*2)
				return 0.0f;

			//	Interpolate between max +/- falloffDistance
			return ((maxAngle + falloffDistance*2) - gazeAngle) / (falloffDistance * 2.0f);
		}
		
		//	According to Google: Avg adult head circumference is 53cm for females, 57cm for males
		private static float AveragePersonHeadRadiusMeters = Units.Centimeter * 8.7535f;

		private Vector3 GetGazeRayFromEulerRotation(Vector2 rotation)
		{
			return new Vector3(
					(float)(Math.Sin(-rotation.Y) * Math.Cos(rotation.X)),
					(float)Math.Sin(rotation.X),
					-(float)(Math.Cos(-rotation.Y) * Math.Cos(rotation.X))
				);
		}

		private Vector3? GetBestGazeRay(FaceData faceSource, out Vector3 gazeSourcePosition)
		{
// 			if (faceSource.LeftEyeGazeQualityLevel >= faceSource.RightEyeGazeQualityLevel && faceSource.LeftEyeGazeQualityLevel > 2)
// 			{
// 				gazeSourcePosition = faceSource.LeftEyePosition;
// 				return GetGazeRayFromEulerRotation(faceSource.LeftEyeGazeRotation);
// 			}
// 
// 			if (faceSource.RightEyeGazeQualityLevel >= faceSource.LeftEyeGazeQualityLevel && faceSource.RightEyeGazeQualityLevel > 2)
// 			{
// 				gazeSourcePosition = faceSource.RightEyePosition;
// 				return GetGazeRayFromEulerRotation(faceSource.RightEyeGazeRotation);
// 			}
// 
// 			if (faceSource.HeadPositionConfidence < 0.0f)
// 			{
// 				gazeSourcePosition = Vector3.Zero;
// 				return null;
// 			}

			gazeSourcePosition = faceSource.HeadPosition;
			return GetGazeRayFromEulerRotation(new Vector2(faceSource.HeadRotation.X, faceSource.HeadRotation.Y));
		}
	}
}
