using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

/*
 * Provide an alternative SqlConnection pool, default pool hits max too quickly
 */

namespace Construct.Server.Models.Data
{
	class DataStoreConnectionPool : IDisposable
	{
		String m_ConnectionString;
		ConcurrentQueue<SqlConnection> m_AvailableConnections = new ConcurrentQueue<SqlConnection>();

		public DataStoreConnectionPool(String connectionString)
		{
			m_ConnectionString = connectionString;

			//	Generate 4 connections initially
			for (int i = 0; i < 4; i++)
			{
				m_AvailableConnections.Enqueue(new SqlConnection(connectionString));
			}
		}

		public SqlConnection GetUnusedConnection()
		{
			SqlConnection availableConnection;
			if (!m_AvailableConnections.TryDequeue(out availableConnection))
			{
				availableConnection = new SqlConnection(m_ConnectionString);
			}

			return availableConnection;
		}

		public void FinishConnection(SqlConnection connection)
		{
			m_AvailableConnections.Enqueue(connection);
		}

		public void FreeAllConnections()
		{
			while (!m_AvailableConnections.IsEmpty)
			{
				SqlConnection conn;
				if (m_AvailableConnections.TryDequeue(out conn))
					conn.Close();
			}
		}

		public void Dispose()
		{
			FreeAllConnections();
		}
	}
}
