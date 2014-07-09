using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * The DatabaseWriter maintains a "Column-Map", which is a mapping of various column names and position
 *	within the written CSV. Generally implemented via "List<String> columnMap", columnMap[5] would
 *	indicate the column label for the 6th column in the CSV.
 *	
 * ex:
 * A, B, C, D, E, X, G, ...
 * ., ., ., ., ., ., ., ...
 * ., ., ., ., ., ., ., ...
 * 
 * X is the 6th column, and X is the 6th element in the columnMap. The columnMap's contents would be:
 * { "A", "B", "C", "D", "E", "X", "G", ... }
 *
 */

namespace SMFramework
{
	public class StreamNotOpenException : Exception
	{
		public override string Message
		{
			get { return "Unable to close the stream because the stream was not open."; }
		}
	}

	public class DatabaseWriter : IDisposable
	{
		StreamWriter m_Writer = null;
		List<String> m_ColumnMap = null;

		public void Dispose()
		{
			if( m_Writer != null )
				m_Writer.Dispose();
		}

		/* Opens the writer without a column-map, meaning that the columns specified by the first DataSnapshot define
		 * the standard for the entire database.
		 */
		public void Open(String targetFile)
		{
			if (m_Writer != null)
			{
				throw new Exception(
					"DatabaseWriter unable to write to file '" + targetFile + "'\nThe write-stream is already open."
					);
			}

			m_Writer = new StreamWriter(targetFile, false);
		}


		/* Allows the user to write with a predetermined column-map; snapshots that are missing a column
		 * specified in columnMap are automatically given a value for that column with the default of 0.
		 * Snapshots that have an extra column are discarded and a warning is displayed.
		 */
		public void Open(String targetFile, List<String> columnMap)
		{
			if (m_Writer != null)
			{
				throw new Exception(
					"DatabaseWriter unable to write to file '" + targetFile + "'\nThe write-stream is already open."
					);
			}

			if (WriterUtilities.ColumnMapHasDuplicates(columnMap))
			{
				throw new Exception(
					"DatabaseWriter unable to write to file '" + targetFile + "'\nThe specified columnMap contains duplicate labels."
					);
			}

			m_Writer = new StreamWriter(targetFile, false);

			m_ColumnMap = columnMap;

			WriterUtilities.WriteHeader(m_ColumnMap, m_Writer);
		}

		public void WriteSnapshot(DataSnapshot snapshot)
		{
			if (m_ColumnMap == null)
			{
				m_ColumnMap = WriterUtilities.GenerateMappingFromSnapshot(snapshot);
				WriterUtilities.WriteHeader(m_ColumnMap, m_Writer);
			}

			WriterUtilities.FillMissingColumns(snapshot, m_ColumnMap);
			WriterUtilities.WriteSnapshot(snapshot, m_Writer, m_ColumnMap);
		}

		public void Close()
		{
			if (m_Writer == null)
				throw new StreamNotOpenException();

			m_Writer.Close();
			m_Writer = null;
		}
	}

	internal static class WriterUtilities
	{
		public static void WriteHeader(List<String> columnMap, StreamWriter writer)
		{
			/* Timestamp is a mandatory column */
			writer.Write(DatabaseFormatMapping.TimestampColumnLabel + ",");

			for (int i = 0; i < columnMap.Count; i++)
			{
				writer.Write(columnMap[i] + ",");
			}

			writer.WriteLine();
		}

		public static void WriteSnapshot(DataSnapshot snapshot, StreamWriter writer, List<String> columnMap)
		{
			/* Manually write out timestamp */
			writer.Write(snapshot.TimeStamp.ToStringUTC() + ",");

			for (int i = 0; i < columnMap.Count; i++)
			{
				writer.Write(snapshot.Data[columnMap[i]] + ",");
			}

			writer.WriteLine();
		}

		public static bool SnapshotConformsToMapping(DataSnapshot snapshot, List<String> columnMap)
		{
			return
				GetExtraSnapshotColumns(snapshot, columnMap).Count == 0; /* &&
				GetMissingSnapshotColumns(snapshot, columnMap).Count == 0; */
		}

		public static void FillMissingColumns(DataSnapshot snapshot, List<String> columnMap)
		{
			foreach (String column in columnMap)
			{
				if (!snapshot.Data.ContainsKey(column))
					snapshot.Data.Add(column, 0);
			}
		}

		public static List<String> GetExtraSnapshotColumns(DataSnapshot snapshot, List<String> columnMap)
		{
			return new List<String>(snapshot.Data.Keys.Except(columnMap));
		}

		public static List<String> GetMissingSnapshotColumns(DataSnapshot snapshot, List<String> columnMap)
		{
			return new List<String>(columnMap.Except(snapshot.Data.Keys));
		}

		public static List<String> GenerateMappingFromSnapshot(DataSnapshot sourceSnapshot)
		{
			return new List<String>(sourceSnapshot.Data.Keys);
		}

		public static bool ColumnMapHasDuplicates(List<String> columnMap)
		{
			return columnMap.GroupBy(n => n).Any(a => a.Count() > 1);
		}
	}
}
