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
	public partial class QuestionParameter
	{
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
		
		private Guid propertyTypeIDs;
		public virtual Guid PropertyTypeID 
		{ 
		    get
		    {
		        return this.propertyTypeIDs;
		    }
		    set
		    {
		        this.propertyTypeIDs = value;
		    }
		}
		
		private Guid questionIDs;
		public virtual Guid QuestionID 
		{ 
		    get
		    {
		        return this.questionIDs;
		    }
		    set
		    {
		        this.questionIDs = value;
		    }
		}
		
		private Property properties;
		public virtual Property Property 
		{ 
		    get
		    {
		        return this.properties;
		    }
		    set
		    {
		        this.properties = value;
		    }
		}
		
		private Question questions;
		public virtual Question Question 
		{ 
		    get
		    {
		        return this.questions;
		    }
		    set
		    {
		        this.questions = value;
		    }
		}
		
	}
}