using System;
using System.Text;

namespace faceAPISensorDriver.SensorWrapper
{
	struct HeadPose
	{
		public float HeadX, HeadY, HeadZ;
		public float LeftEyeX, LeftEyeY, LeftEyeZ;
		public float RightEyeX, RightEyeY, RightEyeZ;
		public float HeadRotationRadiansX, HeadRotationRadiansY, HeadRotationRadiansZ;
		public float Confidence;

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(String.Format("[Confidence: {0:N4}] ", Confidence));
			sb.Append(String.Format("[HeadPos: {0:N4},{1:N4},{2:N4}] ", HeadX, HeadY, HeadZ));
			sb.Append(String.Format("[LeftEye: {0:N4},{1:N4},{2:N4}] ", LeftEyeX, LeftEyeY, LeftEyeZ));
			sb.Append(String.Format("[RightEye: {0:N4},{1:N4},{2:N4}] ", RightEyeX, RightEyeY, RightEyeZ));
			sb.Append(String.Format("[HeadRot: {0:N4},{1:N4},{2:N4}] ", HeadRotationRadiansX, HeadRotationRadiansY, HeadRotationRadiansZ));
			return sb.ToString();
		}
	}
}
