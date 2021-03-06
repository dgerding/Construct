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
using Microsoft.SqlServer.Types;
using Construct.Server.Entities;

namespace Construct.Server.Entities	
{
	public partial class Item
	{
		private DateTime sourceTimes;
		public virtual DateTime SourceTime
		{
			get
			{
				return this.sourceTimes;
			}
			set
			{
				this.sourceTimes = value;
			}
		}
		
		private Guid sourceIds;
		public virtual Guid SourceId
		{
			get
			{
				return this.sourceIds;
			}
			set
			{
				this.sourceIds = value;
			}
		}
		
		private DateTime recordCreationTimes;
		public virtual DateTime RecordCreationTime
		{
			get
			{
				return this.recordCreationTimes;
			}
			set
			{
				this.recordCreationTimes = value;
			}
		}
		
		private string longitudes;
		public virtual string Longitude
		{
			get
			{
				return this.longitudes;
			}
			set
			{
				this.longitudes = value;
			}
		}
		
		private SqlGeography locations;
		public virtual SqlGeography Location
		{
			get
			{
				return this.locations;
			}
			set
			{
				this.locations = value;
			}
		}
		
		private string latitudes;
		public virtual string Latitude
		{
			get
			{
				return this.latitudes;
			}
			set
			{
				this.latitudes = value;
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
		
		private Source sources;
		public virtual Source Source
		{
			get
			{
				return this.sources;
			}
			set
			{
				this.sources = value;
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
