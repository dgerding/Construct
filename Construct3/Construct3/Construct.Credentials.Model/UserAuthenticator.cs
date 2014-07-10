using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Credentials.Model
{
    public class UserAuthenticator
    {
        public EntitiesModel UserEntities;

        public UserAuthenticator()
        {
            //serverConnector = new ConstructServerConnectionTester();
        }

        private string connectionString;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public string IsUserAuthentic(string theServerName, string theUserName, string thePassword)
        {
            if (ConstructServerConnectionTester.IsCredentialsServerAvailable())
            {
                //TODO
                UserEntities = new EntitiesModel(@"server=tcp:mc7t5hoo37.database.windows.net;database=ConstructServers;user id=ConstructServerLogin;password=!!TheWoodsAreLovelyDarkAndDeep??;trusted_connection=False;encrypt=True");

                var theUser = from User aUser in UserEntities.Users where aUser.UserName == theUserName select aUser;

                //User user = DataManager.LoginUser( userName, password );
                if (theUser.Count() == 1)
                {
                    Guid id = theUser.First().ID;

                    var somePassword = from PasswordCredential aPassword in UserEntities.PasswordCredentials where aPassword.UserID == id select aPassword;//theUser.First().ID select aPassword;

                    if (somePassword.Count() > 0)
                    {
                        if (somePassword.First().Password == thePassword)
                        {
                            var theServer = from Server aServer in UserEntities.Servers where aServer.ServerName == theServerName select aServer;

                            if (theServer.Count() > 0)
                            {
                                connectionString = theServer.First().ConnectionString;
                                return connectionString;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}