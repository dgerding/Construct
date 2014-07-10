namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class ScopedTypedChildNode<T, P> : ChildNode<T, P>, ITypedNode<T>, IScopedNode<T>
        {
            public string Type { get; protected set; }
            public abstract T SetType(string typeName);

            public Scope? Scope { get; protected set; }
            public abstract T SetScope(Scope scope);

            protected ScopedTypedChildNode(P parent)
                : base(parent)
            {
            }
        }
    }
}