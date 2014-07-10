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
		private ObjectContainer myContainer = null;
		static private %ClassName% the%ClassName% = null;

		public %ClassName%()
		{
		}
		public %ClassName%( ObjectContainer aContainer )
		{
			myContainer = aContainer;
		}

		static public Database Database()
		{
			return null;
		}

		static public IObjectScope ObjectScope()
		{
			return null;
		}

		static public IObjectScope GetNewObjectScope()
		{
			return null;
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

		#region IDBConnection Members

		public %ProductNamespace%.ObjectContainer.ChangeSet Load( FillInstructionData instr )
		{
			object inst = instr.CopyTo( new %WebserviceNamespace%.FillInstructionData() );
			object erg = new %WebserviceNamespace%.%ProxyClassName%().Load(
										(%WebserviceNamespace%.FillInstructionData)inst );
			%ProductNamespace%.ObjectContainer.ChangeSet ccData = null;
			if (erg != null)
				ccData = new ObjectContainer.ChangeSet( erg );
			return ccData;
		}

		public void						Save( ObjectContainer.ChangeSet changes )
		{
			object chg = changes.CopyTo( new %WebserviceNamespace%.ChangeSet() );
			new %WebserviceNamespace%.%ProxyClassName%().Save(
										(%WebserviceNamespace%.ChangeSet)chg );
		}

		public ObjectContainer.ChangeSet	Sync( ObjectContainer.ChangeSet changes )
		{
			object chg = changes.CopyTo( new %WebserviceNamespace%.ChangeSet() );
			object erg = new %WebserviceNamespace%.%ProxyClassName%().Sync(
											(%WebserviceNamespace%.ChangeSet)chg );
			%ProductNamespace%.ObjectContainer.ChangeSet ccData = null;
			if (erg != null)
				ccData = new ObjectContainer.ChangeSet( erg );
			return ccData;
		}

		#endregion
	}
}
