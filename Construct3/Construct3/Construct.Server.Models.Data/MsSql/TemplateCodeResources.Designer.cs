﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Construct.Server.Models.Data.MsSql {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class TemplateCodeResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TemplateCodeResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Construct.Server.Models.Data.MsSql.TemplateCodeResources", typeof(TemplateCodeResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Data;
        ///using System.Data.Sql;
        ///using Microsoft.SqlServer.Server;
        ///using System.Data.SqlClient;
        ///using System.Data.SqlTypes;
        ///using System.Xml;
        ///using System.Text.RegularExpressions;
        ///using System.Text;
        ///
        ///public partial class TABLENAMETrigger
        ///{
        ///    [SqlTrigger(Name = @&quot;PROPERTYVALUENAMETrigger&quot;, Target = &quot;[dbo].[TABLENAME]&quot;, Event = &quot;FOR INSERT, UPDATE, DELETE&quot;)]
        ///    public static void PROPERTYVALUENAMETrigger()
        ///    {
        ///        SqlCommand command;
        ///        SqlTriggerContext tr [rest of string was truncated]&quot;;.
        /// </summary>
        public static string clrTriggerCs {
            get {
                return ResourceManager.GetString("clrTriggerCs", resourceCulture);
            }
        }
    }
}
