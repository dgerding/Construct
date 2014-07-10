using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Construct.UX.ViewModels
{
    public class ApplicationSessionInfo : INotifyPropertyChanged
    {
        private bool isAuthenticated = false;
        private readonly PropertyChangedEventArgs isAuthenticatedChangedEventArgs =
            new PropertyChangedEventArgs("IsAuthenticated");
        public bool IsAuthenticated
        {
            get
            {
                return isAuthenticated;
            }
            set
            {
                if (this.isAuthenticated != value)
                {
                    isAuthenticated = value;
                    NotifyPropertyChanged(this.isAuthenticatedChangedEventArgs);
                }
            }
        }

        private bool isDefaultPrincipalSet = false;
        private readonly PropertyChangedEventArgs isDefaultPrincipalSetChangedEventArgs =
            new PropertyChangedEventArgs("IsDefaultPrincipalSet");
        //private PropertyChangedEventArgs identityChangedEventArgs = new PropertyChangedEventArgs("Identity");
        //private Construct.Users.Model.Identity identity;
        //public Construct.Users.Model.Identity Identity
        //{
        //    get { return identity; }
        //    set
        //    {
        //        if (identity != value)
        //        {
        //            identity = value;
        //            NotifyPropertyChanged(identityChangedEventArgs);
        //        }
        //    }
        //}
        public bool IsDefaultPrincipalSet
        {
            get
            {
                return isDefaultPrincipalSet;
            }
            set
            {
                if (this.isDefaultPrincipalSet != value)
                {
                    isDefaultPrincipalSet = value;
                    NotifyPropertyChanged(this.isDefaultPrincipalSetChangedEventArgs);
                }
            }
        }

        private string password = "";
        private readonly PropertyChangedEventArgs passwordChangedEventArgs = new PropertyChangedEventArgs("Password");
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (this.password != value)
                {
                    password = value;
                    NotifyPropertyChanged(this.passwordChangedEventArgs);
                }
            }
        }

        private string applicationLogMessage = "";
        private readonly PropertyChangedEventArgs applicationLogMessageChangedEventArgs = new PropertyChangedEventArgs("ApplicationLogMessage");
        public string ApplicationLogMessage
        {
            get
            {
                return applicationLogMessage;
            }
            set
            {
                if (this.applicationLogMessage != value)
                {
                    applicationLogMessage = value;
                    NotifyPropertyChanged(this.applicationLogMessageChangedEventArgs);
                }
            }
        }

        private string connectionString = "";
        private readonly PropertyChangedEventArgs connectionStringChangedEventArgs = new PropertyChangedEventArgs("ConnectionString");
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                if (this.connectionString != value)
                {
                    connectionString = value;
                    NotifyPropertyChanged(this.connectionStringChangedEventArgs);
                }
            }
        }

        private int userId = 0;
        private readonly PropertyChangedEventArgs userIdChangedEventArgs = new PropertyChangedEventArgs("UserId");
        public int UserId
        {
            get
            {
                return userId;
            }
            set
            {
                if (this.userId != value)
                {
                    userId = value;
                    NotifyPropertyChanged(this.userIdChangedEventArgs);
                }
            }
        }

        private string userName = "";
        private readonly PropertyChangedEventArgs userNameChangedEventArgs = new PropertyChangedEventArgs("UserName");
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if (this.userName != value)
                {
                    userName = value;
                    NotifyPropertyChanged(this.userNameChangedEventArgs);
                }
            }
        }

        private string workingDirectoryPath = "";
        private readonly PropertyChangedEventArgs workingDirectoryChangedEventArgs = new PropertyChangedEventArgs("WorkingDirectoryPath");
        public string WorkingDirectoryPath
        {
            get
            {
                return workingDirectoryPath;
            }
            set
            {
                if (this.workingDirectoryPath != value)
                {
                    workingDirectoryPath = value;
                    NotifyPropertyChanged(this.workingDirectoryChangedEventArgs);
                }
            }
        }

        public string WorkingDirectoryAssembliesPath
        {
            get
            {
                return Path.Combine(workingDirectoryPath, "Assemblies");
            }
        }
        public string HostName
        {
            get;
            set;
        }
        public int Port
        {
            get;
            set;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion

        public bool areAllValuesReady()
        {
            if ((this.UserName != null) && (this.Password != null) && (this.ConnectionString != null))
            {
                if ((this.UserName.Length > 0) && (this.Password.Length > 0) && (this.ConnectionString.Length > 0))
                {
                    return true;
                }
            }

            return false;
        }
    }
}