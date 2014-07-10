using System;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Construct.Server.Models.Data.PropertyValues.Services;
using Construct.Server.Models.Data.PropertyValue;
using Construct.UX.ViewModels.Questions.QuestionsServiceReference;

namespace Construct.UX.ViewModels.Questions
{
    public class ViewModel : ViewModels.ViewModel
    {
        private Expression expression;
        private ModelClient client;

        private ObservableCollection<DataType> dataTypes;
        public ObservableCollection<DataType> DataTypes
        {
            get 
            {
                if (dataTypes == null)
                {
                    dataTypes = new ObservableCollection<DataType>();
                }
                return dataTypes; 
            }
            set 
            {
                dataTypes = value;
            }
        }

        private ObservableCollection<PropertyType> propertyTypes;
        public ObservableCollection<PropertyType> PropertyTypes
        {
            get
            {
                if (propertyTypes == null)
                {
                    propertyTypes = new ObservableCollection<PropertyType>();
                }
                return propertyTypes;
            }
            set
            {
                propertyTypes = value;
            }
        }

        private ObservableCollection<Question> observableQuestions;
        public ObservableCollection<Question> ObservableQuestions
        {
            get
            {
                if (observableQuestions == null)
                {
                    observableQuestions = new ObservableCollection<Question>();
                }
                return observableQuestions;
            }
            set
            {
                observableQuestions = value;
            }
        }

        private IIntPropertyValueService intService;
        private IBooleanPropertyValueService boolService;
        private IStringPropertyValueService stringService;
        private IDoublePropertyValueService doubleService;
        private ISinglePropertyValueService floatService;
        private IGuidPropertyValueService guidService;
        private IByteArrayPropertyValueService byteArrayService;

        public ObservableCollection<IntPropertyValue> IntPropertyValues = new ObservableCollection<IntPropertyValue>();
        public ObservableCollection<BooleanPropertyValue> BoolPropertyValues = new ObservableCollection<BooleanPropertyValue>();
        public ObservableCollection<StringPropertyValue> StringPropertyValues = new ObservableCollection<StringPropertyValue>();
        public ObservableCollection<DoublePropertyValue> DoublePropertyValues = new ObservableCollection<DoublePropertyValue>();
        public ObservableCollection<SinglePropertyValue> FloatPropertyValues = new ObservableCollection<SinglePropertyValue>();
        public ObservableCollection<GuidPropertyValue> GuidPropertyValues = new ObservableCollection<GuidPropertyValue>();
        public ObservableCollection<ByteArrayPropertyValue> ByteArrayPropertyValues = new ObservableCollection<ByteArrayPropertyValue>();

        public ViewModel() : base("Questions")
        {
        }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo)
            : base(applicationSessionInfo, "Questions")
        {
        }

        public override void Load()
        {
            dataTypes = new ObservableCollection<DataType>(GetDataTypes());
            propertyTypes = new ObservableCollection<PropertyType>();
        }

        private ModelClient GetModel()
        {
            if (client == null || client.State == CommunicationState.Closed || client.State == CommunicationState.Closing || client.State == CommunicationState.Faulted)
            {
                client = new ModelClient();
                client.Open();
            }
            return client;
        }

        //private Construct.Server.Entities.Adapters.Question question;
        //private Construct.Server.Entities.Adapters.DataType backingType;
        //public void CreateNewQuestion()
        //{
        //    //backingType = new Server.Entities.Adapters.DataType();
        //    //backingType.ID = Guid.NewGuid();
        //    //backingType.IsCoreType = false;
        //    //backingType.IsReadOnly = false;

        //    //question = new Server.Entities.Adapters.Question();
        //    //question.ID = backingType.ID;
        //}

        //public void SetName(string name)
        //{
        //    //backingType.Name = name;
        //}
        //public void SetFullName(string fullname)
        //{
        //    //backingType.FullName = fullname;
        //}
        //public void SetExpression(Expression expression)
        //{
        //    this.expression = expression;
        //    //question.LinqExpression = expression.ToString();
        //}
        public Guid AddQuestion(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;
            Guid ID = typed.ID;
            Guid propertyID = typed.PropertyID;
            Guid dataTypeID = typed.DataTypeID;
            string expression = typed.Expression;

            Question newQuestion = new Question();

            newQuestion.ID = ID;
            newQuestion.PropertyID = propertyID;
            newQuestion.DataTypeID = dataTypeID;
            newQuestion.LinqExpression = expression;

            client = GetModel();
            if (client.AddQuestion(newQuestion))
            {
                lock (observableQuestions)
                {
                    observableQuestions.Add(newQuestion);
                }
                return newQuestion.ID;
            }
            else
            {
                return Guid.Empty;
            }

        }

