using System;

namespace EmailReader
{
	class EmailReaderDriver
	{
		static void Main(string[] args)
		{
			EmailReaderSensor obj = new EmailReaderSensor(args);
			Console.ReadLine();
		}
	}
}
