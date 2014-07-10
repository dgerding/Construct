using System;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public abstract class Node: INode
        {
            protected Node()
            {
                Init();
            }

            protected abstract void Init();

            public abstract string Generate();

            public static string ConvertScope(Scope scope)
            {
                string result = "";

                int privateResult = (int)scope & (int)Scope.Private;
                int protectedResult = (int)scope & (int)Scope.Protected;
                int publicResult = (int)scope & (int)Scope.Public;
                int virtualResult = (int)scope & (int)Scope.Virtual;
                int overrideResult = (int)scope & (int)Scope.Override;

                if (privateResult == (int)Scope.Private)
                    result += "private ";
                else if (publicResult == (int)Scope.Public)
                    result += "public ";
                else if (protectedResult == (int)Scope.Protected)
                    result += "protected ";

                if (virtualResult == (int)Scope.Virtual)
                    result += "virtual ";
                else if (overrideResult == (int)Scope.Override)
                    result += "override ";

                return result;
            }
        }
    }
}