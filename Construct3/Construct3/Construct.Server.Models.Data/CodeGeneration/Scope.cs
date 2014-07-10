namespace Construct.Server.Models.Data.CodeGeneration
{
    public partial class CodeDefinition : IFluentDefinition
    {
        public enum Scope
        {
            Private = 1,
            Protected = 2,
            Public = 4,
            Virtual = 6,
            Override = 8
        }
    }
}