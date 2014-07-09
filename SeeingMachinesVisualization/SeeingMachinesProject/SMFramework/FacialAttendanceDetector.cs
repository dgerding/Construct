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
		//	0 is not facially attending, 1 is facially attending
		public float GetFacialAttendance(FaceData person, FaceData target)
		{
			//	Treat a person's head as a uniform sphere, compare the angle between gaze and ray-between-subjects to the angle
			//		between ray-between-subjects and ray-to-side-of-person's-head.

			Vector3 gazeSourcePosition;
			Vector3? possiblePersonGaze = this.GetBestGazeRay(person, out gazeSourcePosition);
			if (!possiblePersonGaze.HasValue)
				return 0.0f;

			Vector3 personGaze = possiblePersonGaze.Value;
			Vector3 rayToTarget = target.HeadPosition - gazeSourcePosition;
			rayToTarget.Normalize();
			personGaze.Normalize();

			//	The point on the side of their head is relative to the position of the viewer
			Vector3 pointOnSideOfTargetsHead = target.HeadPosition + new Vector3(-rayToTarget.Z, rayToTarget.Y, rayToTarget.X) * AveragePersonHeadRadiusMeters;
			Vector3 rayToSideOfTargetsHead = pointOnSideOfTargetsHead - gazeSourcePosition;
			rayToSideOfTargetsHead.Normalize();

			float gazeAngle = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(rayToTarget, personGaze)));
			float maxAngle = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(rayToTarget, rayToSideOfTargetsHead)));

			float falloffDistance = maxAngle * 1.5f; // Magic number

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
			if (faceSource.LeftEyeGazeQualityLevel >= faceSource.RightEyeGazeQualityLevel && faceSource.LeftEyeGazeQualityLevel > 1)
			{
				gazeSourcePosition = faceSource.LeftEyePosition;
				return GetGazeRayFromEulerRotation(faceSource.LeftEyeGazeRotation);
			}

			if (faceSource.RightEyeGazeQualityLevel >= faceSource.LeftEyeGazeQualityLevel && faceSource.RightEyeGazeQualityLevel > 1)
			{
				gazeSourcePosition = faceSource.RightEyePosition;
				return GetGazeRayFromEulerRotation(faceSource.RightEyeGazeRotation);
			}

			if (faceSource.HeadPositionConfidence < 0.0f)
			{
				gazeSourcePosition = Vector3.Zero;
				return null;
			}

			gazeSourcePosition = faceSource.HeadPosition;
			return GetGazeRayFromEulerRotation(new Vector2(faceSource.HeadRotation.X, faceSource.HeadRotation.Y));
		}
	}
}
