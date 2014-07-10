using System;
using System.Collections.Generic;
using System.Linq;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class MethodDefinition<T> : ChildNode<T, ClassDefinition>
            where T : MethodDefinition<T>
        {
            protected HeaderDefinition header;
            protected BodyDefinition body;

            internal MethodDefinition(ClassDefinition parent)
                : base(parent)
            {
                Parent = parent;
                header = new HeaderDefinition(this);
                body = new BodyDefinition(this);
            }

            protected override void Init()
            {
            }

            public override string Generate()
            {
                string result;
                result = header.Generate();
                result += body.Generate();
                return result;
            }

            public abstract ArgumentDefinition Argument { get; }

            public MethodDefinition<T> SetBody(string body)
            {
                this.body.SetBody(body);
                return this;
            }

            public class HeaderDefinition : ChildNode<HeaderDefinition, MethodDefinition<T>>
            {
                public readonly IList<ArgumentDefinition> arguments = new List<ArgumentDefinition>();

                public HeaderDefinition(MethodDefinition<T> parent)
                    : base(parent)
                {
                }

                public override string Generate()
                {
                    string result;
                    result = "(";
                    if (arguments.Count() > 0)
                    {
                        result += arguments.First().Generate();
                        IEnumerable<ArgumentDefinition> argumentDefinitions = arguments.Skip(1);
                        foreach (ArgumentDefinition argument in argumentDefinitions)
                        {
                            result += ", " + argument.Generate(); 
                        }
                    }
                    result += ")";
                    return result;
                }
            }

            public class ArgumentDefinition : TypedNamedChildNode<ArgumentDefinition, MethodDefinition<T>>
            {
                internal ArgumentDefinition(MethodDefinition<T> parent)
                    : base(parent)
                {
                }

                public override ArgumentDefinition SetType(string typeName)
                {
                    Type = typeName;
                    return this;
                }

                public override ArgumentDefinition SetName(string name)
                {
                    Name = name;
                    return this;
                }
            }

            public class BodyDefinition : ChildNode<BodyDefinition, MethodDefinition<T>>
            {
                private string body;

                internal BodyDefinition(MethodDefinition<T> parent)
                    : base(parent)
                {
                }

                public override string Generate()
                {
                    string result;
                    result = "\r\n\t\t{\r\n";
                    result += body + "\r\n";
                    result += "\t\t}\r\n";
                    return result;
                }

                public BodyDefinition SetBody(string body)
                {
                    this.body = body;
                    return this;
                }
            }
        }

        public class TypedMethodDefinition : MethodDefinition<TypedMethodDefinition>, ITypedNode<TypedMethodDefinition>
        {
            internal TypedMethodDefinition(ClassDefinition parent) : base(parent)
            {
            }

            public string Type { get; protected set; }

            public override ArgumentDefinition Argument
            {
                get
                {
                    ArgumentDefinition definition = new ArgumentDefinition(this);
                    header.arguments.Add(definition);
                    return definition;
                }
            }

            public TypedMethodDefinition SetType(string type)
            {
                Type = type;
                return this;
            }

            protected override void Init()
            {
                Type = "void";
                base.Init();
            }

            public override string Generate()
            {
                return
                      Type + " " +
                      base.Generate();
            }
        }

        public class NamedMethodDefinition : MethodDefinition<NamedMethodDefinition>, INamedNode<NamedMethodDefinition>
        {
            #region INamedNode<NamedMethodDefinition> Members
            public string Name { get; protected set; }

            public NamedMethodDefinition SetName(string name)
            {
                Name = name;
                return this;
            }

            #endregion

            public override ArgumentDefinition Argument
            {
                get
                {
                    ArgumentDefinition definition = new ArgumentDefinition(this);
                    header.arguments.Add(definition);
                    return definition;
                }
            }

            internal NamedMethodDefinition(ClassDefinition parent) : base(parent)
            {
            }

            protected override void Init()
            {
                Name = Guid.NewGuid().ToString();
                base.Init();
            }

            public override string Generate()
            {
                return
                      Name + " " +
                      base.Generate();
            }
        }

        public class ScopedMethodDefinition : MethodDefinition<ScopedMethodDefinition>, IScopedNode<ScopedMethodDefinition>
        {
            #region IScopedNode<ScopedMethodDefinition> Members
            public Scope? Scope { get; protected set; }

            public ScopedMethodDefinition SetScope(Scope scope)
            {
                Scope = scope;
                return this;
            }

            #endregion

            public override ArgumentDefinition Argument
            {
                get
                {
                    ArgumentDefinition definition = new ArgumentDefinition(this);
                    header.arguments.Add(definition);
                    return definition;
                }
            }

            internal ScopedMethodDefinition(ClassDefinition parent) : base(parent)
            {
            }

            protected override void Init()
            {
                Scope = CodeDefinition.Scope.Private;
                base.Init();
            }

            public override string Generate()
            {
                return
                      ConvertScope(Scope) +
                      base.Generate();
            }

            public static string ConvertScope(Scope? scope)
            {
                string result = "";

                if (scope.HasValue)
                {
                    int privateResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Private;
                    int protectedResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Protected;
                    int publicResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Public;
                    int virtualResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Virtual;
                    int overrideResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Override;

                    if (privateResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Private)
                        result += "private ";
                    else if (publicResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Public)
                        result += "public ";
                    else if (protectedResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Protected)
                        result += "protected ";

                    if (virtualResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Virtual)
                        result += "virtual ";
                    else if (overrideResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Override)
                        result += "override ";
                }
                return result;
            }
        }

        public class ScopedNamedMethodDefinition : NamedMethodDefinition, IScopedNode<ScopedNamedMethodDefinition>
        {
            #region IScopedNode<ScopedNamedMethodDefinition> Members
            public Scope? Scope { get; protected set; }

            public ScopedNamedMethodDefinition SetScope(Scope scope)
            {
                Scope = scope;
                return this;
            }

            #endregion

            internal ScopedNamedMethodDefinition(ClassDefinition parent) : base(parent)
            {
            }

            protected override void Init()
            {
                this.Scope = CodeDefinition.Scope.Private;
                base.Init();
            }

            public override string Generate()
            {
                return
                      ConvertScope(Scope) +
                      base.Generate();
            }

            public static string ConvertScope(Scope? scope)
            {
                string result = "";

                if (scope.HasValue)
                {
                    int privateResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Private;
                    int protectedResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Protected;
                    int publicResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Public;
                    int virtualResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Virtual;
                    int overrideResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Override;

                    if (privateResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Private)
                        result += "private ";
                    else if (publicResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Public)
                        result += "public ";
                    else if (protectedResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Protected)
                        result += "protected ";

                    if (virtualResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Virtual)
                        result += "virtual ";
                    else if (overrideResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Override)
                        result += "override ";
                }
                return result;
            }
        }

        public class TypedNamedMethodDefinition : NamedMethodDefinition, ITypedNode<TypedNamedMethodDefinition>
        {
            internal TypedNamedMethodDefinition(ClassDefinition parent) : base(parent)
            {
            }

            protected override void Init()
            {
                this.Type = "System.Object";
                base.Init();
            }

            public override string Generate()
            {
                return
                      Type + " " +
                      base.Generate();
            }

            public string Type { get; private set; }

            public TypedNamedMethodDefinition SetType(string type)
            {
                Type = type;
                return this;
            }
        }

        public class ScopedNamedTypedMethodDefinition : TypedNamedMethodDefinition, IScopedNode<ScopedNamedTypedMethodDefinition>
        {
            public CodeDefinition.Scope? Scope { get; protected set; }

            public ScopedNamedTypedMethodDefinition SetScope(CodeDefinition.Scope scope)
            {
                Scope = scope;
                return this;
            }

            internal ScopedNamedTypedMethodDefinition(ClassDefinition parent) : base(parent)
            {
            }

            protected override void Init()
            {
                Scope = CodeDefinition.Scope.Private;
                base.Init();
            }

            public override string Generate()
            {
                return
                      ConvertScope(Scope) +
                      base.Generate();
            }

            public static string ConvertScope(Scope? scope)
            {
                string result = "";

                if (scope.HasValue)
                {
                    int privateResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Private;
                    int protectedResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Protected;
                    int publicResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Public;
                    int virtualResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Virtual;
                    int overrideResult = (int)scope.Value & (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Override;

                    if (privateResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Private)
                        result += "private ";
                    else if (publicResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Public)
                        result += "public ";
                    else if (protectedResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Protected)
                        result += "protected ";

                    if (virtualResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Virtual)
                        result += "virtual ";
                    else if (overrideResult == (int)Construct.Server.Models.Data.CodeGeneration.CodeDefinition.Scope.Override)
                        result += "override ";
                }
                return result;
            }
        }
    }
}