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
	public partial class HumanReadableSensor : Source
	{
		private Guid sensorTypeSourceIDs;
		public virtual Guid SensorTypeSourceID
		{
			get
			{
				return this.sensorTypeSourceIDs;
			}
			set
			{
				this.sensorTypeSourceIDs = value;
			}
		}
		
		private Guid sensorHostIDs;
		public virtual Guid SensorHostID
		{
			get
			{
				return this.sensorHostIDs;
			}
			set
			{
				this.sensorHostIDs = value;
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
		
		private bool isHealthies;
		public virtual bool IsHealthy
		{
			get
			{
				return this.isHealthies;
			}
			set
			{
				this.isHealthies = value;
			}
		}
		
		private DateTime? installedFromServerDates;
		public virtual DateTime? InstalledFromServerDate
		{
			get
			{
				return this.installedFromServerDates;
			}
			set
			{
				this.installedFromServerDates = value;
			}
		}
		
		private string currentRendezvous;
		public virtual string CurrentRendezvous
		{
			get
			{
				return this.currentRendezvous;
			}
			set
			{
				this.currentRendezvous = value;
			}
		}
		
	}
}
#pragma warning restore 1591
