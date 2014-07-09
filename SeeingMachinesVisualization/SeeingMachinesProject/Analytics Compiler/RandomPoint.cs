using Microsoft.Xna.Framework;
using SMFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsCompiler
{
	class RandomPoint : SMFramework.Testing.LabPoint
	{
		public RandomPoint(Vector3 position, float variance)
		{
			m_RootPosition = position;
			Variance = variance;
		}

		public bool IsValid
		{
			get { return true; }
		}

		public void InterpretData(DataSnapshot dataSource)
		{
		}

		public String SourceSensor
		{
			get;
			set;
		}

		public String Label
		{
			get { return "Random Point [" + m_RootPosition + "] [Variance " + Variance + "]"; }
		}

		public Vector3 Position
		{
			get
			{
				return m_RootPosition + new Vector3(
						(float)(m_RandomGenerator.NextDouble() * 2 - 1) * Variance,
						(float)(m_RandomGenerator.NextDouble() * 2 - 1) * Variance,
						(float)(m_RandomGenerator.NextDouble() * 2 - 1) * Variance
					);
			}

			set
			{
				m_RootPosition = value;
			}
		}

		private Vector3 m_RootPosition;
		private static Random m_RandomGenerator = new Random();

		float Variance;
	}
}
