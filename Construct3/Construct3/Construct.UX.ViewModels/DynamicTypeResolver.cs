using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Construct.UX.ViewModels
{
    public class DynamicTypeResolver : DataContractResolver
    {
        public override Type ResolveName(string type, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            return knownTypeResolver.ResolveName(type, typeNamespace, declaredType, null);
        }

        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out System.Xml.XmlDictionaryString typeName, out System.Xml.XmlDictionaryString typeNamespace)
        {
            if (false)
            {
                XmlDictionary dictionary = new XmlDictionary();
                typeName = dictionary.Add(type.Name);
                typeNamespace = dictionary.Add("http://localhost:8080/" + type.Assembly.FullName);
                return true;
            }
            else
            {
                return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
            }
        }
    }
}
