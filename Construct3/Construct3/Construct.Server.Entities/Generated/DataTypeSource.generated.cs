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
	public partial class DataTypeSource
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
		
		private bool isCategories;
		public virtual bool IsCategory 
		{ 
		    get
		    {
		        return this.isCategories;
		    }
		    set
		    {
		        this.isCategories = value;
		    }
		}
		
		private bool isReadOnlies;
		public virtual bool IsReadOnly 
		{ 
		    get
		    {
		        return this.isReadOnlies;
		    }
		    set
		    {
		        this.isReadOnlies = value;
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
		
		private Guid? parentIDs;
		public virtual Guid? ParentID 
		{ 
		    get
		    {
		        return this.parentIDs;
		    }
		    set
		    {
		        this.parentIDs = value;
		    }
		}
		
		private DataTypeSource dataTypeSource1;
		public virtual DataTypeSource DataTypeSource1 
		{ 
		    get
		    {
		        return this.dataTypeSource1;
		    }
		    set
		    {
		        this.dataTypeSource1 = value;
		    }
		}
		
		private IList<Source> sources = new List<Source>();
		public virtual IList<Source> Sources 
		{ 
		    get
		    {
		        return this.sources;
		    }
		}
		
		private IList<DataType> dataTypes = new List<DataType>();
		public virtual IList<DataType> DataTypes 
		{ 
		    get
		    {
		        return this.dataTypes;
		    }
		}
		
		private IList<Constant> constants = new List<Constant>();
		public virtual IList<Constant> Constants 
		{ 
		    get
		    {
		        return this.constants;
		    }
		}
		
		private IList<DataTypeSource> dataTypeSources = new List<DataTypeSource>();
		public virtual IList<DataTypeSource> DataTypeSources 
		{ 
		    get
		    {
		        return this.dataTypeSources;
		    }
		}
		
	}
}