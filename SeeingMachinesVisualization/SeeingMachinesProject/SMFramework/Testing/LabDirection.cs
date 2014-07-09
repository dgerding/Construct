using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMFramework.Testing
{
	public interface LabDirection
	{
		void InterpretData(DataSnapshot dataSource);

		bool IsValid
		{
			get;
		}

		String Label
		{
			get;
		}

		String SourceSensor
		{
			get;
			set;
		}

		Vector3 FirstPoint
		{
			get;
		}

		Vector3 SecondPoint
		{
			get;
		}

		Vector3 Direction
		{
			get;
		}

		Vector3 DirectionNormal
		{
			get;
		}
	}

	public class FixedDirection : LabDirection
	{
		public FixedDirection(Vector3 firstPoint, Vector3 secondPoint)
		{
			FirstPoint = firstPoint;
			SecondPoint = secondPoint;
		}

		public void InterpretData(DataSnapshot dataSource)
		{
		}

		public bool IsValid
		{
			get { return true; }
		}

		public String SourceSensor { get; set; }

		public Vector3 FirstPoint { get; set; }
		public Vector3 SecondPoint { get; set; }
		public Vector3 DirectionNormal { get { return Direction / Direction.Length(); } }
		public Vector3 Direction { get { return SecondPoint - FirstPoint; } }

		public String Label
		{
			get { return "Fixed Direction Pos:" + FirstPoint + " Dir:" + DirectionNormal; }
		}
	}

	public class HeadOrientationDirection : LabDirection
	{
		public void InterpretData(DataSnapshot dataSource)
		{
			if (!dataSource.ContainsKeyContaining(SourceSensor))
			{
				IsValid = false;
				return;
			}

			FirstPoint = dataSource.ComposeVector3(SourceSensor + " Head");
			SecondPoint = FirstPoint + dataSource.ComposeVector3(SourceSensor + " HeadRotation");
			IsValid = true;
		}

		public String SourceSensor
		{
			get;
			set;
		}

		public bool IsValid
		{
			get;
			private set;
		}

		public HeadOrientationDirection()
		{
			IsValid = false;
		}

		public HeadOrientationDirection(String sourceSensorLabel)
		{
			SourceSensor = sourceSensorLabel;
			IsValid = false;
		}

		public String Label
		{
			get { return SourceSensor + " Head Orientation"; }
		}

		public Vector3 FirstPoint { get; set; }
		public Vector3 SecondPoint { get; set; }
		public Vector3 DirectionNormal { get { return Direction / Direction.Length(); } }
		public Vector3 Direction { get { return SecondPoint - FirstPoint; } }
	}

	public class VergenceDirection : LabDirection
	{
		public void InterpretData(DataSnapshot dataSource)
		{
			if (!dataSource.ContainsKeyContaining(SourceSensor))
			{
				IsValid = false;
				return;
			}

			FirstPoint = dataSource.ComposeVector3(SourceSensor + " Head");
			SecondPoint = dataSource.ComposeVector3(SourceSensor + " Vergence");
			IsValid = true;
		}

		public String SourceSensor
		{
			get;
			set;
		}

		public bool IsValid
		{
			get;
			private set;
		}

		public VergenceDirection()
		{
			IsValid = false;
		}

		public VergenceDirection(String sourceSensorLabel)
		{
			SourceSensor = sourceSensorLabel;
			IsValid = false;
		}

		public String Label
		{
			get { return SourceSensor + " Vergence Direction"; }
		}

		public Vector3 FirstPoint { get; set; }
		public Vector3 SecondPoint { get; set; }
		public Vector3 DirectionNormal { get { return Direction / Direction.Length(); } }
		public Vector3 Direction { get { return SecondPoint - FirstPoint; } }
	}

	public class PointToFixedPointDirection : LabDirection
	{
		public PointToFixedPointDirection()
		{
			IsValid = false;
		}

		public PointToFixedPointDirection(String sourceSensorLabel)
		{
			IsValid = false;
			SourceSensor = sourceSensorLabel;
		}

		public PointToFixedPointDirection(String sourceSensorLabel, LabPoint sourcePoint, Vector3 fixedPoint)
		{
			IsValid = false;
			SourceSensor = sourceSensorLabel;
			FirstPointSource = sourcePoint;
			SecondPoint = fixedPoint;
		}

		public PointToFixedPointDirection(LabPoint sourcePoint, Vector3 fixedPoint)
		{
			FirstPointSource = sourcePoint;
			SecondPoint = fixedPoint;
			IsValid = false;
		}

		public void InterpretData(DataSnapshot source)
		{
			if (FirstPointSource != null)
			{
				if (!source.ContainsKeyContaining(SourceSensor))
				{
					IsValid = false;
					return;
				}

				FirstPointSource.InterpretData(source);
				IsValid = true;
			}
			else
			{
				IsValid = false;
			}
		}

		public bool IsValid
		{
			get;
			private set;
		}

		public LabPoint FirstPointSource;

		public Vector3 FirstPoint { get { return FirstPointSource.Position; } }

		public Vector3 SecondPoint { get; set; }

		public String SourceSensor
		{
			get;
			set;
		}

		public String Label
		{
			get { return "Line of " + SourceSensor + " " + FirstPointSource.Label + " to Fixed Point " + SecondPoint; }
		}

		public Vector3 DirectionNormal { get { return Direction / Direction.Length(); } }
		public Vector3 Direction { get { return SecondPoint - FirstPoint; } }
	}

	public class EyeGazeDirection : LabDirection
	{
		public bool IsValid
		{
			get;
			private set;
		}

		public String SourceSensor { get; set; }

		public String Label
		{
			get { return SourceSensor + " " + Eye + " Gaze"; }
		}

		public enum EyeIndex
		{
			Left,
			Right,
			Unassigned
		}

		public EyeIndex Eye = EyeIndex.Unassigned;

		public Vector3 FirstPoint { get; internal set; }
		public Vector3 SecondPoint { get; internal set; }

		public Vector3 Direction { get { return SecondPoint - FirstPoint; } }
		public Vector3 DirectionNormal { get { return Direction / Direction.Length(); } }

		public EyeGazeDirection()
		{
			IsValid = false;
		}

		public EyeGazeDirection(String sourceSensorLabel)
		{
			IsValid = false;
			SourceSensor = sourceSensorLabel;
		}

		public EyeGazeDirection(EyeIndex eye)
		{
			IsValid = false;
			Eye = eye;
		}

		public EyeGazeDirection(String sourceSensorLabel, EyeIndex eye)
		{
			IsValid = false;
			SourceSensor = sourceSensorLabel;
			Eye = eye;
		}

		public void InterpretData(DataSnapshot source)
		{
			if (!source.ContainsKeyContaining(SourceSensor) || Eye == EyeIndex.Unassigned)
			{
				IsValid = false;
				return;
			}

			FirstPoint = source.ComposeVector3(String.Format("{0} {1}Pupil", SourceSensor, Eye));

			Vector2 gazeRotation = source.ComposeVector2(String.Format("{0} {1}EyeGazeRotation"));
			Vector3 gazeVector = new Vector3(
					(float)(Math.Sin(gazeRotation.Y) * Math.Cos(gazeRotation.X)),
					(float)Math.Sin(gazeRotation.X),
					-(float)(Math.Cos(gazeRotation.Y) * Math.Cos(gazeRotation.X))
				) * 5.0F * Units.Centimeter;

			SecondPoint = FirstPoint + gazeVector;

			IsValid = true;
		}
	}
}
