using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public partial class SensorCommand
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

        private Guid _sensorTypeSourceID;
        [DataMember]
        public virtual Guid SensorTypeSourceID
        {
            get
            {
                return this._sensorTypeSourceID;
            }
            set
            {
                this._sensorTypeSourceID = value;
            }
        }

        private string _commandName;
        [DataMember]
        public virtual string CommandName
        {
            get
            {
                return this._commandName;
            }
            set
            {
                this._commandName = value;
            }
        }

        private List<SensorCommandParameter> sensorCommandParameters;
        [DataMember]
        public virtual List<SensorCommandParameter> SensorCommandParameters
        {
            get
            {
                if (sensorCommandParameters == null)
                {
                    sensorCommandParameters = new List<SensorCommandParameter>();
                }
                return this.sensorCommandParameters;
            }
            set
            {
                this.sensorCommandParameters = value;
            }
        }

        public static implicit operator Entities.SensorCommand(Adapters.SensorCommand adapter)
        {
            Entities.SensorCommand result = new Entities.SensorCommand();

            result.CommandName = adapter.CommandName;
            result.ID = adapter.ID;
            result.SensorTypeSourceID = adapter.SensorTypeSourceID;

            return result;
        }

        public static implicit operator Entities.Adapters.SensorCommand(Entities.SensorCommand entity)
        {
            Adapters.SensorCommand result = new Adapters.SensorCommand();

            result.CommandName = entity.CommandName;
            result.ID = entity.ID;
            result.SensorTypeSourceID = entity.SensorTypeSourceID;
            
            foreach (Entities.SensorCommandParameter param in entity.SensorCommandParameters)
            {
                result.SensorCommandParameters.Add(param);
            }
            return result;
        }
    }
}
