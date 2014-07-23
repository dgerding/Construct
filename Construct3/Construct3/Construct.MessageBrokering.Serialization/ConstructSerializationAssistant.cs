using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Construct.MessageBrokering;
using Construct.Server.Entities;
using Construct.Server.Models.Data.PropertyValue;
using System.Data.SqlClient;

namespace Construct.MessageBrokering.Serialization
{
    public class ConstructSerializationAssistant
    {
        private Action<Guid, string, string, BooleanPropertyValue> boolPropertyValuePersistanceDelegate;
		private Action<Guid, string, string, ByteArrayPropertyValue> byteArrayPropertyValuePersistanceDelegate;
		private Action<Guid, string, string, DateTimePropertyValue> dateTimePropertyValuePersistanceDelegate;
		private Action<Guid, string, string, DoublePropertyValue> doublePropertyValuePersistanceDelegate;
		private Action<Guid, string, string, GuidPropertyValue> guidPropertyValuePersistanceDelegate;
		private Action<Guid, string, string, IntPropertyValue> intPropertyValuePersistanceDelegate;
		private Action<Guid, string, string, SinglePropertyValue> singlePropertyValuePersistanceDelegate;
		private Action<Guid, string, string, StringPropertyValue> stringPropertyValuePersistanceDelegate;
        

        private Dictionary<string, Dictionary<string, string>> payloadTypesDictionaries = new Dictionary<string, Dictionary<string, string>>();

        public ConstructSerializationAssistant
        (
			Action<Guid, string, string, BooleanPropertyValue> boolPersistanceDelegate = null,
			Action<Guid, string, string, ByteArrayPropertyValue> byteArrayPersistanceDelegate = null,
			Action<Guid, string, string, DateTimePropertyValue> dateTimePersistanceDelegate = null,
			Action<Guid, string, string, DoublePropertyValue> doublePersistanceDelegate = null,
			Action<Guid, string, string, GuidPropertyValue> guidPersistanceDelegate = null,
			Action<Guid, string, string, IntPropertyValue> intPersistanceDelegate = null,
			Action<Guid, string, string, SinglePropertyValue> singlePersistanceDelegate = null,
			Action<Guid, string, string, StringPropertyValue> stringPersistanceDelegate = null
        )
        {
            LoadTypes();
            this.boolPropertyValuePersistanceDelegate = boolPersistanceDelegate;
            this.byteArrayPropertyValuePersistanceDelegate = byteArrayPersistanceDelegate;
            this.dateTimePropertyValuePersistanceDelegate = dateTimePersistanceDelegate;
            this.doublePropertyValuePersistanceDelegate = doublePersistanceDelegate;
            this.guidPropertyValuePersistanceDelegate = guidPersistanceDelegate;
            this.intPropertyValuePersistanceDelegate = intPersistanceDelegate;
            this.singlePropertyValuePersistanceDelegate = singlePersistanceDelegate;
            this.stringPropertyValuePersistanceDelegate = stringPersistanceDelegate;
        } 

        private Dictionary<string, Type> itemDataTypes = new Dictionary<string,Type>();
        public Dictionary<string, Type> ItemDataTypes 
        {
            get { return itemDataTypes; }
            private set  { itemDataTypes = value; }
        }

        private Dictionary<string, Dictionary<string, PropertyInfo>> propertyTables = new Dictionary<string,Dictionary<string,PropertyInfo>>();
        public Dictionary<string, Dictionary<string, PropertyInfo>> PropertyTables
        {
            get { return propertyTables; }
            private set { propertyTables = value; }
        }

        private Dictionary<string, Dictionary<string, FieldInfo>> fieldTables = new Dictionary<string,Dictionary<string,FieldInfo>>();
        public Dictionary<string, Dictionary<string, FieldInfo>> FieldTables
        {
            get { return fieldTables; }
            private set { fieldTables = value; }
        }

        private Dictionary<string, Dictionary<string, Guid>> propertyIDTables = new Dictionary<string,Dictionary<string,Guid>>();
        public Dictionary<string, Dictionary<string, Guid>> PropertyIDTables
        {
            get { return propertyIDTables; }
            private set { propertyIDTables = value; }
        }

