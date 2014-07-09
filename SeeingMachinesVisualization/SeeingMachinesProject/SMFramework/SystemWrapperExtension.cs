using System;
using SystemWrapper.IO;

/*
 * Extensions for the SystemWrapper library, which wraps various System components for mocking.
 * 
 */

namespace SMFramework
{
	public static class SystemWrapperExtension
	{
		public static void WriteLine(this IStreamWriterWrap writer)
		{
			writer.Write(writer.NewLine);
		}

		public static void WriteLine(this IStreamWriterWrap writer, String value)
		{
			writer.Write(value + writer.NewLine);
		}
	}
}
