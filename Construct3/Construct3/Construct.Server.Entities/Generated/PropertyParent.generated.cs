#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the ClassGenerator.ttinclude code generation file.
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
	public partial class PropertyParent : Property
	{
		private Guid parentDataTypeIDs;
		public virtual Guid ParentDataTypeID
		{
			get
			{
				return this.parentDataTypeIDs;
			}
			set
			{
				this.parentDataTypeIDs = value;
			}
		}
		
		private DataType dataTypes;
		public virtual DataType DataType
		{
			get
			{
				return this.dataTypes;
			}
			set
			{
				this.dataTypes = value;
			}
		}
		
	}
}
#pragma warning restore 1591
