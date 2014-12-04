using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Construct.Server.Models.Data.MsSql
{
    static public class ExecuteMsSqlScript
    {
		static public bool GoStoredProcedure(string connectionString, string procedureName)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.CommandText = procedureName;

				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception e)
				{
					return false;
				}

				return true;
			}
		}

        static public bool Go(string connectionString, string sqlFile)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "";

                using (FileStream strm = File.OpenRead(sqlFile))
                {
                    StreamReader reader = new StreamReader(strm);
                    sql = reader.ReadToEnd();
                }

                Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string[] lines = regex.Split(sql);

                SqlTransaction transaction = connection.BeginTransaction();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.Transaction = transaction;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            cmd.CommandText = line;
                            //cmd.CommandType = CommandType.Text;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                }

				try
				{
					transaction.Commit();
				}
				catch (Exception e)
				{
					transaction.Rollback();
					return false;
				}

                return true;
            }
        }
    }
}