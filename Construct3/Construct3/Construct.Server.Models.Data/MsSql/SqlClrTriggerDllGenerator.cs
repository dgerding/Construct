using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;

namespace Construct.Server.Models.Data.MsSql
{
    public static class SqlClrTableTriggerDllGenerator
    {
        public static void Compile(string dllUncPath, string tableName)
        {
            //System.Reflection.Assembly result;

            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
            CompilerInfo compilerInformation = CodeDomProvider.GetCompilerInfo("cs");
            CompilerParameters compilerParameters = compilerInformation.CreateDefaultCompilerParameters();
            compilerParameters.GenerateInMemory = false;
            compilerParameters.GenerateExecutable = false;
            compilerParameters.OutputAssembly = tableName.Replace("z_", "") + ".dll";
            compilerParameters.TreatWarningsAsErrors = false;
            compilerParameters.IncludeDebugInformation = false;
            //compilerParameters.ReferencedAssemblies.

        
            

            compilerParameters.OutputAssembly = Path.Combine(dllUncPath, tableName.Replace("z_","") + ".dll");

            
            ///using System;
            compilerParameters.ReferencedAssemblies.Add(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll");
            //Using System.Core
            compilerParameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Core.dll");
            //using System.Data;
            compilerParameters.ReferencedAssemblies.Add(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.Data.dll");
            
            //using System.Data.Sql;
            //using Microsoft.SqlServer.Server;
            //using System.Data.SqlClient;
            //using System.Data.SqlTypes;
            //compilerParameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Microsoft SQL Server\100\SDK\Assemblies");
            
            //using System.Xml;
            //compilerParameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v2.0.50727\System.Xml.dll");
            
            //using System.Xml.Linq
            compilerParameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Xml.Linq.dll");

            /*
            
            //


            using System.Text.RegularExpressions;
            using System.Text;using System.Data;
            using System.Data.Sql;
            using Microsoft.SqlServer.Server;
            using System.Data.SqlClient;
            using System.Data.SqlTypes;
            using System.Xml;
            using System.Text.RegularExpressions;
            using System.Text;*/
            

            string code = TemplateCodeResources.clrTriggerCs;

            CompilerResults results = null;
            results = provider.CompileAssemblyFromSource(compilerParameters, code);
            

            
        }
    }
}