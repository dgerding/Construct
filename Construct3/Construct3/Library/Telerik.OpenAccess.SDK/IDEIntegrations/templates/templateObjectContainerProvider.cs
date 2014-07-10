//Copyright (c) %CompanyName%.  All rights reserved.
//
using System;
using System.Collections;
using %ProductNamespace%;
using %ProductNamespace%.Util;

namespace %DefaultNamespace%
{
	/// <summary>
	/// Summary description for %ClassName%.
	/// </summary>
	public class %ClassName% : IObjectScopeProvider, IObjectContainerProvider, IDBConnection
	{
		private Database myDatabase = null;
		private IObjectScope myScope = null;
		private ObjectContainer myContainer = null;

		static private %ClassName% the%ClassName% = null;

		public %ClassName%()
		{
		}

		public %ClassName%( ObjectContainer aContainer )
		{
			this.myContainer = aContainer;
		}

		static public Database Database()
		{
			if( the%ClassName% == null )
				the%ClassName% = new %ClassName%();

			if( the%ClassName%.myDatabase == null )
			{
				the%ClassName%.myDatabase = %ProductNamespace%.Database.Get( "%ConnectionName%" );
				try
				{ the%ClassName%.myDatabase.Properties.ConnectionTimeout = 10000;}
				catch (Exception)
				{}
			}
			return the%ClassName%.myDatabase;
		}

		static public IObjectScope ObjectScope()
		{
			Database();

			if( the%ClassName%.myScope == null )
				the%ClassName%.myScope = GetNewObjectScope();

			return the%ClassName%.myScope;
		}

		static public IObjectScope GetNewObjectScope()
		{
			Database db = Database();

			IObjectScope newScope = db.GetObjectScope(%GetObjectScopeParameters%);
			return newScope;
		}
		static public ObjectContainer ObjectContainer()
		{
			if( the%ClassName% == null )
				the%ClassName% = new %ClassName%();
			if( the%ClassName%.myContainer == null )
				the%ClassName%.myContainer = GetNewObjectContainer();

			return the%ClassName%.myContainer;
		}
		static public ObjectContainer	GetNewObjectContainer()
		{
			ObjectContainer	oc = new ObjectContainer();
			oc.DBConnection = new %ClassName%( oc );
			oc.AutoSync = true;
			return oc;
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
				try
				{ the%ClassName%.myDatabase.Properties.ConnectionTimeout = 10000;}
				catch (Exception)
				{}
            }
        }

		#region IDBConnection Members

		public %ProductNamespace%.ObjectContainer.ChangeSet Load( FillInstructionData instrData )
		{
			ObjectContainer.ChangeSet	ccData = null;
			IObjectScope				os = %ClassName%.GetNewObjectScope();

			os.Transaction.Begin();
			try
			{
				FillInstruction	fillInstr = FillInstruction.Deserialize( instrData );

				ccData = fillInstr.Execute( os );
			}
			finally
			{
				os.Transaction.Rollback();
				os.Dispose();
			}
			return ccData;
		}

		public void						Save( ObjectContainer.ChangeSet changes )
		{
			IObjectScope		os = %ClassName%.GetNewObjectScope();
			try
			{
				%ProductNamespace%.ObjectContainer.CommitChanges( changes, %ProductNamespace%.ObjectContainer.Verify.Changed, os, true, false );
			}
			finally
			{
				os.Dispose();
			}
		}

		public ObjectContainer.ChangeSet	Sync( ObjectContainer.ChangeSet changes )
		{
			ObjectContainer.ChangeSet	ccData = null;
			IObjectScope		os = %ClassName%.GetNewObjectScope();
			try
			{
				ccData = %ProductNamespace%.ObjectContainer.CommitChanges( changes, %ProductNamespace%.ObjectContainer.Verify.Changed, os, true, true );
			}
			finally
			{
				os.Dispose();
			}
			return ccData;
		}
		#endregion
	}
}
