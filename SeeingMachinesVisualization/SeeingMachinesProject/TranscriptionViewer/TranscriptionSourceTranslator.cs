using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranscriptionViewer
{
	class TranscriptionSourceTranslator
	{
		private static Dictionary<Guid, String> TranslationTable = new Dictionary<Guid, String>()
		{
			//	Values pulled from Construct3.Sources_Sensor table
			{ new Guid("7D4BAAAA-C43D-4B50-8984-B964B46C3D71"), "Station 1" },
			{ new Guid("A7894DC9-0DF1-4E12-822A-FC29739EA35F"), "Station 2" },
			{ new Guid("8CA4F65E-14CF-4B18-AC56-BCD68E05C87D"), "Station 3" },
			{ new Guid("373DF4FC-B813-4DBA-A3A7-5C5CCA3F33ED"), "Station 4" }
		};

		public String TranslateSourceToName(Guid sourceId)
		{
			//return "Station 1";

			if (!TranslationTable.ContainsKey(sourceId))
				return null;

			return TranslationTable[sourceId];
		}


		public Guid TranslateNameToSource(String name)
		{
			if (!TranslationTable.ContainsValue(name))
				return Guid.Empty;

			return TranslationTable.First(pair => pair.Value == name).Key;
		}
	}
}
