#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Data.Common;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.OpenAccess.Metadata.Fluent.Advanced;
using Construct.Server.Entities;


namespace Construct.Server.Entities	
{
	public partial class Source
	{
		private Guid dataTypeSourceIDs;
		public virtual Guid DataTypeSourceID 
		{ 
		    get
		    {
		        return this.dataTypeSourceIDs;
		    }
		    set
		    {
		        this.dataTypeSourceIDs = value;
		    }
		}
		
		private Guid iDs;
		public virtual Guid ID 
		{ 
		    get
		    {
		        return this.iDs;
		    }
		    set
		    {
		        this.iDs = value;
		    }
		}
		
		private DataTypeSource dataTypeSources;
		public virtual DataTypeSource DataTypeSource 
		{ 
		    get
		    {
		        return this.dataTypeSources;
		    }
		    set
		    {
		        this.dataTypeSources = value;
		    }
		}
		
		private IList<Item> items = new List<Item>();
		public virtual IList<Item> Items 
		{ 
		    get
		    {
		        return this.items;
		    }
		}
		
		private IList<SessionSource> sessionSources = new List<SessionSource>();
		public virtual IList<SessionSource> SessionSources 
		{ 
		    get
		    {
		        return this.sessionSources;
		    }
		}
		
	}
}