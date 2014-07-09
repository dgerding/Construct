using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMFramework
{
	public class DataSnapshot
	{
		public Dictionary<String, double> Data = new Dictionary<string, double>();
		public DateTime TimeStamp;

		public DataSnapshot()
		{
			TimeStamp = DateTime.UtcNow;
		}

		public DataSnapshot(DateTime snapshotTime)
		{
			TimeStamp = snapshotTime;
		}

		public bool ContainsKeyContaining(String value)
		{
			foreach (var pair in Data)
			{
				if (pair.Key.Contains(value))
					return true;
			}

			return false;
		}

		public void Write(String key, double value)
		{
			Data.Add(key, value);
		}

		public void Write(String componentBaseName, Vector2 value)
		{
			Data.Add(componentBaseName + "X", value.X);
			Data.Add(componentBaseName + "Y", value.Y);
		}

		public void Write(String componentBaseName, Vector3 value)
		{
			Data.Add(componentBaseName + "X", value.X);
			Data.Add(componentBaseName + "Y", value.Y);
			Data.Add(componentBaseName + "Z", value.Z);
		}

		public void Write(String componentBaseName, List<Vector2> values)
		{
			for (int i = 0; i < values.Count; i++)
				Write(componentBaseName + i, values[i]);
		}

		public void Write(String componentBaseName, List<Vector3> values)
		{
			for (int i = 0; i < values.Count; i++)
				Write(componentBaseName + i, values[i]);
		}

		public double ComposeDouble(String componentBaseName)
		{
			double result = 0.0;

			if (Data.ContainsKey(componentBaseName))
				result = Data[componentBaseName];

			return result;
		}

		public float ComposeFloat(String componentBaseName)
		{
			float result = 0.0F;

			if (Data.ContainsKey(componentBaseName))
				result = (float)Data[componentBaseName];

			return result;
		}

		public Vector3 ComposeVector3(String componentBaseName)
		{
			Vector3 result = Vector3.Zero;

			if (Data.ContainsKey(componentBaseName + "X"))
				result.X = (float)Data[componentBaseName + "X"];
			if (Data.ContainsKey(componentBaseName + "Y"))
				result.Y = (float)Data[componentBaseName + "Y"];
			if (Data.ContainsKey(componentBaseName + "Z"))
				result.Z = (float)Data[componentBaseName + "Z"];

			return result;
		}

		public Vector2 ComposeVector2(String componentBaseName)
		{
			Vector2 result = Vector2.Zero;

			if (Data.ContainsKey(componentBaseName + "X"))
				result.X = (float)Data[componentBaseName + "X"];
			if (Data.ContainsKey(componentBaseName + "Y"))
				result.Y = (float)Data[componentBaseName + "Y"];

			return result;
		}

		public List<Vector3> ComposeVector3List(String componentBaseName)
		{
			//	If there isn't even a 0'th element there isn't any data for this component, return null
			if (!Data.ContainsKey(componentBaseName + "0X"))
				return null;

			List<Vector3> result = new List<Vector3>();

			for (int i = 0; Data.ContainsKey(componentBaseName + i + "X"); i++)
			{
				result.Add(this.ComposeVector3(componentBaseName + i));
			}

			return result;
		}

		public List<Vector2> ComposeVector2List(String componentBaseName)
		{
			if (!Data.ContainsKey(componentBaseName + "0X"))
				return null;

			List<Vector2> result = new List<Vector2>();

			for (int i = 0; Data.ContainsKey(componentBaseName + i + "X"); i++)
			{
				result.Add(this.ComposeVector2(componentBaseName + i));
			}

			return result;
		}

		public double this[String dataName]
		{
			get
			{
				return Data[dataName];
			}
			set
			{
				Data[dataName] = value;
			}
		}
	}
}
