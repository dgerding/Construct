using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class Question
    {
        private string _linqExpression;
        [DataMember]
        public string LinqExpression
        {
            get
            {
                return this._linqExpression;
            }
            set
            {
                this._linqExpression = value;
            }
        }

        private Guid _iD;
        [DataMember]
        public Guid ID
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

        private Guid propertyID;
        [DataMember]
        public Guid PropertyID
        {
            get
            {
                return this.propertyID;
            }
            set
            {
                this.propertyID = value;
            }
        }

        private Guid dataTypeID;
        [DataMember]
        public Guid DataTypeID
        {
            get
            {
                return this.dataTypeID;
            }
            set
            {
                this.dataTypeID = value;
            }
        }
        public static implicit operator Adapters.Question(Entities.Question question)
        {
            var result = new Adapters.Question();

            result.ID = question.ID;
            result.LinqExpression = question.LinqExpression;
            result.DataTypeID = question.DataTypeID;
            result.PropertyID = question.PropertyID;

            return result;
        }

        public static implicit operator Entities.Question(Adapters.Question question)
        {
            var result = new Entities.Question();

            result.ID = question.ID;
            result.LinqExpression = question.LinqExpression;
            result.DataTypeID = question.DataTypeID;
            result.PropertyID = question.PropertyID;

            return result;
        }
    }
}
