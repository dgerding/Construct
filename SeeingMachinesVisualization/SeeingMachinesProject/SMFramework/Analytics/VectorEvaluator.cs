using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMFramework.Analytics
{
	public class VectorEvaluator
	{
		#region Internal Members
		LinkedList<Vector3> m_CollectedData = new LinkedList<Vector3>();
		#endregion

		/// <summary>
		///	Adds a vector to the data set for analysis.
		/// </summary>
		/// <param name="data">The data to be added to the set.</param>
		public void AddData(Vector3 data)
		{
			m_CollectedData.AddLast(data);
		}

		public void Clear()
		{
			m_CollectedData.Clear();
		}

		/// <summary>
		/// Total number of vectors in the collected data.
		/// </summary>
		public int Count
		{
			get
			{
				return m_CollectedData.Count;
			}
		}

		/// <summary>
		/// The standard deviation of the vectors in the collected data, calculated on a per-axis basis.
		/// </summary>
		public Vector3 StandardDeviationOnAxes
		{
			get
			{
				Vector3 mean = this.Mean;

				Vector3 standardDeviation = Vector3.Zero;
				foreach (Vector3 currentVector in m_CollectedData)
				{
					standardDeviation.X += Math.Abs(currentVector.X - mean.X);
					standardDeviation.Y += Math.Abs(currentVector.Y - mean.Y);
					standardDeviation.Z += Math.Abs(currentVector.Z - mean.Z);
				}

				return standardDeviation / Count;
			}
		}

		/// <summary>
		/// Standard deviation of the magnitude of the vectors within the collected data.
		/// </summary>
		public float StandardDeviationByMagnitude
		{
			get
			{
				Vector3 mean = this.Mean;

				float standardDeviation = 0.0F;
				foreach (Vector3 currentVector in m_CollectedData)
				{
					standardDeviation += (mean - currentVector).Length();
				}

				return standardDeviation / Count;
			}
		}

		/// <summary>
		/// Standard deviation of the direction of the vectors within the collected data. The direction
		/// is a dot-product result. An angle can be retrieved by using arc-cosine.
		/// </summary>
		public float StandardDeviationByDirection
		{
			get
			{
				Vector3 mean = this.Mean;
				mean.Normalize();
				float standardDeviation = 0.0F;
				foreach (Vector3 currentVector in m_CollectedData)
				{
					currentVector.Normalize();
					standardDeviation += 1.0F - Vector3.Dot(mean, currentVector);
				}

				return standardDeviation / Count;
			}
		}

		/// <summary>
		/// Returns a vector that has averaged components of all vectors in the collected data.
		/// </summary>
		public Vector3 Mean
		{
			get
			{
				Vector3 result = Vector3.Zero;
				foreach (Vector3 vector in m_CollectedData)
				{
					result += vector;
				}
				return result / Count;
			}
		}

		/// <summary>
		/// Returns a vector whose individual components are the maximum of all vectors in the collected data.
		/// </summary>
		public Vector3 Max
		{
			get
			{
				Vector3 result = Vector3.One * -100000.0F;
				foreach (Vector3 vector in m_CollectedData)
				{
					if (vector.X > result.X)
						result.X = vector.X;
					if (vector.Y > result.Y)
						result.Y = vector.Y;
					if (vector.Z > result.Z)
						result.Z = vector.Z;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns a vector whose individual components are the minimum of all vectors in the collected data.
		/// </summary>
		public Vector3 Min
		{
			get
			{
				Vector3 result = Vector3.One * 100000.0F;
				foreach (Vector3 vector in m_CollectedData)
				{
					if (vector.X < result.X)
						result.X = vector.X;
					if (vector.Y < result.Y)
						result.Y = vector.Y;
					if (vector.Z < result.Z)
						result.Z = vector.Z;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns the vector whose length is the max length in comparison to all vectors in the collected data.
		/// </summary>
		public Vector3 MaxVectorByLength
		{
			get
			{
				Vector3 maxLengthVector = Vector3.Zero;
				float maxLength = 0.0F;
				float currentLength;
				foreach (Vector3 vector in m_CollectedData)
				{
					currentLength = vector.LengthSquared ();
					if (currentLength > maxLength)
					{
						maxLength = currentLength;
						maxLengthVector = vector;
					}
				}
				return maxLengthVector;
			}
		}

		/// <summary>
		/// Returns the vector whose length is the min length in comparison to all vectors in the collected data.
		/// </summary>
		public Vector3 MinVectorByLength
		{
			get
			{
				Vector3 minLengthVector = Vector3.Zero;
				float minLength = 0.0F;
				float currentLength;
				foreach (Vector3 vector in m_CollectedData)
				{
					currentLength = vector.LengthSquared ();
					if (currentLength < minLength)
					{
						minLength = currentLength;
						minLengthVector = vector;
					}
				}
				return minLengthVector;
			}
		}

		/// <summary>
		/// Returns a sorted copy of the vectors in the collected data. Vectors are sorted by length.
		/// </summary>
		public List<Vector3> VectorsSortedByLength
		{
			get
			{
				List<Vector3> sortedCopy = new List<Vector3>(m_CollectedData.Count);
				foreach (Vector3 vector in m_CollectedData)
				{
					sortedCopy.Add(vector);
				}
				sortedCopy.Sort(delegate(Vector3 a, Vector3 b)
				{
					return (a.Length()).CompareTo(b.Length());
				});

				return sortedCopy;
			}
		}

		/// <summary>
		/// Returns the vector that defines the first quartile by length.
		/// </summary>
		public Vector3 FirstQuartileVectorByLength
		{
			get
			{
				if (m_CollectedData.Count == 0)
					return Vector3.Zero;

				List<Vector3> sortedData = VectorsSortedByLength;

				int quartileIndex = (sortedData.Count / 4) * 1;
				return sortedData[quartileIndex];
			}
		}

		/// <summary>
		/// Returns the vector that defines the second quartile (median) by length.
		/// </summary>
		public Vector3 SecondQuartileVectorByLength
		{
			get
			{
				if (m_CollectedData.Count == 0)
					return Vector3.Zero;

				List<Vector3> sortedData = VectorsSortedByLength;

				int quartileIndex = (sortedData.Count / 4) * 2;
				return sortedData[quartileIndex];
			}
		}

		/// <summary>
		/// Returns the vector that defines the third quartile by length.
		/// </summary>
		public Vector3 ThirdQuartileVectorByLength
		{
			get
			{
				if (m_CollectedData.Count == 0)
					return Vector3.Zero;

				List<Vector3> sortedData = VectorsSortedByLength;

				int quartileIndex = (sortedData.Count / 4) * 3;
				return sortedData[quartileIndex];
			}
		}
	}
}
