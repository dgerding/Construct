using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class SensorHostType
    {
        private string _sensorHostTypeName;
        [DataMember]
        public virtual string SensorHostTypeName
        {
            get
            {
                return this._sensorHostTypeName;
            }
            set
            {
                this._sensorHostTypeName = value;
            }
        }

        private Guid? _parentID;
        [DataMember]
        public virtual Guid? ParentID
        {
            get
            {
                return this._parentID;
            }
            set
            {
                this._parentID = value;
            }
        }

        private bool _isCategory;
        [DataMember]
        public virtual bool IsCategory
        {
            get
            {
                return this._isCategory;
            }
            set
            {
                this._isCategory = value;
            }
        }

        private Guid _iD;
        [DataMember]
        public virtual Guid ID
        {
            get
            {
                return this._iD;
            }
            set
            {
                this._iD = value;
            }
        }

        public static implicit operator Entities.SensorHostType(Adapters.SensorHostType adapter)
        {
            Entities.SensorHostType result = new Entities.SensorHostType();

            result.ID = adapter.ID;
            result.IsCategory = adapter.IsCategory;
            result.ParentID = adapter.ParentID;
            result.SensorHostTypeName = adapter.SensorHostTypeName;

            return result;
        }
        public static implicit operator Adapters.SensorHostType(Entities.SensorHostType entity)
        {
            Adapters.SensorHostType result = new Adapters.SensorHostType();

            result.ID = entity.ID;
            result.IsCategory = entity.IsCategory;
            result.ParentID = entity.ParentID;
            result.SensorHostTypeName = entity.SensorHostTypeName;

            return result;
        }
    }
}
