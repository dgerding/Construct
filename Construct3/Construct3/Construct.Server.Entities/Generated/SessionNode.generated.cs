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
	public partial class SessionNode
	{
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
		
		private Guid sessionDesignNodeIDs;
		public virtual Guid SessionDesignNodeID 
		{ 
		    get
		    {
		        return this.sessionDesignNodeIDs;
		    }
		    set
		    {
		        this.sessionDesignNodeIDs = value;
		    }
		}
		
		private SessionDesignNode sessionDesignNodes;
		public virtual SessionDesignNode SessionDesignNode 
		{ 
		    get
		    {
		        return this.sessionDesignNodes;
		    }
		    set
		    {
		        this.sessionDesignNodes = value;
		    }
		}
		
	}
}
