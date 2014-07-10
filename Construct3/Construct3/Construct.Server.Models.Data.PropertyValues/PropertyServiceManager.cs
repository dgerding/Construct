using System;
using System.Linq;
using Construct.Server.Entities;
using System.ServiceModel;
using Construct.Server.Models.Data.PropertyValues.Services;
using Construct.Utilities.Shared;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.PropertyValue
{
    public static class PropertyServiceManager
    {
        public static System.Collections.ObjectModel.ReadOnlyCollection<Uri> GetUris(Uri serverUri, DataType dataType, PropertyType propertyType, string connectionString)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<Uri> result = null;

            result = StartService(serverUri, dataType, propertyType, connectionString).BaseAddresses;

            return result;
        }

        public static ServiceHost StartService(Uri serverUri, DataType dataType, PropertyType propertyType, string connectionString)
        {
            Uri result = null;
            ServiceHost host = null;
            dynamic instance;

			Uri connectionUri = UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType.Name, propertyType.Name);

            switch (propertyType.DataType.FullName)
            {
                case "System.Boolean":
                    instance = new BooleanPropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.Byte[]":
                    instance = new ByteArrayPropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.DateTime":
                    instance = new DateTimePropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.Double":
                    instance = new DoublePropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.Int32":
                    instance = new IntPropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.Guid":
                    instance = new GuidPropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.Single":
                    instance = new SinglePropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                case "System.String":
                    instance = new StringPropertyValueService(connectionUri, dataType, propertyType, connectionString);
                    result = instance.Location;
                    break;
                default:
                    //TODO: This switch will need to update support PropertyTypes that are, themselves, complex or composite
                    throw new NotImplementedException();
            }

            host = new ServiceHost(instance, new Uri[] { result });
            try
            {
                host.Open();
            }
            catch (InvalidOperationException e)
            {
                // This exception should hopefully only be thrown when a service tries to start on an endpoint already in use. It's a terrible hack of
                // a fix, but the property value service manager was not made well and therefore doesn't have an better way to avoid trying to create an endpoint
                // that might already be in use.
            }

// 			if (!serviceHostCache.ContainsKey(connectionUri))
// 			{
// 				serviceHostCache.Add(connectionUri, host);
// 			}
// 			else if (serviceHostCache[connectionUri].State != CommunicationState.Opened)
// 			{
// 				try
// 				{
// 					serviceHostCache[connectionUri].Close();
// 				}
// 				catch (Exception e) { }
// 				
// 				serviceHostCache[connectionUri] = host;
// 			}

            return host;
        }




//         public static ServiceHost StartService(Uri serverUri, string dataType, string propertyType, string connectionString)
//         {
//             return StartServiceLogic(serverUri, dataType, propertyType, connectionString);
//         }
// 
//         private static ServiceHost StartServiceLogic(Uri serverUri, string dataType, string propertyType, string connectionString)
//         {
//             Uri result = null;
//             ServiceHost host = null;
//             dynamic instance;
//             string propertyDataTypeName = null;
// 
//             using(EntitiesModel context = new EntitiesModel(connectionString))
//             {
//                 var matches = from propertyTypeTemp in context.PropertyTypes
//                               where propertyTypeTemp.DataType.Name == dataType
//                               where propertyTypeTemp.Name == propertyType
//                               select propertyTypeTemp;
// 
//                 propertyDataTypeName = matches.First().DataType.FullName;
//             }
// 
//             switch (propertyDataTypeName)
//             {
//                 case "System.Boolean":
//                     instance = new BooleanPropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.Byte[]":
//                     instance = new ByteArrayPropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.DateTime":
//                     instance = new DateTimePropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.Double":
//                     instance = new DoublePropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.Int32":
//                     instance = new IntPropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.Guid":
//                     instance = new GuidPropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.Single":
//                     instance = new SinglePropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 case "System.String":
//                     instance = new StringPropertyValueService(UriUtility.CreatePropertyValueServiceEndpointFromServerEndpoint(serverUri, dataType, propertyType), dataType, propertyType, connectionString);
//                     result = instance.Location;
//                     break;
//                 default:
//                     //TODO: This switch will need to update support PropertyTypes that are, themselves, complex or composite
//                     throw new NotImplementedException();
//             }
// 
//             host = new ServiceHost(instance, new Uri[] { result });
//             host.Open();
// 
//             return host;
//         }
    }
}