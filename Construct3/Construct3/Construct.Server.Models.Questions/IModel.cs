using System;
using System.Linq;
using System.ServiceModel;
using Construct.Server.Entities;
using System.Collections.Generic;

namespace Construct.Server.Models.Questions
{
    [ServiceContract]
    public interface IModel : Models.IModel
    {
        [OperationContract(Name = "AddQuestion")]
        bool Add(Entities.Adapters.Question question);

        [OperationContract(Name = "AddQuestionTypeSource")]
        bool Add(Entities.Adapters.QuestionTypeSource question);

        [OperationContract]
        IEnumerable<Entities.Adapters.DataType> GetDataTypes();

        [OperationContract]
        IEnumerable<Entities.Adapters.Property> GetProperties();

        [OperationContract]
        IEnumerable<Entities.Adapters.PropertyParent> GetPropertyParents();

        [OperationContract]
        IEnumerable<Entities.Adapters.PropertyType> GetPropertyTypes();

        [OperationContract]
        IEnumerable<Entities.Adapters.Question> GetQuestions();
    }
}