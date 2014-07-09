using System.Runtime.InteropServices;

namespace faceAPISensorDriver.SensorWrapper
{
	/* http://limbioliong.wordpress.com/2011/06/16/returning-strings-from-a-c-api/ */
	class FaceRecognizer
	{
		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_Start")]
		extern public static void Start();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_Stop")]
		extern public static void Stop();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_HeadPoseDataAvailable")]
		extern public static bool HeadPoseDataAvailable();

		public static HeadPose GetCurrentHeadPose()
		{
			HeadPose result = new HeadPose();

			result.Confidence = GetCurrentHeadPose_Confidence();

			result.HeadX = GetCurrentHeadPose_HeadX();
			result.HeadY = GetCurrentHeadPose_HeadY();
			result.HeadZ = GetCurrentHeadPose_HeadZ();

			result.LeftEyeX = GetCurrentHeadPose_LeftEyeX();
			result.LeftEyeY = GetCurrentHeadPose_LeftEyeY();
			result.LeftEyeZ = GetCurrentHeadPose_LeftEyeZ();

			result.RightEyeX = GetCurrentHeadPose_RightEyeX();
			result.RightEyeY = GetCurrentHeadPose_RightEyeY();
			result.RightEyeZ = GetCurrentHeadPose_RightEyeZ();

			result.HeadRotationRadiansX = GetCurrentHeadPose_HeadRotationRadiansX();
			result.HeadRotationRadiansY = GetCurrentHeadPose_HeadRotationRadiansY();
			result.HeadRotationRadiansZ = GetCurrentHeadPose_HeadRotationRadiansZ();

			AdvanceCurrentHeadPose();

			return result;
		}




		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_AdvanceCurrentHeadPose")]
		extern private static float AdvanceCurrentHeadPose();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_HeadX")]
		extern private static float GetCurrentHeadPose_HeadX();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_HeadY")]
		extern private static float GetCurrentHeadPose_HeadY();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_HeadZ")]
		extern private static float GetCurrentHeadPose_HeadZ();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_LeftEyeX")]
		extern private static float GetCurrentHeadPose_LeftEyeX();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_LeftEyeY")]
		extern private static float GetCurrentHeadPose_LeftEyeY();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_LeftEyeZ")]
		extern private static float GetCurrentHeadPose_LeftEyeZ();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_RightEyeX")]
		extern private static float GetCurrentHeadPose_RightEyeX();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_RightEyeY")]
		extern private static float GetCurrentHeadPose_RightEyeY();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_RightEyeZ")]
		extern private static float GetCurrentHeadPose_RightEyeZ();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansX")]
		extern private static float GetCurrentHeadPose_HeadRotationRadiansX();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansY")]
		extern private static float GetCurrentHeadPose_HeadRotationRadiansY();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_HeadRotationRadiansZ")]
		extern private static float GetCurrentHeadPose_HeadRotationRadiansZ();

		[DllImport("faceAPISensor.dll", EntryPoint = "FaceRecognizer_GetCurrentHeadPose_Confidence")]
		extern private static float GetCurrentHeadPose_Confidence();
	}
}