        private void LoadTypes()
        {
            DirectoryInfo assemblyDirInfo = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            FileInfo[] generatedTypeFiles = assemblyDirInfo.GetFiles("Construct.Types.*.dll");
            foreach (FileInfo file in generatedTypeFiles)
            {
                Assembly typeAssembly = null;
                Type[] dynamicTypes = null;
                try
                {
                    typeAssembly = Assembly.LoadFile(file.FullName);
                    dynamicTypes = typeAssembly.GetExportedTypes();
                }
                catch (Exception e)
                {
                    throw e;
                }
                foreach (Type type in dynamicTypes.Where(type => type.BaseType.Name.Contains("FluentMetadataSource") == false))
                {
                    string itemName = type.Name;
                    if (ItemDataTypes.Keys.Contains(itemName) == false)
                    {
						ItemDataTypes.Add(itemName, type);
                    }

                    if (PropertyTables.Keys.Contains(itemName) == false)
                    {
                        PropertyTables.Add(itemName, new Dictionary<string, PropertyInfo>());

                        PropertyInfo[] props = type.GetProperties();
                        foreach (PropertyInfo propInfo in props)
                        {
                            PropertyTables[itemName].Add(propInfo.Name, propInfo);
                        }
                    }

                    if (FieldTables.Keys.Contains(itemName) == false)
                    {
                        FieldTables.Add(itemName, new Dictionary<string, FieldInfo>());

                        FieldInfo[] fields = type.GetFields();
                        foreach (FieldInfo fieldInfo in fields)
                        {
                            FieldTables[itemName].Add(fieldInfo.Name, fieldInfo);
                        }
                    }
                }
            }
        }

        public void AddPropertyIDTable(string newDataType, Dictionary<string, Guid> newPropertyIDTable)
        {
            if (PropertyIDTables.Keys.Contains(newDataType) == false)
            {
                PropertyIDTables.Add(newDataType, newPropertyIDTable);
            }
        }

        public void Persist(string json, Guid itemID)
        {
            object jsonContainer = JsonConvert.DeserializeObject(json);
            string itemName = (jsonContainer as JObject)["Instance"]["DataName"].Value<string>();
            Persist(jsonContainer as JObject, itemName, itemID);
        }

        private void Persist(JObject jObject, string itemName, Guid theItemID)
        {
            Dictionary<string, object> propertyKeyValuePairs = new Dictionary<string, object>();

            JEnumerable<JToken> payloadTypeTokens = jObject["PayloadTypes"].Children();
            JToken itemInstanceTokens = jObject["Instance"];
            Guid dataTypeSourceID = Guid.Parse(itemInstanceTokens["DataTypeSourceID"].Value<string>());
            Guid sourceID = Guid.Parse(itemInstanceTokens["SourceID"].Value<string>());

            if (payloadTypesDictionaries.Keys.Contains(itemName) == false)
            {
                payloadTypesDictionaries.Add(itemName, new Dictionary<string, string>());
                AddPayloadTypesFromJson(payloadTypeTokens, itemName);
            }

            var itemPayloadTokens = jObject["Instance"]["Payload"];
            RecurseItemStructure(itemPayloadTokens, itemName, propertyKeyValuePairs);

            if (ItemDataTypes.ContainsKey(itemName) == false ||
                PropertyTables.ContainsKey(itemName) == false ||
                FieldTables.ContainsKey(itemName) == false)
            {
                LoadTypes();
            }

            Guid itemID = theItemID;
            foreach (string key in propertyKeyValuePairs.Keys)
            {
                string typeName = null;

                if (PropertyTables[itemName].ContainsKey(key))
                {
                    typeName = PropertyTables[itemName][key].PropertyType.Name;
                }
                else if (FieldTables[itemName].ContainsKey(key))
                {
                    typeName = FieldTables[itemName][key].FieldType.Name;
                }
                else
                {
                    typeName = String.Empty;
                }

                switch (typeName)
                {
                    // mirror this for other types below
                    case "Boolean":
                        PersistBooleanPropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "Byte[]":
                        PersistByteArrayPropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "DateTime":
                        PersistDateTimePropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "Double":
                        PersistDoublePropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "Guid":
                        PersistGuidPropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "Int32":
                        PersistIntPropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "Single":
                        PersistSinglePropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    case "String":
                        PersistStringPropertyValue(itemName, propertyKeyValuePairs, itemInstanceTokens, sourceID, PropertyIDTables[itemName][key], itemID, key);
                        break;
                    default:
                        break;
                }
            }
        }

        private void PersistBooleanPropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens, 
                                                    Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            BooleanPropertyValue obj = new BooleanPropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (bool)propertyKeyValuePairs[key];

