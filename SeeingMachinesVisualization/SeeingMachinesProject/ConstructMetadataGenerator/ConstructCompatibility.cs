using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructMetadataGenerator
{
	static class ConstructCompatibility
	{
		/* Only used for strings right now, but Construct supports types such as 'string' and 'System.String', but specifying
		 *	"String" isn't recognized as a datatype (since it only supports 'string' and 'System.String'. This maintains a
		 *	mapping of things such as 'String' -> 'string', referring to the same type but using a string that Construct
		 *	recognizes.
		 */

		public static String AutoMap(String typeName)
		{
			if (!Mapping.ContainsKey(typeName))
				return typeName;

			return Mapping[typeName];
		}

		public static Dictionary<String, String> Mapping = new Dictionary<string, string>()
		{
			{ "String", "string" } // Supports "System.String" and "string"
		};
	}
}
