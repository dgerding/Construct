using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMFrameworkTests
{
	[TestClass]
	public class DatabaseWriterTest
	{
		static String TargetTestFilename = "test.csv";
		static List<String> TestColumnMap = new List<String>()
		{
				"Column A",
				"Column B",
				"Column C",
				"Column D"
		};

		static String TestHeader =
@"SeeingMachines PDB V:102
Time (UTC),Column A,Column B,Column C,Column D,
";

		[TestMethod]
		public void TestOpen_NoColumnMap()
		{
			String testFile = TargetTestFilename + RandomString.Generate();

			DatabaseWriter writer = new DatabaseWriter();
			writer.Open(testFile);
			writer.Close();
		}

		[TestMethod]
		public void TestOpen_HasColumnMap()
		{
			String testFile = TargetTestFilename + RandomString.Generate();

			DatabaseWriter writer = new DatabaseWriter();
			writer.Open(testFile, TestColumnMap);
			writer.Close();

			String actual = File.ReadAllText(testFile);
			Assert.AreEqual(actual, TestHeader);
		}

		[TestMethod, ExpectedException(typeof(StreamNotOpenException))]
		public void TestClose()
		{
			DatabaseWriter writer = new DatabaseWriter();
			writer.Close();
		}

		[TestMethod]
		public void TestGetExtraSnapshotColumns()
		{
			DataSnapshot testSnapshot = new DataSnapshot();
			testSnapshot.Data.Add("Column A", 0);
			testSnapshot.Data.Add("ninjas", 0);
			testSnapshot.Data.Add("C", 0);
			testSnapshot.Data.Add("Column D", 0);
			testSnapshot.Data.Add("reddit", 0);

			var result = WriterUtilities.GetExtraSnapshotColumns(testSnapshot, TestColumnMap);
			var expected = new List<String>()
			{
				"ninjas",
				"C",
				"reddit"
			};

			if (
				result.Count == expected.Count &&
				!result.Except(expected).Any()
				)
			{
				//	They contain the same elements; we don't worry about duplicates since
				//		dictionaries can't contain duplicates. Duplicates in the column-map
				//		aren't a concern for this function
				return;
			}
			else
			{
				Assert.Fail();
			}
		}

		[TestMethod]
		public void TestColumnMapHasDuplicates()
		{
			var duplicateList = new List<String>() {
				"A",
				"B",
				"C",
				"D",
				"D",
				"E"
			};
			Assert.IsTrue(WriterUtilities.ColumnMapHasDuplicates(duplicateList));

			var validList = new List<String>() {
				"A",
				"B",
				"C",
				"D",
				"E"
			};
			Assert.IsFalse(WriterUtilities.ColumnMapHasDuplicates(validList));
		}

		[TestMethod]
		public void TestFillMissingColumns()
		{
			List<String> expectedColumns = new List<String> {
				"A",
				"B",
				"Tyler",
				"Left",
				"Red",
				"Green"
			};

			DataSnapshot missingSnapshot = new DataSnapshot();
			missingSnapshot.Data = new Dictionary<String, double> {
				{ "Tyler", 0.0 },
				{ "Green", 0.0},
			};

			WriterUtilities.FillMissingColumns(missingSnapshot, expectedColumns);

			bool listsMatch;

			listsMatch =
				missingSnapshot.Data.Keys.Count == expectedColumns.Count &&
				!missingSnapshot.Data.Keys.Except(expectedColumns).Any();

			Assert.IsTrue(listsMatch);


			/* Throw in a column that the column-map doesn't have; the function should
			 * ignore the extra column, causing the snapshot columns and expected columns
			 * to be correctly different.
			 */
			missingSnapshot.Data = new Dictionary<string, double> {
				{ "Tyler", 0.0 },
				{ "Green", 0.0 },
				{ "New Column", 0.0 }
			};

			listsMatch =
				missingSnapshot.Data.Keys.Count == expectedColumns.Count &&
				!missingSnapshot.Data.Keys.Except(expectedColumns).Any();
			Assert.IsFalse(listsMatch);
		}

		[TestMethod]
		public void TestGetMissingSnapshotColumns()
		{
			List<String> expected = new List<String> {
				"Column B",
				"Column C"
			};

			DataSnapshot snapshot = new DataSnapshot();
			snapshot.Data = new Dictionary<string,double>{
				{ "Column A", 0.0 },
				{ "Column D", 0.0 }
			};

			List<String> missing = WriterUtilities.GetMissingSnapshotColumns(snapshot, TestColumnMap);

			Assert.IsTrue(
				missing.Count == expected.Count &&
				!missing.Except(expected).Any()
				);
		}

		[TestMethod]
		public void TestGenerateMappingFromSnapshot()
		{
			DataSnapshot fakeSample = new DataSnapshot();
			fakeSample.Data = new Dictionary<string,double> {
				{ "Column A", 0.0 },
				{ "Column B", 0.0 },
				{ "Column C", 0.0 },
				{ "Column D", 0.0 }
			};

			List<String> mapping = WriterUtilities.GenerateMappingFromSnapshot(fakeSample);

			Assert.IsTrue(
				fakeSample.Data.Keys.Count == mapping.Count &&
				!fakeSample.Data.Keys.Except(TestColumnMap).Any()
				);
		}

		[TestMethod]
		public void TestSnapshotConformsToMapping()
		{
			DataSnapshot correctSnapshot;
			DataSnapshot extraColumnsSnapshot;
			DataSnapshot missingColumnsSnapshot;
			DataSnapshot extraMissingColumnsSnapshot;

			correctSnapshot = new DataSnapshot();
			correctSnapshot.Data = new Dictionary<string,double>{
				{ "Column A", 0.0 },
				{ "Column B", 0.0 },
				{ "Column C", 0.0 },
				{ "Column D", 0.0 }
			};

			extraColumnsSnapshot = new DataSnapshot();
			extraColumnsSnapshot.Data = new Dictionary<string, double>{
				{ "Column A", 0.0 },
				{ "Column B", 0.0 },
				{ "Column C", 0.0 },
				{ "Column D", 0.0 },
				{ "Column G", 0.0 },
				{ "Column E", 0.0 }
			};

			missingColumnsSnapshot = new DataSnapshot();
			missingColumnsSnapshot.Data = new Dictionary<string,double>{
				{ "Column A", 0.0 },
				{ "Column B", 0.0 },
				{ "Column C", 0.0 },
			};

			extraMissingColumnsSnapshot = new DataSnapshot();
			extraMissingColumnsSnapshot.Data = new Dictionary<string,double>{
				{ "Column A", 0.0 },
				{ "Column C", 0.0 },
				{ "Column F", 0.0 },
				{ "Column 4", 0.0 }
			};

			Assert.IsTrue(WriterUtilities.SnapshotConformsToMapping(correctSnapshot, TestColumnMap));
			Assert.IsFalse(WriterUtilities.SnapshotConformsToMapping(extraColumnsSnapshot, TestColumnMap));
			Assert.IsTrue(WriterUtilities.SnapshotConformsToMapping(missingColumnsSnapshot, TestColumnMap));
			Assert.IsFalse(WriterUtilities.SnapshotConformsToMapping(extraMissingColumnsSnapshot, TestColumnMap));
		}

		[TestMethod]
		public void TestWriteSnapshot()
		{
			String expected = (new DateTime(0)).ToStringUTC() + ",1,2,3,4," + Environment.NewLine;

			DataSnapshot snapshot = new DataSnapshot(new DateTime(0));
			snapshot.Data = new Dictionary<string, double>{
				{ "Column A", 1.0 },
				{ "Column B", 2.0 },
				{ "Column C", 3.0 },
				{ "Column D", 4.0 }
			};

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			WriterUtilities.WriteSnapshot(snapshot, writer, TestColumnMap);
			writer.Flush();

			String actual = Encoding.UTF8.GetString(stream.ToArray());

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void TestWriteHeader()
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			WriterUtilities.WriteHeader(TestColumnMap, writer);
			writer.Flush();

			String text = Encoding.UTF8.GetString(stream.ToArray());

			Assert.AreEqual(text, TestHeader);
		}
	}
}
