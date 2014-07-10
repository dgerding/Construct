using System;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public partial class AssemblyDefinition : CodeDefinition.NamedNode<AssemblyDefinition>
        {
            public class ReferenceDefinition : NamedChildNode<ReferenceDefinition, AssemblyDefinition>
            {
                public string Namespace { get; private set; }
                public string Path { get; private set; }

                public string FullName
                {
                    get
                    {
                        string result = "";

                        if (string.IsNullOrEmpty(Path))
                        {
                            if (string.IsNullOrEmpty(Name) == false)
                            {
                                result = Name;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Name) == false)
                            {
                                result = System.IO.Path.Combine(Path, Name);
                            }
                        }

                        return result;
                    }
                }

                internal ReferenceDefinition(AssemblyDefinition parent)
                    : base(parent)
                {
                    Path = "";
                    Name = "";
                }

                protected override void Init()
                {
                    SetName("System");
                    base.Init();
                }

                public override ReferenceDefinition SetName(string name)
                {
                    Name = name + ".dll";
                    Namespace = name;
                    return this;
                }

                public ReferenceDefinition SetNamespace(string value)
                {
                    Namespace = value;
                    return this;
                }

                public ReferenceDefinition SetPath(string path)
                {
                    Path = path;
                    return this;
                }
                
                public override string Generate()
                {
                    string result = "";
                    if (Namespace != null)
                    {
                        result = "using " + Namespace + ";\r\n";
                    }
                    return result;
                }
            }
        }
    }
}