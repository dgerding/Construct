using System;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class TypedNamedChildNode<T, P> : NamedChildNode<T, P>, ITypedNode<T>
        {
            public string Type { get; protected set; }
            public abstract T SetType(string typeName);

            protected TypedNamedChildNode(P parent)
                : base(parent)
            {
            }

            protected override void Init()
            {
                SetType("System.Object");
                SetName(Guid.NewGuid().ToString());
            }

            public override string Generate()
            {
                string result;
                result = Type + " " + Name;
                return result;
            }

            public abstract override T SetName(string name);
        }
    }
}