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
	public partial class Visualizer
	{
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
		
		private string layoutStrings;
		public virtual string LayoutString
		{
			get
			{
				return this.layoutStrings;
			}
			set
			{
				this.layoutStrings = value;
			}
		}
		
		private IList<Visualization> visualizations = new List<Visualization>();
		public virtual IList<Visualization> Visualizations
		{
			get
			{
				return this.visualizations;
			}
		}
		
	}
}
#pragma warning restore 1591
