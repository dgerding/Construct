using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Construct.MessageBrokering
{
	class JsonConfigurator
	{
		private static bool HasConfigured = false;
		public static void Run()
		{
			if (HasConfigured)
				return;

			//	DateTime Milliseconds fix
			JsonConvert.DefaultSettings = () =>
			{
				JsonSerializerSettings settings = new JsonSerializerSettings();
				IsoDateTimeConverter converter = new IsoDateTimeConverter();
				converter.DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss.fffK";
				settings.Converters.Add(converter);
				return settings;
			};

			HasConfigured = true;
		}
	}
}
