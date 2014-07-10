using System;
using System.ComponentModel;
using System.Linq;
using Construct.Utilities.Shared;

namespace Construct.UX.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        protected readonly string regionName;

        public ViewModel(string regionName)
        {
            this.regionName = regionName;
        }

        public ViewModel(ApplicationSessionInfo theSessionInfo, string regionName) : this(regionName)
        {
            SessionInfo = theSessionInfo;
        }

        private string remoteAddress;
        protected string RemoteAddress
        {
            get
            {
                if (remoteAddress == null)
                {
                    //remoteAddress = UriUtility.CreateStandardConstructServiceEndpointUri("net.pipe", "Data", SessionInfo.HostName, Guid.Empty, SessionInfo.Port).AbsoluteUri;
                    remoteAddress = UriUtility.CreateStandardConstructServiceEndpointUri("http", regionName, SessionInfo.HostName, Guid.Empty, SessionInfo.Port).AbsoluteUri;
                    return remoteAddress;
                }
                return remoteAddress;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected void NotifyPropertyChanged(string e)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e));
            }
        }

        #endregion

        protected PropertyChangedEventArgs sessionInfoChangedEventArgs = new PropertyChangedEventArgs("SessionInfo");
        protected ApplicationSessionInfo sessionInfo;

        public ApplicationSessionInfo SessionInfo
        {
            get
            {
                return sessionInfo;
            }
            set
            {
                if (this.sessionInfo != value)
                {
                    sessionInfo = value;
                    Load();
                    NotifyPropertyChanged(this.sessionInfoChangedEventArgs);
                }
            }
        }

        public abstract void Load();

        #region IDisposable Members

        public virtual void Dispose()
        {
        }
        #endregion
    }
}