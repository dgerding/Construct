using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMFramework.Analytics
{
	public class FloatEvaluator
	{
		#region Internal Members
		LinkedList<float> m_CollectedData = new LinkedList<float>();
		#endregion

		public void AddFloat(float value)
		{
			m_CollectedData.AddLast(value);
		}

		public void Clear()
		{
			m_CollectedData.Clear();
		}

		// TODO: Implement confidence generation from FloatEvaluator
		//public static float GenerateConfidence (float target)
		//{
		//	FloatEvaluator evaluator = new FloatEvaluator();

		//	if (StandardDeviation == 0.0f) return 1.0f;

		//	float deviationToMinimumErrorDistance = Math.Max(0.0f, StandardDeviation - target - Min);

		//	float quickMaxLength = Max;
		//	float unweightedConfidence = (1 - deviationToMinimumErrorDistance / quickMaxLength) * (target / quickMaxLength);

		//	return Math.Max(0.0f, Math.Min(1.0f, unweightedConfidence));
		//}

		public int Count
		{
			get
			{
				return m_CollectedData.Count;
			}
		}

		public float StandardDeviation
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				float mean = Mean;
				float sumDeviation = 0.0F;
				foreach (float value in m_CollectedData)
				{
					float currentAbs = Math.Abs(value - mean);
					if (!float.IsNaN(currentAbs))
						sumDeviation += currentAbs;
				}

				return sumDeviation / Count;
			}
		}

		public float Mean
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				return m_CollectedData.Average();
			}
		}

		public float Max
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				return m_CollectedData.Max();
			}
		}

		public float Min
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				/* Observed behavior: LinkedList's min can break if a value is NaN, use
				 *	a manual implementation to avoid this*/

				float min = float.MaxValue;
				foreach (float value in m_CollectedData)
					if (value < min)
						min = value;

				return min;
			}
		}

		public float AbsMin
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				float absMin = float.MaxValue;
				foreach (float value in m_CollectedData)
					if (Math.Abs(value) < absMin)
						absMin = Math.Abs(value);

				return absMin;
			}
		}

		public List<float> SortedFloats
		{
			get
			{
				List<float> sorted = new List<float>();

				foreach (float value in m_CollectedData)
				{
					sorted.Add(value);
				}

				sorted.Sort();

				return sorted;
			}
		}

		public float FirstQuartile
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				int quartileIndex = (Count / 4) * 1;
				return SortedFloats[quartileIndex];
			}
		}

		public float SecondQuartile
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				int quartileIndex = (Count / 4) * 2;
				return SortedFloats[quartileIndex];
			}
		}

		public float ThirdQuartile
		{
			get
			{
				if (Count == 0)
					return 0.0F;

				int quartileIndex = (Count / 4) * 3;
				return SortedFloats[quartileIndex];
			}
		}
	}
}
