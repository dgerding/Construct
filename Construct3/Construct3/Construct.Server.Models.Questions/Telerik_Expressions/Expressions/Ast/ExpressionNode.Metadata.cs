using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal partial class ExpressionNode : IMetadataProvider
	{
		private Dictionary<string, object> metadata;

		IDictionary<string, object> IMetadataProvider.Metadata
		{
			get
			{
				if (this.metadata == null)
				{
					this.metadata = new Dictionary<string, object>();
				}

				return this.metadata;
			}
		}
	}
}
