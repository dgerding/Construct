using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	interface IStreamDataSource
	{
		//	Property ID / Source ID / Data
		event Action<String, String, object> OnData;

		void Start();
		void Stop();
	}
}
