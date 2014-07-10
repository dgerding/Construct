namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public interface INode
        {
            string Generate();
        }
    }
}