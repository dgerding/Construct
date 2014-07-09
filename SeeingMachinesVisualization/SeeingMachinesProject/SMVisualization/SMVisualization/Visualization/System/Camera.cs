using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using SMFramework;

namespace SMVisualization.Visualization
{
	public class Camera
	{
		public static Matrix ProjectionMatrix
		{
			get
			{
				return Matrix.CreatePerspectiveFieldOfView(
					MathHelper.ToRadians(45.0f),
					SMVisualization.Instance.GraphicsDevice.Viewport.AspectRatio,
					0.05f,
					500.0f);
			}
		}

		public static Matrix ViewspaceMatrix
		{
			get
			{
				return
					Matrix.CreateTranslation(-Position) *
					Matrix.CreateRotationY(-Rotation.Y) *
					Matrix.CreateRotationX(-Rotation.X);
			}
		}

		private static Vector3 m_Position = new Vector3(0, 0, 0);
		public static Vector3 Position
		{
			get { return m_Position; }
			set { m_Position = value; }
		}

		private static Vector2 m_Rotation = new Vector2(0, 0);
		public static Vector2 Rotation
		{
			get
			{
				return m_Rotation;
			}

			set
			{
				m_Rotation = value;

				//	Y axis should wrap
				while (m_Rotation.Y > Math.PI * 2)
					m_Rotation.Y -= (float)Math.PI * 2;
				while (m_Rotation.Y < 0)
					m_Rotation.Y += (float)Math.PI * 2;

				//	X axis should be clamped to [-PI/2, PI/2] (-90,90) so that you can't look "more than straight up"
				//		or "more than straight down"
				if (m_Rotation.X < -Math.PI / 2)
					m_Rotation.X = (float)-Math.PI / 2;
				if (m_Rotation.X > Math.PI / 2)
					m_Rotation.X = (float)Math.PI / 2;
			}
		}



		public static float WalkSpeed = 1.5f * Units.Meter; // Meters per second

		public static float MouseDragScale = 0.002f;



		public enum MoveDirection
		{
			Forward,
			Backward,
			Left,
			Right,
			Up,
			Down
		}

		public static void Move(MoveDirection direction)
		{
			switch (direction)
			{
				case (MoveDirection.Forward):
					{
						Move(Rotation.X, Rotation.Y + (float)Math.PI);
						break;
					}

				case (MoveDirection.Backward):
					{
						Move(Rotation.X, Rotation.Y);
						break;
					}

				case (MoveDirection.Left):
					{
						Move(Rotation.X, Rotation.Y - (float)Math.PI / 2);
						break;
					}

				case (MoveDirection.Right):
					{
						Move(Rotation.X, Rotation.Y + (float)Math.PI / 2);
						break;
					}

				case (MoveDirection.Up):
					{
						m_Position.Y += WalkSpeed * SMVisualization.TimeDelta;
						break;
					}

				case (MoveDirection.Down):
					{
						m_Position.Y -= WalkSpeed * SMVisualization.TimeDelta;
						break;
					}
			}
		}

		public static void Move(float xangle, float yangle)
		{
			Position += new Vector3(
				(float)Math.Sin(yangle) * WalkSpeed * SMVisualization.TimeDelta,
				0.0f,
				(float)Math.Cos(yangle) * WalkSpeed * SMVisualization.TimeDelta
				);
		}

		public static void TurnByMouseDelta(float dx, float dy)
		{
			Vector2 newRotation = Rotation;
			newRotation.X += dy * MouseDragScale;
			newRotation.Y += dx * MouseDragScale;
			Rotation = newRotation;
		}
	}
}
