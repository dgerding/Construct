using System;
using System.Linq;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.Windows.Zip;
using NLog;
using Construct.MessageBrokering;
using Construct.MessageBrokering.Serialization;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValue;
using Construct.Server.Models.Data.CodeGeneration;
using Construct.Utilities.Shared;
using Construct.Server.Models.Data.MsSql;
using System.ServiceModel;
using System.Xml.Linq;
using System.Threading;
using Construct.MessageBrokering.TransponderService;
using System.Threading.Tasks;

namespace Construct.Server.Models.Data
{
    /// <summary>
    /// Model of Data tier.
    /// </summary>
    public class Model : Models.Model, IModel
    {
        private readonly Broker broker = null;
        private string connectionString = null;
        private Uri serverServiceUri = null;
        private MetadataSource source = null;
        private ConstructSerializationAssistant assistant;

        private List<int> assignedPorts = new List<int>();

        private int thisPortBase = 0;

        private Dictionary<Guid, ReadOnlyCollection<Uri>> propertyValueServiceEndpoints;

        private DataStoreConnectionPool connectionPool;
        private SwitchingItemPersistor itemPersistor;
	    private ItemRealtimeStreamer itemStreamer;

        private bool isDatabaseReachable = false;

        public bool IsDatabaseReachable
        {
            get
            {
                return isDatabaseReachable;
            }
        }

        private bool isDatabaseSchemaCurrent = false;

        public bool IsDatabaseSchemaCurrent
        {
            get 
            { 
                return isDatabaseSchemaCurrent; 
            }
        }

        private bool isModelFaultedOnConstruction = false;

        public bool IsModelFaultedOnConstruction
        {
            get
            {
                return isModelFaultedOnConstruction;
            }
        }

        private void CheckDatabaseStatus()
        {
            return;
        }

        public Model(Uri serverServiceUri, string connectionString, Models.IServer server)
        {

            Name = "Data";

            this.connectionString = connectionString;
            
            logger.Trace("instantiating Data model");

            try
            {
                
                using (EntitiesModel entitiesModel = GetNewModel())
                {
                    logger.Trace("checking database connection and schema");

                    var connectionState = entitiesModel.Connection.State;

                    isDatabaseReachable = true;

                    Telerik.OpenAccess.ISchemaHandler schemaHandler = entitiesModel.GetSchemaHandler();
                    string script = null;
                    if (schemaHandler.DatabaseExists())
                    {
                        script = schemaHandler.CreateUpdateDDLScript(null);
                        isDatabaseSchemaCurrent = false;
                    }
                    else
                    {
                        schemaHandler.CreateDatabase();
                        script = schemaHandler.CreateDDLScript();
                    }
                    if (!string.IsNullOrEmpty(script))
                    {
                        isDatabaseSchemaCurrent = false;
                        entitiesModel.Connection.Close();
                        schemaHandler.ExecuteDDLScript(script);
                    }
                    else
                    {
                        isDatabaseSchemaCurrent = true;
                    }
    
                    if (isDatabaseReachable && isDatabaseSchemaCurrent)
                    { 
                        this.serverServiceUri = serverServiceUri;

                        thisPortBase = serverServiceUri.Port + (int)UriUtility.ServicePortOffsets.Data;
                        propertyValueServiceEndpoints = new System.Collections.Generic.Dictionary<Guid, ReadOnlyCollection<Uri>>();
            
                        broker = server.Broker;
                        broker.OnItemReceived += HandleItem;
                        Name = "Data";
            
                        //TypesAssemblyCreator assemblyCreator = new TypesAssemblyCreator(entitiesModel);
						//var dataTypeList = entitiesModel.DataTypes.Where(dt => dt.IsCoreType == false).ToList();
						//foreach (var dataType in dataTypeList)
						//	assemblyCreator.ReturnTypeAssembly(dataType);

						//Parallel.ForEach(dataTypeList, (dataType) => assemblyCreator.ReturnTypeAssembly(dataType));

                        InitializeSerializationAssistant();

                        InitializePropertyValueServicesAndBuildEndpointDictionary();
						
                        InitializeModelCache();

                        connectionPool = new DataStoreConnectionPool(connectionString);
                        itemPersistor = new SwitchingItemPersistor(assistant, connectionString, "ItemsCache");
						//	SigR port on 15999
						itemStreamer = new ItemRealtimeStreamer("http://*:15999/00000000-0000-0000-0000-000000000000/Data/", assistant);
                    }
                }
            }
            catch (Exception e)
            {
                isModelFaultedOnConstruction = true;
                
                //logger.LogException(, "Constructor for Model.Data failed", e);
            }
        }