            if (boolPropertyValuePersistanceDelegate != null)
            {
                boolPropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistByteArrayPropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                    Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            ByteArrayPropertyValue obj = new ByteArrayPropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (byte[])propertyKeyValuePairs[key];

            if (byteArrayPropertyValuePersistanceDelegate != null)
            {
				byteArrayPropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistDateTimePropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                    Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            DateTimePropertyValue obj = new DateTimePropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (DateTime)propertyKeyValuePairs[key];

            if (dateTimePropertyValuePersistanceDelegate != null)
            {
				dateTimePropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistDoublePropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                    Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            DoublePropertyValue obj = new DoublePropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (double)propertyKeyValuePairs[key];

            if (doublePropertyValuePersistanceDelegate != null)
            {
				doublePropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistGuidPropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            GuidPropertyValue obj = new GuidPropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (Guid)propertyKeyValuePairs[key];

            if (guidPropertyValuePersistanceDelegate != null)
            {
				guidPropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistIntPropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            IntPropertyValue obj = new IntPropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
					obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
				else
					obj.Interval = null;
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (int)propertyKeyValuePairs[key];

            if (stringPropertyValuePersistanceDelegate != null)
            {
				intPropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistSinglePropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                    Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            SinglePropertyValue obj = new SinglePropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (Single)propertyKeyValuePairs[key];

            if (singlePropertyValuePersistanceDelegate != null)
            {
				singlePropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

		private void PersistStringPropertyValue(string itemName, Dictionary<string, object> propertyKeyValuePairs, JToken itemInstanceTokens,
                                                    Guid sourceID, Guid propertyID, Guid itemID, string key)
        {
            StringPropertyValue obj = new StringPropertyValue();
            obj.ItemID = itemID;
            obj.PropertyID = propertyID;
            obj.StartTime = DateTime.Parse(itemInstanceTokens["TimeStamp"].Value<string>());
            obj.Latitude = itemInstanceTokens["Latitude"].Value<double>().ToString();
            obj.Longitude = itemInstanceTokens["Longitude"].Value<double>().ToString();
            try
            {
				if (itemInstanceTokens.Contains("Interval"))
	                obj.Interval = Int32.Parse(itemInstanceTokens["Interval"].Value<double>().ToString());
            }
            catch (Exception ex)
            {
                obj.Interval = null;
            }

            obj.Value = (string)propertyKeyValuePairs[key];
			//	Escape strings
			obj.Value = obj.Value.Replace("'", "''");

            if (stringPropertyValuePersistanceDelegate != null)
            {
				stringPropertyValuePersistanceDelegate(sourceID, itemName, key, obj);
            }
        }

        public dynamic GetItem(string json, Guid itemID)
        {
            object jsonContainer = JsonConvert.DeserializeObject(json);
            string itemName = (jsonContainer as JObject)["Instance"]["DataName"].Value<string>();
            return GetItem(jsonContainer as JObject, itemName, itemID);
        }

        private dynamic GetItem(JObject jObject, string itemName, Guid itemID)
        {
            Dictionary<string, object> propertyKeyValuePairs = new Dictionary<string, object>();

            JEnumerable<JToken> payloadTypeTokens = jObject["PayloadTypes"].Children();
			
            if (payloadTypesDictionaries.Keys.Contains(itemName) == false)
            {
                payloadTypesDictionaries.Add(itemName, new Dictionary<string, string>());
                AddPayloadTypesFromJson(payloadTypeTokens, itemName);
            }

            JToken itemPayloadTokens = jObject["Instance"]["Payload"];
            RecurseItemStructure(itemPayloadTokens, itemName, propertyKeyValuePairs);

            if (ItemDataTypes.Keys.Contains(itemName) == false)
            {
                LoadTypes();
            }
            dynamic newItem = Activator.CreateInstance(ItemDataTypes[itemName]);
            newItem.ItemID = itemID;

            foreach (string key in propertyKeyValuePairs.Keys)
            {
                if (propertyKeyValuePairs[key] as String == "DBNull")
                {
                    try
                    {
                        propertyTables[itemName][key].SetValue(newItem, Activator.CreateInstance(propertyTables[itemName][key].PropertyType), null);
                    }
                    catch (KeyNotFoundException propNotFound)
                    {
                        //TODO: we should fucking do something here
                        try
                        {
                            fieldTables[itemName][key].SetValue(newItem, Activator.CreateInstance(fieldTables[itemName][key].FieldType));
                        }
                        catch (KeyNotFoundException fieldNotFound)
                        {
                            //TODO: we should fucking do something here
                        }
                    }

                }
                else if (propertyTables[itemName].Keys.Contains(key))
                {
                    propertyTables[itemName][key].SetValue(newItem, propertyKeyValuePairs[key], null);
                }
                else if (fieldTables[itemName].Keys.Contains(key))
                {
                    fieldTables[itemName][key].SetValue(newItem, propertyKeyValuePairs[key]);
                }
                else if (propertyTables[itemName].Keys.Count == 3 && key == "Payload")
                {
                    PropertyInfo prop = propertyTables[itemName].Where(p => p.Key != "ItemID").Where(p => p.Key != "RecordCreationDate").Single().Value;
                    prop.SetValue(newItem, propertyKeyValuePairs[key], null);
                }
                else if (fieldTables[itemName].Keys.Count == 3 && key == "Payload")
                {
                    FieldInfo field = fieldTables[itemName].Where(p => p.Key != "ItemID").Where(p => p.Key != "RecordCreationDate").Single().Value;
                    field.SetValue(newItem, propertyKeyValuePairs[key]);
                }
                else
                {
                    throw new ApplicationException(String.Format("Could not find {0} as a field or property in tables of known members", key));
                }
            }

            newItem.RecordCreationDate = DateTime.Now;
            return newItem;
        }

        private void RecurseItemStructure(JToken tokens, string itemName, Dictionary<string, object> propertyKeyValuePairs)
        {
            if (tokens is JValue)
            {
                var valueToSet = Deserialize(tokens, itemName);
                string name = ((JProperty)tokens.Parent).Name;
                if (name == "Payload")
                {
                    string pairKey = null;
                    if (propertyTables[itemName].Count == 3)
                    {
                        pairKey = propertyTables[itemName].Where(p => p.Key != "ItemID").Where(p => p.Key != "RecordCreationDate").Single().Value.Name;
                    }
                    else if(fieldTables[itemName].Count == 3)
                    {
                        pairKey = fieldTables[itemName].Where(p => p.Key != "ItemID").Where(p => p.Key != "RecordCreationDate").Single().Value.Name;
                    }
                    propertyKeyValuePairs.Add(pairKey.Substring(0, 1).ToUpper() + pairKey.Substring(1, pairKey.Length - 1), valueToSet);
                }
                else
                {
                    propertyKeyValuePairs.Add(name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1), valueToSet);
                }
            }
            else
            {
                foreach (JToken token in tokens.Children())
                {
                    RecurseItemStructure(token, itemName, propertyKeyValuePairs);
                }
            }
        }

        public Item GetItemHeader(string json, Guid itemID)
        {
            object jsonContainer = JsonConvert.DeserializeObject(json);
            JToken itemHeaderTokens = (jsonContainer as JObject)["Instance"];

            Item ret = new Item();
            ret.SourceTime = DateTime.Parse(itemHeaderTokens["TimeStamp"].Value<string>());
            ret.Latitude = itemHeaderTokens["Latitude"].Value<double>().ToString();
            ret.Longitude = itemHeaderTokens["Longitude"].Value<double>().ToString();
            ret.RecordCreationTime = DateTime.Now;
            ret.DataTypeID = Guid.Parse(itemHeaderTokens["DataTypeID"].Value<string>());
            ret.SourceId = Guid.Parse(itemHeaderTokens["SourceID"].Value<string>());
            ret.ID = itemID;

            return ret;
        }

        public Telemetry GetTelemetry(string json)
        {
            return JsonConvert.DeserializeObject<Telemetry>(json);
        }

        public Command GetCommand(string json)
        {
            return JsonConvert.DeserializeObject<Command>(json);
        }

		private object Deserialize(JToken token, string itemName)
		{
			JEnumerable<JToken> subTokens = token.Children();
			List<object> list = new List<object>();
			foreach (JToken propertyToken in subTokens)
			{
				list.Add(Deserialize(propertyToken, itemName));
			}
			if (list.Count > 1)
			{
				return list;
			}
			else
			{
				return GetTypedObjectFromToken(token, itemName);
			}
		}

        private object GetTypedObjectFromToken(JToken token, string itemName)
        {
            JToken tokenParent = token.Parent;
            switch (payloadTypesDictionaries[itemName][((JProperty)tokenParent).Name])
            {
                case "Byte[]":
                    return Convert.FromBase64String(tokenParent.First.Value<string>());
                    //break;
                case "Byte":
                    return Byte.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Guid":
                    return Guid.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Int32":
                    return Int32.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Int64":
                    return Int64.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Double":
                    return Double.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Single":
                    return Single.Parse(tokenParent.First.Value<string>());
                    //break;
                case "DateTime":
                    return DateTime.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Boolean":
                    return Boolean.Parse(tokenParent.First.Value<string>());
                    //break;
                case "Char":
                    return Char.Parse(tokenParent.First.Value<string>());
                    //break;
                case "String":
                    return tokenParent.First.Value<string>();
                    //break;
                case "DBNull":
                    return "DBNull";
                    //break;
                default:
                    return "BAD_TOKEN_TYPE_VALUE";
                    //break;
            }
        }
        private void AddPayloadTypesFromJson(JEnumerable<JToken> tokens, string itemName)
        {
			Dictionary<String,String> itemProperties = payloadTypesDictionaries[itemName];
            if (itemProperties.Keys.Contains("RecordCreationDate") == false)
            {
                payloadTypesDictionaries[itemName].Add("RecordCreationDate", "DateTime");
            }
            if (itemProperties.Keys.Contains("TimeStamp") == false)
            {
                itemProperties.Add("TimeStamp", "DateTime");
            }
            foreach (JToken type in tokens)
            {
				if (!itemProperties.ContainsKey(((JProperty)type).Name))
	                itemProperties.Add(((JProperty)type).Name, type.First.Value<string>());
            }
        }
    }
}
