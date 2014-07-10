using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMVisualization
{
	//	Defaults are set to what looks nicest (having everything enabled is too much information, looks very crowded)
	public class SubjectRenderOptions
	{
		public bool DrawHeadModel = true;
		public bool DrawFaceTexture = false;
		public bool DrawEyeBrows = false;
		public bool DrawEyes = true;
		public bool DrawPupils = false;
		public bool DrawGazeRay = false;
		public bool DrawExtendedGazeRay = false;
		public bool DrawMouth = false;
		public bool DrawSubject = true;
		public bool DrawCameraStream = false;
		public bool DrawLeftEyeGaze = true;
		public bool DrawRightEyeGaze = true;
		public bool DrawFacialAttendanceInfo = false;
		public bool DrawHeadOrientationAttendance = true;
	}
}
