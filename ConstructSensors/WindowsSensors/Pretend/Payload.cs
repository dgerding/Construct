using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pretend
{
	class PretendPayload
	{
		public PretendPayload(int value)
		{
			Thing = value;
		}

		public int Thing { get; set; }
	}
}
