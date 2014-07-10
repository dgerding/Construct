namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public interface IScopedNode<T>: INode
        {
            Scope? Scope { get; }
            T SetScope(Scope scope);
        }
    }
}