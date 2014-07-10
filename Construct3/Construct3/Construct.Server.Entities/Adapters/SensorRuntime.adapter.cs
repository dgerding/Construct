using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class SensorRuntime
    {
        [DataMember]
        public virtual string Version { get; set; }
        [DataMember]
        public virtual DateTime? RecCreationDate { get; set; }
        [DataMember]
        public virtual byte[] InstallerZip { get; set; }
        [DataMember]
        public virtual string InstallerXml { get; set; }
        [DataMember]
        public virtual string InstallerUri { get; set; }
        [DataMember]
        public virtual Guid ID { get; set; }
        [DataMember]
        public virtual Guid SensorTypeSourceID { get; set; }
        [DataMember]
        public virtual string CacheUri { get; set; }

        public static implicit operator Adapters.SensorRuntime(Entities.SensorRuntime entity)
        {
            return new Adapters.SensorRuntime()
            {
                CacheUri = entity.CacheUri,
                ID = entity.ID,
                InstallerUri = entity.InstallerUri,
                InstallerXml = entity.InstallerXml,
                InstallerZip = entity.InstallerZip,
                RecCreationDate = entity.RecCreationDate,
                SensorTypeSourceID = entity.SensorTypeSourceID
            };
        }
        public static implicit operator Entities.SensorRuntime(Adapters.SensorRuntime adapter)
        {
            return new Entities.SensorRuntime()
            {
                CacheUri = adapter.CacheUri,
                ID = adapter.ID,
                InstallerUri = adapter.InstallerUri,
                InstallerXml = adapter.InstallerXml,
                InstallerZip = adapter.InstallerZip,
                RecCreationDate = adapter.RecCreationDate,
                SensorTypeSourceID = adapter.SensorTypeSourceID
            };
        }
    }
}