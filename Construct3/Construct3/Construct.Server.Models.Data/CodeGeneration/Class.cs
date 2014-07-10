using System;
using System.Collections.Generic;
using System.Linq;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public class ClassDefinition : ScopedNamedChildNode<ClassDefinition, NamespaceDefinition>
        {
            private IList<INode> nodes = new List<INode>();
            private IList<AttributeDefinition> attributes = new List<AttributeDefinition>();
            private string inheritsFrom;

            internal ClassDefinition(NamespaceDefinition parent)
                :base(parent)
            {
            }

            protected override void Init()
            {
                Scope = CodeDefinition.Scope.Public;
                base.Init();
            }

            public override ClassDefinition SetName(string name)
            {
                Name = name;
                return this;
            }

            public override ClassDefinition SetScope(Scope scope)
            {
                Scope = scope;
                return this;
            }

            public ClassDefinition SetInheritance(string typeName)
            {
                inheritsFrom = typeName;
                return this;
            }

            public PropertyDefinition Property
            {
                get 
                {
                    PropertyDefinition definition = new PropertyDefinition(this);
                    nodes.Add(definition);
                    return definition;
                }
            }
            public FieldDefinition Field
            {
                get 
                {
                    FieldDefinition definition = new FieldDefinition(this);
                    nodes.Add(definition);
                    return definition;
                }
            }
            public ScopedNamedTypedMethodDefinition Method
            {
                get 
                {
                    ScopedNamedTypedMethodDefinition definition = new ScopedNamedTypedMethodDefinition(this);
                    nodes.Add(definition);
                    return definition;
                }
            }
            public ClassDefinition.AttributeDefinition Attribute
            {
                get 
                {
                    AttributeDefinition definition = new AttributeDefinition(this);
                    attributes.Add(definition);
                    return definition; 
                }
            }
            public ClassDefinition.ConstructorDefinition Constructor
            {
                get
                {
                    ConstructorDefinition definition = new ConstructorDefinition(this);
                    nodes.Add(definition);
                    return definition;
                }
            }

            public override string Generate()
            {
                string result = "";

                foreach (AttributeDefinition attribute in attributes)
                {
                    result += attribute.Generate();
                }

                result += Scope.HasValue ? ConvertScope(Scope.Value) : "";
                result += "class ";
                result += Name;
                if (inheritsFrom != null)
                {
                    result += " : " + inheritsFrom;
                }
                result += "\r\n\t";
                result += "{";
                foreach (INode node in nodes)
                {
                    result += "\r\n\t\t";
                    result += node.Generate();
                }
                result += "\r\n\t";
                result += "}";
                return result;
            }

            public class AttributeDefinition : TypedNamedChildNode<AttributeDefinition, ClassDefinition>
            {
                internal AttributeDefinition(ClassDefinition parent)
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
                    result += "]\r\n\t";
                    return result;
                }

                public override AttributeDefinition SetType(string typeName)
                {
                    Type = typeName;
                    return this;
                }
            }

            public class ConstructorDefinition : ScopedMethodDefinition
            {
                private List<BaseArgumentDefinition> baseArguments = new List<BaseArgumentDefinition>();

                public BaseArgumentDefinition BaseArgument
                {
                    get
                    {
                        BaseArgumentDefinition node;
                        node = new BaseArgumentDefinition(this);
                        baseArguments.Add(node);
                        return node;
                    }
                }

                internal ConstructorDefinition(ClassDefinition parent) 
                    : base(parent)
                {
                }

                public override string Generate()
                {
                    string result;
                    result = Scope.HasValue ? ConvertScope(Scope.Value) + " " : "";
                    result += Parent.Name;
                    result += header.Generate();
                    result = "(";

                    if (baseArguments.Count > 0)
                    {
                        result += "\r\n\t\t\t: base(";
                        result += baseArguments.First().Generate();
                        IEnumerable<BaseArgumentDefinition> argumentDefinitions = baseArguments.Skip(1);
                        foreach (BaseArgumentDefinition argument in argumentDefinitions)
                        {
                            result += ", " + argument.Generate();
                        }
                        result += ")";
                    }

                    result += body.Generate();
                    return result;
                }

                public class BaseArgumentDefinition : NamedChildNode<BaseArgumentDefinition, ConstructorDefinition>
                {
                    internal BaseArgumentDefinition(ConstructorDefinition parent)
                        : base(parent)
                    {
                    }
                    public override string Generate()
                    {
                        return Name;
                    }

                    public override BaseArgumentDefinition SetName(string name)
                    {
                        this.Name = name;
                        return this;
                    }
                }
            }
        }
    }
}