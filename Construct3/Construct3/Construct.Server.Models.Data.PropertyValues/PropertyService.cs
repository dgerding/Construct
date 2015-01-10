using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using Construct.Server.Entities;
using Construct.Utilities.Shared;
using NLog;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValue
{

    public interface IPropertyService<P>
        where P : new()
    {

        IEnumerable<P> GetAll();

        IEnumerable<P> GetAfter(DateTime time);
    }

    public abstract class PropertyService<P, T> : IPropertyService<P>
        where P : new()
    {
        private string dataTypeName;
        private string propertyName;
        private string connectionString;
        private string propertyTypeName;
        private EntitiesModel context = null;
        private SqlConnection conn;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PropertyService(Uri serverUri, DataType type, Property property, string connectionString)
        {
            logger.Trace("instantiating Sources model");

            Location = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, type.Name, property.Name);

            this.connectionString = connectionString;
            this.dataTypeName = type.Name;
            this.propertyName = property.Name;
            this.context = new EntitiesModel(connectionString);

            conn = new SqlConnection(connectionString);
        }
        public PropertyService(Uri serverUri, string type, string property, string connectionString)
        {
            logger.Trace("instantiating Sources model");

            Location = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, type, property);

            this.connectionString = connectionString;
            this.dataTypeName = type;
            this.propertyName = property;
            this.context = new EntitiesModel(connectionString);

            conn = new SqlConnection(connectionString);
        }


        protected abstract T GetValue(SqlDataReader reader);

        public IEnumerable<P> GetAll()
        {
            return QueryDatabaseForPropertyTableEntries();
        }

        private IEnumerable<P> QueryDatabaseForPropertyTableEntries(DateTime? startTime = null, DateTime? endTime = null)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "GetAllPropertyValues";
			//	Set full CommandText before assigning parameters
			//	http://stackoverflow.com/questions/27790144/difference-between-sql-query-and-sqlclient-operation
	        if (startTime.HasValue && !endTime.HasValue)
		        comm.CommandText += "After";
	        if (startTime.HasValue && endTime.HasValue)
		        comm.CommandText += "Between";

            comm.Parameters.Add("@dataTypeName", SqlDbType.NVarChar).Value = dataTypeName;
            comm.Parameters.Add("@propertyName", SqlDbType.NVarChar).Value = propertyName;
            if (startTime.HasValue)
            {
				var startString = startTime.Value.ToUniversalTime().ToString("O");

				if (endTime.HasValue)
				{
					comm.Parameters.Add("@startTime", SqlDbType.DateTime2).Value = startTime.Value.ToUniversalTime();
					comm.Parameters.Add("@endTime", SqlDbType.DateTime2).Value = endTime.Value.ToUniversalTime();
				}
				else
				{
					comm.Parameters.Add("@dateTime", SqlDbType.DateTime2).Value = startString;
				}
            }

            SqlDataReader reader = comm.ExecuteReader();


            List<P> propertyValueCollection = new List<P>();
            while (reader.Read())
            {
                dynamic propertyValue = new P();

                propertyValue.ItemID = reader.GetGuid(0);
                propertyValue.SourceID = reader.GetGuid(1);
                propertyValue.PropertyID = reader.GetGuid(2);

                if (reader.IsDBNull(3) == false)
                {
                    propertyValue.Interval = reader.GetInt32(3);
                }
                else
                {
                    propertyValue.Interval = null;
                }
	            propertyValue.StartTime = DateTime.SpecifyKind(reader.GetDateTime(4), DateTimeKind.Utc);
                propertyValue.Latitude = reader.GetString(6);
                propertyValue.Longitude = reader.GetString(7);

                object tempValue = GetValue(reader);
                propertyValue.Value = (T)tempValue;

                P result = propertyValue;
                
                if (startTime.HasValue == false || (startTime.HasValue == true && startTime.Value < propertyValue.StartTime))
                {
                    propertyValueCollection.Add(result);
                }
            }

            reader.Close();
            reader.Dispose();
                    
            comm.Dispose();

            return propertyValueCollection.AsEnumerable();
        }

        public IEnumerable<P> GetAfter(DateTime time)
        {
            return QueryDatabaseForPropertyTableEntries(time);
        }

		public IEnumerable<P> GetBetween(DateTime start, DateTime end)
		{
			return QueryDatabaseForPropertyTableEntries(start, end);
		}

        public Uri Location
        {
            get;
            private set;
        }
        /*
        private PropertyCollection<P, T> GetAllFromSqlServerDatabase()
        {
            PropertyCollection<P, T> returnCollection = new PropertyCollection<P, T>();

            try
            {
                SqlConnection sqlConnection1 = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "StoredProcedureName";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                }
            
                sqlConnection1.Close();
            }
            catch (Exception e)
            {
                //TODO: Handle logging here

            }

            return returnCollection;
        }
        */
    }
}