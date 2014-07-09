using System;

namespace SMFrameworkTests
{
	public class RandomString
	{
		public static Random g_Random;

		public static String Generate(uint characterCount = 6)
		{
			String result = "";

			lock (g_Random)
			{
				for (int i = 0; i < characterCount; i++)
					result += (char)(g_Random.NextDouble() * (90 - 65) + 65);
			}

			return result;
		}
	}
}
