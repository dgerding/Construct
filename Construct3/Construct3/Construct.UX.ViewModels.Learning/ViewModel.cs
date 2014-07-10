using System;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using Construct.MessageBrokering;
using Construct.UX.ViewModels.Learning.LearningServiceReference;


namespace Construct.UX.ViewModels.Learning
{
    public class ViewModel : ViewModels.ViewModel
    {
        private ModelClient client;

        public ViewModel()
            : base("Learning")
        {
        }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo)
            : base(applicationSessionInfo, "Learning")
        {
        }

        public override void Load()
        {
        
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
    }
}