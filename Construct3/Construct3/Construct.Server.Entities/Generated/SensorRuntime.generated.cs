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
	public partial class SensorRuntime
	{
        //todo: what up with the virtuality
		private DateTime? recCreationDates;
		public virtual DateTime? RecCreationDate 
		{ 
		    get
		    {
		        return this.recCreationDates;
		    }
		    set
		    {
		        this.recCreationDates = value;
		    }
		}
		
		private byte[] installerZips;
		public virtual byte[] InstallerZip 
		{ 
		    get
		    {
		        return this.installerZips;
		    }
		    set
		    {
		        this.installerZips = value;
		    }
		}
		
		private string installerXmls;
		public virtual string InstallerXml 
		{ 
		    get
		    {
		        return this.installerXmls;
		    }
		    set
		    {
		        this.installerXmls = value;
		    }
		}
		
		private string installerUris;
		public virtual string InstallerUri 
		{ 
		    get
		    {
		        return this.installerUris;
		    }
		    set
		    {
		        this.installerUris = value;
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
		
		private string cacheUris;
		public virtual string CacheUri 
		{ 
		    get
		    {
		        return this.cacheUris;
		    }
		    set
		    {
		        this.cacheUris = value;
		    }
		}
		
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
		
		private SensorTypeSource sensorTypeSources;
		public virtual SensorTypeSource SensorTypeSource 
		{ 
		    get
		    {
		        return this.sensorTypeSources;
		    }
		    set
		    {
		        this.sensorTypeSources = value;
		    }
		}
		
	}
}