using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMVisualization.Visualization
{
	public class ShapeHelper
	{
		public static void DrawEllipse(Vector3 position, Vector3 size, PrimitiveBatch targetBatch)
		{
			DrawEllipse(position, size, 20, targetBatch);
		}

		public static void DrawEllipse(Vector3 position, Vector3 size, int radialPrecision, PrimitiveBatch targetBatch)
		{
			float radialUnit = (float)(Math.PI / radialPrecision);

			targetBatch.ActiveType = PrimitiveBatch.ListType.TriangleStrip;

			for (float sphericalRingPosition = (float)(Math.PI / 2); sphericalRingPosition < 3.0 / 2.0 * Math.PI; sphericalRingPosition += radialUnit)
			{
				targetBatch.ActiveType = PrimitiveBatch.ListType.TriangleStrip;

				for (float sphericalRadialPosition = 0.0f; sphericalRadialPosition <= Math.PI * 2.0 + radialUnit / 2.0; sphericalRadialPosition += radialUnit)
				{
					Vector3 top, bottom;
					top = new Vector3(
						(float)(Math.Cos(sphericalRadialPosition) * Math.Sin(sphericalRingPosition + (float)(Math.PI / 2))),
						(float)Math.Sin(sphericalRingPosition),
						(float)(Math.Sin(sphericalRadialPosition) * Math.Sin(sphericalRingPosition + (float)(Math.PI / 2)))
						);

					bottom = new Vector3(
						(float)(Math.Cos(sphericalRadialPosition) * Math.Sin(sphericalRingPosition + (float)(Math.PI / 2) + radialUnit)),
						(float)Math.Sin(sphericalRingPosition + radialUnit),
						(float)(Math.Sin(sphericalRadialPosition) * Math.Sin(sphericalRingPosition + (float)(Math.PI / 2) + radialUnit))
						);

					targetBatch.AddVertex(top * (size / 2) + position);
					targetBatch.AddVertex(bottom * (size / 2) + position);
				}
			}

			targetBatch.ActiveType = PrimitiveBatch.ListType.Single;
		}

		/*
		public static Vector3 GetPositionWithinEllipse(Vector3 ellipseSize, float radialNormal, float azimuthNormal)
		{
			Vector3 evaluatedPositionFromSpherical = new Vector3(
				(float)(Math.Cos(radialNormal * Math.PI * 2.0) * Math.Sin(azimuthNormal * (3.0 / 2.0 * Math.PI) + Math.PI / 2)),
				(float)Math.Sin(azimuthNormal * (3.0 / 2.0 * Math.PI)),
				(float)(Math.Sin(radialNormal * Math.PI * 2.0) * Math.Sin(azimuthNormal * (3.0 / 2.0 * Math.PI) + Math.PI / 2))
				);

			return evaluatedPositionFromSpherical * ellipseSize;
		}*/

		public static void DrawBox(Vector3 position, Vector3 size, PrimitiveBatch targetBatch)
		{
			//	Top
			targetBatch.AddQuad(
				new Vector3(position.X - size.X / 2, position.Y + size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y + size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y + size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y + size.Y / 2, position.Z + size.Z / 2)
				);

			//	Bottom
			targetBatch.AddQuad(
				new Vector3(position.X - size.X / 2, position.Y - size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y - size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y - size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y - size.Y / 2, position.Z + size.Z / 2)
				);

			//	Front
			targetBatch.AddQuad(
				new Vector3(position.X - size.X / 2, position.Y + size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y + size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y - size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y - size.Y / 2, position.Z + size.Z / 2)
				);

			//	Back
			targetBatch.AddQuad(
				new Vector3(position.X - size.X / 2, position.Y + size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y + size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y - size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y - size.Y / 2, position.Z - size.Z / 2)
				);

			//	Left
			targetBatch.AddQuad(
				new Vector3(position.X - size.X / 2, position.Y + size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y + size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y - size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X - size.X / 2, position.Y - size.Y / 2, position.Z - size.Z / 2)
				);

			//	Right
			targetBatch.AddQuad(
				new Vector3(position.X + size.X / 2, position.Y + size.Y / 2, position.Z - size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y + size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y - size.Y / 2, position.Z + size.Z / 2),
				new Vector3(position.X + size.X / 2, position.Y - size.Y / 2, position.Z - size.Z / 2)
				);
		}
	}
}
