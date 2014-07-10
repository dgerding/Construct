using System;
using System.Linq;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition
    {
        public abstract class TypedNode<T> : Node, ITypedNode<T>
        {
            public string Type { get; protected set; }
            public abstract T SetType(string type);
        }
    }
}