using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalibrationTool
{
	class DataSlot
	{
		public enum Type
		{
			Recording,
			DataCapture,
			Unknown
		}

		public Type DataType = Type.Unknown;

		public CaptureChunk CaptureChunk;
		public RecordedChunk RecordedChunk;

		public int Index;
	}
}
