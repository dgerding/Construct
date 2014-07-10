using System;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public class NamespaceDefinition: NamedChildNode<NamespaceDefinition, AssemblyDefinition>
        {
            IList<ClassDefinition> classes = new List<ClassDefinition>();

            internal NamespaceDefinition(AssemblyDefinition parent)
                 :base(parent)
            {
            }

            protected override void Init()
            {
                // TODO: Implement this method
                //throw new NotImplementedException();
            }

            public override NamespaceDefinition SetName(string name)
            {
                Name = name;
                return this;
            }

            public ClassDefinition Class
            {
                get 
                {
                    ClassDefinition definition = new ClassDefinition(this);
                    classes.Add(definition);
                    return definition;
                }
            }

            public override string Generate()
            {
                string result;
                result = "namespace " + Name;
                result += "\r\n";
                result += "{";

                foreach (ClassDefinition enumerable in classes)
                {
                    result += "\r\n\t";
                    result += enumerable.Generate();
                }

                result += "\r\n";
                result += "}";
                return result;
            }
        }
    }
}