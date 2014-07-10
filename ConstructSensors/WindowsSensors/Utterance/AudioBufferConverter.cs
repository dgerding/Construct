using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utterance
{
	//	All methods expect raw PCM.

	//	http://www.codeproject.com/Articles/501521/How-to-convert-between-most-audio-formats-in-NET
	class AudioBufferConverter
	{
		public float[] ConvertToFloat(byte[] data, int bitsPerSample)
		{
			switch (bitsPerSample)
			{
				case (8): return Convert8BitToFloat(data);
				case (16): return Convert16BitToFloat(data);
				case (24): return Convert24BitToFloat(data);
				case (32): return Convert32BitToFloat(data);
				default: return null;
			}
		}

		public float[] Convert8BitToFloat(byte[] data)
		{
			float[] result = new float[data.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = data[i] / (float)byte.MaxValue;
			}

			return result;
		}

		public float[] Convert16BitToFloat(byte[] data)
		{
			float[] result = new float[data.Length / 2];
			for (int i = 0; i < result.Length; i++)
			{
				short currentValue = BitConverter.ToInt16(data, i * 2);
				result[i] = currentValue / (float)short.MaxValue;
			}

			return result;
		}

		public float[] Convert24BitToFloat(byte[] data)
		{
			float[] result = new float[data.Length / 3];
			byte[] convertBuffer = new byte[4];
			convertBuffer[3] = 0;
			for (int i = 0; i < result.Length; i++)
			{
				//	Assume little-endian
				Array.Copy(data, i * 3, convertBuffer, 0, 3);
				int currentValue = BitConverter.ToInt32(convertBuffer, 0);
				result[i] = currentValue / (float)0x7FFFFF;
			}

			return result;
		}

		public float[] Convert32BitToFloat(byte[] data)
		{
			float[] result = new float[data.Length / 4];
			for (int i = 0; i < result.Length; i++)
			{
				int currentValue = BitConverter.ToInt32(data, i * 4);
				result[i] = (float)(currentValue / (double)int.MaxValue);
			}
			return result;
		}
	}
}
