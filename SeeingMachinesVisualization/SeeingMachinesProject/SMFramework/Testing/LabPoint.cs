using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMFramework.Testing
{
	public interface LabPoint
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

		Vector3 Position
		{
			get;
			set;
		}
	}

	public class EyePoint : LabPoint
	{
		public enum EyeIndex
		{
			Left,
			Right,
			Unassigned
		}

		public EyeIndex Eye = EyeIndex.Unassigned;

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

		public EyePoint(EyeIndex index)
		{
			IsValid = false;
		}

		public EyePoint(String sourceSensorLabel, EyeIndex index)
		{
			SourceSensor = sourceSensorLabel;
			IsValid = false;
		}

		public void InterpretData(DataSnapshot dataSource)
		{
			if (!dataSource.ContainsKeyContaining(SourceSensor))
			{
				IsValid = false;
				return;
			}

			switch (Eye)
			{
				case EyeIndex.Left:
					Position = dataSource.ComposeVector3(SourceSensor + " LeftEye");
					break;

				case EyeIndex.Right:
					Position = dataSource.ComposeVector3(SourceSensor + " RightEye");
					break;

				default:
					DebugOutputStream.SlowInstance.WriteLine("EyePoint.InterpretData - Invalid EyeIndex specified: " + Eye.ToString());
					break;
			}

			IsValid = true;
		}

		public String Label
		{
			get { return SourceSensor + " " + Eye.ToString() + " Eye Position"; }
		}

		public Vector3 Position
		{
			get;
			set;
		}
	}

	public class FixedPoint : LabPoint
	{
		public Vector3 Position
		{
			get;
			set;
		}

		public FixedPoint(Vector3 position)
		{
			Position = position;
		}

		public String SourceSensor
		{
			get;
			set;
		}

		public bool IsValid { get { return true; } }

		public String Label
		{
			get { return "Fixed Point " + Position.ToString(); }
		}

		public void InterpretData(DataSnapshot dataSource)
		{
		}
	}

	public class PupilPoint : LabPoint
	{
		public enum PupilIndex
		{
			Left,
			Right,
			Unassigned
		}

		public PupilIndex Pupil = PupilIndex.Unassigned;

		public bool IsValid
		{
			get;
			private set;
		}

		public String SourceSensor
		{
			get;
			set;
		}

		public PupilPoint(PupilIndex pupil)
		{
			Pupil = pupil;
			IsValid = false;
		}

		public PupilPoint(String sourceSensorLabel, PupilIndex pupil)
		{
			SourceSensor = sourceSensorLabel;
			Pupil = pupil;
			IsValid = false;
		}

		public void InterpretData(DataSnapshot dataSource)
		{
			if (!dataSource.ContainsKeyContaining(SourceSensor))
			{
				IsValid = false;
				return;
			}

			switch (Pupil)
			{
				case PupilIndex.Left:
					Position = dataSource.ComposeVector3(SourceSensor + " LeftPupil");
					break;

				case PupilIndex.Right:
					Position = dataSource.ComposeVector3(SourceSensor + " RightPupil");
					break;

				default:
					DebugOutputStream.SlowInstance.WriteLine("PupilPoint.InterpretData - Invalid PupilIndex specified: " + Pupil.ToString());
					break;
			}

			IsValid = true;
		}

		public String Label
		{
			get { return SourceSensor + " " + Pupil.ToString() + " Pupil Position"; }
		}

		public Vector3 Position
		{
			get;
			set;
		}
	}

	public class HeadPoint : LabPoint
	{
		public void InterpretData(DataSnapshot dataSource)
		{
			if (!dataSource.ContainsKeyContaining(SourceSensor))
			{
				IsValid = false;
				return;
			}

			Position = dataSource.ComposeVector3(SourceSensor + " Head");
			IsValid = true;
		}

		public bool IsValid
		{
			get;
			private set;
		}

		public String SourceSensor
		{
			get;
			set;
		}

		public HeadPoint()
		{
			IsValid = false;
		}

		public HeadPoint(String sourceSensorLabel)
		{
			SourceSensor = sourceSensorLabel;
			IsValid = false;
		}

		public Vector3 Position
		{
			get;
			set;
		}

		public String Label
		{
			get { return SourceSensor + " Head Position"; }
		}
	}
}
