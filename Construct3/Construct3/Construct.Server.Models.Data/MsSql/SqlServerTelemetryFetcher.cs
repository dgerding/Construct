using Construct.Server.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NLog;

namespace Construct.Server.Models.Data.MsSql
{
	public class SqlServerTelemetryFetcher
	{

        protected static Logger logger = LogManager.GetCurrentClassLogger();
		public SqlConnection Connection { get; private set; }

		public SqlServerTelemetryFetcher(SqlConnection connection)
		{
			this.Connection = connection;
		}

		public DateTime? GetEarliestTimeForTypeAndSource(Guid dataTypeId, Guid sourceId)
		{
			SqlCommand command = Connection.CreateCommand();
			command.CommandText = @"SELECT MIN(RecordCreationTime) FROM Items_Item
									WHERE SourceId=@sourceID
									AND DataTypeID=@dataTypeID";
			command.Parameters.Add("@sourceID", System.Data.SqlDbType.UniqueIdentifier).Value = sourceId;
			command.Parameters.Add("@dataTypeID", System.Data.SqlDbType.UniqueIdentifier).Value = dataTypeId;

			var dataReader = command.ExecuteReader();

			if (dataReader.FieldCount == 0)
				return null;

			DateTime earliestTime;
			try
			{
				if (!dataReader.Read())
					return null;

				earliestTime = dataReader.GetDateTime(0);
				return earliestTime;
			}
			catch (Exception e)
			{
                return null;
			}
		}

		public DateTime? GetLatestTimeForTypeAndSource(Guid dataTypeId, Guid sourceId)
		{
			SqlCommand command = Connection.CreateCommand();
			command.CommandText = @"SELECT MAX(RecordCreationTime) FROM Items_Item
									WHERE SourceId=@sourceID
									AND DataTypeID=@dataTypeID";
			command.Parameters.Add("@sourceID", System.Data.SqlDbType.UniqueIdentifier).Value = sourceId;
			command.Parameters.Add("@dataTypeID", System.Data.SqlDbType.UniqueIdentifier).Value = dataTypeId;

			var dataReader = command.ExecuteReader();

			if (dataReader.FieldCount == 0)
				return null;

			DateTime earliestTime;
			try
			{
				if (!dataReader.Read())
					return null;

				earliestTime = dataReader.GetDateTime(0);
				return earliestTime;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public uint GetNumberOfItemsInTimeSpan(DateTime startTime, DateTime endTime, Guid? itemTypeId, Guid? sourceId)
		{
			SqlCommand command = Connection.CreateCommand();
			command.CommandText = @"SELECT COUNT(RecordCreationTime) FROM Items_Item WHERE RecordCreationTime BETWEEN @startTime AND @endTime";
			command.Parameters.Add("@startTime", System.Data.SqlDbType.DateTime2).Value = startTime;
			command.Parameters.Add("@endTime", System.Data.SqlDbType.DateTime2).Value = endTime;
			if (itemTypeId.HasValue)
			{
				command.CommandText += " AND DataTypeID=@itemTypeId";
				command.Parameters.Add("@itemTypeId", System.Data.SqlDbType.UniqueIdentifier).Value = itemTypeId.Value;
			}

			if (sourceId.HasValue)
			{
				command.CommandText += " AND SourceId=@sourceId";
				command.Parameters.Add("@sourceId", System.Data.SqlDbType.UniqueIdentifier).Value = sourceId.Value;
			}

			try
			{
				var reader = command.ExecuteReader();
				if (reader.FieldCount == 0)
					return 0;

				reader.Read();
				return (uint)reader.GetInt32(0);
			}
			catch (Exception e)
			{
				return 0;
			}
		}
	}
}
