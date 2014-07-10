using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class Sensor
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

        private Guid sensorTypeSourceID;
        [DataMember]
        public virtual Guid SensorTypeSourceID
        {
            get
            {
                return this.sensorTypeSourceID;
            }
            set
            {
                this.sensorTypeSourceID = value;
            }
        }

        private Guid dataTypeSourceID;
        [DataMember]
        public virtual Guid DataTypeSourceID
        {
            get
            {
                return this.dataTypeSourceID;
            }
            set
            {
                this.dataTypeSourceID = value;
            }
        }

        private Guid _sensorHostID;
        [DataMember]
        public virtual Guid SensorHostID
        {
            get
            {
                return this._sensorHostID;
            }
            set
            {
                this._sensorHostID = value;
            }
        }

        private bool _isHealthy;
        [DataMember]
        public virtual bool IsHealthy
        {
            get
            {
                return this._isHealthy;
            }
            set
            {
                this._isHealthy = value;
            }
        }

        private DateTime? _installedFromServerDate;
        [DataMember]
        public virtual DateTime? InstalledFromServerDate
        {
            get
            {
                return this._installedFromServerDate;
            }
            set
            {
                this._installedFromServerDate = value;
            }
        }

        private string _currentRendezvous;
        [DataMember]
        public virtual string CurrentRendezvous
        {
            get
            {
                return this._currentRendezvous;
            }
            set
            {
                this._currentRendezvous = value;
            }
        }


        public static implicit operator Entities.Sensor(Adapters.Sensor adapter)
        {
            Entities.Sensor result = new Entities.Sensor();

            result.CurrentRendezvous = adapter.CurrentRendezvous;
            result.ID = adapter.ID;
            result.InstalledFromServerDate = adapter.InstalledFromServerDate;
            result.IsHealthy = adapter.IsHealthy;
            result.SensorHostID = adapter.SensorHostID;
            result.SensorTypeSourceID = adapter.SensorTypeSourceID;
            result.DataTypeSourceID = adapter.DataTypeSourceID;

            return result;
        }
        public static implicit operator Adapters.Sensor(Entities.Sensor entity)
        {
            Adapters.Sensor result = new Adapters.Sensor();

            result.CurrentRendezvous = entity.CurrentRendezvous;
            result.ID = entity.ID;
            result.InstalledFromServerDate = entity.InstalledFromServerDate;
            result.IsHealthy = entity.IsHealthy;
            result.SensorHostID = entity.SensorHostID;
            result.SensorTypeSourceID = entity.SensorTypeSourceID;
            result.DataTypeSourceID = entity.DataTypeSourceID;

            return result;
        }
    }
}
