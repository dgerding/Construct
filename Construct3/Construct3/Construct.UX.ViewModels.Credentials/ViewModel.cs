using System;
using System.Linq;
using NLog;

namespace Construct.UX.ViewModels.Credentials
{
    public class ViewModel
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ConstructCredentialsService.ModelClient credentialsClient;

        public ViewModel()
        {
            credentialsClient = new ConstructCredentialsService.ModelClient();
        }

        public event Action DatabaseReset;
        public event Action LoginError;
        public event Action<string> LoginSuccess;

        public void WipeDatabase(string connectionString)
        {
            credentialsClient.BeginReset(connectionString, OnDatabaseReset, null);
        }

        public void LoadTestItems(string connectionString)
        {
            credentialsClient.LoadTestItemData(connectionString);
        }

        public void Login(string constructServerName, string userName, string password)
        {
            if (LoginSuccess != null)
                LoginSuccess("data source=daisy.colum.edu;initial catalog=Construct3;persist security info=True;user id=Construct;password=ConstructifyConstructifyConstructify");
            return;
            try
            {
                if (credentialsClient.IsValidationServerAvailable())
                {
                    if (credentialsClient.AreCredentialsAuthentic(constructServerName, userName, password))
                    {
                        string connectionString = credentialsClient.GetConstructServerConnectionString(constructServerName, userName, password);

                        if (credentialsClient.IsConstructServerAvailable(connectionString))
                        {
                            if (credentialsClient.AreConstructServerCoreEntitiesValid(connectionString))
                            {
                                if (LoginSuccess != null)
                                    LoginSuccess(connectionString);
                                return;
                                //TODO: Is above return masking a real problem with comm below.?
                            }
                            else
                            {
                                if (LoginError != null)
                                    LoginError();
                            }
                        }
                        else
                        {
                            if (LoginError != null)
                                LoginError();
                        }
                    }
                    else
                    {
                        if (LoginError != null)
                            LoginError();
                    }
                }
                else
                {
                    if (LoginError != null)
                        LoginError();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorException("Login failed in ViewModel.Credentials", ex);
                if (LoginError != null)
                    LoginError();
                
            }
        }

        protected void OnDatabaseReset(IAsyncResult result)
        {
            if (DatabaseReset != null)
                DatabaseReset();
        }
    }
}