        public Guid AddQuestionTypeSource(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            client = GetModel();

            return Guid.Empty;
        }

        public List<DataType> GetDataTypes()
        {
            client = GetModel();
            List<DataType> ret = client.GetDataTypes();
            return ret;
        }

        public List<Property> GetProperties()
        {
            client = GetModel();
            List<Property> ret = client.GetProperties();
            return ret;
        }

        public List<PropertyParent> GetPropertyParents()
        {
            client = GetModel();
            List<PropertyParent> ret = client.GetPropertyParents();
            return ret;
        }

        public List<PropertyType> GetPropertyTypes()
        
        {   client = GetModel();
            List<PropertyType> ret = client.GetPropertyTypes();
            return ret;
        }

        public List<Question> GetQuestions()
        {
            client = GetModel();
            List<Question> ret = client.GetQuestions();
            return ret;
        }

        public void GetIntPropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<IIntPropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<IIntPropertyValueService>(httpBinding, myEndpoint);

            intService = channelFactory.CreateChannel();
            foreach (var prop in intService.GetAll())
            {
                IntPropertyValues.Add(prop);
            }
        }

        public void GetBoolPropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<IBooleanPropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<IBooleanPropertyValueService>(httpBinding, myEndpoint);

            boolService = channelFactory.CreateChannel();
            foreach (var prop in boolService.GetAll())
            {
                BoolPropertyValues.Add(prop);
            }
        }

        public void GetStringPropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<IStringPropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<IStringPropertyValueService>(httpBinding, myEndpoint);

            stringService = channelFactory.CreateChannel();
            foreach (var prop in stringService.GetAll())
            {
                StringPropertyValues.Add(prop);
            }
        }

        public void GetDoublePropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<IDoublePropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<IDoublePropertyValueService>(httpBinding, myEndpoint);

            doubleService = channelFactory.CreateChannel();
            foreach (var prop in doubleService.GetAll())
            {
                DoublePropertyValues.Add(prop);
            }
        }

        public void GetFloatPropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<ISinglePropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<ISinglePropertyValueService>(httpBinding, myEndpoint);

            floatService = channelFactory.CreateChannel();
            foreach (var prop in floatService.GetAll())
            {
                FloatPropertyValues.Add(prop);
            }
        }

        public void GetGuidPropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<IGuidPropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<IGuidPropertyValueService>(httpBinding, myEndpoint);

            guidService = channelFactory.CreateChannel();
            foreach (var prop in guidService.GetAll())
            {
                GuidPropertyValues.Add(prop);
            }
        }

        public void GetByteArrayPropertyValues(string dataTypeName, string propertyName)
        {
            ChannelFactory<IByteArrayPropertyValueService> channelFactory = null;
            EndpointAddress myEndpoint = new EndpointAddress(String.Format("http://localhost:8000/00000000-0000-0000-0000-000000000000/{0}/{1}", dataTypeName, propertyName));
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            channelFactory = new ChannelFactory<IByteArrayPropertyValueService>(httpBinding, myEndpoint);

            byteArrayService = channelFactory.CreateChannel();
            foreach (var prop in byteArrayService.GetAll())
            {
                ByteArrayPropertyValues.Add(prop);
            }
        }

        public void ClearPropertyValues()
        {
            if (IntPropertyValues.Count != 0)
            {
                IntPropertyValues.Clear();
            }

            if (BoolPropertyValues.Count != 0)
            {
                BoolPropertyValues.Clear();
            }

            if (StringPropertyValues.Count != 0)
            {
                StringPropertyValues.Clear();
            }

            if (DoublePropertyValues.Count != 0)
            {
                DoublePropertyValues.Clear();
            }

            if (FloatPropertyValues.Count != 0)
            {
                FloatPropertyValues.Clear();
            }

            if (GuidPropertyValues.Count != 0)
            {
                GuidPropertyValues.Clear();
            }

            if (ByteArrayPropertyValues.Count != 0)
            {
                ByteArrayPropertyValues.Clear();
            }
        }
    }
}