namespace Telerik.Expressions
{
	internal enum ExpressionNodeType
	{
        /// <summary>
        /// Binary.
        /// </summary>
		Binary,

        /// <summary>
        /// Constant.
        /// </summary>
		Constant,

        /// <summary>
        /// Function.
        /// </summary>
		Function,

        /// <summary>
        /// Index.
        /// </summary>
		Index,

        /// <summary>
        /// Member.
        /// </summary>
		Member,

        /// <summary>
        /// Unary.
        /// </summary>
		Unary
	}
}