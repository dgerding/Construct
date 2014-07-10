namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public interface ITypedNode<T> : INode
        {
            string Type { get; }
            T SetType(string type);
        }
    }
}