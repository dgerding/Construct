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
using NLog;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public class DataTypeCreator
    {
        private Entities.EntitiesModel context;
        private string connectionString;
        private SensorTypeSource queuedSensorTypeSource = null;
        private IList<DataType> queuedDataTypes = new List<DataType>();
        private IList<PropertyType> queuedPropertyTypes = new List<PropertyType>();

        public DataTypeCreator(Entities.EntitiesModel context, string connectionString)
        {
            this.context = context;
            this.connectionString = connectionString;
        }

        private void executeSqlStoredProcedure(string commandName, Dictionary<string, object> sqlParams)
        {
            SqlConnection conn = new SqlConnection();

            conn.ConnectionString = connectionString;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand comm = conn.CreateCommand();
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = commandName;

            if (sqlParams != null)
            {
                foreach (KeyValuePair<string, object> kvp in sqlParams)
                    comm.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            }
            SqlDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            reader.Close();
        }

        private bool isValidDataTypePropertyCreationInfo(DataTypePropertyCreationInfo dataTypePropertyCreationInfo)
        {
			//if (context.DataTypes.Where(t => t.FullName == dataTypePropertyCreationInfo.Type).Count() == 1 ||
			//	context.DataTypes.Where(t => t.Name == dataTypePropertyCreationInfo.Type).Count() == 1)

			/* Linq hack */
			//	TODO: Linq hack, replace
			foreach (var dataType in context.DataTypes)
            {
				if (dataType.Name == dataTypePropertyCreationInfo.Type || dataType.FullName == dataTypePropertyCreationInfo.Type)
	                return true;
            }

            return false;
        }

        //TODO: Rename to CreateDataType
        public bool CreateConstructType(DataTypeCreationInfo dataTypeInfo)
        {
            if (this.IsValidDataType(dataTypeInfo))
            {
                DataType dataType = new DataType();
                dataType.ID = dataTypeInfo.DataTypeID;
                dataType.Name = dataTypeInfo.TypeName;
                dataType.FullName = dataTypeInfo.TypeName;
                dataType.DataTypeSourceID = dataTypeInfo.DataTypeSourceID;
                queuedDataTypes.Add(dataType);

                foreach (var dataTypePropertyInfo in dataTypeInfo.Properties)
                {
                    if (this.isValidDataTypePropertyCreationInfo(dataTypePropertyInfo))
                    {
                        PropertyType propertyType = new PropertyType();
                        DataType propertyDataType = null;

						/* Linq code foreach hack (using foreach instead of Linq since expression doesn't evaluate correctly under Linq) */
						//	TODO: Unhack linq
						//if (context.DataTypes.Where(target => target.FullName == dataTypePropertyInfo.Type).Count() == 1)
						//{
						//	propertyDataType = context.DataTypes.Where(target => target.FullName == dataTypePropertyInfo.Type).Single();
						//}
						//else
						//{
						//	propertyDataType = context.DataTypes.Where(target => target.Name == dataTypePropertyInfo.Type).Single();
						//}

						foreach (var currentDataType in context.DataTypes)
						{
							if (currentDataType.FullName == dataTypePropertyInfo.Type) {
								propertyDataType = currentDataType;
								break;
							}

							if (currentDataType.Name == dataTypePropertyInfo.Type) {
								propertyDataType = currentDataType;
								break;
							}
						}

                        propertyType.ID = dataTypePropertyInfo.ID;
                        propertyType.Name = dataTypePropertyInfo.Name;
                        propertyType.ParentDataTypeID = dataType.ID;
                        propertyType.PropertyDataTypeID = propertyDataType.ID;
                        propertyType.DataType = propertyDataType;
                        queuedPropertyTypes.Add(propertyType);

						AddPropertyValuePersistenceSupport(dataTypeInfo.TypeName, propertyType);
                    }
                    else
                    {
                        throw new ApplicationException("Import type failed - datatype property type invalid.");
                    }
                }

                //AddItemHeaderProperties(dataType);

	            return true;
            }

	        return false;
        }

        public void AddPropertyValuePersistenceSupport(string dataTypeName, PropertyType propertyType)
        {
            bool isByteArray = false;
            string storedProcedureName = "";

            switch (propertyType.DataType.FullName)
            {
                case "System.Boolean":
                    storedProcedureName = "CreateBooleanPropertyValueTable";
                    break;
                case "System.Int32":
                    storedProcedureName = "CreateInt32PropertyValueTable";
                    break;
                case "System.Single":
                    storedProcedureName = "CreateSinglePropertyValueTable";
                    break;
                case "System.Double":
                    storedProcedureName = "CreateDoublePropertyValueTable";
                    break;
                case "System.Guid":
                    storedProcedureName = "CreateGuidPropertyValueTable";
                    break;
                case "System.DateTime":
                    storedProcedureName = "CreateDateTimePropertyValueTable";
                    break;
                case "System.String":
                    storedProcedureName = "CreateStringPropertyValueTable";
                    break;
                case "System.Byte[]":
                    isByteArray = true;
                    storedProcedureName = "CreateByteArrayPropertyValueTable";
                    break;
                default:
                    Exception e = new Exception("DataTypeCreator AddProperyValuePersistenceSupport could not find matching type.");
                    throw e;
                    //break;
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@datatypeName", dataTypeName);
            parameters.Add("@propertyName", ((Property)propertyType).Name);

            executeSqlStoredProcedure(storedProcedureName, parameters);
            

            if (isByteArray)
            {
                Dictionary<string, object> byteArrayParameters = new Dictionary<string, object>();
                byteArrayParameters.Add("@tableName ", "z_" + dataTypeName + "_" + propertyType.Name);
                executeSqlStoredProcedure("CreateInsertProcedureForByteArrayTable", byteArrayParameters); 
            }
        }

        //private void AddItemHeaderProperties(DataType dataType)
        //{
        //    PropertyType itemIDpropertyType = new PropertyType();
        //    DataType guidDataType = context.DataTypes.Where(target => target.FullName == "System.Guid").Single();

        //    itemIDpropertyType.ID = Guid.NewGuid();
        //    itemIDpropertyType.Name = "ItemID";
        //    itemIDpropertyType.ParentDataTypeID = dataType.ID;
        //    itemIDpropertyType.DataType = guidDataType;
        //    itemIDpropertyType.PropertyDataTypeID = guidDataType.ID;
        //    queuedPropertyTypes.Add(itemIDpropertyType);

        //    PropertyType recCreationPropertyType = new PropertyType();
        //    DataType dateTimeDataType = context.DataTypes.Where(target => target.FullName == "System.DateTime").Single();

        //    recCreationPropertyType.ID = Guid.NewGuid();
        //    recCreationPropertyType.Name = "RecordCreationDate";
        //    recCreationPropertyType.ParentDataTypeID = dataType.ID;
        //    recCreationPropertyType.DataType = dateTimeDataType;
        //    recCreationPropertyType.PropertyDataTypeID = dateTimeDataType.ID;
        //    queuedPropertyTypes.Add(recCreationPropertyType);
        //}

        private bool IsValidDataType(DataTypeCreationInfo dataTypeInfo)
        {
            if (dataTypeInfo.DataTypeSourceParentID == null || dataTypeInfo.DataTypeSourceParentID == Guid.Empty)
            {
                throw new ApplicationException("Invalid parent source provided to CreateConstructType.");
            }

            var parentSourceType = context.DataTypeSources.Where(t => t.ID == dataTypeInfo.DataTypeSourceParentID).SingleOrDefault();
            if (parentSourceType == null)
            {
                throw new ApplicationException("Invalid parent source provided to CreateConstructType.");
            }

            return true;
        }

        private bool IsValidImportType(XDocument document, string workingDirectory)
        {
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Construct.Server.Models.Data.ConstructSource.xsd");

                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("", XmlReader.Create(stream));

                document.Validate(schemas, (s, ex) =>
                {
                    throw ex.Exception;
                }); 

                return true;
            }
            catch
            {
                throw new ApplicationException("ConstructSource Schema Definition file is problematic.");
            }
        }

        public void ImportSensorDataTypeSource(string theXml)
        {
            XDocument document = XDocument.Parse(theXml);
            string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (this.IsValidImportType(document, executingDirectory))
            {
                queuedSensorTypeSource = GetSensorTypeSourceFromXElement(document);

                var existingSourceTypeIDCheck = context.DataTypeSources.Where(t => t.ID == queuedSensorTypeSource.ID).SingleOrDefault();
                var existingSourceTypeNameCheck = context.DataTypeSources.Where(t => t.Name == queuedSensorTypeSource.Name).SingleOrDefault();

                if (existingSourceTypeIDCheck == null && existingSourceTypeNameCheck == null)
                { 
                    List<Assembly> addedAssemblies = new List<Assembly>();
                    foreach (XElement dataTypeNode in document.Root.Nodes())
                    {
                        DataTypeCreationInfo dataTypeCreationInfo = GetDataTypeInfoFromXElement(queuedSensorTypeSource, dataTypeNode);
	                    if (!CreateConstructType(dataTypeCreationInfo))
	                    {
		                    // ...
	                    }
                    }

                    CommitQueuedDataType();
                }
                else
                {
                    queuedSensorTypeSource = null;
                }
            }
        }

        public void CommitQueuedDataType()
        {
            context.Add(queuedSensorTypeSource);
            context.SaveChanges();

            foreach (DataType dataType in queuedDataTypes)
            {
                context.Add(dataType);
            }
            context.SaveChanges();

            foreach (PropertyType propertyType in queuedPropertyTypes)
            {
                context.Add(propertyType);
            }
            context.SaveChanges();

            queuedSensorTypeSource = null;
            queuedDataTypes.Clear();
            queuedPropertyTypes.Clear();
        }

        private DataTypeCreationInfo GetDataTypeInfoFromXElement(DataTypeSource dataTypeSource, XElement dataTypeNode)
        {
            DataTypeCreationInfo dataTypeInfo = new DataTypeCreationInfo();

            dataTypeInfo.DataTypeID = Guid.Parse(dataTypeNode.Attribute("ID").Value);
            dataTypeInfo.SourceName = dataTypeSource.Name;
            dataTypeInfo.TypeName = dataTypeNode.Attribute("Name").Value;
            dataTypeInfo.DataTypeSourceParentID = (Guid)dataTypeSource.ParentID;
            dataTypeInfo.DataTypeSourceID = dataTypeSource.ID;

            foreach (XElement dataTypeProperty in dataTypeNode.Nodes())
            {
                string name = dataTypeProperty.Attribute("Name").Value;
                string type = dataTypeProperty.Attribute("Type").Value;
                Guid ID = Guid.Parse(dataTypeProperty.Attribute("ID").Value);

                DataTypePropertyCreationInfo dataPropertyInfo = new DataTypePropertyCreationInfo(name, type, ID); // TODO: Make decision about referencing type names by string

				/* LINQ is weird with this, hack it together with a foreach for now */
				//if (context.DataTypes.Where(t => t.FullName == type).Count() == 1 ||
				//	context.DataTypes.Where(t => t.Name == type).Count() == 1)

				//	TODO: Unhack Linq
				bool matched = false;
				foreach (var dataType in context.DataTypes)
                {
					if (dataType.Name == type || dataType.FullName == type)
					{
						matched = true;
						break;
					}
                }

				if (matched)
				{
					dataTypeInfo.Properties.Add(dataPropertyInfo);
				}
				else
				{
					throw new ApplicationException("Import type failed - datatype property type does not exist.");
				}
            }

            return dataTypeInfo;
        }

        private SensorTypeSource GetSensorTypeSourceFromXElement(XDocument document)
        {
            //Use root attribtutes from import xml to create TypeSource entity
            SensorTypeSource sensorTypeSource = new SensorTypeSource();

            sensorTypeSource.Name = document.Root.Attribute("Name").Value;
            sensorTypeSource.ID = Guid.Parse(document.Root.Attribute("ID").Value);
            sensorTypeSource.SensorHostTypeID = Guid.Parse(document.Root.Attribute("SensorHostTypeID").Value);
            sensorTypeSource.Version = document.Root.Attribute("Version").Value;
            sensorTypeSource.IsCategory = false;
            sensorTypeSource.IsReadOnly = false;

            Guid attemptedSensorTypeSourceParentID = Guid.Parse(document.Root.Attribute("ParentID").Value);
            //Check existing parent
            var existingParentDataTypeSource = context.DataTypeSources.Where(t => t.ID == attemptedSensorTypeSourceParentID).SingleOrDefault();
            if (existingParentDataTypeSource == null)
            {
                throw new ApplicationException("Typesource ParentID not found");
            }
            else
            {
                sensorTypeSource.ParentID = attemptedSensorTypeSourceParentID;
            }

            return sensorTypeSource;
        }
    }
}