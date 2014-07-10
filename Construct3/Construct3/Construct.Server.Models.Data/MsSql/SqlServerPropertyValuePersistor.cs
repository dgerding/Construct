using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValue;

namespace Construct.Server.Models.Data.MsSql

{
    public static class SqlServerPropertyValuePersistor
    {
        public static void AddBooleanPropertyValue(SqlConnection conn, string dataTypeName, string propertyName, BooleanPropertyValue booleanArrayPropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}",dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddBooleanPropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = booleanArrayPropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = booleanArrayPropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = booleanArrayPropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = booleanArrayPropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = booleanArrayPropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.Bit).Value = booleanArrayPropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();
        }

        public static void AddByteArrayPropertyValue(SqlConnection conn, string dataTypeName, string propertyName, ByteArrayPropertyValue byteArrayPropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "z_InsertByteArrayInto_" + tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = byteArrayPropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = byteArrayPropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = byteArrayPropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = byteArrayPropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = byteArrayPropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.VarBinary).Value = byteArrayPropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();
            
        }

        public static void AddDateTimePropertyValue(SqlConnection conn, string dataTypeName, string propertyName, DateTimePropertyValue dateTimePropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddDateTimePropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = dateTimePropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = dateTimePropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = dateTimePropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = dateTimePropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = dateTimePropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.DateTime2).Value = dateTimePropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();
        }

        public static void AddDoublePropertyValue(SqlConnection conn, string dataTypeName, string propertyName, DoublePropertyValue doublePropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddDoublePropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = doublePropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = doublePropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = doublePropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = doublePropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = doublePropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.Float).Value = doublePropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();
            
        }

        public static void AddGuidPropertyValue(SqlConnection conn, string dataTypeName, string propertyName, GuidPropertyValue guidPropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddGuidPropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = guidPropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = guidPropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = guidPropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = guidPropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = guidPropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.UniqueIdentifier).Value = guidPropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();
        }

        public static void AddIntPropertyValue(SqlConnection conn, string dataTypeName, string propertyName, IntPropertyValue intPropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddIntPropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = intPropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = intPropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = intPropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = intPropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = intPropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.Int).Value = intPropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();
        }

        public static void AddSinglePropertyValue(SqlConnection conn, string dataTypeName, string propertyName, SinglePropertyValue singlePropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddSinglePropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = singlePropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = singlePropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = singlePropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = singlePropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = singlePropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.Real).Value = singlePropertyValue.Value;

			comm.ExecuteNonQuery();
            //SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            //reader.Close();
        }

        public static void AddStringPropertyValue(SqlConnection conn, string dataTypeName, string propertyName, StringPropertyValue stringPropertyValue, Guid SourceID)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string tableName = string.Format("z_{0}_{1}", dataTypeName, propertyName);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "AddStringPropertyValue";
            comm.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
            comm.Parameters.Add("@SourceID", SqlDbType.UniqueIdentifier).Value = SourceID;
            comm.Parameters.Add("@PropertyID", SqlDbType.UniqueIdentifier).Value = stringPropertyValue.PropertyID;
            comm.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier).Value = stringPropertyValue.ItemID;
            comm.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = stringPropertyValue.StartTime;
            comm.Parameters.Add("@Interval", DBNull.Value);
            comm.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = stringPropertyValue.Latitude;
            comm.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = stringPropertyValue.Longitude;
            comm.Parameters.Add("@Value", SqlDbType.NVarChar).Value = stringPropertyValue.Value;

			comm.ExecuteNonQuery();
			//SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
			//reader.Close();

        }
    }
}
