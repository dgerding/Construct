//Copyright (c) %CompanyName%.  All rights reserved.
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using %ProductNamespace%;
using %ProductNamespace%.Util;

namespace %DefaultNamespace%
{
	/// <summary>
	/// Summary description for %ClassName%.
	/// </summary>
	[WebService(Namespace="%DefaultNamespace%")]
	public class %ClassName% : System.Web.Services.WebService
	{
		public %ClassName%()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

#region %ProductName% generated code
		[WebMethod]
		public %ProductNamespace%.ObjectContainer.ChangeSet Load( FillInstructionData instr )
		{
			try
			{
				return new ObjectContainerProvider1().Load( instr );
			}
			catch (Exception exc)
			{
				string msg = exc.Message;
				throw;
			}
		}
		[WebMethod]
		public void Save( %ProductNamespace%.ObjectContainer.ChangeSet changes )
		{
			try
			{
				new ObjectContainerProvider1().Save( changes );
			}
			catch (Exception exc)
			{
				string msg = exc.Message;
				throw;
			}
			return;
		}
		[WebMethod]
		public %ProductNamespace%.ObjectContainer.ChangeSet Sync( %ProductNamespace%.ObjectContainer.ChangeSet changes )
		{
			try
			{
				return new ObjectContainerProvider1().Sync( changes );
			}
			catch (Exception exc)
			{
				string msg = exc.Message;
				throw;
			}
		}
		#endregion
	}
}
