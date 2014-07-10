using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class SensorTypeSource
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
        [DataMember]
        public Guid SensorHostTypeID { get; set; }
        [DataMember]
        public string Version { get; set; }

        public static implicit operator Adapters.SensorTypeSource(Entities.SensorTypeSource sensorTypeSource)
        {
            var result = new Adapters.SensorTypeSource();

            result.ID = sensorTypeSource.ID;
            result.IsCategory = sensorTypeSource.IsCategory;
            result.IsReadOnly = sensorTypeSource.IsReadOnly;
            result.Name = sensorTypeSource.Name;
            result.ParentID = sensorTypeSource.ParentID;
            result.SensorHostTypeID = sensorTypeSource.SensorHostTypeID;
            result.Version = sensorTypeSource.Version;

            return result;
        }

        public static implicit operator Entities.SensorTypeSource(Adapters.SensorTypeSource sensorTypeSource)
        {
            var result = new Entities.SensorTypeSource();

            result.ID = sensorTypeSource.ID;
            result.IsCategory = sensorTypeSource.IsCategory;
            result.IsReadOnly = sensorTypeSource.IsReadOnly;
            result.Name = sensorTypeSource.Name;
            result.ParentID = sensorTypeSource.ParentID;
            result.SensorHostTypeID = sensorTypeSource.SensorHostTypeID;
            result.Version = sensorTypeSource.Version;

            return result;
        }
    }
}