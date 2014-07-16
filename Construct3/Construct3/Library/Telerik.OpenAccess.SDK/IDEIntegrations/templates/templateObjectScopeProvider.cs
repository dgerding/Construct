//Copyright (c) %CompanyName%.  All rights reserved.
//
// usage example:
//
// // Get ObjectScope from ObjectScopeProvider
// IObjectScope scope = %ClassName%.ObjectScope();
// // start transaction
// scope.Transaction.Begin();
// // create new persistent object person and add to scope
// Person p = new Person();
// scope.Add(p);
// // commit transction
// scope.Transaction.Commit();
//

using %ProductNamespace%;
using %ProductNamespace%.Util;
//OABPE using System.Web;
//OABPE using System.Threading;

namespace %DefaultNamespace%
{
	/// <summary>
	/// This class provides an object context for connected database access.
	/// </summary>
	/// <remarks>
	/// This class can be used to obtain an IObjectScope instance required for a connected database
	/// access.
	/// </remarks>
	public class %ClassName% : IObjectScopeProvider
	{
		private Database myDatabase;
		private IObjectScope myScope;

		static private %ClassName% the%ClassName%;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks></remarks>
		public %ClassName%()
		{
		}

        /// <summary>
		/// Adjusts for dynamic loading when no entry assembly is available/configurable.
		/// </summary>
		/// <remarks>
        /// When dynamic loading is used, the configuration path from the
        /// applications entry assembly to the connection setting might be broken.
        /// This method makes up the necessary configuration entries.
        /// </remarks>
        static public void AdjustForDynamicLoad()
        {
            if( the%ClassName% == null )
                the%ClassName% = new %ClassName%();

            if( the%ClassName%.myDatabase == null )
            {
                string assumedInitialConfiguration =
                           "<%RootNodeName%>" +
                               "<references>" +
                                   "<reference assemblyname='PLACEHOLDER' configrequired='True'/>" +
                               "</references>" +
                           "</%RootNodeName%>";
                System.Reflection.Assembly dll = the%ClassName%.GetType().Assembly;
                assumedInitialConfiguration = assumedInitialConfiguration.Replace(
                                                    "PLACEHOLDER", dll.GetName().Name);
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(assumedInitialConfiguration);
                Database db = %ProductNamespace%.Database.Get("%ConnectionName%", 
                                            xmlDoc.DocumentElement,
                                            new System.Reflection.Assembly[] { dll } );

                the%ClassName%.myDatabase = db;
            }
        }

		/// <summary>
		/// Returns the instance of Database for the connectionId 
		/// specified in the Enable Project Wizard.
		/// </summary>
		/// <returns>Instance of Database.</returns>
		/// <remarks></remarks>
		static public Database Database()
		{
			if( the%ClassName% == null )
				the%ClassName% = new %ClassName%();

			if( the%ClassName%.myDatabase == null )
				the%ClassName%.myDatabase = %ProductNamespace%.Database.Get( "%ConnectionName%" );

			return the%ClassName%.myDatabase;
		}

		/// <summary>
		/// Returns the instance of ObjectScope for the application.
		/// </summary>
		/// <returns>Instance of IObjectScope.</returns>
		/// <remarks></remarks>
		static public IObjectScope ObjectScope()
		{
			Database();

			if( the%ClassName%.myScope == null )
				the%ClassName%.myScope = GetNewObjectScope();

			return the%ClassName%.myScope;
		}

		/// <summary>
		/// Returns the new instance of ObjectScope for the application.
		/// </summary>
		/// <returns>Instance of IObjectScope.</returns>
		/// <remarks></remarks>
		static public IObjectScope GetNewObjectScope()
		{
			Database db = Database();

			IObjectScope newScope = db.GetObjectScope(%GetObjectScopeParameters%);
			return newScope;
		}
        //OABPE /// <summary>
		//OABPE /// Returns the new instance of the ObjectScope using the HttpContext aproach described in the best practices articles.
		//OABPE /// </summary>
		//OABPE /// <returns>Instance of IObjectScope.</returns>
		//OABPE /// <remarks></remarks> 
        //OABPE public static IObjectScope GetPerRequestScope(HttpContext context) 
        //OABPE { 
        //OABPE     string key = HttpContext.Current.GetHashCode().ToString("x") + Thread.CurrentContext.ContextID.ToString(); 
        //OABPE     IObjectScope scope; 
        //OABPE     if (context == null) 
        //OABPE     { 
        //OABPE         scope = ObjectScopeProvider1.GetNewObjectScope(); 
        //OABPE     } 
        //OABPE     else 
        //OABPE     { 
        //OABPE         scope = (IObjectScope)context.Items[key]; 
        //OABPE         if (scope == null) 
        //OABPE         { 
        //OABPE             scope = ObjectScopeProvider1.GetNewObjectScope(); 
        //OABPE             context.Items[key] = scope; 
        //OABPE         } 
        //OABPE     } 
        //OABPE     return scope; 
        //OABPE }  
	}
}
