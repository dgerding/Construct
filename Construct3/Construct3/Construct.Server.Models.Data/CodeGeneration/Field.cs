using System.Collections.Generic;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class ScopedTypedNamedChildNode<T, P> : ScopedTypedChildNode<T, P>, INamedNode<T>
        {
            protected ScopedTypedNamedChildNode(P parent)
                : base(parent)
            {
            }

            public override abstract T SetType(string typeName);

            public override abstract T SetScope(Scope scope);

            protected override abstract void Init();

            public override abstract string Generate();

            #region INamedNode<FieldDefinition> Members

            public string Name { get; protected set; }

            public abstract T SetName(string name);
            #endregion
        }

        public class FieldDefinition : ScopedTypedNamedChildNode<FieldDefinition, ClassDefinition>
        {
            private IList<FieldDefinition.AttributeDefinition> attributes = new List<AttributeDefinition>();
            private string initialValue;

            internal FieldDefinition(ClassDefinition parent)
                : base(parent)
            {
                Parent = parent;
            }

            public FieldDefinition.AttributeDefinition Attribute
            {
                get 
                {
                    AttributeDefinition definition = new AttributeDefinition(this);
                    attributes.Add(definition);
                    return definition;
                }
            }

            public FieldDefinition SetInitialValue(string value)
            {
                initialValue = value;
                return this;
            }

            public override FieldDefinition SetName(string name)
            {
                Name = name;
                return this;
            }

            public override FieldDefinition SetScope(Scope scope)
            {
                Scope = scope;
                return this;
            }

            public override string Generate()
            {
                string result;
                result = Scope.HasValue ? ConvertScope(Scope.Value) : "private ";
                result += Type + " ";
                result += Name;
                if (initialValue != null)
                    result += " = " + initialValue;
                result += ";";
                return result;
            }

            public override FieldDefinition SetType(string typeName)
            {
                Type = typeName;
                return this;
            }

            public class AttributeDefinition : TypedNamedChildNode<AttributeDefinition, FieldDefinition>
            {
                internal AttributeDefinition(FieldDefinition parent)
                    : base(parent)
                {
                }

                public override AttributeDefinition SetName(string name)
                {
                    Name = name;
                    return this;
                }

                public override string Generate()
                {
                    string result;
                    result = "[";
                    result += Name;
                    result += "]";
                    return result;
                }

                public override AttributeDefinition SetType(string typeName)
                {
                    Type = typeName;
                    return this;
                }
            }

            protected override void Init()
            {
            }
        }
    }
}