using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMFramework.Testing
{
	public interface ValidityIdentifier
	{
		bool IsNull();
		void SetFromSnapshot(String value, DataSnapshot snapshot);
	}

	public class Vector3ValidityIdentifier : ValidityIdentifier
	{
		public Vector3ValidityIdentifier(Vector3 nullValue) { NullValue = nullValue; }
		public bool IsNull() { return (Value - NullValue).LengthSquared() < 0.001F; }
		public void SetFromSnapshot(String value, DataSnapshot snapshot) { Value = snapshot.ComposeVector3(value); }
		public Vector3 NullValue;
		internal Vector3 Value;
	}

	public class Vector2ValidityIdentifier : ValidityIdentifier
	{
		public Vector2ValidityIdentifier(Vector2 nullValue) { NullValue = nullValue; }
		public bool IsNull() { return (Value - NullValue).LengthSquared() < 0.001F; }
		public void SetFromSnapshot(String value, DataSnapshot snapshot) { Value = snapshot.ComposeVector2(value); }
		public Vector2 NullValue;
		internal Vector2 Value;
	}

	public class FloatValidityIdentifier : ValidityIdentifier
	{
		public FloatValidityIdentifier(float nullValue) { NullValue = nullValue; }
		public bool IsNull() { return Math.Abs(Value - NullValue) < 0.001F; }
		public void SetFromSnapshot(String value, DataSnapshot snapshot) { Value = (float)snapshot.Data[value]; }
		public float NullValue;
		internal float Value;
	}
}
