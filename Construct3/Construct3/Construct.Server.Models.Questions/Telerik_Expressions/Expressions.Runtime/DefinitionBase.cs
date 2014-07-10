using System.Collections.Generic;
using System;

namespace Telerik.Expressions
{
	internal abstract class DefinitionBase : IMetadataProvider
	{
		private readonly string name;
		private readonly DefinitionMetadata metadata;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public DefinitionMetadata Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		IDictionary<string, object> IMetadataProvider.Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		protected DefinitionBase(string name, DefinitionMetadata metadata)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			this.metadata = metadata;
			this.name = name;
		}

		protected DefinitionBase(string name) 
			: this(name, DefinitionMetadata.Empty())
		{
		}
	}
}
