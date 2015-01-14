using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Construct.MessageBrokering
{
    public class SensorSerializationAssistant
    {
        private Dictionary<string, Dictionary<string, string>> payloadTypesDictionary = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> typeHeaderStrings = new Dictionary<string, string>();
        private List<string> primitiveTypes = new List<string>() { "Byte[]", "Int32", "Guid", "Double", "Single", "DateTime", "Boolean", "Char", "String" };

        public string GetJson(Construct.MessageBrokering.Data data)
        {
            SetJsonHeader(data);
            string baseJson = JsonConvert.SerializeObject(data).Replace('"', '\'');
            return String.Format("{0}{1} }}", GetJsonHeader(data.DataName), baseJson);
        }

        public string GetJsonHeader(string dataName)
        {
            if (typeHeaderStrings.Keys.Contains(dataName) == false)
            {
                return string.Empty;
            }
            if (typeHeaderStrings[dataName] == null)
            {
                return string.Empty;
            }
            return String.Format("{{ {0},'Instance' : ", typeHeaderStrings[dataName]);
        }

        public void SetJsonHeader(Construct.MessageBrokering.Data data)
        {
            if (payloadTypesDictionary.Keys.Contains(data.DataName) == false)
            {
                payloadTypesDictionary.Add(data.DataName, new Dictionary<string, string>());
            }

            if (payloadTypesDictionary[data.DataName].Count == 0)
            {
                AddPayloadTypesFromData(data);
            }
            SetTypeHeaderString(data.DataName);
        }

        public Construct.MessageBrokering.Data GetItem(string json)
        {
            object jsonContainer = JsonConvert.DeserializeObject(json);

            JEnumerable<JToken> payloadTypeTokens = (jsonContainer as JObject)["PayloadTypes"].Children();
            JToken itemTokens = (jsonContainer as JObject)["Instance"];

            string dataName = itemTokens["DataName"].Value<string>();

            if (payloadTypesDictionary.Keys.Contains(dataName) == false)
            {
                payloadTypesDictionary.Add(dataName, new Dictionary<string, string>());
            }

            if (payloadTypesDictionary[dataName].Count == 0)
            {
                AddPayloadTypesFromJson(payloadTypeTokens, dataName);
            }

            Construct.MessageBrokering.Data ret = new Construct.MessageBrokering.Data(-1,
                                                                                      Guid.Parse(itemTokens["SourceID"].Value<string>()),
                                                                                      Guid.Parse(itemTokens["DataTypeSourceID"].Value<string>()),
                                                                                      dataName,
                                                                                      Guid.Parse(itemTokens["DataTypeID"].Value<string>())
                                                                                      );
            ret.BrokerID = Guid.Parse(itemTokens["BrokerID"].Value<string>());
            ret.TimeStamp = itemTokens["TimeStamp"].Value<DateTime>();
	        if (ret.TimeStamp.Kind == DateTimeKind.Unspecified)
		        ret.TimeStamp = DateTime.SpecifyKind(ret.TimeStamp, DateTimeKind.Utc);	// Timestamps are often parsed without zone encodings, they
																						//	should be UTC
            ret.Latitude = itemTokens["Latitude"].Value<double>();
            ret.Longitude = itemTokens["Longitude"].Value<double>();

            ret.Payload = Deserialize(itemTokens["Payload"], dataName);
            return ret;
        }

        private void AddPayloadTypesFromData(Construct.MessageBrokering.Data data)
        {
            Type payloadType = data.Payload.GetType();
            if (IsPrimitive(payloadType.Name))
            {
                if (payloadTypesDictionary[data.DataName].Keys.Contains(payloadType.Name) == false)
                {
                    payloadTypesDictionary[data.DataName].Add("Payload", payloadType.Name);
                }
            }
            //This is in support of sending dynamic data. A <string, object> dictionary can be used in place of a strong typed object as the payload.
            else if (payloadType.Name == "Dictionary`2")
            {
                foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)(data.Payload))
                {
                    payloadTypesDictionary[data.DataName].Add(kvp.Key, kvp.Value.GetType().Name);
                }
            }
            else
            {
                AddPayloadSubTypes(payloadType, data.DataName);
            }
        }

        private void AddPayloadSubTypes(Type payloadType, string dataName)
        {
            IEnumerable<FieldInfo> pubFields = payloadType.GetFields().Where(f => f.IsPublic == true);
            PropertyInfo[] propertyInfos = payloadType.GetProperties();

            foreach (FieldInfo fieldInfo in pubFields)
            {
                if (IsPrimitive(fieldInfo.FieldType.Name))
                {
                    if (payloadTypesDictionary[dataName].Keys.Contains(fieldInfo.Name) == false)
                    {
                        //string fieldToUpper = String.Format("{0}{1}", fieldInfo.Name.Substring(0, 1).ToUpper(), fieldInfo.Name.Substring(1, fieldInfo.Name.Length -1));
                        payloadTypesDictionary[dataName].Add(fieldInfo.Name, fieldInfo.FieldType.Name);
                    }
                }
                else
                {
                    AddPayloadSubTypes(fieldInfo.FieldType, dataName);
                }
            }

            foreach (PropertyInfo propInfo in propertyInfos)
            {
                if (IsPrimitive(propInfo.PropertyType.Name))
                {
                    if (payloadTypesDictionary[dataName].Keys.Contains(propInfo.Name) == false)
                    {
                        payloadTypesDictionary[dataName].Add(propInfo.Name, propInfo.PropertyType.Name);
                    }
                }
                else
                {
                    AddPayloadSubTypes(propInfo.PropertyType, dataName);
                }
            }
        }

        private void SetTypeHeaderString(string dataName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("'PayloadTypes' : {");
            foreach (string key in payloadTypesDictionary[dataName].Keys)
            {
                sb.Append(String.Format("'{0}' : '{1}', ", key, payloadTypesDictionary[dataName][key]));
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            typeHeaderStrings[dataName] = sb.ToString();
        }

        private bool IsPrimitive(string typeName)
        {
            return primitiveTypes.Contains(typeName);
        }

        private void AddPayloadTypesFromJson(JEnumerable<JToken> tokens, string dataName)
        {
            foreach (JToken type in tokens)
            {
                if (payloadTypesDictionary[dataName].Keys.Contains(type.First.Value<string>()) == false)
                {
                    payloadTypesDictionary[dataName].Add(((JProperty)type).Name, type.First.Value<string>());
                }
            }
        }

        private object Deserialize(JToken token, string dataName)
        {
            JEnumerable<JToken> subTokens = default(JEnumerable<JToken>);
            try
            {
                subTokens = token.Children();
            }
            catch (Exception e)
            {
                subTokens = new JEnumerable<JToken>(new List<JToken>() { token });
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (JToken subToken in subTokens)
            {
                if ((subToken is JValue) == false)
                {
                    if (subToken is JProperty)
                    {
                        dictionary.Add(((JProperty)subToken).Name, Deserialize(subToken, dataName));
                    }
                    else if (subToken is JObject)
                    {
                        foreach (JProperty propertyToken in subToken.Children())
                        {
                            dictionary.Add((propertyToken).Name, Deserialize(propertyToken, dataName));
                        }
                    }
                }
                else
                {
                    return GetTypedObjectFromToken(subToken, dataName);
                }
            }
            if (dictionary.Count > 1)
            {
                return dictionary;
            }
            else
            {
				return GetTypedObjectFromToken(token, dataName);
            }
        }

        private object GetTypedObjectFromToken(JToken token, string dataName)
        {
            JToken tokenParent = token.Parent;
            switch (payloadTypesDictionary[dataName][((JProperty)tokenParent).Name])
            {
                case "Byte[]":
                    return Convert.FromBase64String(tokenParent.First.Value<string>());
                    break;
                case "Byte":
                    return Byte.Parse(tokenParent.First.Value<string>());
                    break;
                case "Guid":
                    return Guid.Parse(tokenParent.First.Value<string>());
                    break;
                case "Int32":
                    return Int32.Parse(tokenParent.First.Value<string>());
                    break;
                case "Double":
                    return Double.Parse(tokenParent.First.Value<string>());
                    break;
                case "Single":
                    return Single.Parse(tokenParent.First.Value<string>());
                    break;
                case "DateTime":
                    return DateTime.Parse(tokenParent.First.Value<string>());
                    break;
                case "Boolean":
                    return Boolean.Parse(tokenParent.First.Value<string>());
                    break;
                case "Char":
                    return Char.Parse(tokenParent.First.Value<string>());
                    break;
                case "String":
                    return tokenParent.First.Value<string>();
                    break;
                default:
                    return "BAD_TOKEN_TYPE_VALUE";
                    break;
            }
        }
    }
}