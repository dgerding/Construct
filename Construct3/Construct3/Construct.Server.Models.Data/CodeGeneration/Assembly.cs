using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public static AssemblyDefinition Create
        {
            get 
            { 
                return new AssemblyDefinition(); 
            }
        }

        public partial class AssemblyDefinition : NamedNode<AssemblyDefinition>
        {
            IList<ReferenceDefinition> references;
            IList<NamespaceDefinition> namespaces;
            public string Directory { get; private set; }

            internal AssemblyDefinition()
            {
                references = new List<ReferenceDefinition>();
                namespaces = new List<NamespaceDefinition>();
            }

            public override AssemblyDefinition SetName(string name)
            {
                Name = name;
                return this;
            }

            public AssemblyDefinition SetDirectory(string directory)
            {
                Directory = directory;
                return this;
            }

            public ReferenceDefinition ReferencedAssembly
            {
                get 
                {
                    ReferenceDefinition reference = new ReferenceDefinition(this);
                    references.Add(reference);
                    return reference; 
                }
            }
            public NamespaceDefinition Namespace
            {
                get 
                {
                    NamespaceDefinition definition = new NamespaceDefinition(this);
                    namespaces.Add(definition);
                    return definition; 
                }
            }
            
            public override string Generate()
            {
                string result = "";

                foreach (ReferenceDefinition reference in references)
                {
                    result += reference.Generate();
                }
                foreach (NamespaceDefinition node in namespaces)
                {
                    result += node.Generate();
                }

                return result;
            }

            private string Code;
            public AssemblyDefinition Literal(string code)
            {
                Code = code;
                return this;
            }

            public System.Reflection.Assembly Compile()
            {
                System.Reflection.Assembly result;

                CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                CompilerInfo compilerInformation = CodeDomProvider.GetCompilerInfo("cs");
                CompilerParameters compilerParameters = compilerInformation.CreateDefaultCompilerParameters();
                compilerParameters.GenerateInMemory = true;
                compilerParameters.GenerateExecutable = false;
                compilerParameters.TreatWarningsAsErrors = false;
                compilerParameters.IncludeDebugInformation = false;

                compilerParameters.OutputAssembly = Path.Combine(Directory, Name + ".dll");

                compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
                compilerParameters.ReferencedAssemblies.Add("System.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
                compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

                foreach (ReferenceDefinition reference in references)
                {
                    if (string.IsNullOrEmpty(reference.Path) == false || string.IsNullOrEmpty(reference.Name) == false)
                    {
                        string fullName = reference.FullName;

                        if (String.IsNullOrEmpty(reference.Path) == false)
                        {
                            string referencePath = Path.Combine(Directory, reference.Name);
                            if (File.Exists(referencePath) == false)
                            {
                                File.Copy(reference.FullName, referencePath);
                            }
                        }
                        compilerParameters.ReferencedAssemblies.Add(fullName);
                    }
                }
                string code = Generate();
                if (code != null)
                    Code += code;
                CompilerResults results = null;
                results = provider.CompileAssemblyFromSource(compilerParameters, code);
                System.Reflection.Assembly assembly = results.CompiledAssembly;
                result = assembly;

                return result;
            }
        }
    }
}