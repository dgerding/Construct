using System.Collections.Generic;

namespace Telerik.Expressions
{
	// TODO: consider implementing IDictionary with proxing via storage field.
	internal class DefinitionMetadata : Dictionary<string, object>
	{
		private const string CategoryKey = "Category";
		private const string DescriptionKey = "Description";
		private const string BrowsableKey = "Browsable";

		public static DefinitionMetadata Empty()
		{
			return new DefinitionMetadata(string.Empty, string.Empty) { Browsable = false };
		}

		public string Category
		{
			get
			{
				return (string)this[CategoryKey];
			}
			private set
			{
				this[CategoryKey] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[DescriptionKey];
			}
			private set
			{
				this[DescriptionKey] = value;
			}
		}

		public bool Browsable
		{
			get
			{
				return this.GetValueOrDefault(BrowsableKey, true);
			}
			set
			{
				this[BrowsableKey] = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return
					string.IsNullOrEmpty(this.Description) &&
					string.IsNullOrEmpty(this.Category);
			}
		}

		public DefinitionMetadata(string description, string category)
		{
			this.Description = description;
			this.Category = category;
		}

		private T GetValueOrDefault<T>(string key, T defaultValue)
		{
			object value;
			if (this.TryGetValue(key, out value))
			{
				return (T)value;
			}

			return defaultValue;
		}
	}
}
