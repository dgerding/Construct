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
	public partial class Visualization
	{
		private Guid visualizerIDs;
		public virtual Guid VisualizerID 
		{ 
		    get
		    {
		        return this.visualizerIDs;
		    }
		    set
		    {
		        this.visualizerIDs = value;
		    }
		}
		
		private string names;
		public virtual string Name 
		{ 
		    get
		    {
		        return this.names;
		    }
		    set
		    {
		        this.names = value;
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
		
		private string descriptions;
		public virtual string Description 
		{ 
		    get
		    {
		        return this.descriptions;
		    }
		    set
		    {
		        this.descriptions = value;
		    }
		}
		
		private Guid dataTypeIDs;
		public virtual Guid DataTypeID 
		{ 
		    get
		    {
		        return this.dataTypeIDs;
		    }
		    set
		    {
		        this.dataTypeIDs = value;
		    }
		}
		
		private Guid propertyIDs;
		public virtual Guid PropertyID 
		{ 
		    get
		    {
		        return this.propertyIDs;
		    }
		    set
		    {
		        this.propertyIDs = value;
		    }
		}
		
		private Visualizer visualizers;
		public virtual Visualizer Visualizer 
		{ 
		    get
		    {
		        return this.visualizers;
		    }
		    set
		    {
		        this.visualizers = value;
		    }
		}
		
	}
}
