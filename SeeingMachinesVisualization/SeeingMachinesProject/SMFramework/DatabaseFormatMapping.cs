using System;
using System.Collections.Generic;

namespace SMFramework
{
	/* Maps the column labels in a recording to properties in a FaceData object, which is to be
	 * used with reflection for automatic assignment of properties from a recording. This table is necessary 
	 * for now since columns were originally named manually, not based on the variable names.
	 * A shift in column naming would end up causing most of our existing recordings to be
	 * incompatible with the format we have now.
	 * 
	 * TODO: Update CSV format to match FaceData property names (manually managing this map is error-prone)
	 */
	public class DatabaseFormatMapping
	{
		public static String TimestampColumnLabel = "Time (UTC)";

		public static String GenerateRecordingFilename()
		{
			return "Data/New Recording " + DateTime.Now.ToString("MMM-dd-yyyy   hh-mm-ss-tt") + ".csv";
		}

		public static Dictionary<String, String> ColumnPropertyMap = new Dictionary<String, String>()
		{
			// ColumnName		FaceData Property Name
			{ "LeftEye",	"LeftEyePosition" },
			{ "RightEye",	"RightEyePosition" },

			{ "LeftPupil",	"LeftPupilPosition" },
			{ "RightPupil",	"RightPupilPosition" },


			{ "LeftPupilDiameter",	"LeftPupilDiameter" },
			{ "RightPupilDiameter",	"RightPupilDiameter" },

			{ "LeftEyeClosure",		"LeftEyeClosure" },
			{ "RightEyeClosure",	"RightEyeClosure" },


			{ "LeftEyeGazeRotation",	"LeftEyeGazeRotation" },
			{ "RightEyeGazeRotation",	"RightEyeGazeRotation" },

			{ "AverageBlinkDuration",	"AverageBlinkDuration" },
			{ "BlinkFrequency",			"BlinkFrequency" },
			{ "Vergence",				"VergencePosition" },
			{ "FaceLabFrameIndex",		"FaceLabFrameIndex" },
			{ "Head",					"HeadPosition" },
			{ "HeadPositionConfidence",	"HeadPositionConfidence" },
			{ "HeadRotation",			"HeadRotation" },


			{ "LeftEyeClosureConfidence",	"LeftEyeClosureConfidence" },
			{ "RightEyeClosureConfidence",	"RightEyeClosureConfidence" },

			{ "LeftEyeGazeQualityLevel",	"LeftEyeGazeQualityLevel" },
			{ "RightEyeGazeQualityLevel",	"RightEyeGazeQualityLevel" }
		};
	}
}
