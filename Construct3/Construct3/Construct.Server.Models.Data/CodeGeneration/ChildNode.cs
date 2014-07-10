using System;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class ChildNode<T, P> : Node, IChildNode<T, P>
        {
            public P Parent { get; protected set; }
            public override abstract string Generate();

            protected ChildNode(P parent)
            {
                Parent = parent;
            }

            protected override void Init()
            {
            }
        }

        public abstract class NamedChildNode<T, P> : ChildNode<T, P>, INamedNode<T>
        {
            protected NamedChildNode(P parent)
                : base(parent)
            {
            }

            protected override void Init()
            {
                SetName(Guid.NewGuid().ToString());
            }

            public override abstract string Generate();

            #region INamedNode<T> Members

            public string Name { get; protected set; }
            public abstract T SetName(string name);
            #endregion
        }

        public abstract class NamedNode<T> : Node, INamedNode<T>
            where T : INamedNode<T>
        {
            #region INamedNode<T> Members
            public string Name { get; protected set; }

            public abstract T SetName(string name);

            #endregion

            protected override void Init()
            {
                Name = Guid.NewGuid().ToString();
            }

            public override abstract string Generate();
        }

        public interface INamedNode<T> : INode
        {
            string Name { get; }
            T SetName(string name);
        }
    }
}