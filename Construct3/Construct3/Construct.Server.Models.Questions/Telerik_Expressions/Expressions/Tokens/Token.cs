using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal abstract class Token : IMetadataProvider
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

		public abstract TokenType TokenType { get; }
	}
}