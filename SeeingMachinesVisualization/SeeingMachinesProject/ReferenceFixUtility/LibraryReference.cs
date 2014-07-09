using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceFixUtility
{
	class LibraryReference
	{
		public String ReferenceName;
		public String HintPath;
		public String Version;
		public String Culture;
		public String PublicKeyToken;
		public String ProcessorArchitecture;

		public override string ToString()
		{
			String result = "";
			result += ReferenceName;

			if (Version != null)
				result += ", Version=" + Version;

			if (Culture != null)
				result += ", Culture=" + Culture;

			if (PublicKeyToken != null)
				result += ", PublicKeyToken=" + PublicKeyToken;

			if (ProcessorArchitecture != null)
				result += ", processorArchitecture=" + ProcessorArchitecture;

			return result;
		}
	}
}
