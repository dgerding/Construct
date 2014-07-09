using System;

namespace ConstructMetadataGenerator
{
	class ItemWrapper<T>
	{
		public ItemWrapper()
		{

		}

		public ItemWrapper(T value)
		{
			Value = value;
		}

		public ItemWrapper(T value, String textValue)
		{
			Value = value;
			TextValue = textValue;
		}

		public T Value;
		public String TextValue = null; // Leaving null causes a fallback to Value.ToString, or "Unknown" if Value is null.

		public override string ToString()
		{
			if (TextValue != null)
				return TextValue;

			if (Value != null)
				return Value.ToString();

			return "Unknown";
		}
	}
}
