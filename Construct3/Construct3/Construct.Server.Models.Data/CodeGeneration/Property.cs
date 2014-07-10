using System;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public class PropertyDefinition : ScopedTypedNamedChildNode<PropertyDefinition, ClassDefinition>
        {
            public string Getter { get; private set; }
            public string Setter { get; private set; }

            internal PropertyDefinition(ClassDefinition parent)
                : base(parent)
            {
            }

            protected override void Init()
            {
                // TODO: Implement this method
                //throw new NotImplementedException();
            }

            public override PropertyDefinition SetName(string name)
            {
                Name = name;
                Getter = null;
                Setter = null;
                return this;
            }

            public override PropertyDefinition SetScope(Scope scope)
            {
                Scope = scope;
                return this;
            }

            public PropertyDefinition SetGetter(string method)
            {
                Getter = method;
                return this;
            }

            public PropertyDefinition SetSetter(string method)
            {
                Setter = method;
                return this;
            }

            public override string Generate()
            {
                string result;
                result = Scope.HasValue ? ConvertScope(Scope.Value) : "";
                result += Type + " ";
                result += Name;
                result += " {";

                if (Getter != null)
                {
                    result += " get { " + Getter + " } ";
                }
                else if (Setter == null)
                    result += " get;";

                if (Setter != null)
                {
                    result += " set { " + Setter + " } ";
                }
                else if (Getter == null)
                    result += " set;";
                
                result += "}";
                return result;
            }

            public override PropertyDefinition SetType(string typeName)
            {
                Type = typeName;
                return this;
            }
        }
    }
}