        private void InitializeSerializationAssistant()
        {
            assistant = new ConstructSerializationAssistant(AddBooleanPropertyValue,
                AddByteArrayPropertyValue,
                AddDateTimePropertyValue,
                AddDoublePropertyValue,
                AddGuidPropertyValue,
                AddIntPropertyValue,
                AddSinglePropertyValue,
                AddStringPropertyValue);
            try
            {
                using (EntitiesModel model = GetNewModel())
                {
                    foreach (DataType dataType in model.DataTypes)
                    {
                        AddPropertyIDHelper(dataType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddPropertyIDHelper(DataType dataType)
        {
            var propertyIds = new Dictionary<string, Guid>();
	        var propertyTypes = new Dictionary<String, String>();
            foreach (PropertyParent prop in dataType.PropertyParents)
            {
                string capitalizedPropName = String.Format("{0}{1}", prop.Name.Substring(0, 1).ToUpper(), prop.Name.Substring(1, prop.Name.Length - 1));
                propertyIds.Add(capitalizedPropName, prop.ID);
				propertyTypes.Add(capitalizedPropName, (prop as PropertyType).DataType.Name);
            }

            if (propertyIds.Count > 0)
                assistant.AddPropertyIDTable(dataType.Name, propertyIds);
			if (propertyTypes.Count > 0)
				assistant.AddPropertyTypeTable(dataType.Name, propertyTypes);
        }

        private SourcesDataSummary GenerateSourcesDataSummary(Guid[] sourceIds)
        {
            SourcesDataSummary result = new SourcesDataSummary();

            using (var model = GetNewModel())
            {
                var typesOfSources = model.SensorTypeSources.Where(sts => sts.Sensors.Any(s => sourceIds.Contains(s.ID)));
                List<DataType> typesEmittedBySources = new List<DataType>();

                foreach (var sourceType in typesOfSources)
                    foreach (var emittedDataType in sourceType.DataTypes)
                        if (!typesEmittedBySources.Any(matchedType => matchedType.ID == emittedDataType.ID))
                            typesEmittedBySources.Add(emittedDataType);

                List<PropertyParent> propertiesInTypes = typesEmittedBySources.SelectMany(dt => dt.PropertyParents).ToList();

                result.DataPropertyNames = new String[propertiesInTypes.Count];
                result.DataPropertyTypes = new Type[propertiesInTypes.Count];
                result.DataPropertyFrequencies = new float[propertiesInTypes.Count];

                int currentPropertyIndex = 0;
                foreach (var property in propertiesInTypes)
                {
                    var propertyType = property as PropertyType;

                    String propertyName = property.DataType.Name + "_" + property.Name;
                    Type propertyPrimitiveType;
                    float propertyFrequency;

                    switch (propertyType.DataType.Name)
                    {
                        case ("bool"):
                            propertyPrimitiveType = typeof(bool);
                            break;
                        case ("string"):
                            propertyPrimitiveType = typeof(string);
                            break;
                        case ("Guid"):
                            propertyPrimitiveType = typeof(Guid);
                            break;
                        case ("long"):
                            propertyPrimitiveType = typeof(long);
                            break;
                        case ("double"):
                            propertyPrimitiveType = typeof(double);
                            break;
                        case ("byte[]"):
                            propertyPrimitiveType = typeof(byte[]);
                            break;
                        case ("float"):
                            propertyPrimitiveType = typeof(float);
                            break;
                        case ("Object"):
                            propertyPrimitiveType = typeof(object);
                            break;
                        case ("int"):
                            propertyPrimitiveType = typeof(int);
                            break;
                        default:
                            propertyPrimitiveType = null;
                            break;
                    }

                    //	TODO: Take into account number of sensors outputting a property

                    //	Hard-coded frequencies TODO: Add estimated frequency output to sensor DTD
                    switch (property.DataType.Name)
                    {
                        case ("Transcription"):
                            propertyFrequency = 0.25f;
                            break;
                        case ("HeadPose"):
                            propertyFrequency = 60.0f;
                            break;
                        case ("FaceData"):
                            propertyFrequency = 60.0f;
                            break;
                        case ("Utterance"):
                            propertyFrequency = 0.25f;
                            break;
                        default:
                            propertyFrequency = 0.0f;
                            break;
                    }
					
                    result.DataPropertyNames[currentPropertyIndex] = propertyName;
                    result.DataPropertyTypes[currentPropertyIndex] = propertyPrimitiveType;
                    result.DataPropertyFrequencies[currentPropertyIndex] = propertyFrequency;

                    currentPropertyIndex++;
                }
            }

            return result;
        }

        public Uri GetPropertyValueEndpoint(Guid propertyID)
        {
            throw new NotImplementedException();
        }

        private void InitializePropertyValueServicesAndBuildEndpointDictionary()
        {
            Entities.DataType[] dataTypes = GetNewModel().DataTypes.ToArray(); //System.Boolean

            foreach (Entities.DataType dataType in dataTypes)
            {
                foreach (Entities.PropertyType propertyType in dataType.PropertyParents)
                {
                    //	TODO: Should this be commented out?
                    //ServiceHost tempServiceHostforPropertyService = PropertyServiceManager.StartService(serverServiceUri, dataType, propertyType);
                    //propertyValueServiceEndpoints.Add(propertyType.ID, tempServiceHostforPropertyService.BaseAddresses);
                }
            }
        }

        public ReadOnlyCollection<Uri> GetUris(Entities.Adapters.DataType dataTypeAdapter, Entities.Adapters.PropertyType propertyTypeAdapter)
        {
            Entities.DataType dataType = dataTypeAdapter;
            Entities.PropertyType propertyType = propertyTypeAdapter;

            return PropertyServiceManager.GetUris(serverServiceUri, dataType, propertyType, connectionString);
        }

        private int m_CacheSize = 8;
        private List<Entities.EntitiesModel> m_ModelsCache = new List<EntitiesModel>();
        private int m_CurrentModelIndex = 0;

        private Entities.EntitiesModel GetNewModel()
        {
            return new EntitiesModel(connectionString);
        }

        private void InitializeModelCache()
        {
            m_ModelsCache = new List<EntitiesModel>(m_CacheSize);
            for (int i = 0; i < m_CacheSize; i++)
            {
                m_ModelsCache.Add(new Entities.EntitiesModel(connectionString));
            }
        }

        public Entities.EntitiesModel GetCachedModel()
        {
            return GetNewModel();

            /*
            int referencedIndex;

            lock (m_ModelsCache)
            {
                //DateTime begin = DateTime.Now;
                //Entities.EntitiesModel result = new Entities.EntitiesModel(connectionString);
                //DateTime end = DateTime.Now;
                //double timeMS = (end - begin).TotalMilliseconds;
                //Debugger.Log(0, "", timeMS.ToString() + "\n");
                if (++m_CurrentModelIndex >= m_CacheSize)
                    m_CurrentModelIndex = 0;

                referencedIndex = m_CurrentModelIndex;
            }

            return m_ModelsCache[referencedIndex];
             */
        }

        private void SchemaUpdateCallbackImplementation(object sender, SchemaUpdateArgs args)
        {
            Telerik.OpenAccess.ISchemaHandler schemaHandler = args.SchemaHandler;
            SchemaUpdateInfo schemaUpdateInfo = schemaHandler.CreateUpdateInfo(new SchemaUpdateProperties() { CheckExtraColumns = false });
            if (schemaUpdateInfo.HasScript)
            {
                if (schemaUpdateInfo.IsExtending)
                {
                    schemaHandler.ExecuteDDLScript(schemaUpdateInfo.Script);
                }
                else
                { 
                    string ddlScript = args.SchemaHandler.CreateUpdateDDLScript(new SchemaUpdateProperties());
                    args.SchemaHandler.ForceExecuteDDLScript(ddlScript);
                }
            }
        }

        private void HandleItem(object sender, string itemJson)
        {
            itemPersistor.HandleItem(itemJson);
            itemStreamer.ProcessItemPayload(itemJson);
        }

        public bool SetContext(string connectionString)
        {
            this.connectionString = connectionString;
            return true;
        }

        public bool AddType(string xml)
        {
            using (Entities.EntitiesModel model = GetNewModel())
            {
				DataTypeCreator creator = new DataTypeCreator(model, connectionString);
				creator.ImportSensorDataTypeSource(xml);

                XDocument document = XDocument.Parse(xml);
                foreach (XElement node in document.Root.Nodes())
                {
                    Guid dataTypeID = Guid.Parse(node.Attribute("ID").Value);
                    AddPropertyIDHelper(model.DataTypes.Where(dt => dt.ID == dataTypeID).Single());

                    List<Entities.Adapters.PropertyType> propertyTypes = new List<Entities.Adapters.PropertyType>();
                    foreach (var propType in model.PropertyTypes.Where(pt => pt.ParentDataTypeID == dataTypeID))
                    {
                        propertyTypes.Add(propType);
                    }
                    AddAggregationTypes(propertyTypes);
                }
            }
            return true;
        }

        public bool AddType(Entities.Adapters.DataTypeSource source, Entities.Adapters.DataType dataTypeAdapter, Entities.Adapters.PropertyType[] properties, bool typeNeedsAggregation = true)
        {
            Entities.DataTypeSource dataTypeSource = source;
            Entities.DataType dataType = dataTypeAdapter;

            using (Entities.EntitiesModel context = GetNewModel())
            {
                DataTypeCreator creator = new DataTypeCreator(context, connectionString);

                if (context.DataTypeSources.Where(type => type.ID == source.ID).Count() == 0)
                {
                    try
                    {
                        context.Add(dataTypeSource);
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                if (context.DataTypes.Where(type => type.ID == dataType.ID).Count() == 0)
                { 
                    try
                    {
                        context.Add(dataType);
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    foreach (Entities.Adapters.PropertyType propertyAdapter in properties)
                    {
                        if (context.Properties.Where(type => type.ID == propertyAdapter.ID).Count() == 0)
                        {
                            Entities.PropertyType property = propertyAdapter;
                            context.Add(property);
                            try
                            {
                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                throw;
                            }
                            PropertyType temp = context.PropertyTypes.Single(pt => pt.ID == property.ID);
                            creator.AddPropertyValuePersistenceSupport(dataType.Name, temp);

                            // This call potentially catches (internally) a silent exception if the service tries to open on a used uri. It's a hack, stay in school kids.
                            ServiceHost tempServiceHostforPropertyService = PropertyServiceManager.StartService(serverServiceUri, dataType, property, connectionString);
                            propertyValueServiceEndpoints.Add(property.ID, tempServiceHostforPropertyService.BaseAddresses);
                        }
                    }

                    AddPropertyIDHelper(context.DataTypes.Where(dt => dt.ID == dataType.ID).Single());

                    if (typeNeedsAggregation)
                    {
                        AddAggregationTypes(properties);
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        private void AddAggregationTypes(IEnumerable<Entities.Adapters.PropertyType> properties)
        {
            foreach (var property in properties)
            {
                if (property.PropertyDataTypeID == Guid.Parse("A6FFA473-3483-43B5-A2A8-B606C565C883"))
                {
                    AddIntAggrTypes(property);
                }
                //if (property.ParentDataTypeID == Guid.Parse())
                //{
                //    other types go here
                //}
            }
        }

        private void AddIntAggrTypes(Entities.Adapters.PropertyType property)
        {
            using (Entities.EntitiesModel context = GetNewModel())
            {
                Guid intPropertyID = context.DataTypes.Where(dt => dt.Name == "Int").Single().ID;
                Guid dateTimePropertyID = context.DataTypes.Where(dt => dt.Name == "DateTime").Single().ID;

                Entities.Adapters.DataTypeSource minDTS = context.DataTypeSources.Where(dts => dts.Name == "Numeric_Aggregator_Min").Single();
                Entities.Adapters.DataTypeSource maxDTS = context.DataTypeSources.Where(dts => dts.Name == "Numeric_Aggregator_Max").Single();
                Entities.Adapters.DataTypeSource averageDTS = context.DataTypeSources.Where(dts => dts.Name == "Numeric_Aggregator_Average").Single();
                Entities.Adapters.DataTypeSource medianDTS = context.DataTypeSources.Where(dts => dts.Name == "Numeric_Aggregator_Median").Single();

                Entities.Adapters.DataType minDataType = new Entities.Adapters.DataType()
                {
                    ID = Guid.NewGuid(),
                    Name = String.Format("{0}_Min_Aggregation", property.Name),
                    FullName = String.Format("{0}_Min_Aggregation", property.Name),
                    IsReadOnly = false,
                    IsCoreType = false,
                    DataTypeSourceID = minDTS.ID
                };
                Entities.Adapters.DataType maxDataType = new Entities.Adapters.DataType()
                {
                    ID = Guid.NewGuid(),
                    Name = String.Format("{0}_Max_Aggregation", property.Name),
                    FullName = String.Format("{0}_Max_Aggregation", property.Name),
                    IsReadOnly = false,
                    IsCoreType = false,
                    DataTypeSourceID = maxDTS.ID
                };
                Entities.Adapters.DataType averageDataType = new Entities.Adapters.DataType()
                {
                    ID = Guid.NewGuid(),
                    Name = String.Format("{0}_Average_Aggregation", property.Name),
                    FullName = String.Format("{0}_Average_Aggregation", property.Name),
                    IsReadOnly = false,
                    IsCoreType = false,
                    DataTypeSourceID = averageDTS.ID
                };
                Entities.Adapters.DataType medianDataType = new Entities.Adapters.DataType()
                {
                    ID = Guid.NewGuid(),
                    Name = String.Format("{0}_Median_Aggregation", property.Name),
                    FullName = String.Format("{0}_Median_Aggregation", property.Name),
                    IsReadOnly = false,
                    IsCoreType = false,
                    DataTypeSourceID = averageDTS.ID
                };

                Entities.Adapters.PropertyType minValue = new Entities.Adapters.PropertyType()
                {
                    ID = Guid.NewGuid(),
                    Name = "Min_Value",
                    ParentDataTypeID = minDataType.ID,
                    PropertyDataTypeID = intPropertyID
                };

                Entities.Adapters.PropertyType maxValue = new Entities.Adapters.PropertyType()
                {
                    ID = Guid.NewGuid(),
                    Name = "Max_Value",
                    ParentDataTypeID = maxDataType.ID,
                    PropertyDataTypeID = intPropertyID
                };

                Entities.Adapters.PropertyType averageValue = new Entities.Adapters.PropertyType()
                {
                    ID = Guid.NewGuid(),
                    Name = "Average_Value",
                    ParentDataTypeID = averageDataType.ID,
                    PropertyDataTypeID = intPropertyID
                };

                Entities.Adapters.PropertyType medianValue = new Entities.Adapters.PropertyType()
                {
                    ID = Guid.NewGuid(),
                    Name = "Median_Value",
                    ParentDataTypeID = medianDataType.ID,
                    PropertyDataTypeID = intPropertyID
                };

                AddType(minDTS, minDataType, new Entities.Adapters.PropertyType[1] { minValue }, false);
                AddType(maxDTS, maxDataType, new Entities.Adapters.PropertyType[1] { maxValue }, false);
                AddType(averageDTS, averageDataType, new Entities.Adapters.PropertyType[1] { averageValue }, false);
                AddType(medianDTS, medianDataType, new Entities.Adapters.PropertyType[1] { medianValue }, false);
            }
        }

        public Entities.DataType GetType(string dataTypeName)
        {
            Entities.DataType result = null;
            using (Entities.EntitiesModel context = GetNewModel())
            {
                result = context.DataTypes.Single(dataType => dataType.FullName == dataTypeName);
            }
            return result;
        }

        public IEnumerable<Entities.Adapters.DataType> GetAllTypes()
        {
            List<Entities.Adapters.DataType> result = new List<Entities.Adapters.DataType>();
            using (Entities.EntitiesModel context = new Entities.EntitiesModel(connectionString))
            {
                foreach (DataType dataType in context.DataTypes)
                {
                    result.Add(dataType);
                }
            }
            return result;
        }

        public void Add(Datum datum)
        {
            throw new NotImplementedException();
        }

        public void AddBooleanPropertyValue(Guid sourceID, string dataTypeName, string propertyName, BooleanPropertyValue booleanPropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddBooleanPropertyValue(connection, dataTypeName, propertyName, booleanPropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddByteArrayPropertyValue(Guid sourceID, string dataTypeName, string propertyName, ByteArrayPropertyValue byteArrayPropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddByteArrayPropertyValue(connection, dataTypeName, propertyName, byteArrayPropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddDateTimePropertyValue(Guid sourceID, string dataTypeName, string propertyName, DateTimePropertyValue dateTimePropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddDateTimePropertyValue(connection, dataTypeName, propertyName, dateTimePropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddDoublePropertyValue(Guid sourceID, string dataTypeName, string propertyName, DoublePropertyValue doublePropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddDoublePropertyValue(connection, dataTypeName, propertyName, doublePropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddGuidPropertyValue(Guid sourceID, string dataTypeName, string propertyName, GuidPropertyValue guidPropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddGuidPropertyValue(connection, dataTypeName, propertyName, guidPropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddIntPropertyValue(Guid sourceID, string dataTypeName, string propertyName, IntPropertyValue intPropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddIntPropertyValue(connection, dataTypeName, propertyName, intPropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddSinglePropertyValue(Guid sourceID, string dataTypeName, string propertyName, SinglePropertyValue singlePropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddSinglePropertyValue(connection, dataTypeName, propertyName, singlePropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        public void AddStringPropertyValue(Guid sourceID, string dataTypeName, string propertyName, StringPropertyValue stringPropertyValue)
        {
            bool isSqlServerPersistence = true;
            SqlConnection connection = connectionPool.GetUnusedConnection();
            if (isSqlServerPersistence)
            {
                SqlServerPropertyValuePersistor.AddStringPropertyValue(connection, dataTypeName, propertyName, stringPropertyValue, sourceID);
            }
            connectionPool.FinishConnection(connection);
        }

        #region Database Maintenance and Debugging related members

        public void EnsureValidWorkingDirectory(string workingDirectory)
        {
            //Todo: validate requirement / virtue of this fucntion
            try
            {
                if (!Directory.Exists(workingDirectory))
                {
                    Directory.CreateDirectory(workingDirectory);
                }

                using (FileStream fileStream = new FileStream("ConstructWorkingDirectoryContents.zip", FileMode.OpenOrCreate))
                {
                    using (ZipPackage package = ZipPackage.Open(fileStream))
                    {
                        foreach (var entry in package.ZipPackageEntries)
                        {
                            if (entry.Attributes != FileAttributes.Directory)
                            {
                                SaveFile(entry, workingDirectory);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw new ApplicationException("EnsureValidWorkingDirectory threw an exception.");
            }
        }

        //Save the unzipped files
        private void SaveFile(ZipPackageEntry entry, string folderPath)
        {
            Stream reader = entry.OpenInputStream();
            var fileName = folderPath + "\\" + entry.FileNameInZip;

            var directoryName = Path.GetDirectoryName(fileName);

            if (directoryName != null)
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                FileStream writer = File.Create(folderPath + "\\" + entry.FileNameInZip);

                int size = 2048;
                byte[] data = new byte[2048];
                try
                {
                    while (true)
                    {
                        size = reader.Read(data, 0, data.Length);
                        if (size > 0)
                            writer.Write(data, 0, size);
                        else
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message + ":" + e.InnerException.Message);
                }
                writer.Close();
            }
        }
        
        #endregion
    }
}