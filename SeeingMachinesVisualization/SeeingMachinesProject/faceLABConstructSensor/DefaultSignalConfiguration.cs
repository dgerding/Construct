using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMFramework;

namespace faceLABConstructSensor
{
	public static class DefaultSignalConfiguration
	{

		private static FaceLabSignalConfiguration m_Config = null;
		public static FaceLabSignalConfiguration Config
		{
			get
			{
				if (m_Config == null)
				{
					m_Config = new FaceLabSignalConfiguration();
					m_Config.Label = "Unlabeled";
					m_Config.Port = 2001;
				}

				return m_Config;
			}
		}
	}
}
