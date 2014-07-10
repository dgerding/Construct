using System;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class ScopedNamedChildNode<T, P> : NamedChildNode<T, P>, IScopedNode<T>
        {
            public Scope? Scope { get; protected set; }
            public abstract T SetScope(Scope scope);

            public override abstract string Generate();

            public override abstract T SetName(string name);

            protected ScopedNamedChildNode(P parent)
                : base(parent)
            {
            }

        }

        public abstract class ScopedChildNode<T, P> : ChildNode<T, P>, IScopedNode<T>
        {
            public Scope? Scope { get; protected set; }
            protected override abstract void Init();

            public abstract T SetScope(Scope scope);

            public override abstract string Generate();

            protected ScopedChildNode(P parent)
                : base(parent)
            {
            }
        }
    }
}