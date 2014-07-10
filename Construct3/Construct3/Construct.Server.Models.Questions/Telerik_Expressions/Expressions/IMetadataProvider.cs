using System.Collections.Generic;

namespace Telerik.Expressions
{
	internal interface IMetadataProvider
	{
		IDictionary<string, object> Metadata { get; }
	}
}