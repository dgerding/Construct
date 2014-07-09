using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMFramework;
using System;
using System.Diagnostics;
using System.IO;

namespace SMFrameworkTests
{
	[TestClass]
	public class AsyncPairedDatabaseTest : AsyncPairedDatabase
	{
		[TestMethod]
		public void TestComponent()
		{
			DataSnapshot
				s1 = new DataSnapshot(new DateTime(0)),
				s2 = new DataSnapshot(new DateTime(0));

			s1.Data.Add("A", 5);
			s1.Data.Add("B", 10);
			s1.Data.Add("C", 15);

			s2.Data.Add("A", 500);
			s2.Data.Add("B", 600);
			s2.Data.Add("C", 700);

			this.WriteQueue.Enqueue(s1);
			this.WriteQueue.Enqueue(s2);

			String target = "recording.csv";
			this.Start(target);
			this.Stop();

			using (StreamReader reader = new StreamReader(target))
			{
				String result = reader.ReadToEnd();

				String expected =
@"SeeingMachines PDB V:102
Time (UTC),A,B,C,
0001-01-01T00:00:00.000Z,5,10,15,
0001-01-01T00:00:00.000Z,500,600,700,
";

				for (int i = 0; i < result.Length; i++)
				{
					if (result[i] != expected[i])
						Debugger.Break();
				}

				Assert.AreEqual(expected, result);
			}
		}
	}
}
