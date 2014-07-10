using System;
using System.ServiceModel;


namespace Construct.Credentials.Model
{
    [ServiceContract]
    interface IModel : IModelBase
    {
        [OperationContract]
        bool AreConstructServerCoreEntitiesValid(string connectionString);

        [OperationContract]
        bool AreCredentialsAuthentic(string serverName, string userName, string password);

        [OperationContract]
        void EnsureCoreEntitiesExist(string connectionString);

        [OperationContract]
        void EnsureTestItem(string connectionString);

        [OperationContract]
        string GetConnectionStringUsingConnectionStringName(string name);

        [OperationContract]
        string GetConstructServerConnectionString(string theServerName, string theUserName, string thePassword);

        [OperationContract]
        bool IsConstructServerAvailable(string connectionString);

        [OperationContract]
        bool IsCredentialsServerAvailable();

        [OperationContract]
        bool IsExistingConnectionString(string theConnectionStringName);

        [OperationContract]
        bool IsValidationServerAvailable();

        [OperationContract]
        void LoadTestItemData(string connectionString);

        [OperationContract]
        void Reset(string connectionString);
    }
}