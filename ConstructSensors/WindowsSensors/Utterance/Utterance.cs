using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utterance
{
	public class Utterance
	{
		public Utterance()
		{

		}

		public Utterance(byte[] wavFileData)
		{
			FullWAVFileData = wavFileData;
			UtteranceEndTime = DateTime.UtcNow;
		}

		public byte[] FullWAVFileData;
		public DateTime UtteranceEndTime;
	}
}
