namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public interface IChildNode<T, P> : INode
        {
            P Parent { get; }
        }
    }
}