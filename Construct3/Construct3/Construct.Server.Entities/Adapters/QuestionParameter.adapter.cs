using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    public class QuestionParameter
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

        public static implicit operator Adapters.QuestionParameter(Entities.QuestionParameter questionParameter)
        {
            var result = new Adapters.QuestionParameter();

            result.ID = questionParameter.ID;

            return result;
        }

        public static implicit operator Entities.QuestionParameter(Adapters.QuestionParameter questionParameter)
        {
            var result = new Entities.QuestionParameter();

            result.ID = questionParameter.ID;

            return result;
        }
    }
}
