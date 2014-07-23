using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;

namespace SapiSensor
{
    class Transcription
	{
		public String TranscribedText { get; set; }
		public DateTime TranscriptionEndTime { get; set; }

		public String AlternativeText1 { get; set; }
		public String AlternativeText2 { get; set; }
		public String AlternativeText3 { get; set; }

		public double Confidence { get; set; }

		public double AlternativeConfidence1 { get; set; }
		public double AlternativeConfidence2 { get; set; }
		public double AlternativeConfidence3 { get; set; }

		public Transcription(RecognitionResult recognition, DateTime recognizedAudioStartTime)
		{
			TranscribedText = recognition.Text;
			Confidence = recognition.Confidence;
			TranscriptionEndTime = recognizedAudioStartTime + recognition.Audio.Duration;

			#region Fill alternatives based on number of alternatives available
			if (recognition.Alternates.Count > 0) {
				AlternativeConfidence1 = recognition.Alternates[0].Confidence;
				AlternativeText1 = recognition.Alternates[0].Text;
			} else {
				AlternativeConfidence1 = 0.0f;
				AlternativeText1 = "";
			}

			if (recognition.Alternates.Count > 1) {
				AlternativeConfidence2 = recognition.Alternates[1].Confidence;
				AlternativeText2 = recognition.Alternates[1].Text;
			} else {
				AlternativeConfidence2 = 0.0f;
				AlternativeText2 = "";
			}

			if (recognition.Alternates.Count > 2) {
				AlternativeConfidence3 = recognition.Alternates[2].Confidence;
				AlternativeText3 = recognition.Alternates[2].Text;
			} else {
				AlternativeConfidence3 = 0.0f;
				AlternativeText3 = "";
			}
			#endregion
		}
    }
}
