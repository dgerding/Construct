using System;

namespace SMFramework
{
	public class ConsoleOutputStream : DebugOutputStream
	{
		public override void WriteLine(string text)
		{
			Console.WriteLine(text);
		}
	}

	public class ExceptionOutputStream : DebugOutputStream
	{
		public override void WriteLine(string text)
		{
			throw new Exception(text);
		}
	}

	public class DebuggerOutputStream : DebugOutputStream
	{
		public override void WriteLine(string text)
		{
			System.Diagnostics.Debug.WriteLine(text);
		}
	}

	public class FakeOutputStream : DebugOutputStream
	{
		public override void WriteLine(string text)
		{
		}
	}

	public abstract class DebugOutputStream
	{
		/// <summary>
		/// Output stream for data that is updated frequently (i.e. text that is continually refreshed)
		/// </summary>
		public static DebugOutputStream FastInstance = new FakeOutputStream();

		/// <summary>
		/// Output stream for data that is rarely updated (i.e. a commandline console)
		/// </summary>
		public static DebugOutputStream SlowInstance = new ConsoleOutputStream();

		public abstract void WriteLine(String text);
	}
}
