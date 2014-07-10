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
        private ObservableCollection<Assembly> persistantTypes = new ObservableCollection<Assembly>();
        private ConstructSerializationAssistant assistant;

        private List<int> assignedPorts = new List<int>();

        private int thisPortBase = 0;

        private Dictionary<Guid, ReadOnlyCollection<Uri>> propertyValueServiceEndpoints;
        private event Action<Models.IModel, dynamic> OnPersist;

		private DataStoreConnectionPool connectionPool;
		private SwitchingItemPersistor itemPersistor;

        public Model(Uri serverServiceUri, string connectionString, Models.IServer server)
        {
            logger.Trace("instantiating Data model");
            
            this.serverServiceUri = serverServiceUri;
            this.OnPersist += server.OnPersist;

            thisPortBase = serverServiceUri.Port + (int)UriUtility.ServicePortOffsets.Data;
            propertyValueServiceEndpoints = new System.Collections.Generic.Dictionary<Guid, ReadOnlyCollection<Uri>>();

            this.connectionString = connectionString;
            broker = server.Broker;
            broker.OnItemReceived += HandleItem;
            Name = "Data";

            persistantTypes.CollectionChanged += PersistantTypesCollectionChanged;

			using (EntitiesModel entitiesModel = GetNewModel())
			{
				TypesAssemblyCreator assemblyCreator = new TypesAssemblyCreator(entitiesModel);
				foreach (Entities.DataType dataType in entitiesModel.DataTypes.Where(dt => dt.IsCoreType == false))
				{
					Assembly assembly = assemblyCreator.ReturnTypeAssembly(dataType);
				}
			}

			InitializeSerializationAssistant();

			InitializePropertyValueServicesAndBuildEndpointDictionary();

			InitializeModelCache();

			connectionPool = new DataStoreConnectionPool(connectionString);
			itemPersistor = new SwitchingItemPersistor(assistant, connectionString, "ItemsCache");
			itemPersistor.OnSqlPersist += delegate(dynamic data)
			{
				if (this.OnPersist != null)
					this.OnPersist(this, data);
			};
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
            var tempDict = new Dictionary<string, Guid>();
            foreach (PropertyParent prop in dataType.PropertyParents)
            {
                string capitalizedPropName = String.Format("{0}{1}", prop.Name.Substring(0, 1).ToUpper(), prop.Name.Substring(1, prop.Name.Length - 1));
                tempDict.Add(capitalizedPropName, prop.ID);
            }
            if (tempDict.Count != 0)
            {
                assistant.AddPropertyIDTable(dataType.Name, tempDict);
            }
        }

		public uint GetNumberOfItemsInTimespan(DateTime startTime, DateTime endTime, Guid? itemTypeId, Guid? sourceId)
		{
			SqlConnection connection = connectionPool.GetUnusedConnection();
			if (connection.State != System.Data.ConnectionState.Open)
				connection.Open();

			SqlServerTelemetryFetcher telemetryFetcher = new SqlServerTelemetryFetcher(connection);
			uint result = telemetryFetcher.GetNumberOfItemsInTimeSpan(startTime, endTime, itemTypeId, sourceId);
			return result;
		}

		public Guid[] GenerateConstructHeaders(Guid[] itemTypeIds)
		{
			List<Guid> result = new List<Guid>();
			
			using (EntitiesModel model = GetNewModel())
			{
				foreach (Guid itemTypeId in itemTypeIds)
				{
					var properties = model.PropertyTypes.Where((type) => type.ParentDataTypeID == itemTypeId);
					result.AddRange(properties.Select(property => property.ID));
				}
			}

			return result.ToArray();
		}

		public String[] GenerateConstructHeaderNames(Guid[] constructHeader)
		{
			using (EntitiesModel model = GetNewModel())
			{
				return model.PropertyTypes.Where(type => constructHeader.Contains(type.ID)).Select(type => type.Name).ToArray();
			}
		}

		public List<object[]> GenerateConstruct(DateTime startTime, DateTime endTime, TimeSpan interval, Guid[] constructHeaders, Guid[] sourceIds)
		{
			List<object[]> result = new List<object[]>();

			Dictionary<Guid, List<dynamic>> propertyValues = new Dictionary<Guid,List<dynamic>>();
			EntitiesModel model;
			try
			{
				model = GetCachedModel();
			}
			catch (Exception e)
			{
				return result;
			}

			//	Gather property values
			foreach (Guid header in constructHeaders)
			{
				try
				{
					//	Connect to the property service
					var property = model.PropertyTypes.First(pt => pt.ID == header);
					var dataType = model.DataTypes.First(dt => dt.ID == property.ParentDataTypeID);
					var service = PropertyServiceManager.StartService(serverServiceUri, dataType, property, connectionString);
					String serviceConnectionString = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverServiceUri, dataType.Name, property.Name).ToString();
					//var client = new TransponderClient(serviceConnectionString);
					dynamic propertyService = service.SingletonInstance;
					List<dynamic> currentProperties = new List<dynamic>();
					currentProperties.AddRange(propertyService.GetBetween(startTime, endTime));

					//	Only reference the sources specified
					currentProperties.RemoveAll(value => !sourceIds.ToList().Contains(value.SourceID));
				}
				catch (Exception e)
				{
					continue;
				}
			}


			//	Generate interval data
			for (DateTime currentTime = startTime; (endTime - currentTime).Ticks > 0; currentTime += interval)
			{
				object[] currentResultRow = new object[constructHeaders.Length];

				//	End-time of the current interval
				DateTime intervalEndTime = currentTime + interval;

				//	Go through each property type
				for (int i = 0; i < constructHeaders.Length; i++)
				{
					//	Get the values within our current interval
					IEnumerable<dynamic> currentPropertyValues;
					try
					{
						currentPropertyValues = propertyValues[constructHeaders[i]].Where(value => (intervalEndTime - value.StartTime).TotalSeconds > 0);
						currentPropertyValues = currentPropertyValues.Where(value => (value.StartTime - currentTime).TotalSeconds > 0);
					}
					catch (Exception e)
					{
						currentResultRow[i] = null;
						continue;
					}

					//	Take the first value we find as the value for this interval (don't bother with aggregation yet)
					var intervalValue = currentPropertyValues.First();
					currentResultRow[i] = intervalValue.Value;
				}

				result.Add(currentResultRow);
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

        private void PersistantTypesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Assembly assembly in e.NewItems)
                {
                    IEnumerable<MetadataSource> sources = FluentMetadataSource.FromAssembly(assembly);
                    foreach (MetadataSource target in sources)
                    {
                        if (source == null)
                        {
                            source = target;
                        }
                        else
                        {
                            string temp = Assembly.GetExecutingAssembly().Location;
                            MetadataContainer temp1 = source.GetModel();
                            MetadataContainer temp2 = target.GetModel();
                            source = new AggregateMetadataSource(source.GetModel(), target.GetModel());
                        }
                    }
                }
            }
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

		public DateTime? GetEarliestItemForTypeAndSource(Guid dataTypeId, Guid sourceId)
		{
			SqlConnection sqlConnection = connectionPool.GetUnusedConnection();
			if (sqlConnection.State != System.Data.ConnectionState.Open)
				sqlConnection.Open();

			SqlServerTelemetryFetcher telemetryFetcher = new SqlServerTelemetryFetcher(sqlConnection);
			return telemetryFetcher.GetEarliestTimeForTypeAndSource(dataTypeId, sourceId);
		}

		public DateTime? GetLatestItemForTypeAndSource(Guid dataTypeId, Guid sourceId)
		{
			SqlConnection sqlConnection = connectionPool.GetUnusedConnection();
			if (sqlConnection.State != System.Data.ConnectionState.Open)
				sqlConnection.Open();

			SqlServerTelemetryFetcher telemetryFetcher = new SqlServerTelemetryFetcher(sqlConnection);
			return telemetryFetcher.GetLatestTimeForTypeAndSource(dataTypeId, sourceId);
		}

        public Entities.EntitiesModel GetCachedModel()
        {
			return GetNewModel();

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
			//	TODO: Lookup Newtonsoft's decimal value type bug
        }

        public bool SetContext(string connectionString)
        {
            this.connectionString = connectionString;
            return true;
        }

        private Type GetType(Entities.DataType dataType)
        {
            Assembly assembly = null;
            Entities.DataType theDataType = null;
            object result = null;

            using (Entities.EntitiesModel model = GetNewModel())
            {
                TypesAssemblyCreator creator = new TypesAssemblyCreator(model);
                theDataType = dataType;
                assembly = creator.ReturnTypeAssembly(theDataType);
                if (persistantTypes.Contains(assembly) == false)
                {
                    persistantTypes.Add(assembly);
                }
            }

            result = assembly.CreateInstance(String.Format("Construct.Types.{0}", theDataType.Name.Replace(' ', '_')));

            return result.GetType();
        }

        public bool AddType(string xml)
        {
            using (Entities.EntitiesModel model = GetNewModel())
            {
                DataTypeCreator creator = new DataTypeCreator(model, connectionString);
                List<Assembly> assemblies = (List<Assembly>)creator.ImportSensorDataTypeSource(xml);

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
                
                foreach (Assembly assembly in assemblies)
                {
                    persistantTypes.Add(assembly);
                }
                if (assemblies.Count == 0)
                {
                    return false;
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

                Type newType = GetType(dataType);
                Assembly assembly = newType.Assembly;
                persistantTypes.Add(assembly);
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

        private readonly Dictionary<Entities.DataType, Assembly> collectionTypeAssemblies = new Dictionary<Entities.DataType, Assembly>();
        private readonly string assembliesDirectory = "C:\\Construct\\Assemblies";

        private IEnumerable GetLiveCollection(Entities.DataType dataType)
        {
            Assembly assembly = null;
            string dataTypeName = dataType.Name;
            string nameSpace = "Construct.Collections";
            string className = dataTypeName + "ObservableCollection";
            string fullName = String.Format("{0}.{1}", nameSpace, className);

            if (collectionTypeAssemblies.Keys.Contains(dataType) == false)
            {
                CodeDefinition.AssemblyDefinition assemblyDefinition = CodeDefinition.Create;
                assemblyDefinition
                                  .SetName(fullName)
                                  .SetDirectory(assembliesDirectory)
                                  .ReferencedAssembly
                                  .SetPath(assembliesDirectory)
                                  .SetName("Construct.Types." + dataTypeName)
                                  .SetNamespace("Construct.Types")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetPath(assembliesDirectory)
                                  .SetName("Construct.Server.Models.Data")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetPath(assembliesDirectory)
                                  .SetName("Telerik.OpenAccess")
                                  .SetNamespace("Telerik.OpenAccess")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetPath(assembliesDirectory)
                                  .SetName("Telerik.OpenAccess.35.Extensions")
                                  .SetNamespace(null)
                                  .Parent
                                  .ReferencedAssembly
                                  .SetName("System.Xaml")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetName("System.Data")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetName("System.Core")
                                  .SetNamespace(null)
                                  .Parent
                                  .ReferencedAssembly
                                  .SetNamespace("System.Linq")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetNamespace("System.Collections")
                                  .Parent
                                  .ReferencedAssembly
                                  .SetNamespace("System.Collections.Generic")
                                  .Parent
                                  .Namespace
                                  .SetName(nameSpace)
                                  .Class
                                  .SetName(className)
                                  .SetScope(CodeDefinition.Scope.Public)
                                  .Field
                                  .SetType(String.Format("LiveCollection<{0}>", dataTypeName))
                                  .SetName("liveCollection")
                                  .SetInitialValue(String.Format("new LiveCollection<{0}>()", dataTypeName))
                                  .Parent
                                  .Property
                                  .SetScope(CodeDefinition.Scope.Public)
                                  .SetType(String.Format("LiveCollection<{0}>", dataTypeName))
                                  .SetName("LiveCollection")
                                  .SetGetter("return liveCollection;")
                ;

                assembly = assemblyDefinition.Compile();
                collectionTypeAssemblies.Add(dataType, assembly);
            }
            else
            {
                assembly = collectionTypeAssemblies[dataType];
            }
            dynamic result = null;
            try
            {
                result = assembly.CreateInstance(fullName);
            }
            catch (FileNotFoundException exception)
            {
                Debug.WriteLine(exception.Message);
                throw exception;
            }
            catch (TargetInvocationException exception)
            {
                Debug.WriteLine(exception.Message);
                throw exception;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            dynamic resultRepository = result.Repository;
            return resultRepository;
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
