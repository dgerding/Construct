using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class SensorCommandParameter
    {
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

        private string _key;
        [DataMember]
        public virtual string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }

        private string _value;
        [DataMember]
        public virtual string Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        private Guid _sensorCommandID;
        [DataMember]
        public virtual Guid SensorCommandID
        {
            get
            {
                return this._sensorCommandID;
            }
            set
            {
                this._sensorCommandID = value;
            }
        }

        public static implicit operator Entities.SensorCommandParameter(Adapters.SensorCommandParameter adapter)
        {
            Entities.SensorCommandParameter result = new Entities.SensorCommandParameter();

            result.ID = adapter.ID;
            result.Key = adapter.Key;
            result.SensorCommandID = adapter.SensorCommandID;
            result.Value = adapter.Value;

            return result;
        }

        public static implicit operator Adapters.SensorCommandParameter(Entities.SensorCommandParameter entity)
        {
            Adapters.SensorCommandParameter result = new Adapters.SensorCommandParameter();

            result.ID = entity.ID;
            result.Key = entity.Key;
            result.SensorCommandID = entity.SensorCommandID;
            result.Value = entity.Value;

            return result;
        }
    }
}
