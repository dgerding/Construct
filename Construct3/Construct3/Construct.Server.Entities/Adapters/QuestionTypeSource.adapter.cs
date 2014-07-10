using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Construct.Server.Entities.Adapters
{
    [DataContract]
    public class QuestionTypeSource
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

        public static implicit operator Adapters.QuestionTypeSource(Entities.QuestionTypeSource questionTypeSource)
        {
            var result = new Adapters.QuestionTypeSource();

            result.ID = questionTypeSource.ID;
            result.IsCategory = questionTypeSource.IsCategory;
            result.IsReadOnly = questionTypeSource.IsReadOnly;
            result.Name = questionTypeSource.Name;
            result.ParentID = questionTypeSource.ParentID;

            return result;
        }

        public static implicit operator Entities.QuestionTypeSource(Adapters.QuestionTypeSource questionTypeSource)
        {
            var result = new Entities.QuestionTypeSource();

            result.ID = questionTypeSource.ID;
            result.IsCategory = questionTypeSource.IsCategory;
            result.IsReadOnly = questionTypeSource.IsReadOnly;
            result.Name = questionTypeSource.Name;
            result.ParentID = questionTypeSource.ParentID;

            return result;
        }
    }
}
