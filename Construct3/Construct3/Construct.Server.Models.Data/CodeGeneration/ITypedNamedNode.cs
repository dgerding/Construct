using System;
using System.Linq;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class TypedNamedNode<T> : ITypedNode<T>, INamedNode<T>
        {
            public string Name { get; protected set; }

            public abstract T SetName(string name);

            public string Generate()
            {
                string result;
                result = Type + " " + Name;
                return result;
            }

            public string Type { get; protected set; }

            public abstract T SetType(string type);
        }
    }
}