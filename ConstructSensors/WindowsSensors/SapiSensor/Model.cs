using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SapiSensor
{
    class Transcription
    {
		public String TranscribedText
		{
			get;
			private set;
		}

		public Transcription(String text)
		{
			TranscribedText = text;
		}
    }
}
