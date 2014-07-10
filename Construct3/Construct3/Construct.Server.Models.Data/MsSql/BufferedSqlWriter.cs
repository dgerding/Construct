using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace Construct.Server.Models.Data.MsSql
{
	/* UNUSED CODE. Intended to be used in SqlServerPropertyValuePersistor as an optimization,
	 *	this code has been shelved for use for now. */


	class TableBuffer
	{
		public DataTable DataTable;
		public DateTime LastCommitTime;
	}

	class PropertyTableFactory
	{
		private static Type BoolType		= typeof(bool);
		private static Type ByteArrayType	= typeof(byte[]);
		private static Type DateTimeType	= typeof(DateTime);
		private static Type DoubleType		= typeof(double);
		private static Type GuidType		= typeof(Guid);
		private static Type IntType			= typeof(int);
		private static Type SingleType		= typeof(float);
		private static Type StringType		= typeof(string);

		public DataTable CreateNewPropertyTableForType(Type propertyType)
		{
			DataTable newTable = new DataTable();
			newTable.Columns.Add(new DataColumn("ItemID", GuidType));
			newTable.Columns.Add(new DataColumn("SourceID", GuidType));
			newTable.Columns.Add(new DataColumn("PropertyID", GuidType));
			newTable.Columns.Add(new DataColumn("Interval", IntType)); // Type?
			newTable.Columns.Add(new DataColumn("StartTime", DateTimeType));
			newTable.Columns.Add(new DataColumn("Value", propertyType));
			newTable.Columns.Add(new DataColumn("Latitude", StringType));
			newTable.Columns.Add(new DataColumn("Longitude", StringType));

			return newTable;
		}
	}

	public class BufferedSqlWriter
	{
		Dictionary<String, TableBuffer> m_BufferedData = new Dictionary<string, TableBuffer>();
		String m_ConnectionString;

		readonly int m_MaxTimeBetweenCommitMS = 2000;
		readonly int m_MaxRowCountForCommit = 1000;

		public BufferedSqlWriter(String connectionString)
		{
			m_ConnectionString = connectionString;
		}

		public void AddPropertyValue(String dataTypeName, String propertyName, object propertyValue, Guid sourceID)
		{
			String tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
			TableBuffer buffer = GetTableBuffer(tableName);
			DataTable bufferTable = buffer.DataTable;

			//	Run insert
			//bufferTable.



			DataTable tableForCommit = null;
			lock (buffer)
			{
				if (TableRequiresCommit(buffer))
				{
					//	Run quick table swap
					tableForCommit = buffer.DataTable;
					buffer.DataTable = new DataTable();
				}
			}
			if (tableForCommit != null)
				ThreadPool.QueueUserWorkItem(RunBulkDataCopy, tableForCommit);
		}

		private void RunBulkDataCopy(object dataTable)
		{
			//	This DataTable should no longer be referenced by the BufferedSqlWriter, and so all
			//	operations on this table are "safe" (since only this method/thread has access to it)
			DataTable tableForCommit = dataTable as DataTable;

			//	TODO: Pull from connection cache?
			using (SqlConnection conn = new SqlConnection(m_ConnectionString))
			{
				using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
				{
					bulkCopy.DestinationTableName = tableForCommit.TableName;
					bulkCopy.WriteToServer(tableForCommit);
					bulkCopy.Close();
				}
			}
		}

		private bool TableRequiresCommit(TableBuffer tableBuffer)
		{
			return (DateTime.Now - tableBuffer.LastCommitTime).Milliseconds >= m_MaxTimeBetweenCommitMS ||
					tableBuffer.DataTable.Rows.Count >= m_MaxRowCountForCommit;
		}

		private TableBuffer GetTableBuffer(String tableName)
		{
			if (m_BufferedData.ContainsKey(tableName))
				return m_BufferedData[tableName];

			//	Note: Should invoke PropertyTableFactory
			TableBuffer newBuffer = new TableBuffer();
			newBuffer.DataTable = new DataTable(tableName);
			newBuffer.LastCommitTime = DateTime.Now;


			//	We permit multiple threads to try to create a new TableBuffer if one doesn't exist,
			//	but only allow one of them to actually insert.
			lock (m_BufferedData)
			{
				if (!m_BufferedData.ContainsKey(tableName))
					m_BufferedData.Add(tableName, newBuffer);
			}

			//	Reference m_BufferedData for return; if multiple threads try to create, they should
			//		all return the same object.
			return m_BufferedData[tableName];
		}
	}
}
