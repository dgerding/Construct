using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class DataTypeSource 
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public bool IsCategory { get; set; }
        [DataMember]
        public bool IsReadOnly { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid? ParentID { get; set; }

        public static implicit operator Adapters.DataTypeSource(Entities.DataTypeSource dataTypeSource)
        {
            var result = new Adapters.DataTypeSource();
            
            result.ID = dataTypeSource.ID;
            result.IsCategory = dataTypeSource.IsCategory;
            result.IsReadOnly = dataTypeSource.IsReadOnly;
            result.Name = dataTypeSource.Name;
            result.ParentID = dataTypeSource.ParentID;

            return result;
        }

        public static implicit operator Entities.DataTypeSource(Adapters.DataTypeSource dataTypeSource)
        {
            var result =  new Entities.DataTypeSource();

            result.ID = dataTypeSource.ID;
            result.IsCategory = dataTypeSource.IsCategory;
            result.IsReadOnly = dataTypeSource.IsReadOnly;
            result.Name = dataTypeSource.Name;
            result.ParentID = dataTypeSource.ParentID;

            return result;
        }
    }
}