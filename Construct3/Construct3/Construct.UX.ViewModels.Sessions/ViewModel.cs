using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Dynamic;
using Construct.UX.ViewModels.Sessions.SessionsServiceReference;


namespace Construct.UX.ViewModels.Sessions
{
    public class ViewModel : ViewModels.ViewModel
    {
        private CallbackImplementation callback = new CallbackImplementation();
        private InstanceContext instanceContext = null;
        private ModelClient client = null;

        public ViewModel()
            : base("Sessions")
        {
        }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo)
            : base(applicationSessionInfo, "Sessions")
        {
        }

        public override void Load()
        {
            ObservableSessions = new ObservableCollection<Session>(GetSessions());
            ObservableSources = new ObservableCollection<Source>(GetSources());
            ObservableSessionSources = new ObservableCollection<SessionSource>(GetSessionSources());
        }

        private ModelClient GetModel()
        {
            if (client == null || client.State == CommunicationState.Closed || client.State == CommunicationState.Closing || client.State == CommunicationState.Faulted)
            {
                instanceContext = new InstanceContext(callback);
                client = new ModelClient(instanceContext, "WsDualHttpBinding", RemoteAddress);
                client.Open();
            }
            return client;
        }

        private ObservableCollection<Session> observableSessions;
        public ObservableCollection<Session> ObservableSessions
        {
            get
            {
                return observableSessions;
            }
            set
            {
                observableSessions = value;
            }
        }

        private ObservableCollection<Source> observableSources;
        public ObservableCollection<Source> ObservableSources
        {
            get
            {
                return observableSources;
            }
            set
            {
                observableSources = value;
            }
        }

        private ObservableCollection<SessionSource> observableSessionSources;
        public ObservableCollection<SessionSource> ObservableSessionSources
        {
            get
            {
                return observableSessionSources;
            }
            set
            {
                observableSessionSources = value;
            }
        }

        public Guid AddSession(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid ID = typed.ID;
            string friendlyName = typed.FriendlyName;
            DateTime startTime = typed.StartTime;
            long interval = typed.Interval;

            Session session = new Session();
            session.ID = ID;
            session.FriendlyName = friendlyName;
            session.StartTime = startTime;
            session.Interval = interval;

            client = GetModel();
            if (client.AddSession(session))
            {
                ObservableSessions.Add(session);
                return session.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid AddSessionSource(object parameter)
        {
            dynamic typed = parameter as ExpandoObject;

            Guid ID = typed.ID;
            Guid sessionID = typed.SessionID;
            Guid sourceID = typed.SourceID;

            SessionSource sessionSource = new SessionSource();
            sessionSource.ID = ID;
            sessionSource.SessionID = sessionID;
            sessionSource.SourceID = sourceID;

            client = GetModel();
            if (client.AddSessionSource(sessionSource))
            {
                ObservableSessionSources.Add(sessionSource);
                return sessionSource.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public List<Session> GetSessions()
        {
            client = GetModel();
            List<Session> ret = client.GetSessions();
            return ret;
        }

        public List<Source> GetSources()
        {
            client = GetModel();
            List<Source> ret = client.GetSources();
            return ret;
        }

        public List<SessionSource> GetSessionSources()
        {
            client = GetModel();
            List<SessionSource> ret = client.GetSessionSources();
            return ret;
        }
    }
}